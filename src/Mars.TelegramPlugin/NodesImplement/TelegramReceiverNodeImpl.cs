using Mars.Nodes.Core;
using Mars.Nodes.Host.Shared;
using Mars.TelegramPlugin.Nodes;
using Mars.TelegramPlugin.Services;
using Microsoft.Extensions.Logging;

namespace Mars.TelegramPlugin.NodesImplement;

internal class TelegramReceiverNodeImpl : INodeImplement<TelegramReceiverNode>
{
    private readonly ILogger<TelegramReceiverNodeImpl> _logger;

    public TelegramReceiverNode Node { get; }
    public IRuntimeNodeScope RNS { get; set; }
    Node INodeImplement.Node => Node;

    public TelegramReceiverNodeImpl(TelegramReceiverNode node,
                                    IRuntimeNodeScope rns,
                                    ILogger<TelegramReceiverNodeImpl> logger)
    {
        Node = node;
        RNS = rns;
        Node.Config = RNS.GetConfig(node.Config);
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
