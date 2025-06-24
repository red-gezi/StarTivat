using System.Collections.Generic;
//对局外事件
//事件类型
public partial class OutBattleEventData : EventData
{
    //当前触发的buff
    Buff TriggerBuff { get; set; }
    //buff的所属系列
    public List<int> TargetBuffIndex { get; set; }
    public List<Character> DefaultTargets { get; set; }
    public List<string> TargetTags { get; set; }
    public int TargetValue { get; set; }

    Dictionary<RoomType, float> RoomWight { get; set; } = new()
    {
        { RoomType.EventRoom,1 },
        { RoomType.BattleRoom,1 },
    };
}
