using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebMessages.Models.DTOs.Messages;

public class MessageRequest
{
    public Guid FromDeviceId { get; set; }
    public Guid ToDeviceId { get; set; }

    public byte[] CipherText { get; set; }      // mensagem cifrada
    public byte[] Nonce { get; set; }           // nonce único por mensagem
    public byte[] EphemeralPub { get; set; }    // chave pública efêmera do remetente
}
