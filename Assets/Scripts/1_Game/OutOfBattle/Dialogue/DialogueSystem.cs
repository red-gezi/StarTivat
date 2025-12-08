
// 解析器类
using Sirenix.Utilities;
using System;
using System.Linq;

public class DialogueSystem
{
    public static Node Parse(string OccurrenceText)
    {
        var lines = OccurrenceText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();
        var startNode = new Node() { CurrentNodeType = NodeType.Start };

        Node currentNode = startNode;
        lines.RemoveAll(x => x == "");
        foreach (var line in lines)
        {
            // 解析为注释节点
            if (line.StartsWith("//"))
            {

            }
            //解析为讲述者节点
            else if (line.Contains(":") && !line.StartsWith("["))
            {
                // 这是讲述者行，格式为 "讲述者：内容"
                var separatorIndex = line.IndexOf(":");
                var speaker = line.Substring(0, separatorIndex);
                var text = line.Substring(separatorIndex + 1);

                var speakerNode = new Node
                {
                    CurrentNodeType = NodeType.Speaker,
                    Speaker = speaker,
                    Text = text,
                    ParentNode = currentNode
                };

                if (currentNode != null)
                {
                    currentNode.NextNodes.Add(speakerNode);
                }
                //currentNode = speakerNode;
            }

            //解析为行为节点

            else if (line.StartsWith("[Action]"))
            {
                var actionNode = new Node
                {
                    CurrentNodeType = NodeType.Action,
                    Tag = line.Substring(8).Trim(),
                    ParentNode = currentNode
                };

                if (currentNode != null)
                {
                    currentNode.NextNodes.Add(actionNode);
                }
            }
            else if (line.StartsWith("[End]"))
            {
                var endNode = new Node
                {
                    CurrentNodeType = NodeType.End,
                    ParentNode = currentNode
                };
                if (currentNode != null)
                {
                    currentNode.NextNodes.Add(endNode);

                }
                currentNode = currentNode.ParentNode ?? currentNode;
            }
            else if (line.StartsWith("[Back]"))
            {
                var backNode = new Node
                {
                    CurrentNodeType = NodeType.Back,
                    ParentNode = currentNode
                };
                if (currentNode != null)
                {
                    currentNode.NextNodes.Add(backNode);
                }
                currentNode = currentNode.ParentNode;
            }
            else if (line.StartsWith("[BranchBack]"))
            {
                var backNode = new Node
                {
                    CurrentNodeType = NodeType.BranchBack,
                    ParentNode = currentNode,
                    Tag = line.Substring(line.IndexOf(']') + 1),
                };
                if (currentNode != null)
                {
                    currentNode.NextNodes.Add(backNode);
                }
                currentNode = currentNode.ParentNode;

            }
            else if (line.StartsWith("[Branch]"))
            {
                var branchNode = new Node
                {
                    CurrentNodeType = NodeType.Branch,
                    ParentNode = currentNode,
                    Tag = line.Substring(line.IndexOf(']') + 1),
                };
                if (currentNode != null)
                {
                    currentNode.NextNodes.Add(branchNode);
                }
                currentNode = branchNode;
            }
            //解析为分支标签节点
            else if (line.StartsWith("[") && line.Contains("]"))
            {
                var branchNode = new Node
                {
                    CurrentNodeType = NodeType.BranchTag,
                    ParentNode = currentNode,
                    Tag = line.Substring(1, line.IndexOf(']') - 1),
                    Text = line.Substring(line.IndexOf(']') + 1)

                };
                if (currentNode != null)
                {
                    currentNode.NextNodes.Add(branchNode);
                }
                currentNode = branchNode;
            }

            else
            {
                Console.WriteLine("当前行无法解析" + line);
            }
        }

        // 返回第一个节点作为根节点
        return startNode;
    }
    ///////////////////////////单步运行///////////////////////////////
    public static Node? currentNode;
    public static Node? currentBranchNode;
    public static void Start(Node node)
    {
        currentNode = node;
        Step();
    }
    public static void Step()
    {
        if (currentNode == null)
        {
            Console.WriteLine("已结束");
            return;
        }
        switch (currentNode.CurrentNodeType)
        {
            case NodeType.Speaker:
                //调用台词
                Log.Show($"{currentNode.Speaker}:{currentNode.Text}");
                currentNode = currentNode.GetNextNode();
                break;
            case NodeType.Branch:
                //调用选项
                Log.Show($"展开选项面板，请选择:{currentNode.Text}");
                OutOfBattleUIManager.Instance.CloseStepCanves();
                OutOfBattleUIManager.Instance.OpenOccurrenceOptionContent();
                currentNode.NextNodes
                    .Where(node => node.CurrentNodeType == NodeType.BranchTag)
                    .ForEach(node => OutOfBattleUIManager.Instance.AddOccurrenceOption(node.Text));
                break;
            case NodeType.Reward:
                //调用选项
                Log.Show($"展开奖励面板，请选择:{currentNode.Text}");
                OutOfBattleUIManager.Instance.CloseStepCanves();
                OutOfBattleUIManager.Instance.OpenRewardContent();
                currentNode.NextNodes
                    .Where(node => node.CurrentNodeType == NodeType.BranchTag)
                    .ForEach(node => OutOfBattleUIManager.Instance.AddReward(node.Text));
                break;
            case NodeType.Action:
                Log.Show($"执行行动{currentNode.Tag}");
                currentNode = currentNode.GetNextNode();
                break;
            case NodeType.Back:
                //返回分支的最后一个选项
                currentNode = currentNode.ParentNode?.ParentNode?.NextNodes.LastOrDefault();
                break;
            case NodeType.BranchBack:
                //返回分支的最后一个选项
                currentNode = currentNode.ParentNode?.GetNextNode();
                break;
            case NodeType.End:
                currentNode = null;
                break;
            case NodeType.BranchTag:
                currentNode = currentNode?.NextNodes.FirstOrDefault();
                break;
            
            default:
                currentNode = currentNode.GetNextNode() ?? currentNode?.NextNodes.FirstOrDefault();
                break;
        }
    }
    public static void Select(int index)
    {
        currentNode = currentNode.NextNodes[index - 1];
        Log.Show($"玩家选择了 {currentNode.Text}");
    }
}
