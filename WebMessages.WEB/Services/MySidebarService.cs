using WebMessages.WEB.Models;

namespace WebMessages.WEB.Services;

public class MySidebarService
{
    private bool _showSidebar = false;
    private List<MySidebarItem> _contacts = new();
    private MySidebarItem? _selectedContact = null;

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
    public MySidebarItem? SelectedContact
    {
        get => _selectedContact;
        set
        {
            _selectedContact = value;
            NotifyStateChanged();
        }
    }

    public event Func<Task>? OnChange;

    public async Task NotifyStateChanged()
    {
        if (OnChange != null)
        {
            await OnChange.Invoke();
        }
    }
}
