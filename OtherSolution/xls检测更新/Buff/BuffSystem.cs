using Newtonsoft.Json;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.IO;

class BuffSystem
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
        int sheetsCount = workbook.Worksheets.Count;
        var worksheet_Ch = workbook.Worksheets[0];
        string workbookName = worksheet_Ch.Name;
        int dialogueColCount = worksheet_Ch.Columns.Length;
        int dialogueRowCount = worksheet_Ch.Rows.Length;
        List<BuffData> buffDatas = new List<BuffData>();
        for (int i = 2; i <= dialogueRowCount; i++)
        {
            string tag = worksheet_Ch[i, 1].DisplayedText;
            if (tag != "")
            {
                BuffData currentBuffData = new BuffData();
                currentBuffData.Tag = worksheet_Ch[i, 1].DisplayedText;
                currentBuffData.IconName = worksheet_Ch[i, 2].DisplayedText;
                currentBuffData.Type = worksheet_Ch[i, 3].GetXlsData<int>();
                //写入中文表数据
                currentBuffData.Name[workbookName] = worksheet_Ch[i, 4].DisplayedText;
                currentBuffData.Text[workbookName] = worksheet_Ch[i, 5].DisplayedText;
                for (int j = 1; j <= sheetsCount-1; j++)
                {
                    Worksheet currentWorksheet = workbook.Worksheets[j];
                   string newWorkbookName = currentWorksheet.Name;
                    currentBuffData.Name[newWorkbookName] = currentWorksheet[i, 3].DisplayedText;
                    currentBuffData.Text[newWorkbookName] = currentWorksheet[i, 5].DisplayedText;
                }
                buffDatas.Add(currentBuffData);
            }
        }

        // 将解析结果写入文件
        File.WriteAllText(DirePath + @"\Buff.json", JsonConvert.SerializeObject(buffDatas, Formatting.Indented));
        Console.WriteLine("新Buff数据更新完毕");
    }
}


