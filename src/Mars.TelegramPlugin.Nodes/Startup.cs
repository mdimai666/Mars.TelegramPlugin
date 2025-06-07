using Mars.Nodes.Core;
using Mars.Plugin.Front.Abstractions;
using Mars.TelegramPlugin.Nodes.Forms;
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

        NodesLocator.RegisterAssembly(typeof(TelegramSenderNode).Assembly);
        NodeFormsLocator.RegisterAssembly(typeof(TelegramSenderNodeForm).Assembly);
    }

    public void ConfigureApplication(WebAssemblyHost app)
    {
#if DEBUG
        Console.WriteLine("> plugin ConfigureApplication!" + Locale.Username);
#endif
    }
}

