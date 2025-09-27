using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using WebMessages.WEB.Helpers;
using WebMessages.WEB.Models;
using WebMessages.WEB.Models.Messages;
using WebMessages.WEB.Models.Users;
using WebMessages.WEB.Services;

namespace WebMessages.WEB.Modals;

public partial class MySidebar
{
    [Inject] private MySidebarService MySidebarService { get; set; } = default!;
    [Inject] private HttpClient HttpClient { get; set; } = default!;

    private string UsernameSearch { get; set; } = string.Empty;
    private CancellationTokenSource? _cts;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await Task.Delay(10);
        StateHasChanged();
    }

    private async Task OnInputChanged(ChangeEventArgs e)
    {
        UsernameSearch = e.Value?.ToString() ?? string.Empty;
        await SearchUsersAsync();
    }

    private async Task SearchUsersAsync()
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        if (string.IsNullOrWhiteSpace(UsernameSearch))
        {
            MySidebarService.Contacts = new List<MySidebarItem>();

            var user = await AuthenticateUserAsync(Helper.LoggedDevice);

            if (user == null || user.Username != Helper.LoggedUser.Username)
                throw new Exception("Error authentication the User.");

            Helper.LoggedDevice = user.Devices
                .FirstOrDefault(c => c.PublicKey.SequenceEqual(Helper.LoggedDevice.PublicKey));

            var messagesReceived = user.Devices.SelectMany(d => d.MessagesReceived).ToList();
            var messagesSent = user.Devices.SelectMany(d => d.MessagesSent).ToList();

            await UpdateContacts(messagesReceived, messagesSent);
            return;
        }

        try
        {
            await Task.Delay(300, token);

            var url = $"Users/search?username={Uri.EscapeDataString(UsernameSearch)}";
            var response = await HttpClient.GetAsync(url, token);

            response.EnsureSuccessStatusCode();

            if (!token.IsCancellationRequested && response is not null)
            {
                var users = await response.Content.ReadFromJsonAsync<List<User>>();

                MySidebarService.Contacts = users?
                    .Select(u => new MySidebarItem { User = u, Messages = new List<Message>() }) // To Do: adicionar a primeira mensagem e melhorar o design
                    .ToList() ?? new List<MySidebarItem>();

            }
        }
        catch (TaskCanceledException)
        {
            // ignorar cancelamento
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro na busca: {ex.Message}");
        }
    }

    private void OnContactClick(MySidebarItem mySidebarItem)
    {
        MySidebarService.SelectedContact = mySidebarItem;
        // fazer com que quando seleciona, apagar a lista de mensagens e adicionar as 10 primeiras
    }

    private async Task<User?> AuthenticateUserAsync(Device device)
    {
        var userDevice = new UserDevice
        {
            User = Helper.LoggedUserCredentials,
            Device = device
        };

        var queryInfo = new QueryInfo
        {
            PageNumber = 1,
            PageSize = 1
        };

        var json = JsonSerializer.Serialize(userDevice);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await HttpClient.PostAsync(
            $"Users/login?pageSize={queryInfo.PageSize}&pageNumber={queryInfo.PageNumber}",
            content);

        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<User>(responseBody, options);
    }

    private async Task UpdateContacts(List<Message> messagesReceived, List<Message> messagesSent)
    {
        try
        {
            foreach (var message in messagesReceived)
            {
                await ProcessMessageAsync(message, decrypt: true);
            }

            foreach (var message in messagesSent)
            {
                await ProcessMessageAsync(message, decrypt: true);
            }

            MySidebarService.ShowSidebar = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        StateHasChanged();
    }

    private async Task ProcessMessageAsync(Message message, bool decrypt)
    {
        var user = await GetUserByDeviceIdAsync(message.ToDeviceId);

        if (decrypt)
        {
            var keyPair = await KeyStorageService.GetKeysAsync(Helper.LoggedUser.Username);
            message.Decrypted = E2E.Decrypt(message, keyPair.PrivateKey);
        }

        var contact = MySidebarService.Contacts.FirstOrDefault(c => c.User.Id == user.Id);
        if (contact != null)
        {
            contact.Messages.Add(message);
        }
        else
        {
            MySidebarService.Contacts.Add(new MySidebarItem
            {
                User = user,
                Messages = new List<Message> { message }
            });
        }
    }
    private async Task<User> GetUserByDeviceIdAsync(Guid deviceId)
    {
        var url = $"Users/getUserByDeviceId?deviceId={deviceId}";
        var response = await HttpClient.GetAsync(url) ?? throw new Exception("Erro getting response");

        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<User>(responseBody, options);
    }
}
