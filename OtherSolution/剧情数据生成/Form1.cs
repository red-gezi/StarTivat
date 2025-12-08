
using System.Diagnostics;
using static System.Windows.Forms.Design.AxImporter;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;

namespace 剧情数据生成
{
    public partial class Form1 : Form
    {
        int CurrentlineIndex { get; set; } = 1;
        public Form1()
        {
            InitializeComponent();
            textBox.MouseClick += TextBox_MouseClick;
            textBox.KeyUp += TextBox_KeyUp;
            textBox.TextChanged += TextBox_TextChanged;
            Task.Run(() =>
            {
                while (true)
                {
                    Console.WriteLine("点击回车进入单步执行");
                    Console.ReadLine();
                    var text = textBox.Text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();
                    var rootNode = TextNodeSystem.Parse(text);
                    TextNodeSystem.Start(rootNode);

                    while (true)
                    {
                        Console.WriteLine("点击回车执行单步执行");
                        Console.ReadLine();
                        TextNodeSystem.Step();
                        if (TextNodeSystem.currentNode == null)
                        {
                            Console.WriteLine("已结束");
                            break;
                        }
                    }
                }
            });
        }

        private void TextBox_TextChanged(object? sender, EventArgs e) => UpdateLineIndex();
        private void TextBox_KeyUp(object? sender, KeyEventArgs e) => UpdateLineIndex();
        private void TextBox_MouseClick(object? sender, MouseEventArgs e) => UpdateLineIndex();



        private void UpdateLineIndex()
        {
            // 计算当前光标所在行
            int cursorPos = textBox.SelectionStart;
            string text = textBox.Text.Substring(0, Math.Min(cursorPos, textBox.Text.Length));
            CurrentlineIndex = text.Count(c => c == '\n') + 1;
            lineIndex.Text = $"当前编辑行：{CurrentlineIndex}";
        }
        private void InsertTextAtLine(int line, string textToInsert)
        {
            // 获取当前文本行数组
            string[] lines = textBox.Text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            // 调整行号为0基索引
            int insertIndex = Math.Max(0, Math.Min(line - 1, lines.Length));

            // 将要插入的文本分割成行
            string[] insertLines = textToInsert.Split(new[] { "\r\n", "\r", "\n", "\\n" }, StringSplitOptions.None);

            // 创建新的行列表
            List<string> newLines = new List<string>(lines);
            newLines.RemoveAt(insertIndex);
            // 在指定位置插入文本
            newLines.InsertRange(insertIndex, insertLines);

            // 重新组合文本
            string newText = string.Join(Environment.NewLine, newLines);

            // 更新TextBox内容
            textBox.Text = newText;

            // 将光标定位到插入位置
            int cursorPosition = 0;
            for (int i = 0; i < insertIndex + 1 && i < newLines.Count; i++)
            {
                cursorPosition += newLines[i].Length + Environment.NewLine.Length;
            }

            // 设置光标位置
            textBox.SelectionStart = cursorPosition;
            textBox.ScrollToCaret();
            textBox.Focus();
            UpdateLineIndex();
        }
        private async void CreatFlowchart_Click(object sender, EventArgs e)
        {
            var text = textBox.Text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();
            var rootNode = TextNodeSystem.Parse(text);
            // 生成PlantUML文本
            string plantUmlContent = StoryFlowchartGenerator.GenerateFlowchart(rootNode);
            Console.WriteLine(plantUmlContent);
            // 保存PlantUML文本到文件
            StoryFlowchartGenerator.SaveFlowchartToFile(plantUmlContent, "story_flowchart.txt");
            // 生成图片（需要PlantUML环境）
            await StoryFlowchartGenerator.GenerateImageFromPlantUml(plantUmlContent, "story_flowchart.png");
            Process.Start("explorer.exe", "story_flowchart.png");
        }
        private void Insert_Branch_Click(object sender, EventArgs e)
        {
            string itemText = $@"[SelectBranch]{tagBranch.Text}\n\n[BranchBack]{tagBranch.Text}";
            InsertTextAtLine(CurrentlineIndex, itemText);
        }
        private void Insert_Rand_Click(object sender, EventArgs e)
        {
            string itemText = $@"[RandBranch]{tagBranch.Text}1 1\n\n[BranchBack]{tagBranch.Text}";
            InsertTextAtLine(CurrentlineIndex, itemText);
        }
        private void Insert_Reward_Click(object sender, EventArgs e)
        {
            string itemText = $@"[RewardBranch]{tagBranch.Text}1 1\n\n[BranchBack]{tagBranch.Text}";
            InsertTextAtLine(CurrentlineIndex, itemText);
        }
        private void InsertBackBranchItem_Click(object sender, EventArgs e)
        {
            string itemText = $@"[{tagBranchItem.Text}]{tagBranchItemText.Text}\n\n[Back]{tagBranchItem.Text}";
            InsertTextAtLine(CurrentlineIndex, itemText);
        }

        private void InsertEndBranchItem_Click(object sender, EventArgs e)
        {
            string itemText = $@"[{tagBranchItem.Text}]{tagBranchItemText.Text}\n\n[End]{tagBranchItem.Text}";
            InsertTextAtLine(CurrentlineIndex, itemText);
        }

        private void Insert_Action_Click(object sender, EventArgs e)
        {
            InsertTextAtLine(CurrentlineIndex, $@"[Action]{tagAction.Text}\n");
        }
        private void Insert_Back_Click(object sender, EventArgs e)
        {
            InsertTextAtLine(CurrentlineIndex, $@"[Back]{tagBack.Text}\n");
        }

        private void Insert_End_Click(object sender, EventArgs e)
        {
            InsertTextAtLine(CurrentlineIndex, $@"[End]{tagEnd.Text}\n");
        }

        private void Insert_Talk1_Click(object sender, EventArgs e)
        {
            InsertTextAtLine(CurrentlineIndex, $@"{speaker1.Text}:{speakerText1.Text}\n");
        }

        private void Insert_Talk2_Click(object sender, EventArgs e)
        {
            InsertTextAtLine(CurrentlineIndex, $@"{speaker2.Text}:{speakerText2.Text}\n");
        }

        private void Insert_Talk3_Click(object sender, EventArgs e)
        {
            InsertTextAtLine(CurrentlineIndex, $@"{speaker3.Text}:{speakerText3.Text}\n");
        }

        private void Insert_Talk4_Click(object sender, EventArgs e)
        {
            InsertTextAtLine(CurrentlineIndex, $@"{speaker4.Text}:{speakerText4.Text}\n");
        }
        private void TextClear_Click(object sender, EventArgs e)
        {
            // 设置光标位置
            textBox.Text = "";
            textBox.SelectionStart = 0;
            textBox.ScrollToCaret();
            textBox.Focus();
            UpdateLineIndex();
        }
        private void TextCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBox.Text);
        }


    }
}
