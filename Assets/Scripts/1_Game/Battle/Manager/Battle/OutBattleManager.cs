using System.Collections.Generic;
using System.Linq;
public class OutBattleManager
{

    public static List<Buff> GetCurrentBuff() => new List<Buff>(GameManager.gameData.outBattleData.Buffs);
    //每局初始化一个新的
    public static void AddBuff(Buff buff)
    {
        GameManager.gameData.outBattleData.Buffs.Add(buff);
    }
    public static void RemoveBuff(Buff buff)
    {
        GameManager.gameData.outBattleData.Buffs.Remove(buff);
    }
    public static void ChangeGold(int count)
    {
        GameManager.gameData.outBattleData.Gold+=count;
    }
}
