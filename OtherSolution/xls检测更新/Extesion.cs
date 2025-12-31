using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Linq;
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