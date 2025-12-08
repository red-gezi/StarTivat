using Newtonsoft.Json;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace xls检测更新
{
    class Program
    {
        static DateTime lastChangeTime;
        static FileStream fs;
        static string direPath;
        static string filePath;
        static Dictionary<string, Dictionary<string, string>> textTranslate = new Dictionary<string, Dictionary<string, string>>();
        static List<string> supportLanguage = new List<string>();
        static void Main(string[] args)
        {
            Workbook workbook = new Workbook();
            direPath = Directory.GetCurrentDirectory().Replace(@"\OtherSolution\xls检测更新\bin\Debug\net6.0", "") + @"\Assets\GameResource\GameData\";
            filePath = direPath + "Occurrence.xlsx";
            Task.Run(async () =>
            {
                while (true)
                {
                    //if (lastChangeTime != new FileInfo(filePath).LastWriteTime)
                    //{
                    //    Console.WriteLine("进行自动更新" + DateTime.Now);
                    //    fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    //    Console.WriteLine(workbook);
                    //    Console.WriteLine(fs);
                    //    workbook.LoadFromStream(fs);
                    //    lastChangeTime = new FileInfo(filePath).LastWriteTime;
                    //    XlsToJson(workbook);
                    //    fs.Dispose();
                    //}
                    await Task.Delay(100);
                }
            });
            while (true)
            {
                Console.WriteLine("可回车进行手动更新");
                Console.ReadLine();
                fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                workbook.LoadFromStream(fs);
                XlsToJson(workbook);
                fs.Dispose();
            }
        }

        private static void XlsToJson(Workbook workbook)
        {
            string workbookName = "Ch";

            var storyText = workbook.Worksheets[workbookName]; // 假设新表格也叫Story，或使用实际表名
            int storyColCount = storyText.Columns.Length;
            int storyRowCount = storyText.Rows.Length;
            List<DialogModel> dialogModels = new List<DialogModel>();

            for (int i = 2; i <= storyRowCount; i++)
            {
                DialogModel currentDialogModel = new DialogModel();

                string tag = storyText[i, 1].DisplayedText;
                string imageName = storyText[i, 2].DisplayedText;
                string sideColor = storyText[i, 3].DisplayedText;
                string name = storyText[i, 4].DisplayedText;
                string storyContent = storyText[i, 5].DisplayedText;
                if (tag != "")
                {
                    currentDialogModel.Tag = tag;
                    currentDialogModel.ImageName = imageName;
                    currentDialogModel.SideColor = sideColor;
                    currentDialogModel.Name[workbookName] = name;
                    currentDialogModel.Story[workbookName] = storyContent;
                    dialogModels.Add(currentDialogModel);
                }
            }

            // 将解析结果写入文件
            File.WriteAllText(direPath + @"\Occurrence.json", JsonConvert.SerializeObject(dialogModels, Formatting.Indented));
            Console.WriteLine("新剧情故事更新完毕");
        }
        public class DialogModel
        {
            public string Tag { get; set; }
            public string ImageName { get; set; }
            public string SideColor { get; set; }
            public Dictionary<string, string> Name { get; set; } = new();
            public Dictionary<string, string> Story { get; set; } = new();
        }
    }

    static class Extesion
    {
        public static int GetIndex(this Worksheet worksheet, string tag)
        {
            for (int i = 1; i <= worksheet.Columns.Length; i++)
            {
                if (worksheet[1, i].DisplayedText == tag)
                {
                    return i;
                }
            }
            //Console.WriteLine($"无法检索到{tag}，请确认是否在单人和多人卡牌中都存在该属性");
            return 0;
        }
        public static List<int> GetIndexs(this Worksheet worksheet, string tag)
        {
            List<int> indexs = new List<int>();
            for (int i = 1; i <= worksheet.Columns.Length; i++)
            {
                if (worksheet[1, i].DisplayedText.Contains(tag))
                {
                    indexs.Add(i);
                }
            }
            return indexs;
        }
        public static T GetXlsData<T>(this CellRange ranges)
        {
            if (ranges.DisplayedText != "")
            {
                return (T)Convert.ChangeType(ranges.Value, typeof(T).IsEnum ? typeof(int) : typeof(T));
            }
            else
            {
                return default;
            }
        }
        public static int ToEnumIndex(this string data, params string[] text)
        {
            return text.Contains(data) ? text.ToList().IndexOf(data) : 0;
        }
    }
}


