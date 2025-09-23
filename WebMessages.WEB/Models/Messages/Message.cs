using System.Text.Json.Serialization;

namespace WebMessages.WEB.Models.Messages;

public class Message
{
    public Guid FromDeviceId { get; set; }
    public Guid ToDeviceId { get; set; }

    public string CipherText { get; set; }      // mensagem cifrada
    public string Nonce { get; set; }           // nonce unico por mensagem
    public string EphemeralPub { get; set; }    // chave publica efemera do remetente
    public MessageStatus Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? DeliveredAt { get; set; }
    public DateTimeOffset? ReadAt { get; set; }

    [JsonIgnore]
    public string Decrypted { get; set; } = string.Empty;
}

public enum MessageStatus : byte
{
    Pending = 0,
    Delivered = 1,
    Read = 2
}
