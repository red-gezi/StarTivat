using System;
using System.Collections.Generic;
using System.Linq;

public class OccurrenceCore
{
    //private static BaseOccurrenceList CurrentOccurrenceList { get; set; }
    //初始化,装载一个特定模式下事件的拷贝数据，可以在游戏过程中被修改
    private static Dictionary<Type, List<Occurrence>> AllOccurrenceList { get; set; } = new();
    public static void AddOccurrenceList(Type occurrenceName, List<Occurrence> occurrenceList)
    {
        AllOccurrenceList.Add(occurrenceName, occurrenceList.Clone());
    }
    //查询
    public static Occurrence GetOccurrence<T>(T occurrenceName) where T : Enum
    {
        int ID = Convert.ToInt32(occurrenceName);
        if (!AllOccurrenceList.ContainsKey(typeof(T)))
        {
            Log.Show("总事件列表不包含该事件枚举类型,请在上方代码注册");
            return null;
        }
        var currentOccurrenceList = AllOccurrenceList[typeof(T)];
        return currentOccurrenceList.FirstOrDefault(occurrence => occurrence.ID == ID).Clone();
    }
}
