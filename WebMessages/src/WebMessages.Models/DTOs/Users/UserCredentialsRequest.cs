namespace WebMessages.Models.DTOs.Users;

public class UserCredentialsRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
// criar o isvalid