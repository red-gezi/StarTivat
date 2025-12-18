using System.Collections.Generic;

public partial class Chara_BuffList : BaseBuffList
{
    //buff总表
    public new static List<Buff> Buffs = new();
    public static new void Init()
    {
        Buffs = new()
        {
            new Buff()
            .RegisterName( Chara_BuffName.人物天赋1)
            .RegisterEvent<InBattleEventData>(BuffTriggerType.After, BuffEventType.BattleStart,async (data) =>
            {
                data.ThisBuff.SetFlag("lastTarget",data.Receiver);
            })
            .RegisterEvent<SkillData>(BuffTriggerType.After, BuffEventType.Hit,async (data) =>
            {
                
                if (data.Receiver==data.ThisBuff.GetFlag<Character>("lastTarget"))
                {
                    data.ThisBuff.layers++;
                }
                else
                {
                    data.ThisBuff.SetFlag("lastTarget",data.Receiver);
                    data.ThisBuff.layers=1;
                }
            })
        };
    }
}
