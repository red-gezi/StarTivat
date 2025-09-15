using System.Collections.Generic;
public class OutBattleManager
{

    public static List<Buff> GetCurrentBuff() => new(GameManager.gameData.CurrentOutBattleData.Buffs);
    //每局初始化一个新的
    public static void AddBuff(Buff buff)
    {
        GameManager.gameData.CurrentOutBattleData.Buffs.Add(buff);
    }
    public static void RemoveBuff(Buff buff)
    {
        GameManager.gameData.CurrentOutBattleData.Buffs.Remove(buff);
    }
    public static void ChangeGold(int count)
    {
        GameManager.gameData.CurrentOutBattleData.Gold += count;
    }
}
