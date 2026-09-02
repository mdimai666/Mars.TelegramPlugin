using Mars.Plugin.Front;
using Mars.Plugin.Front.Abstractions;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Mars.TelegramPlugin.Nodes;

public class TelegramPluginFront : IWebAssemblyPluginFront
{
    public void ConfigureServices(WebAssemblyHostBuilder builder)
    {
    }

    public void ConfigureApplication(WebAssemblyHost app)
    {
        app.Services.AutoFrontRegisterHelper([GetType().Assembly]);
    }
}
