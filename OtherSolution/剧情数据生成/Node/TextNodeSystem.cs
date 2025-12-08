
// 解析器类
public class TextNodeSystem
{
    public static Node Parse(List<string> lines)
    {
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
            else if (line.StartsWith("[SelectBranch]"))
            {
                var branchNode = new Node
                {
                    CurrentNodeType = NodeType.SelectBranch,
                    ParentNode = currentNode,
                    Tag = line.Substring(line.IndexOf(']') + 1),
                };
                if (currentNode != null)
                {
                    currentNode.NextNodes.Add(branchNode);
                }
                currentNode = branchNode;
            }
            else if (line.StartsWith("[RandBranch]"))
            {
                var branchNode = new Node
                {
                    CurrentNodeType = NodeType.RandBranch,
                    ParentNode = currentNode,
                    Tag = line.Substring(line.IndexOf(']') + 1),
                };
                if (currentNode != null)
                {
                    currentNode.NextNodes.Add(branchNode);
                }
                currentNode = branchNode;
            }
            else if (line.StartsWith("[ConditionBranch]"))
            {
                var branchNode = new Node
                {
                    CurrentNodeType = NodeType.ConditionBranch,
                    ParentNode = currentNode,
                    Tag = line.Substring(line.IndexOf(']') + 1),
                };
                if (currentNode != null)
                {
                    currentNode.NextNodes.Add(branchNode);
                }
                currentNode = branchNode;
            }
            else if (line.StartsWith("[RewardBranch]"))
            {
                var branchNode = new Node
                {
                    CurrentNodeType = NodeType.RewardBranch,
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
    //public static void Show(Node node, int layer = 0)
    //{
    //    foreach (var currentNode in node.NextNodes)
    //    {
    //        switch (currentNode.CurrentNodeType)
    //        {
    //            case NodeType.Speaker:
    //                Console.Write(new String(' ', layer * 2));
    //                Console.WriteLine($"{currentNode.Speaker}:{currentNode.Text}");
    //                break;
    //            case NodeType.SelectBranch:
    //                Console.Write(new String(' ', layer * 2));
    //                Console.WriteLine($"请选择:{currentNode.Text}");
    //                Show(currentNode, layer + 1);
    //                break;
    //            case NodeType.Action:
    //                Console.Write(new String(' ', layer * 2));
    //                Console.WriteLine($"执行行动{currentNode.Tag}");
    //                //Show(currentNode, (layer + 1) * 2);
    //                break;
    //            case NodeType.Back:
    //                break;
    //            case NodeType.End:
    //                break;
    //            case NodeType.BranchTag:
    //                Console.Write(new String(' ', layer * 2));
    //                Console.WriteLine($"选择{currentNode.Tag}:{currentNode.Text}");
    //                Show(currentNode, (layer + 1) * 2);
    //                break;
    //            default:
    //                break;
    //        }
    //    }
    //}
    public static void Run(Node node)
    {
        foreach (var currentNode in node.NextNodes)
        {
            switch (currentNode.CurrentNodeType)
            {
                case NodeType.Speaker:
                    //调用台词
                    Console.WriteLine($"{currentNode.Speaker}:{currentNode.Text}");
                    //Console.Write("。。。。。。请回车");
                    //Console.ReadLine();
                    break;
                case NodeType.SelectBranch:
                    //调用选项
                    Console.WriteLine($"请选择:{currentNode.Text}");
                    for (int i = 0; i < currentNode.NextNodes.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}  {currentNode.NextNodes[i].Text}");
                    }
                    var select = int.Parse(Console.ReadLine());
                    Node selectNode = currentNode.NextNodes[select - 1];
                    Console.WriteLine($"玩家选择了 {selectNode.Text}");
                    Run(selectNode);
                    break;
                case NodeType.Action:
                    Console.WriteLine($"执行行动{currentNode.Tag}");
                    break;
                case NodeType.Back:
                    break;
                case NodeType.End:
                    break;
                case NodeType.BranchTag:

                    Run(currentNode);
                    break;
                default:
                    break;
            }
        }

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
                Console.WriteLine($"{currentNode.Speaker}:{currentNode.Text}");
                currentNode = currentNode.GetNextNode();
                break;
            case NodeType.SelectBranch:
                //调用选项
                Console.WriteLine($"展开选项面板，请选择:{currentNode.Text}");
                for (int i = 0; i < currentNode.NextNodes.Where(node => node.CurrentNodeType == NodeType.BranchTag).Count(); i++)
                {
                    Console.WriteLine($"{i + 1}  {currentNode.NextNodes[i].Text}");
                }
                Select();
                break;
            case NodeType.Action:
                Console.WriteLine($"执行行动{currentNode.Tag}");
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
    public static void Select()
    {
        var select = int.Parse(Console.ReadLine());
        currentNode = currentNode.NextNodes[select - 1];
        Console.WriteLine($"玩家选择了 {currentNode.Text}");
    }
}
