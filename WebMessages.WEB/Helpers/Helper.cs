using System.Text;
using WebMessages.WEB.Models;
using WebMessages.WEB.Models.Users;
using WebMessages.WEB.Services;
using static System.Net.Mime.MediaTypeNames;

namespace WebMessages.WEB.Helpers;

public static class Helper
{
    public static User? LoggedUser { get; set; } = new User();
    public static Device? LoggedDevice { get; set; } = new Device();
    public static KeyStorageService KeyStorageService { get; set; }
}
