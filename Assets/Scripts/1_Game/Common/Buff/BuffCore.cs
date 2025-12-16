using System;
using System.Collections.Generic;
using System.Linq;

public class BuffCore
{
    //初始化,装载一个特定模式下buff的拷贝数据，可以在游戏过程中被修改
    private static Dictionary<Type, List<Buff>> AllBuffList { get; set; } = new();
    public static void AddBuffList(Type buffName, List<Buff> buffList)
    {
      var s=  new List<Buff>(buffList);
        AllBuffList.Add(buffName, buffList);
    }
    //查询
    public static Buff GetBuff<T>(T buffName) where T : Enum
    {
        int ID = Convert.ToInt32(buffName);
        if (!AllBuffList.ContainsKey(typeof(T)))
        {
            Log.Show("总事件列表不包含该buff枚举类型,请在上方代码注册");
            return null;
        }
        var currentBuffList = AllBuffList[typeof(T)];
        return currentBuffList.FirstOrDefault(buff => buff.ID == ID).Clone();
    }
}
