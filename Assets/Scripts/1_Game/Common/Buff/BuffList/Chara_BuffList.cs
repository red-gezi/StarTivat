using System.Collections.Generic;

public partial class Chara_BuffList : BaseBuffList
{
    //buff总表
    public new static List<Buff> Buffs = new();
    public new static void Init()
    {
        Buffs = new()
        {
            new Buff()
            .RegisterName( Chara_BuffName.人物天赋1)
            .RegisterEvent<BattleEventData>(BuffTriggerType.On, BuffEventType.TurnStart,async (data) =>
            {
                var thisBuff=BuffSystem.GetBuff(Chara_BuffName.人物天赋1);
                thisBuff.SetFlag("lastTarget",null);
            })
            .RegisterEvent<SkillData>(BuffTriggerType.On, BuffEventType.Hit,async (data) =>
            {
                var thisBuff=BuffSystem.GetBuff(Chara_BuffName.人物天赋1);
                if (data.Receiver==thisBuff.GetFlag<Character>("lastTarget"))
                {
                    thisBuff.layers++;
                }
                else
                {
                    thisBuff.SetFlag("lastTarget",data.Receiver);
                    thisBuff.layers++;
                }
            })
        };
    }
}
