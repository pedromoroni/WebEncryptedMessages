using WebMessages.WEB.Components;
using WebMessages.WEB.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<KeyStorageService>();

builder.Services.AddScoped(sp =>
{
    var handler = new HttpClientHandler
    {
        // Ignora certificados inválidos apenas para dev
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };

    var client = new HttpClient(handler)
    {
        BaseAddress = new Uri(builder.Configuration.GetSection("WebMessagesAPI:Endpoint").Value ?? string.Empty),
        Timeout = TimeSpan.FromMinutes(2) // aumenta timeout só para testes dev
    };

    return client;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
