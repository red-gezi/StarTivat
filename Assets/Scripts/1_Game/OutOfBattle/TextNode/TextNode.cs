using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts._1_Game.OutOfBattle.TextNode
{
    public class TextNode : MonoBehaviour
    {

      
        internal class Program
        {
            static void Main(string[] args)
            {
                var text = File.ReadAllLines("text.txt").ToList();
                var rootNode = TextNodeSystem.Parse(text);
                //TextNodeSystem.Show(rootNode);
                while (true)
                {
                    TextNodeSystem.Run(rootNode);
                    Console.WriteLine("对话已结束，回车已经下一轮对话");
                    Console.WriteLine("——————————————————————————————————");
                    //Console.ReadLine();
                }
                //Console.WriteLine(JsonConvert.SerializeObject(rootNode, Formatting.Indented));
            }
        }
    }

    // 节点类型枚举
    public enum NodeType
    {
        Start,
        Speaker,  // 讲述者节点
        Branch,   // 分支节点
        BranchTag,   // 分支标签节点
        Action,   // 动作节点
        Back,     // 返回节点
        End,       // 结束节点

    }

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

                else if (line.StartsWith("[action]"))
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
                else if (line.StartsWith("[end]"))
                {
                    //var endNode = new Node { CurrentNodeType = NodeType.End };
                    //if (currentNode != null)
                    //{
                    //    currentNode.NextNodes.Add(endNode);
                    //}
                    currentNode = currentNode.ParentNode;
                }
                else if (line.StartsWith("[back]"))
                {
                    var backNode = new Node { CurrentNodeType = NodeType.Back };
                    if (currentNode != null)
                    {
                        currentNode.NextNodes.Add(backNode);
                    }
                    currentNode = currentNode.ParentNode;
                }
                else if (line.StartsWith("[branch]"))
                {
                    var branchNode = new Node
                    {
                        CurrentNodeType = NodeType.Branch,
                        ParentNode = currentNode
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
        public static void Show(Node node, int layer = 0)
        {
            foreach (var currentNode in node.NextNodes)
            {
                switch (currentNode.CurrentNodeType)
                {
                    case NodeType.Speaker:
                        Console.Write(new String(' ', layer * 2));
                        Console.WriteLine($"{currentNode.Speaker}:{currentNode.Text}");
                        break;
                    case NodeType.Branch:
                        Console.Write(new String(' ', layer * 2));
                        Console.WriteLine($"请选择:{currentNode.Text}");
                        Show(currentNode, layer + 1);
                        break;
                    case NodeType.Action:
                        Console.Write(new String(' ', layer * 2));
                        Console.WriteLine($"执行行动{currentNode.Tag}");
                        //Show(currentNode, (layer + 1) * 2);
                        break;
                    case NodeType.Back:
                        break;
                    case NodeType.End:
                        break;
                    case NodeType.BranchTag:
                        Console.Write(new String(' ', layer * 2));
                        Console.WriteLine($"选择{currentNode.Tag}:{currentNode.Text}");
                        Show(currentNode, (layer + 1) * 2);
                        break;
                    default:
                        break;
                }
            }
        }
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
                    case NodeType.Branch:
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
    }
}
