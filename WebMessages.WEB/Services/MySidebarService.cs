using WebMessages.WEB.Models;

namespace WebMessages.WEB.Services;

public class MySidebarService
{
    private bool _showSidebar = false;
    private List<MySidebarItem> _contacts = new();

    public List<MySidebarItem> Contacts
    {
        get => _contacts;
        set
        {
            _contacts = value;
            NotifyStateChanged();
        }
    }

    public bool ShowSidebar
    {
        get => _showSidebar;
        set
        {
            _showSidebar = value;
            NotifyStateChanged();
        }
    }

    public event Func<Task>? OnChange;

    private void NotifyStateChanged()
    {
        if (OnChange != null)
        {
            _ = OnChange.Invoke(); // chama sem esperar, pois é evento
        }
    }
}
