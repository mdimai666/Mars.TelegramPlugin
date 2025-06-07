using Mars.Core.Extensions;
using Mars.Nodes.Core;
using Mars.Nodes.Core.Implements;
using Mars.TelegramPlugin.Nodes;
using Mars.TelegramPlugin.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Mars.TelegramPlugin.NodesImplement;

public class TelegramSenderNodeImpl : INodeImplement<TelegramSenderNode>, INodeImplement
{
    private readonly ILogger<TelegramSenderNodeImpl> _logger;

    public TelegramSenderNode Node { get; }
    public IRED RED { get; set; }
    Node INodeImplement<Node>.Node => Node;

    public TelegramSenderNodeImpl(TelegramSenderNode node, IRED red)
    {
        Node = node;
        RED = red;

        Node.Config = RED.GetConfig(node.Config);
        _logger = RED.ServiceProvider.GetRequiredService<ILogger<TelegramSenderNodeImpl>>();
    }

    public async Task Execute(NodeMsg input, ExecuteAction callback)
    {
        try
        {
            //_logger.LogTrace("Execute");

            var bm = RED.ServiceProvider.GetRequiredService<TelegramManager>();
            var bot = bm.GetBot(Node.Config.Value);

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
            RED.DebugMsg(new DebugMessage { NodeId = Node.Id, Message = ex.Message, Level = Mars.Core.Models.MessageIntent.Error });
        }
    }
}
