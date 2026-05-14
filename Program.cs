using AndroideIOT.Components;
using AndroideIOT.Configuration;
using AndroideIOT.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.Configure<ThingSpeakSettings>(
    builder.Configuration.GetSection("ThingSpeak"));

builder.Services.Configure<AiSettings>(
    builder.Configuration.GetSection("Anthropic"));

builder.Services.AddHttpClient<IThingSpeakService, ThingSpeakService>();
builder.Services.AddHttpClient<IAIService, AnthropicAIService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
