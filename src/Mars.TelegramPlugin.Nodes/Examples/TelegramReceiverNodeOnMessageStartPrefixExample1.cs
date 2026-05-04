using Mars.Nodes.Core;
using Mars.Nodes.Core.Nodes;
using Mars.Nodes.Core.StringFunctions;
using Mars.Nodes.Core.Utils;

namespace Mars.TelegramPlugin.Nodes.Examples;

public class TelegramReceiverNodeOnMessageStartPrefixExample1 : INodeExample<TelegramReceiverNode>
{
    public string Name => "On recive message start with prefix";
    public string Description => "On recive message start with prefix";

    public IReadOnlyCollection<Node> Handle(IEditorState editorState)
    {
        return NodesWorkflowBuilder.Create()
            .AddNext(new TelegramReceiverNode())
            .AddNext(new VariableSetNode { Setters = [new() { ValuePath = "msg.Payload", Expression = "msg.Payload.Text", Operation = VariableSetOperation.Set }] })
            .AddNext(new SwitchNode { Conditions = [new() { Value = "msg.Payload.ToString().StartsWith(\"!\")" }] })
            .AddNext(new StringNode { Operations = [new() { Method = nameof(StringNodeOperationUtils.RemovePrefix), ParameterValues = ["!"] }] })
            .AddNext(new DebugNode())
            .Build();
    }
}
