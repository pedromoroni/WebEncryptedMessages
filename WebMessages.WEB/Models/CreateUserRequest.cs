using WebMessages.WEB.Models.Users;

namespace WebMessages.WEB.Models;

public class CreateUserRequest
{
    public UserRequest Credentials { get; set; } = new();
    public string ConfirmPassword { get; set; } = string.Empty;
}
