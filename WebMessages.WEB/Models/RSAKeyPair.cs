using Microsoft.JSInterop;
using System;
using System.Security.Cryptography;

namespace WebMessages.WEB.Models;

public class RSAKeyPair
{
    public string PublicKey { get; set; }
    public string PrivateKey { get; set; }
}
