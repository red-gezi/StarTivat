using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class OutBattleBuffList : IBaseBuffList
{
    public static OutBattleBuffList BuffList { get; set; }
    public static void Init() => BuffList = new();
    public Buff GetBuff(int bufferId) => Buffs.FirstOrDefault(buff => buff.id == bufferId).Clone();
    public enum BufferName
    {
        基础局外事件

    }
    //buff总表
    public List<Buff> Buffs = new()
    {
        new Buff((int)BufferName.基础局外事件){rank=1}
            .Register<OutBattleEventData>( BuffTriggerType.On, BuffEventType.ItemGain, async eventData=>
            {
                //根据算出符合要求的真实的的buffid
                //将真正的目标buff
                //打开ui，显示获得的道具
                // 产生一个物品获得事件
                //角色获得道具
                await Task.Delay(1000);
                foreach (int index in eventData.TargetBuffIndex)
                {
                    Buff targetBuff = eventData.BelongBuffList.GetBuff(index);
                    OutBattleManager.CurrentOutBattleInfo.AddBuff(targetBuff);
                    eventData.TargetBuffs = new List<Buff> { targetBuff };
                    eventData.AddLog($"已获得道具{(MoNiYuZhouBuffList.BufferName)targetBuff.id},尝试触发道具的获得效果");
                    // 等待异步任务完成
                    await BuffEventManager.TriggerTargetEventAsync(BuffEventType.ItemGainEffect, eventData);
                }
            })
            .Register<OutBattleEventData>( BuffTriggerType.On, BuffEventType.ItemGain, async eventData=>
            {
                    //根据算出符合要求的真实的的buffid
                //将真正的目标buff
                //打开ui，显示获得的道具
                // 产生一个物品获得事件
                //角色获得道具
                await Task.Delay(1000);
                foreach (int index in eventData.TargetBuffIndex)
                {
                    Buff targetBuff = eventData.BelongBuffList.GetBuff(index);
                    OutBattleManager.CurrentOutBattleInfo.AddBuff(targetBuff);
                    eventData.TargetBuffs = new List<Buff> { targetBuff };
                    eventData.AddLog($"已获得道具{(MoNiYuZhouBuffList.BufferName)targetBuff.id},尝试触发道具的获得效果");
                    // 等待异步任务完成
                    await BuffEventManager.TriggerTargetEventAsync(BuffEventType.ItemGainEffect, eventData);
                }
            })
    };
}