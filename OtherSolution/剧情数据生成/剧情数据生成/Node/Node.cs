
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
// 表示一个节点
public class Node
{
    [JsonConverter(typeof(StringEnumConverter))]
    public NodeType CurrentNodeType { get; set; }

    public string Speaker { get; set; } = ""; // 用于SpeakerNode的讲述者名称
    public string Text { get; set; } = "";    // 用于SpeakerNode的讲述内容
    public string Tag { get; set; } = "";
    [JsonIgnore]
    public Node ParentNode { get; set; }
    public List<Node> NextNodes { get; set; } = new List<Node>();
}
