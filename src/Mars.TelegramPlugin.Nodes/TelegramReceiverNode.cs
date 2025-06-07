using Mars.Core.Attributes;
using Mars.Nodes.Core;
using Mars.Nodes.Core.Nodes;

namespace Mars.TelegramPlugin.Nodes;

[FunctionApiDocument("./_plugin/Mars.TelegramPlugin/nodes/docs/TelegramReceiverNode/TelegramReceiverNode{.lang}.md")]
public class TelegramReceiverNode : Node
{
    public InputConfig<TelegramConfigNode> Config { get; set; }

    public TelegramReceiverNode()
    {
        HaveInput = false;
        Color = "#56abdc";
        Outputs = new List<NodeOutput> { new NodeOutput() };
        Icon = "/_plugin/Mars.TelegramPlugin/nodes/img/telegram.png";
    }

}
