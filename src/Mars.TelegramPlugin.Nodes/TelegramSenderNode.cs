using System.ComponentModel.DataAnnotations;
using Mars.Core.Attributes;
using Mars.Nodes.Core;
using Mars.Nodes.Core.Fields;

namespace Mars.TelegramPlugin.Nodes;

[FunctionApiDocument("./_plugin/Mars.TelegramPlugin/nodes/docs/TelegramSenderNode/TelegramSenderNode{.lang}.md")]
[Display(GroupName = "telegram")]
public class TelegramSenderNode : Node
{
    public InputConfig<TelegramConfigNode> Config { get; set; }

    [Display(Name = "Chat Id", Description = "must be '123456789' or '@channelusername' ")]
    public string ChatId { get; set; } = "";

    public TelegramSenderNode()
    {
        Color = "#56abdc";
        Inputs = [new()];
        Outputs = [new NodeOutput()];
        Icon = "/_plugin/Mars.TelegramPlugin/nodes/img/telegram.png";
    }

}
