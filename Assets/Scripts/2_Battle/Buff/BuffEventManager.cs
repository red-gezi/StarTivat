using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class BuffEventManager
{
    #region 对外开放事件接口
    public static void BoracstEvent(BuffEventType eventType)
    {
        BoracstTargetEvent(eventType, new BattleEventData());
    }
    ////////////////////////////////////////////////////////////////////////局外////////////////////////////////////////////////////////////////////////
    //获得奇物,可指定id，-1代表随机
    public static async Task GetItem(IBaseBuffList buffList, List<int> buffIndex)
    {
        Debug.Log("调用指令获得物品");
        var data = new OutBattleEventData()
        {
            BelongBuffList = buffList,
            TargetBuffIndex = buffIndex,
            ListenerBuffs = OutBattleManager.GetCurrentBuff(),
        };
        await TriggerTargetEventAsync(BuffEventType.ItemGain, data);
    }
    public static async void GetRandomItem(IBaseBuffList buffList, int num, List<BuffType> buffTypes)
    {
        Debug.Log("调用指令获得物品");
        var data = new OutBattleEventData()
        {
            BelongBuffList = buffList,
            TargetBuffIndex = new List<int>(num),
            ListenerBuffs = OutBattleManager.GetCurrentBuff(),
        };
        await TriggerTargetEventAsync(BuffEventType.ItemGain, data);
    }
    public static async Task GetGoldAsync(int num)
    {
        Debug.Log("调用指令获得金钱" + num);
        var data = new OutBattleEventData()
        {
            TargetValue = num,
            ListenerBuffs = OutBattleManager.GetCurrentBuff(),
        };
        await TriggerTargetEventAsync(BuffEventType.GoldGain, data, async eventData =>
        {
            //算出真实事件
            //打开ui
            //await Task.Delay(1000);
            OutBattleEventData outBattleEventData = ((OutBattleEventData)eventData);

            outBattleEventData.AddLog($"已获得金币{outBattleEventData.TargetValue}");
            OutBattleManager.CurrentOutBattleInfo.Gold += outBattleEventData.TargetValue;
        });
    }
    ////////////////////////////////////////////////////////////////////////局内////////////////////////////////////////////////////////////////////////
    public static void CharaAction(Character character)
    {
        Debug.Log("调用指令获得物品");
        var data = new OutBattleEventData()
        {
            TargetBuffIndex = buffIndex,
            ListenerBuffs = OutBattleManager.GetCurrentBuff(),
        };
        await TriggerTargetEventAsync(BuffEventType.ItemGain, data, async eventData =>
        {
            //根据算出符合要求的真实的的buffid
            //将真正的目标buff
            //打开ui，显示获得的道具
            // 产生一个物品获得事件
            //角色获得道具
            await Task.Delay(1000);
            OutBattleEventData outBattleEventData = ((OutBattleEventData)eventData);
            foreach (int index in outBattleEventData.TargetBuffIndex)
            {
                Buff targetBuff = buffList.GetBuff(index);
                OutBattleManager.CurrentOutBattleInfo.AddBuff(targetBuff);
                outBattleEventData.TargetBuffs = new List<Buff> { targetBuff };
                outBattleEventData.AddLog($"已获得道具{(MoNiYuZhouBuffList.BufferName)targetBuff.id},尝试触发道具的获得效果");
                // 等待异步任务完成
                await TriggerTargetEventAsync(BuffEventType.ItemGainEffect, outBattleEventData);
            }
        });
    }

    #endregion
    #region 内部事件处理类型
    /// <summary>
    /// 广播类型事件：通知所有角色，遍历身上buff 触发某个种类事件，如回合开始通知角色身上的buff调用回合开始事件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="eventType"></param>
    /// <param name="data"></param>
    public static void BoracstTargetEvent<T>(BuffEventType eventType, T data) where T : GameEventData
    {
        BattleManager.CurrentBattle.charaList.ForEach(chara =>
        {
            chara.Buffs.ForEach(async buff =>
            {
                //广播通知所有角色检查是否有事件触发前相关连锁效果
                BattleManager.CurrentBattle.charaList.ForEach(chara =>
                {
                    chara.Buffs.ForEach(async buff =>
                    {
                        await buff.TriggerAsync(BuffTriggerType.Before, eventType, data);
                    });
                });
                await buff.TriggerAsync(BuffTriggerType.On, eventType, data);
                //广播通知所有角色检查是否有事件触发后相关连锁效果
                BattleManager.CurrentBattle.charaList.ForEach(chara =>
                {
                    chara.Buffs.ForEach(async buff =>
                    {
                        await buff.TriggerAsync(BuffTriggerType.After, eventType, data);
                    });
                });
            });
        });
    }
    /// <summary>
    /// 指定类型事件：目标buff生效后执行buff上的对应的事件，如获得某奇物时触发其获得事件
    ///触发多个目标的buff（技能/奇物/祝福/天赋）的某个事件效果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="eventType"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static async Task TriggerTargetEventAsync<T>(BuffEventType eventType, T data) where T : GameEventData
    {
        foreach (var targetBuff in data.TargetBuffs)
        {
            //Debug.Log($"触发{(MoNiYuZhouBuffList.BufferName)targetBuff.id}的{eventType}事件");
            foreach (var buff in data.ListenerBuffs)
            {
                await buff.TriggerAsync(BuffTriggerType.Before, eventType, data);
            }
            await targetBuff.TriggerAsync(BuffTriggerType.On, eventType, data);
            // After触发
            foreach (var buff in data.ListenerBuffs)
            {
                await buff.TriggerAsync(BuffTriggerType.After, eventType, data);
            }
        }
    }
    /// <summary>
    /// 指定类型事件：目标buff生效后执行buff执行固定流程事件，如角色死亡播放死亡动画，角色获得奇物打开获得ui界面等
    /// 触发目标buff（技能/奇物/祝福/天赋）的某个事件效果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="eventType"></param>
    /// <param name="data"></param>
    /// <param name="effect"></param>
    /// <returns></returns>
    public static async Task TriggerTargetEventAsync<T>(BuffEventType eventType, T data, Func<GameEventData, Task> effect) where T : GameEventData
    {
        //Debug.Log($"触发{eventType}的固定流程");
        //Debug.Log($"触发{eventType}的固定流程前");

        foreach (var buff in data.ListenerBuffs)
        {
            await buff.TriggerAsync(BuffTriggerType.Before, eventType, data);
        }
        //Debug.Log($"触发{eventType}的固定流程时");
        await effect.Invoke(data);
        // After触发
        //Debug.Log($"触发{eventType}的固定流程后");
        foreach (var buff in data.ListenerBuffs)
        {
            await buff.TriggerAsync(BuffTriggerType.After, eventType, data);
        }
    }
    #endregion

}
