using Microsoft.JSInterop;
using System.Security.Cryptography;
using WebMessages.WEB.Models;

namespace WebMessages.WEB.Services;

public class KeyStorageService
{
    private readonly IJSRuntime _jsRuntime;

    public KeyStorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task SaveKeyAsync(string username, string keyName, byte[] value)
    {
        string storageKey = $"{username}_{keyName}";
        string base64Value = Convert.ToBase64String(value);
        await _jsRuntime.InvokeVoidAsync("idb.setItem", "Keys", storageKey, base64Value);
    }


    private async Task<byte[]> GetKeyAsync(string username, string keyName)
    {
        string storageKey = $"{username}_{keyName}";
        string base64Key = await _jsRuntime.InvokeAsync<string>("idb.getItem", "Keys", storageKey);

        if (string.IsNullOrEmpty(base64Key))
            return Array.Empty<byte>();

        return Convert.FromBase64String(base64Key);
    }



    public static ECDHKeyPair GenerateECDHKeys()
    {
        using var ecdh = ECDiffieHellman.Create(ECCurve.NamedCurves.nistP256);

        return new ECDHKeyPair
        {
            // Exporta a chave pública em formato padrão (SubjectPublicKeyInfo)
            PublicKey = ecdh.PublicKey.ExportSubjectPublicKeyInfo(),
            // Exporta a chave privada em formato PKCS#8
            PrivateKey = ecdh.ExportPkcs8PrivateKey()
        };
    }


    public async Task<ECDHKeyPair> GetKeysAsync(string username)
    {
        var privateKey = await GetKeyAsync(username, "PrivateKey");
        var publicKey = await GetKeyAsync(username, "PublicKey");

        if (privateKey.Length == 0 || publicKey.Length == 0)
        {
            var keys = GenerateECDHKeys();
            await SaveKeyAsync(username, "PrivateKey", keys.PrivateKey);
            await SaveKeyAsync(username, "PublicKey", keys.PublicKey);
            return keys;
        }

        return new ECDHKeyPair
        {
            PrivateKey = privateKey,
            PublicKey = publicKey
        };
    }
}
