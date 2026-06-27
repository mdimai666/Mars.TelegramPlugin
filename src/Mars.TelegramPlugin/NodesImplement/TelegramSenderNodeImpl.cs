using Mars.Core.Extensions;
using Mars.Nodes.Core;
using Mars.Nodes.Host.Shared;
using Mars.TelegramPlugin.Nodes;
using Mars.TelegramPlugin.Services;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Mars.TelegramPlugin.NodesImplement;

internal class TelegramSenderNodeImpl : INodeImplement<TelegramSenderNode>
{
    private readonly ILogger<TelegramSenderNodeImpl> _logger;
    private readonly TelegramManager _telegramManager;

    public TelegramSenderNode Node { get; }
    public IRuntimeNodeScope RNS { get; set; }
    Node INodeImplement.Node => Node;

    public TelegramSenderNodeImpl(TelegramSenderNode node,
                                    IRuntimeNodeScope rns,
                                    TelegramManager telegramManager,
                                    ILogger<TelegramSenderNodeImpl> logger)
    {
        Node = node;
        RNS = rns;
        Node.Config = RNS.GetConfig(node.Config);
        _telegramManager = telegramManager;
        _logger = logger;
    }

    public async Task Execute(NodeMsg input, ExecuteAction callback, ExecutionParameters parameters)
    {
        try
        {
            //_logger.LogTrace("Execute");

            var bot = _telegramManager.GetBot(Node.Config.Value);

            ArgumentNullException.ThrowIfNull(bot, nameof(bot));

            var chat = new ChatId(Node.ChatId);
            //_logger.LogTrace($"chat: Username={chat.Username}, Identifier={chat.Identifier}");
            var message = input.Payload.ToString()!;

            var result = await bot.SendMessage(chat, message);
            _logger.LogTrace($"username: {Node.ChatId}, sended: {result.Text?.TextEllipsis(30)}");

            input.Payload = result;
            callback(input);
            //_logger.LogTrace("end");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            RNS.DebugMsg(new DebugMessage { NodeId = Node.Id, Message = ex.Message, Level = Mars.Core.Models.MessageIntent.Error });
        }
    }
}
