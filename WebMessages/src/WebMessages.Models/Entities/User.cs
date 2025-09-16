using System.ComponentModel.DataAnnotations;

namespace WebMessages.Models.Entities;

public class User
{
    [Key]
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLogin { get; set; }
    public bool IsDeleted { get; set; }

    public virtual ICollection<Message> MessagesSent { get; set; } = new List<Message>();
    public virtual ICollection<Message> MessagesReceived { get; set; } = new List<Message>();
    public virtual ICollection<Device> Devices { get; set; } = new List<Device>();
}