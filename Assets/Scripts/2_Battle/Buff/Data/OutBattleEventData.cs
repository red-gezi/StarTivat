using System.Collections.Generic;
//对局外事件
//事件类型
public partial class OutBattleEventData : GameEventData
{
    //当前触发的buff
    Buff TriggerBuff { get; set; }
    //buff的所属系列
    public IBaseBuffList BelongBuffList { get; set; }
    public List<int> TargetBuffIndex { get; set; }
    public List<Character> DefaultTargets { get; set; }
    public  List<string> TargetTags{  get; set; }
    public int TargetValue {  get; set; }
}
