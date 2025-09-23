using System.Security.Cryptography;
using System.Text;
using WebMessages.WEB.Models;

namespace WebMessages.WEB.Helpers
{
    public static class E2E
    {
        private const int AesKeyLength = 32; // 256 bits

        // --- Encrypt ---
        public static Message Encrypt(
            string plainText,
            Guid fromDeviceId,
            Guid toDeviceId,
            byte[] recipientPublicKey)
        {
            // 1) Gerar par efêmero P-256
            using var eph = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP256);
            byte[] ephPub = eph.PublicKey.ExportSubjectPublicKeyInfo();

            // 2) Importar chave pública do destinatário
            using var receiverPubKey = ECDiffieHellman.Create();
            receiverPubKey.ImportSubjectPublicKeyInfo(recipientPublicKey, out _);

            // 3) Calcular shared secret
            byte[] sharedSecret = eph.DeriveKeyMaterial(receiverPubKey.PublicKey);

            // 4) Derivar chave AES com HKDF nativo
            byte[] aesKey = HKDF.DeriveKey(
                HashAlgorithmName.SHA256,
                sharedSecret,
                outputLength: 32,
                info: Encoding.UTF8.GetBytes("aes-gcm")
);

            // 5) AES-GCM
            byte[] nonce = new byte[12];
            RandomNumberGenerator.Fill(nonce);

            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] cipherBytes = new byte[plainBytes.Length];
            byte[] tag = new byte[16];

            using (var aes = new AesGcm(aesKey))
            {
                aes.Encrypt(nonce, plainBytes, cipherBytes, tag);
            }
            Console.WriteLine($"sharedSecret: {Convert.ToHexString(sharedSecret)}");
            Console.WriteLine($"aesKey: {Convert.ToHexString(aesKey)}");

            // 6) Concatenar cipher + tag
            byte[] cipherWithTag = new byte[cipherBytes.Length + tag.Length];
            Buffer.BlockCopy(cipherBytes, 0, cipherWithTag, 0, cipherBytes.Length);
            Buffer.BlockCopy(tag, 0, cipherWithTag, cipherBytes.Length, tag.Length);

            return new Message
            {
                FromDeviceId = fromDeviceId,
                ToDeviceId = toDeviceId,
                CipherText = Convert.ToBase64String(cipherWithTag),
                Nonce = Convert.ToBase64String(nonce),
                EphemeralPub = Convert.ToBase64String(ephPub)
            };
        }

        // --- Decrypt ---
        public static string Decrypt(Message msg, byte[] recipientPrivateKey)
        {
            using var receiver = ECDiffieHellman.Create();
            receiver.ImportPkcs8PrivateKey(recipientPrivateKey, out _);

            using var ephPubKey = ECDiffieHellman.Create();
            ephPubKey.ImportSubjectPublicKeyInfo(Convert.FromBase64String(msg.EphemeralPub), out _);

            // Shared secret
            byte[] sharedSecret = receiver.DeriveKeyMaterial(ephPubKey.PublicKey);

            // AES key via HKDF
            byte[] aesKey = HKDF.DeriveKey(HashAlgorithmName.SHA256, sharedSecret, outputLength: 32, info: Encoding.UTF8.GetBytes("aes-gcm"));
            Console.WriteLine($"sharedSecret: {Convert.ToHexString(sharedSecret)}");
            Console.WriteLine($"aesKey: {Convert.ToHexString(aesKey)}");

            // Decode nonce + ciphertext
            byte[] nonce = Convert.FromBase64String(msg.Nonce);
            byte[] cipherAndTag = Convert.FromBase64String(msg.CipherText);

            if (cipherAndTag.Length < 16)
                throw new Exception("CipherText muito curto.");

            int cipherLength = cipherAndTag.Length - 16;
            byte[] cipherBytes = new byte[cipherLength];
            byte[] tag = new byte[16];

            Buffer.BlockCopy(cipherAndTag, 0, cipherBytes, 0, cipherLength);
            Buffer.BlockCopy(cipherAndTag, cipherLength, tag, 0, 16);

            byte[] plainBytes = new byte[cipherBytes.Length];

            using (var aes = new AesGcm(aesKey))
            {
                if (nonce.Length != 12)
                    throw new Exception($"Nonce inválido. Esperado 12 bytes, mas recebeu {nonce.Length} bytes.");

                aes.Decrypt(nonce, cipherBytes, tag, plainBytes);
            }

            return Encoding.UTF8.GetString(plainBytes);
        }
    }
}
