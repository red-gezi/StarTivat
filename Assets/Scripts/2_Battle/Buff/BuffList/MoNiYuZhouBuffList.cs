using System.Collections.Generic;
using System.Linq;
public class MoNiYuZhouBuffList : IBaseBuffList
{
    public static MoNiYuZhouBuffList BuffList { get; set; }
    public static void Init() => BuffList = new();
    public Buff GetBuff(int bufferId) => Buffs.FirstOrDefault(buff => buff.id == bufferId).Clone();
    public enum BufferName
    {
        获得金币翻倍,
        每次获得奇物后获得30金币,
        获得1个金币翻倍奇物,
        获得奇物数量加一,
        空奇物,
        获得金钱时若金钱大于50则减50并获得欠条,
        欠条,
        //祝福
        祝福_琥珀,
        祝福_苹果酿,
        祝福_妙论派,
        //奇物
        奇物_相遇之缘,
        奇物_纠缠之缘,
        奇物_原石,

    }
    //buff总表
    public List<Buff> Buffs = new()
    {
        new Buff((int)BufferName.获得金币翻倍){rank=1}
        .Register<OutBattleEventData>( BuffTriggerType.Before, BuffEventType.GoldGain, async (data)=>
        {
            data.TargetValue*=2;
            data.AddLog("获得金币修改为"+data.TargetValue);
            //await Task.Delay(1000);
        }),

        new Buff((int)BufferName.每次获得奇物后获得30金币)
        .Register <OutBattleEventData>( BuffTriggerType.After, BuffEventType.ItemGain,async (data)=>
        {
            await GameEventManager.GetGoldAsync(30);
            data.AddLog("获得了碎片" + 30);
        }),

        new Buff((int)BufferName.获得1个金币翻倍奇物)
        .Register<OutBattleEventData>(BuffTriggerType.On,  BuffEventType.ItemGainEffect,async (data)=>
        {
            //await Task.Delay(2000);
            var targets =new List<int>
            {
                (int)MoNiYuZhouBuffList.BufferName.获得金币翻倍,
            };
            await GameEventManager.GetItem(MoNiYuZhouBuffList.BuffList,targets);
            data.AddLog("获得奇物——获得金币翻倍");
            //await Task.Delay(2000);
        }),

        new Buff((int)BufferName.获得奇物数量加一)
        .Register<OutBattleEventData>(BuffTriggerType.Before,  BuffEventType.ItemGain,async (data)=>
        {
            data.AddLog("获得奇物数量加一");
        }),

        new Buff((int)BufferName.空奇物)
        .Register<OutBattleEventData>(BuffTriggerType.Before,  BuffEventType.ItemGain,async (data)=>
        {
            data.AddLog("获得奇物——白板奇物");
        }),

        new Buff((int)BufferName.欠条)
        .Register<OutBattleEventData>(BuffTriggerType.Before,  BuffEventType.ItemGain,async (data)=>
        {
            data.AddLog("获得奇物——欠条");
        }),

        new Buff((int)BufferName.获得金钱时若金钱大于50则减50并获得欠条)
        .Register<OutBattleEventData>(BuffTriggerType.Before,  BuffEventType.GoldGain,async (data)=>
        {
            if (data.TargetValue>50)
            {
                data.TargetValue-=50;
                var targets =new List<int>
                {
                    (int)MoNiYuZhouBuffList.BufferName.欠条,
                };
                await GameEventManager.GetItem(MoNiYuZhouBuffList.BuffList,targets);
                data.AddLog("获得的金钱-50并得到欠条");
            }
        }),


        new Buff((int)BufferName.祝福_妙论派,ElementType.Herb,3, "妙论派","元素反应加成提高<color=yellow>15%</color>")
        .Register<OutBattleEventData>(BuffTriggerType.On,  BuffEventType.ItemGain,async (data)=>
        {
        }),
        new Buff((int)BufferName.祝福_琥珀,ElementType.Geo,2, "琥珀","护盾自然破碎时产生产生共振，对对方全体造成<color=yellow>200%</color>盾量伤害")
        .Register<OutBattleEventData>(BuffTriggerType.Before,  BuffEventType.ItemGain,async (data)=>
        {
        }),
        new Buff((int)BufferName.祝福_苹果酿,ElementType.Anemo,1, "苹果酿","速度提高<color=yellow>20%</color>")
        .Register<OutBattleEventData>(BuffTriggerType.Before,  BuffEventType.ItemGain,async (data)=>
        {
        }),

        new Buff((int)BufferName.奇物_相遇之缘, CurioType.相遇之缘,2, "相遇之缘","能带来奇迹的等价交换之物")
        .Register<OutBattleEventData>(BuffTriggerType.Before,  BuffEventType.ItemGain,async (data)=>
        {
        }),
          new Buff((int)BufferName.奇物_纠缠之缘, CurioType.纠缠之缘,3, "纠缠之缘","能带来没有任何作用的奇迹的等价交换之物")
        .Register<OutBattleEventData>(BuffTriggerType.Before,  BuffEventType.ItemGain,async (data)=>
        {
        }),
            new Buff((int)BufferName.奇物_原石, CurioType.原石,1, "原石","财富的象征")
        .Register<OutBattleEventData>(BuffTriggerType.Before,  BuffEventType.ItemGain,async (data)=>
        {
        }),
    };
}