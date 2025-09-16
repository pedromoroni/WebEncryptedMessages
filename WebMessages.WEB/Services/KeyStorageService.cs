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

    public async Task SaveKeyAsync(string keyName, string value)
    {
        await _jsRuntime.InvokeVoidAsync("idb.setItem", "Keys", keyName, value);
    }

    public async Task<string> GetKeyAsync(string keyName)
    {
        return await _jsRuntime.InvokeAsync<string>("idb.getItem", "Keys", keyName);
    }

    public RSAKeyPair GenerateRSAKeys()
    {
        using (var rsa = RSA.Create(2048))
        {
            return new RSAKeyPair
            {
                PublicKey = Convert.ToBase64String(rsa.ExportSubjectPublicKeyInfo()),
                PrivateKey = Convert.ToBase64String(rsa.ExportPkcs8PrivateKey())
            };
        }
    }

    public async Task<RSAKeyPair> GetKeysAsync()
    {
        var privateKey = await GetKeyAsync("PrivateKey");
        var publicKey = await GetKeyAsync("PublicKey");

        if (string.IsNullOrEmpty(privateKey) || string.IsNullOrEmpty(publicKey))
        {
            var keys = GenerateRSAKeys();
            await SaveKeyAsync("PrivateKey", keys.PrivateKey);
            await SaveKeyAsync("PublicKey", keys.PublicKey);
            return keys;
        }

        return new RSAKeyPair { PrivateKey = privateKey, PublicKey = publicKey };
    }
}