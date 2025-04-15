using System.Collections.Generic;

public class BattleEventData : GameEventData
{
    Buff TriggerBuffer { get; set; }
    public Character Sender { get; set; }
    public Character Target { get; set; }

    public List<Character> DefaultTargets { get; set; }
    //锁定目标无法更改
    public bool IsLockTarget { get; set; }
    //生效目标是否是敌人
    public bool TargetIsEnemy { get; set; }
    public int TargetID { get; set; }
}