namespace WebMessages.WEB.Models;

public class CreateUserRequest
{
    public User Credentials { get; set; } = new();
    public string ConfirmPassword { get; set; } = string.Empty;
}
