using System.ComponentModel.DataAnnotations;
using Mars.Core.Attributes;
using Mars.Nodes.Core.Nodes;

namespace Mars.TelegramPlugin.Nodes;

[FunctionApiDocument("./_plugin/Mars.TelegramPlugin/nodes/docs/TelegramConfigNode/TelegramConfigNode{.lang}.md")]
[Display(GroupName = "telegram")]
public class TelegramConfigNode : ConfigNode
{
    [Required]
    public string Token { get; set; } = "";
    public string BotName { get; set; } = "";
}
