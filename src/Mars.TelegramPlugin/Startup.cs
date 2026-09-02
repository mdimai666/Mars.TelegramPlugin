using Mars.Options.Abstractions.Services;
using Mars.Plugin.Abstractions;
using Mars.Plugin.Kit.Host;
using Mars.Server.Abstractions.Services;
using Mars.TelegramPlugin;
using Mars.TelegramPlugin.Nodes;
using Mars.TelegramPlugin.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

[assembly: MarsPlugin(typeof(MainMarsTelegramPlugin))]

namespace Mars.TelegramPlugin;

public class MainMarsTelegramPlugin : MarsPlugin
{
    public const string PluginPackageName = "mdimai666.Mars.TelegramPlugin";

    public override void ConfigureWebApplicationBuilder(WebApplicationBuilder builder, PluginSettings settings)
    {
        builder.Services.AddSingleton<TelegramManager>();

    }

    public override void ConfigureWebApplication(WebApplication app, PluginSettings settings)
    {
        app.Services.AutoHostRegisterHelper([GetType().Assembly, typeof(TelegramSenderNode).Assembly]);

        var logger = MarsLogger.GetStaticLogger<MainMarsTelegramPlugin>();
        //logger.LogWarning($"> {PluginPackageName} - Work!!!!2" + Locale.Username);

        var op = app.Services.GetRequiredService<IOptionService>();

        //op.RegisterOption<Example1Plugin1>(appendToInitialSiteData: true);
        //op.SetConstOption(new Example1PluginConstOptionForFront() { ForFrontValue = "123" }, appendToInitialSiteData: true);
    }

}
