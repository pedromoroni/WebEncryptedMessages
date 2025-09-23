using WebMessages.Models.DTOs.Devices;
using WebMessages.Models.DTOs.Messages;
using WebMessages.Models.Entities;

namespace WebMessages.Models.DTOs.Users;

public class UserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public virtual ICollection<DeviceDTO> Devices { get; set; } = new List<DeviceDTO>();
}
