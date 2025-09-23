using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebMessages.Models.Entities;

public class Message
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid FromDeviceId { get; set; }
    [Required]
    public Guid ToDeviceId { get; set; }

    public bool Received { get; set; } // vai servir para saber se deve ser enviada na lista de mensagens recebidas do destinatario ou nas mensagens enviadas do rememtente
    public byte[] CipherText { get; set; } // mensagem cifrada
    public byte[] Nonce { get; set; } // nonce unico por mensagem
    public byte[] EphemeralPub { get; set; } // chave public efemera do remetente
    public MessageStatus Status { get; set; } 
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? DeliveredAt { get; set; }
    public DateTimeOffset? ReadAt { get; set; }

    [ForeignKey("FromDeviceId")]
    public Device? FromDevice { get; set; }
    [ForeignKey("ToUserId")]
    public Device? ToDevice { get; set; }
}

public enum MessageStatus : byte
{
    Pending = 0,
    Delivered = 1,
    Read = 2
}

