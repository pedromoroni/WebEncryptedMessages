using System.Security.Cryptography;
using System.Text;
using WebMessages.WEB.Models;
using WebMessages.WEB.Models.Messages;

namespace WebMessages.WEB.Helpers;

public static class E2E
{
    // --- Deriva chave simétrica com HKDF-SHA256 (RFC 5869) ---
    private static byte[] DeriveKey(byte[] sharedSecret, int length = 32, string context = "aes-gcm")
    {
        using var hkdf = new Hkdf(HashAlgorithmName.SHA256, sharedSecret, salt: null, Encoding.UTF8.GetBytes(context));
        byte[] okm = new byte[length];
        hkdf.Fill(okm);
        return okm;
    }

    public static Message Encrypt(
        string plainText,
        Guid fromDeviceId,
        Guid toDeviceId,
        byte[] recipientPublicKey)
    {
        // 1) Criar par efêmero P-256
        using var eph = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP256);
        byte[] ephPub = eph.PublicKey.ExportSubjectPublicKeyInfo();

        // 2) Importar chave pública do destinatário
        using var receiverPubKey = ECDiffieHellman.Create();
        receiverPubKey.ImportSubjectPublicKeyInfo(recipientPublicKey, out _);

        // 3) Calcular segredo compartilhado
        byte[] sharedSecret = eph.DeriveKeyMaterial(receiverPubKey.PublicKey);

        // 4) Derivar chave simétrica (AES-256)
        byte[] aesKey = DeriveKey(sharedSecret);

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

        // Juntar cipher + tag
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


    public static string Decrypt(Message msg, byte[] recipientPrivateKey)
    {
        // 1) importar chave privada do destinatário
        using var receiver = ECDiffieHellman.Create();
        receiver.ImportPkcs8PrivateKey(recipientPrivateKey, out _);


        // 2) importar chave pública efêmera do remetente
        using var ephPubKey = ECDiffieHellman.Create();
        ephPubKey.ImportSubjectPublicKeyInfo(Convert.FromBase64String(msg.EphemeralPub), out _);

        // 3) calcular segredo compartilhado
        byte[] sharedSecret = receiver.DeriveKeyMaterial(ephPubKey.PublicKey);


        // 4) derivar chave simétrica
        byte[] aesKey = DeriveKey(sharedSecret);


        // 5) converter nonce, ciphertext e tag para bytes
        byte[] nonce = Convert.FromBase64String(msg.Nonce);
        byte[] cipherAndTag = Convert.FromBase64String(msg.CipherText);

        // AES-GCM usa 16 bytes no final como tag
        if (cipherAndTag.Length < 16)
            throw new Exception("CipherText muito curto.");


        int cipherLength = cipherAndTag.Length - 16;
        byte[] cipherBytes = new byte[cipherLength];
        byte[] tag = new byte[16];


        Buffer.BlockCopy(cipherAndTag, 0, cipherBytes, 0, cipherLength);
        Buffer.BlockCopy(cipherAndTag, cipherLength, tag, 0, 16);


        // 6) Descriptografar
        byte[] plainBytes = new byte[cipherBytes.Length];
        using (var aes = new AesGcm(aesKey))
        {
            if (nonce.Length != 12)
                throw new Exception($"Invalid Nonce.\nLength: {nonce.Length} bytes.");

            aes.Decrypt(nonce, cipherBytes, tag, plainBytes);
        }


        return Encoding.UTF8.GetString(plainBytes);
    }
}

// --- Classe de suporte para HKDF (RFC 5869) ---
public sealed class Hkdf : IDisposable
{
    private readonly HMAC _hmac;
    private readonly byte[] _prk;
    private int _counter = 1;
    private byte[] _previousBlock = Array.Empty<byte>();

    public Hkdf(HashAlgorithmName hashAlgorithm, byte[] ikm, byte[]? salt, byte[]? info)
    {
        _hmac = new HMACSHA256(salt ?? new byte[32]);
        _prk = _hmac.ComputeHash(ikm);
        _hmac.Key = _prk;
        if (info != null) _previousBlock = info;
    }

    public void Fill(Span<byte> output)
    {
        int filled = 0;
        while (filled < output.Length)
        {
            _hmac.Initialize();
            _hmac.TransformBlock(_previousBlock, 0, _previousBlock.Length, null, 0);
            _hmac.TransformFinalBlock(new[] { (byte)_counter }, 0, 1);

            _previousBlock = _hmac.Hash!;
            _counter++;

            int toCopy = Math.Min(_previousBlock.Length, output.Length - filled);
            _previousBlock.AsSpan(0, toCopy).CopyTo(output.Slice(filled, toCopy));
            filled += toCopy;
        }
    }

    public void Dispose() => _hmac.Dispose();
}