using System.Collections.Generic;
using System.Linq;
//局外玩家的数据列表
public class OutBattleManager
{

    public static OutBattleManager CurrentOutBattleInfo { get; set; }
    public int Gold { get; set; } = 0;
    public List<Buff> Buffs { get; set; } = new();
    public static List<Buff> GetCurrentBuff() => new List<Buff>(CurrentOutBattleInfo.Buffs);
    //每局初始化一个新的
    public static void Init() => CurrentOutBattleInfo = new OutBattleManager();
    public void AddBuff(Buff buff)
    {
        Buffs.Add(buff);
    }
    public void RemoveBuff(Buff buff)
    {
        Buffs.Remove(buff);
    }
}
