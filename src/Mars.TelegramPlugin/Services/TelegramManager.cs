using Mars.Core.Extensions;
using Mars.Nodes.Abstractions.Services;
using Mars.Nodes.Core;
using Mars.Server.Abstractions.Startup;
using Mars.TelegramPlugin.Nodes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Mars.TelegramPlugin.Services;

internal class TelegramManager : IMarsAppLifetimeService
{
    private readonly Dictionary<string, TelegramClientInstance> _clientInstances = [];
    private readonly object _sync = new();
    private readonly INodeService _nodeService;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<TelegramManager> _logger;
    private readonly ILogger<TelegramClientInstance> _instanceLogger;
    private Dictionary<string, string[]> _recepientsConfigIdAndNodeIds = [];

    public TelegramManager(INodeService nodeService, IServiceScopeFactory scopeFactory, ILoggerFactory loggerFactory, ILogger<TelegramManager> logger)
    {
        _nodeService = nodeService;
        _scopeFactory = scopeFactory;
        _logger = logger;
        _instanceLogger = loggerFactory.CreateLogger<TelegramClientInstance>();
        _nodeService.OnAssignNodes += _nodeService_OnAssignNodes;
    }

    private void _nodeService_OnAssignNodes()
    {
        var configNodes = _nodeService.BaseNodes.Values.OfType<TelegramConfigNode>().ToArray();
        RefreshConfigs(configNodes);
        UpdateRecepientsDict();
    }

    public void RefreshConfigs(TelegramConfigNode[] configs)
    {
        _logger.LogTrace("RefreshConfigs, configs: {Count}", configs.Length);

        var configIds = configs.Select(c => c.Id).ToHashSet();
        var toDispose = new List<TelegramClientInstance>();

        lock (_sync)
        {
            // конфиги, которых больше нет в схеме, — освобождаем их клиентов
            foreach (var instance in _clientInstances.Values.Where(v => !configIds.Contains(v.ConfigNode.Id)).ToArray())
            {
                _clientInstances.Remove(instance.ConfigNode.Token);
                toDispose.Add(instance);
            }

            foreach (var config in configs)
            {
                // у конфига ещё нет токена — клиента быть не должно
                if (string.IsNullOrWhiteSpace(config.Token))
                {
                    if (_clientInstances.Values.FirstOrDefault(v => v.ConfigNode.Id == config.Id) is { } empty)
                    {
                        _clientInstances.Remove(empty.ConfigNode.Token);
                        toDispose.Add(empty);
                    }
                    continue;
                }

                if (_clientInstances.TryGetValue(config.Token, out var instance))
                {
                    instance.UpdateConfig(config);
                }
                else
                {
                    // токен сменился у того же конфига — пересоздаём клиента
                    if (_clientInstances.Values.FirstOrDefault(v => v.ConfigNode.Id == config.Id) is { } replaced)
                    {
                        _clientInstances.Remove(replaced.ConfigNode.Token);
                        toDispose.Add(replaced);
                    }

                    _clientInstances.Add(config.Token, new TelegramClientInstance(config, this, _instanceLogger));
                }
            }
        }

        foreach (var instance in toDispose)
            instance.Dispose();
    }

    void UpdateRecepientsDict()
    {
        var nodes = _nodeService.BaseNodes.Values
            .Where(node => !node.Disabled && node is TelegramReceiverNode tg && tg.Config.Value != null)
            .Select(node => (TelegramReceiverNode)node)
            .ToArray();

        lock (_sync)
        {
            _recepientsConfigIdAndNodeIds = nodes
                .GroupBy(s => s.Config.Id)
                .ToDictionary(s => s.Key, s => s.Select(node => node.Id).ToArray());
        }
    }

    public TelegramBotClient? GetBot(TelegramConfigNode? config)
    {
        if (config == null)
            return null;

        lock (_sync)
            return _clientInstances.GetValueOrDefault(config.Token)?.Bot;
    }

    internal void OnReciveMessage(TelegramClientInstance instance, Message message, Telegram.Bot.Types.Enums.UpdateType type)
    {
        _logger.LogTrace("Message received: {Text}", message.Text?.TextEllipsis(20));

        string[] nodeIds;
        lock (_sync)
            nodeIds = _recepientsConfigIdAndNodeIds.GetValueOrDefault(instance.ConfigNode.Id) ?? [];

        foreach (var nodeId in nodeIds)
        {
            _ = InjectToNodeAsync(nodeId, message);
        }
    }

    private async Task InjectToNodeAsync(string nodeId, Message message)
    {
        try
        {
            var input = new NodeMsg { Payload = message };
            input.Add(message);
            await _nodeService.InjectAsync(_scopeFactory, nodeId, input);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to inject telegram message into node {NodeId}", nodeId);
        }
    }

    internal void OnStatusChange(TelegramClientInstance instance, string status)
    {
        _logger.LogTrace("Status change: config='{Config}', status='{Status}'", instance.ConfigNode.Name, status);

        string[] nodeIds;
        lock (_sync)
            nodeIds = _recepientsConfigIdAndNodeIds.GetValueOrDefault(instance.ConfigNode.Id) ?? [];

        foreach (var nodeId in nodeIds)
        {
            _nodeService.BroadcastStatus(nodeId, new NodeStatus { Text = status });
        }
    }

    [StartupOrder(11)]
    public Task OnStartupAsync()
    {
        _nodeService_OnAssignNodes();
        return Task.CompletedTask;
    }
}
