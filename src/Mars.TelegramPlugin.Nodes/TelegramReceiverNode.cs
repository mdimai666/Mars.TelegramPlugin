using System.ComponentModel.DataAnnotations;
using Mars.Core.Attributes;
using Mars.Nodes.Core;
using Mars.Nodes.Core.Fields;

namespace Mars.TelegramPlugin.Nodes;

[FunctionApiDocument("./_plugin/Mars.TelegramPlugin/nodes/docs/TelegramReceiverNode/TelegramReceiverNode{.lang}.md")]
[Display(GroupName = "telegram")]
public class TelegramReceiverNode : Node
{
    public InputConfig<TelegramConfigNode> Config { get; set; }

    public TelegramReceiverNode()
    {
        Inputs = [];
        Color = "#56abdc";
        Outputs = [new NodeOutput()];
        Icon = "/_plugin/Mars.TelegramPlugin/nodes/img/telegram.png";
    }

}
