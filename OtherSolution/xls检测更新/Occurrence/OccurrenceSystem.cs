using Newtonsoft.Json;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.IO;

class OccurrenceSystem
{
    static DateTime lastChangeTime;
    static FileStream fs;
    static string DirePath { get; set; }
    static string FilePath { get; set; }
    static Workbook workbook = new Workbook();
    public static void Init(string direPath, string filePath)
    {
        DirePath = direPath;
        FilePath = direPath + filePath;
    }
    public static void Check()
    {
        if (lastChangeTime != new FileInfo(FilePath).LastWriteTime)
        {
            Console.WriteLine("进行自动更新" + DateTime.Now);
            UpdateJson();
        }
    }
    public static void UpdateJson()
    {
        fs = File.Open(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        workbook.LoadFromStream(fs);
        lastChangeTime = new FileInfo(FilePath).LastWriteTime;
        XlsToJson(workbook);
        fs.Dispose();
    }
    private static void XlsToJson(Workbook workbook)
    {
        string workbookName = "Ch";

        var dialogueText = workbook.Worksheets[workbookName];
        int dialogueColCount = dialogueText.Columns.Length;
        int dialogueRowCount = dialogueText.Rows.Length;
        List<OccurrenceData> dialogModels = new List<OccurrenceData>();

        for (int i = 2; i <= dialogueRowCount; i++)
        {
            OccurrenceData currentDialogModel = new OccurrenceData();

            string tag = dialogueText[i, 1].DisplayedText;
            string imageName = dialogueText[i, 2].DisplayedText;
            string sideColor = dialogueText[i, 3].DisplayedText;
            string name = dialogueText[i, 4].DisplayedText;
            string dialogueContent = dialogueText[i, 5].DisplayedText;
            if (tag != "")
            {
                currentDialogModel.Tag = tag;
                currentDialogModel.ImageName = imageName;
                currentDialogModel.SideColor = sideColor;
                currentDialogModel.Name[workbookName] = name;
                currentDialogModel.Dialogue[workbookName] = dialogueContent;
                dialogModels.Add(currentDialogModel);
            }
        }

        // 将解析结果写入文件
        File.WriteAllText(DirePath + @"\Occurrence.json", JsonConvert.SerializeObject(dialogModels, Formatting.Indented));
        Console.WriteLine("新事件数据更新完毕");
    }
}


