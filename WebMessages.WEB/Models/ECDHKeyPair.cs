using Microsoft.JSInterop;
using System;
using System.Security.Cryptography;

namespace WebMessages.WEB.Models;

public class ECDHKeyPair
{
    public byte[] PublicKey { get; set; }
    public byte[] PrivateKey { get; set; }
}
