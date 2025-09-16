using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebMessages.Models.Entities;

public class Message
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid FromUserId { get; set; }
    [Required]
    public Guid ToUserId { get; set; }

    public byte[] CipherText { get; set; } // mensagem cifrada
    public byte[] Nonce { get; set; } // nonce unico por mensagem
    public byte[] EphemeralPub { get; set; } // chave public efemera do remetente
    public MessageStatus Status { get; set; } 
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? DeliveredAt { get; set; }
    public DateTimeOffset? ReadAt { get; set; }

    [ForeignKey("FromUserId")]
    public User? FromUser { get; set; }
    [ForeignKey("ToUserId")]
    public User? ToUser { get; set; }

}

public enum MessageStatus : byte
{
    Pending = 0,
    Delivered = 1,
    Read = 2
}

