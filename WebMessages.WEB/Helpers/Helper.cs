using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using WebMessages.WEB.Models;
using WebMessages.WEB.Models.Messages;
using WebMessages.WEB.Models.Users;
using WebMessages.WEB.Services;
using static System.Net.Mime.MediaTypeNames;

namespace WebMessages.WEB.Helpers;

public static class Helper
{
    public static User? LoggedUser { get; set; } = new User();
    public static UserCredentialsRequest LoggedUserCredentials { get; set; } = new UserCredentialsRequest();
    public static Device? LoggedDevice { get; set; } = new Device();
}
