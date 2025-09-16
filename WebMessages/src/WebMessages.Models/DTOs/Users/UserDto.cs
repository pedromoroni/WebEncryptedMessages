using WebMessages.Models.Entities;

namespace WebMessages.Models.DTOs.Users;

public class UserDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public virtual ICollection<Message> MessagesSent { get; set; } = new List<Message>();
    public virtual ICollection<Message> MessagesReceived { get; set; } = new List<Message>();
    public virtual ICollection<Device> Devices { get; set; } = new List<Device>();
}
