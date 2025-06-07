using System.ComponentModel.DataAnnotations;
using Mars.Core.Attributes;
using Mars.Nodes.Core;
using Mars.Nodes.Core.Nodes;

namespace Mars.TelegramPlugin.Nodes;

[FunctionApiDocument("./_plugin/Mars.TelegramPlugin/nodes/docs/TelegramSenderNode/TelegramSenderNode{.lang}.md")]
public class TelegramSenderNode : Node
{
    public InputConfig<TelegramConfigNode> Config { get; set; }

    [Display(Name = "Chat Id", Description = "must be '123456789' or '@channelusername' ")]
    public string ChatId { get; set; } = "";

    public TelegramSenderNode()
    {
        HaveInput = true;
        Color = "#56abdc";
        Outputs = new List<NodeOutput> { new NodeOutput() };
        Icon = "/_plugin/Mars.TelegramPlugin/nodes/img/telegram.png";
    }

}
