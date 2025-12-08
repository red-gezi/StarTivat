using PlantUml.Net;
using System.Text;

public class StoryFlowchartGenerator
{
    public static string GenerateFlowchart(Node rootNode)
    {
        var sb = new StringBuilder();
        sb.AppendLine("@startuml");
        var nodeMap = new Dictionary<Node, int>();
        int nodeCounter = 1;
        string start = "";
         List<string> branchBackNodeText=new();
        // 遍历节点并分配序号
        AssignNodeNumbers(rootNode, nodeMap, ref nodeCounter);
        // 生成连接关系
        GenerateConnections(rootNode, nodeMap, sb, ref start, branchBackNodeText);
        sb.AppendLine();
        sb.AppendLine("@enduml");
        return sb.ToString();
    }
    private static void GenerateConnections(Node node, Dictionary<Node, int> nodeMap, StringBuilder sb, ref string parentNodeDisplayText,  List<string> branchBackNodeText, List<string> previousBranchText = null)
    {
        //记录没子选项最后一个节点的显示文本
        List<string> optionTexts = new List<string>() { };
        string previousNodeDisplayText = parentNodeDisplayText;
        for (int i = 0; i < node.NextNodes.Count; i++)
        {
            var currentNode = node.NextNodes[i];
            string currentNodeDisplayText = $"\"{nodeMap[currentNode]}:{GetNodeDisplayText(currentNode)}{currentNode.Tag}\"";

            // 处理当前节点的特殊逻辑
            switch (currentNode.CurrentNodeType)
            {
                case NodeType.Start:
                    break;
                case NodeType.Speaker:
                    if (previousNodeDisplayText == "")
                    {
                        sb.AppendLine($"(*) -->{currentNodeDisplayText}");
                    }
                    else if (previousBranchText != null && previousBranchText.Any())
                    {
                        foreach (string branchText in optionTexts)
                        {
                            sb.AppendLine($"{previousNodeDisplayText} --> {currentNodeDisplayText}");
                        }
                        optionTexts.Clear();
                    }
                    else
                    {
                        sb.AppendLine($"{previousNodeDisplayText} --> {currentNodeDisplayText}");
                    }
                    previousNodeDisplayText = currentNodeDisplayText;
                    break;
                case NodeType.Action:
                    sb.AppendLine($"{previousNodeDisplayText} --> {currentNodeDisplayText}");
                    previousNodeDisplayText = currentNodeDisplayText;
                    break;
                case NodeType.Branch:
                    optionTexts = new();
                    //每次遇到分支节点分配一个返回列表
                    List<string> newBranchBackNodeText = new();
                    sb.AppendLine($"{previousNodeDisplayText} --> {currentNodeDisplayText}");
                    previousNodeDisplayText = currentNodeDisplayText;
                    GenerateConnections(currentNode, nodeMap, sb, ref previousNodeDisplayText, newBranchBackNodeText, optionTexts);
                    break;
                case NodeType.BranchTag:

                    currentNodeDisplayText = $"\"{nodeMap[currentNode]}:{currentNode.Text}\"";
                    //重新指定节点来源为父节点
                    sb.AppendLine($"{parentNodeDisplayText} --> [{GetNodeDisplayText(currentNode)}]{currentNodeDisplayText}");
                    previousNodeDisplayText = currentNodeDisplayText;
                    GenerateConnections(currentNode, nodeMap, sb, ref previousNodeDisplayText, branchBackNodeText, optionTexts);
                    break;
                case NodeType.BranchBack:
                    foreach (string backText in branchBackNodeText)
                    {
                        sb.AppendLine($"{backText} --> {currentNodeDisplayText}");
                    }
                    parentNodeDisplayText = currentNodeDisplayText;
                    break;
                case NodeType.Back:
                    branchBackNodeText.Add(currentNodeDisplayText);
                    sb.AppendLine($"{previousNodeDisplayText} --> {currentNodeDisplayText}");
                    break;
                case NodeType.End:
                    if (previousNodeDisplayText == "")
                    {
                        sb.AppendLine($"(*) -->{currentNodeDisplayText}");
                    }
                    else
                    {
                        sb.AppendLine($"{previousNodeDisplayText} --> {currentNodeDisplayText}");
                    }
                    break;
            }
        }
    }
    private static void AssignNodeNumbers(Node node, Dictionary<Node, int> nodeMap, ref int counter)
    {
        if (node == null || nodeMap.ContainsKey(node))
            return;

        nodeMap[node] = counter++;

        foreach (var nextNode in node.NextNodes)
        {
            AssignNodeNumbers(nextNode, nodeMap, ref counter);
        }
    }
    private static string GetNodeDisplayText(Node node)
    {
        switch (node.CurrentNodeType)
        {
            case NodeType.Start:
                return "开始";
            case NodeType.Speaker:
                return $"{node.Speaker}:{node.Text}";
            case NodeType.Action:
                return $"执行动作";
            case NodeType.Branch:
                return "分支选择";
            case NodeType.BranchTag:
                return $"分支_{node.Tag}";
            case NodeType.BranchBack:
                return "合并分支";
            case NodeType.Back:
                return "返回上层";
            case NodeType.End:
                return "结束";
            default:
                return "未知节点";
        }
    }
    public static void SaveFlowchartToFile(string plantUmlContent, string filePath)
    {
        try
        {
            System.IO.File.WriteAllText(filePath, plantUmlContent, System.Text.Encoding.UTF8);
            Console.WriteLine($"流程图已保存到: {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"保存流程图时出错: {ex.Message}");
        }
    }

    public static async Task GenerateImageFromPlantUml(string plantUmlContent, string imagePath)
    {
        try
        {
            var factory = new RendererFactory();
            var renderer = factory.CreateRenderer(new PlantUmlSettings());

            var imageBytes = await renderer.RenderAsync(plantUmlContent, OutputFormat.Png);
            await System.IO.File.WriteAllBytesAsync(imagePath, imageBytes);

            Console.WriteLine($"流程图图片已生成: {imagePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"生成流程图图片时出错: {ex.Message}");
        }
    }
}
