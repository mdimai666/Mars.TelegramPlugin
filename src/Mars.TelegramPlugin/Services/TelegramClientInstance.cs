using Mars.TelegramPlugin.Nodes;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace Mars.TelegramPlugin.Services;

internal class TelegramClientInstance : IDisposable
{
    private readonly TelegramManager _telegramManager;
    private readonly ILogger<TelegramClientInstance> _logger;

    public TelegramBotClient Bot { get; }
    public CancellationTokenSource CancellationTokenSource { get; init; }
    public DateTime CreatedAt { get; } = new();
    public TelegramConfigNode ConfigNode { get; private set; }

    public TelegramClientInstance(TelegramConfigNode configNode, TelegramManager telegramManager, ILogger<TelegramClientInstance> logger)
    {
        CancellationTokenSource = new();
        ConfigNode = configNode;
        Bot = new TelegramBotClient(ConfigNode.Token, cancellationToken: CancellationTokenSource.Token);
        _telegramManager = telegramManager;
        _logger = logger;

        Bot.OnMessage += Bot_OnMessage;
        Bot.OnError += Bot_OnError;
    }

    internal void UpdateConfig(TelegramConfigNode configNode) => ConfigNode = configNode;

    private Task Bot_OnMessage(Message message, Telegram.Bot.Types.Enums.UpdateType type)
    {
        _telegramManager.OnReciveMessage(this, message, type);
        return Task.CompletedTask;
    }

    private Task Bot_OnError(Exception exception, HandleErrorSource source)
    {
        _logger.LogError(exception, "Telegram receive error for config '{ConfigName}', source: {Source}", ConfigNode.Name, source);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Bot.OnMessage -= Bot_OnMessage;
        Bot.OnError -= Bot_OnError;
        CancellationTokenSource.Cancel();
    }
}
