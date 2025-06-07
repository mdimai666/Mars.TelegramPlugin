using Mars.Host.Shared.Services;
using Mars.Nodes.Core;
using Mars.Nodes.Core.Implements;
using Mars.Plugin.Abstractions;
using Mars.Plugin.PluginHost;
using Mars.TelegramPlugin;
using Mars.TelegramPlugin.Nodes;
using Mars.TelegramPlugin.Nodes.Forms;
using Mars.TelegramPlugin.NodesImplement;
using Mars.TelegramPlugin.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

[assembly: WebApplicationPlugin(typeof(MainMarsTelegramPlugin))]

namespace Mars.TelegramPlugin;

public class MainMarsTelegramPlugin : WebApplicationPlugin
{
    public const string PluginPackageName = "mdimai666.Mars.TelegramPlugin";

    public override void ConfigureWebApplicationBuilder(WebApplicationBuilder builder, PluginSettings settings)
    {
        builder.Services.AddSingleton<TelegramManager>();

    }

    public override void ConfigureWebApplication(WebApplication app, PluginSettings settings)
    {
        NodesLocator.RegisterAssembly(typeof(TelegramSenderNode).Assembly);
        NodeFormsLocator.RegisterAssembly(typeof(TelegramSenderNodeForm).Assembly);
        NodeImplementFabirc.RegisterAssembly(typeof(TelegramSenderNodeImpl).Assembly);

        var logger = MarsLogger.GetStaticLogger<MainMarsTelegramPlugin>();
        //logger.LogWarning($"> {PluginPackageName} - Work!!!!2" + Locale.Username);

        var op = app.Services.GetRequiredService<IOptionService>();

#if DEBUG
        app.UseDevelopingServePluginFilesDefinition(this.GetType().Assembly, settings, [typeof(TelegramPluginFront).Assembly]);
#endif

        //op.RegisterOption<Example1Plugin1>(appendToInitialSiteData: true);
        //op.SetConstOption(new Example1PluginConstOptionForFront() { ForFrontValue = "123" }, appendToInitialSiteData: true);
    }

}
