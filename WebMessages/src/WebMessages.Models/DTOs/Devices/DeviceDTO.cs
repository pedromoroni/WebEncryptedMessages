using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebMessages.Models.DTOs.Messages;
using WebMessages.Models.Entities;

namespace WebMessages.Models.DTOs.Devices;

public class DeviceDTO
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public byte[] PublicKey { get; set; }
    public DateTimeOffset? LastSeen { get; set; }
    public bool IsActive { get; set; }

    public virtual ICollection<MessageDTO> MessagesSent { get; set; } = new List<MessageDTO>();
    public virtual ICollection<MessageDTO> MessagesReceived { get; set; } = new List<MessageDTO>();
}
