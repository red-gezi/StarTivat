using System.Collections.Generic;
//局外玩家的数据列表
public class OutOfBattleData
{
    public int Gold { get; set; } = 0;
    /// <summary>
    /// 局外全局已拥有buff
    /// </summary>
    public List<Buff> Buffs { get; set; } = new();
    //房间概率



}
