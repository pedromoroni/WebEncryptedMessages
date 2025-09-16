using WebMessages.Models;

namespace WebMessages.Models.DTOs.Users;

public class CreateUserRequest
{
    public UserCredentialsRequest Credentials { get; set; } = new();
    public string ConfirmPassword { get; set; } = string.Empty;
}
