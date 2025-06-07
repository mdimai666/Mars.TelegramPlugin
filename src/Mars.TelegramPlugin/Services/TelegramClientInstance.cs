using Mars.TelegramPlugin.Nodes;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Mars.TelegramPlugin.Services;

internal class TelegramClientInstance : IDisposable
{
    private readonly TelegramManager _telegramManager;

    public TelegramBotClient Bot { get; }
    public CancellationTokenSource CancellationTokenSource { get; init; }
    public DateTime CreatedAt { get; } = new();
    public TelegramConfigNode ConfigNode { get; private set; }

    public TelegramClientInstance(TelegramConfigNode configNode, TelegramManager telegramManager)
    {
        CancellationTokenSource = new();
        ConfigNode = configNode;
        Bot = new TelegramBotClient(ConfigNode.Token, cancellationToken: CancellationTokenSource.Token);
        _telegramManager = telegramManager;

        Bot.OnMessage += Bot_OnMessage;
        Bot.OnMakingApiRequest += Bot_OnMakingApiRequest;
        Bot.OnApiResponseReceived += Bot_OnApiResponseReceived;
    }

    private ValueTask Bot_OnApiResponseReceived(ITelegramBotClient botClient, Telegram.Bot.Args.ApiResponseEventArgs args, CancellationToken cancellationToken = default)
    {
#if DEBUG
        Console.WriteLine("Bot_OnApiResponseReceived");
#endif
        return ValueTask.CompletedTask;
    }

    private ValueTask Bot_OnMakingApiRequest(ITelegramBotClient botClient, Telegram.Bot.Args.ApiRequestEventArgs args, CancellationToken cancellationToken = default)
    {
#if DEBUG
        Console.WriteLine("Bot_OnMakingApiRequest");
#endif
        return ValueTask.CompletedTask;
    }

    private Task Bot_OnMessage(Message message, Telegram.Bot.Types.Enums.UpdateType type)
    {
        _telegramManager.OnReciveMessage(this, message, type);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Bot.OnMessage -= Bot_OnMessage;
        CancellationTokenSource.Cancel();
    }
}
