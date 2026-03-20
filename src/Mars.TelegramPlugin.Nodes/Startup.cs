using Mars.Plugin.Front;
using Mars.Plugin.Front.Abstractions;
using Mars.TelegramPlugin.Shared.Resources;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Mars.TelegramPlugin.Nodes;

public class TelegramPluginFront : IWebAssemblyPluginFront
{
    public void ConfigureServices(WebAssemblyHostBuilder builder)
    {
#if DEBUG
        Console.WriteLine("> plugin ConfigureServices!");
#endif
    }

    public void ConfigureApplication(WebAssemblyHost app)
    {
        app.Services.AutoFrontRegisterHelper([GetType().Assembly]);
#if DEBUG
        Console.WriteLine("> plugin ConfigureApplication!" + Locale.Username);
#endif
    }
}
