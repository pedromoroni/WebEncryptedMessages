using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMessages.Models.Entities;

namespace WebMessages.Models.DTOs.Messages;

public class MessageDTO
{
    public Guid FromDeviceId { get; set; }
    public Guid ToDeviceId { get; set; }

    public byte[] CipherText { get; set; } // mensagem cifrada
    public byte[] Nonce { get; set; } // nonce unico por mensagem
    public byte[] EphemeralPub { get; set; } // chave public efemera do remetente
    public MessageStatus Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? DeliveredAt { get; set; }
    public DateTimeOffset? ReadAt { get; set; }
}
