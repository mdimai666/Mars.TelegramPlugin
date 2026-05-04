using Mars.Core.Extensions;
using Mars.Host.Shared.Services;
using Mars.Host.Shared.Startup;
using Mars.Nodes.Core;
using Mars.TelegramPlugin.Nodes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Mars.TelegramPlugin.Services;

internal class TelegramManager : IMarsAppLifetimeService
{
    Dictionary<string, TelegramClientInstance> _clientInstances = new();
    private readonly INodeService _nodeService;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<TelegramManager> _logger;
    private Dictionary<string, string[]> _recepientsConfigIdAndNodeIds = new();

    public TelegramManager(INodeService nodeService, IServiceScopeFactory scopeFactory, ILogger<TelegramManager> logger)
    {
        _nodeService = nodeService;
        _scopeFactory = scopeFactory;
        _logger = logger;
        _nodeService.OnAssignNodes += _nodeService_OnAssignNodes;
    }

    private void _nodeService_OnAssignNodes()
    {
        var configNodes = _nodeService.BaseNodes.Values.OfType< TelegramConfigNode>().ToArray();
        RefreshConfigs(configNodes);
        UpdateRecepientsDict();
    }

    public void RefreshConfigs(TelegramConfigNode[] configs)
    {
        _logger.LogTrace("RefreshConfigs");

        var toRemoveIds = _clientInstances.Values.Select(s => s.ConfigNode.Id).Except(configs.Select(s => s.Id)).ToList();
        toRemoveIds.ForEach(configId => _clientInstances[configId].Dispose());

        foreach (var config in configs)
        {
            //Telegram.Bot.TelegramBotClientOptions
            if (_clientInstances.TryGetValue(config.Token, out var bot))
            {
                //check param update
            }
            else
            {
                _clientInstances.Add(config.Token, new TelegramClientInstance(config, this));
            }
        }
    }

    void UpdateRecepientsDict()
    {
        var nodes = _nodeService.BaseNodes.Values.Where(node => !node.Disabled && node is TelegramReceiverNode tg && tg.Config.Value != null)
                                            .Select(node => (node as TelegramReceiverNode)!).ToArray();

        _recepientsConfigIdAndNodeIds = nodes.GroupBy(s => s.Config.Id).ToDictionary(s => s.Key, s => s.Select(node => node.Id).ToArray());
    }

    public TelegramBotClient? GetBot(TelegramConfigNode? config) => config == null ? null : _clientInstances.GetValueOrDefault(config.Token)?.Bot;

    public async Task<Message> SendMessage(string chatIdOrUsername, string text)
    {
        var bot = _clientInstances.First().Value.Bot;
        return await bot.SendMessage(new ChatId(chatIdOrUsername), text);
    }

    internal void OnReciveMessage(TelegramClientInstance instance, Message message, Telegram.Bot.Types.Enums.UpdateType type)
    {
        _logger.LogWarning($"OnReciveMessage: {message.Text?.TextEllipsis(20)}");

        var nodes = _recepientsConfigIdAndNodeIds.GetValueOrDefault(instance.ConfigNode.Id) ?? [];

        foreach (var _nodeId in nodes)
        {
            var nodeId = _nodeId;
            var input = new NodeMsg { Payload = message };
            input.Add(message);
            _ = _nodeService.InjectAsync(_scopeFactory, nodeId, input);
        }
    }

    internal void OnStatusChange(TelegramClientInstance instance, string status)
    {
        _logger.LogTrace($"OnStatusChange: config='{instance.ConfigNode.Name}', status='{status}'");

        var nodes = _recepientsConfigIdAndNodeIds.GetValueOrDefault(instance.ConfigNode.Id) ?? [];

        foreach (var nodeId in nodes)
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
