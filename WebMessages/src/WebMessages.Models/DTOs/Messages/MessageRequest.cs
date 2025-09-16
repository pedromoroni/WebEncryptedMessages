using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebMessages.Models.DTOs.Messages;

public class MessageRequest
{
    public Guid FromUserId { get; set; }
    public Guid ToUserId { get; set; }
    public byte[] CipherText { get; set; } // mensagem cifrada
}
