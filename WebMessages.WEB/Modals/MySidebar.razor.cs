using Microsoft.AspNetCore.Components;
using System.Net.Http;
using WebMessages.WEB.Models;
using WebMessages.WEB.Models.Users;

namespace WebMessages.WEB.Modals;

public partial class MySidebar
{
    [Parameter]
    public List<MySidebarItem> Contacts { get; set; } = new List<MySidebarItem>();

    private string UsernameSearch{ get; set; } = string.Empty;
    private CancellationTokenSource? _cts;
    private HttpClient httpClient = new HttpClient();

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
        // cancela qualquer busca anterior
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        // evita busca se o texto estiver vazio
        if (string.IsNullOrWhiteSpace(UsernameSearch))
        {
            Contacts.Clear(); // fazer com que pegue os contactos originais
            return;
        }

        try
        {
            await Task.Delay(300, token);

            var url = $"Users/search?username={Uri.EscapeDataString(UsernameSearch)}";
            var response = await httpClient.GetAsync(url, token);

            response.EnsureSuccessStatusCode();

            if (!token.IsCancellationRequested && response is not null)
            {
                var contacts = await response.Content.ReadFromJsonAsync<List<MySidebarItem>>();
                Contacts = contacts ?? new List<MySidebarItem>();
                StateHasChanged();
            }
        }
        catch (TaskCanceledException)
        {
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro na busca: {ex.Message}");
        }
    }

}
