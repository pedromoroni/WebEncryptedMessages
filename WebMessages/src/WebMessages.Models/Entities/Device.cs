using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebMessages.Models.Entities;

public class Device
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid UserId { get; set; }

    public string? Name { get; set; }
    public byte[] PublicKey { get; set; }
    public DateTimeOffset? LastSeen { get; set; }
    public bool IsActive { get; set; }

    [ForeignKey("UserId")]
    public virtual User? User { get; set; }

    public virtual ICollection<Message> MessagesSent { get; set; } = new List<Message>();
    public virtual ICollection<Message> MessagesReceived { get; set; } = new List<Message>();
}