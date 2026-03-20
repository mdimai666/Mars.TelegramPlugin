using Mars.Nodes.Core;
using Mars.Nodes.Core.Implements;
using Mars.TelegramPlugin.Nodes;
using Mars.TelegramPlugin.Services;
using Microsoft.Extensions.Logging;

namespace Mars.TelegramPlugin.NodesImplement;

internal class TelegramReceiverNodeImpl : INodeImplement<TelegramReceiverNode>, INodeImplement
{
    private readonly ILogger<TelegramReceiverNodeImpl> _logger;

    public TelegramReceiverNode Node { get; }
    public IRED RED { get; set; }
    Node INodeImplement<Node>.Node => Node;

    public TelegramReceiverNodeImpl(TelegramReceiverNode node, IRED red, ILogger<TelegramReceiverNodeImpl> logger)
    {
        Node = node;
        RED = red;
        Node.Config = RED.GetConfig(node.Config);
        _logger = logger;
    }

    public Task Execute(NodeMsg input, ExecuteAction callback, ExecutionParameters parameters)
    {
        _ = nameof(TelegramManager.OnReciveMessage);

        _logger.LogTrace("Execute");

        callback(input);

        return Task.CompletedTask;
    }
}
