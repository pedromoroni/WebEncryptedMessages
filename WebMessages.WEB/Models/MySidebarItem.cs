using WebMessages.WEB.Models.Users;

namespace WebMessages.WEB.Models;

public class MySidebarItem
{
    public User User { get; set; }
    public List<Message> Messages { get; set; }
}
