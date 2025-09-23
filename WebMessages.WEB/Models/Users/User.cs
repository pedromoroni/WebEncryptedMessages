using System.Text.Json.Serialization;

namespace WebMessages.WEB.Models.Users;

public class User
{
    public Guid Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public virtual ICollection<Device> Devices { get; set; } = new List<Device>();
}
