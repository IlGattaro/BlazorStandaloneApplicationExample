using BlazorStandaloneApplicationExample;
using BlazorStandaloneApplicationExample.Components;

// 1. ensure unique execution
if (WindowsApplicationServer.CheckForAnotherInstance())
    Environment.Exit(0);

var builder = WebApplication.CreateBuilder(args);

// normal services registration
builder.Services.RegisterApplicationServices();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// 2. configure as application standalone
app.ConfigureAsStandaloneApplication("Blazor Standalone Application Example!");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    //// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts();
}

// I left it commented to show how it originally looked!

// REPLACED!
//var runApp = app.RunAsync();

// 3. use this method to run
app.RunApplication();

// REPLACED!
//await runApp;