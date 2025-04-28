using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class GameEventManager
{
    #region 对外开放事件接口
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
            exceBuff = BaseBuffList.BuffList.GetBuff((int)BaseBuffList.BufferName.BaseBuff),
        };
        await TriggerTargetEventAsync(BuffEventType.ItemGain, data);
    }
    public static async void GetRandomItem(IBaseBuffList buffList, int num, List<BuffTag> buffTypes)
    {
        Debug.Log("调用指令获得物品");
        var data = new OutBattleEventData()
        {
            BelongBuffList = buffList,
            TargetBuffIndex = new List<int>(num),
            ListenerBuffs = OutBattleManager.GetCurrentBuff(),
            exceBuff = BaseBuffList.BuffList.GetBuff((int)BaseBuffList.BufferName.BaseBuff),
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
            exceBuff = BaseBuffList.BuffList.GetBuff((int)BaseBuffList.BufferName.BaseBuff),
        };
        await TriggerTargetEventAsync(BuffEventType.GoldGain, data);
    }
    ////////////////////////////////////////////////////////////////////////局内////////////////////////////////////////////////////////////////////////

    public static async Task<CharaData> GetCurrentCharaData(Character character)
    {
        Debug.Log("查看当前面板数值");
        var data = new CharaData()
        {
            //TargetBuffIndex = buffIndex,
            ListenerBuffs = OutBattleManager.GetCurrentBuff(),
        };
        await TriggerAllEventAsync(BuffEventType.GetCurrentCharaData, data);
        return data;
    }
    public static async Task SendSkillData(SkillData skillData)
    {
        Debug.Log("发送技能数据给对面");
        skillData.ListenerBuffs = skillData.Sender.Buffs;
        skillData.exceBuff = BaseBuffList.BuffList.GetBuff((int)BaseBuffList.BufferName.BaseBuff);
        skillData.AddLog($"发送技能数据给{skillData.Receiver.name}");
        await TriggerAllEventAsync(BuffEventType.SendSkillData, skillData);
        return;
    }
    public static async Task ReceiveSkillData(SkillData skillData)
    {
        if (skillData.Receiver == null)
        {
            return;
        }
        Debug.Log("接收对方发送的技能数据");
        skillData.AddLog($"接收技能数据给{skillData.Receiver.name}");
        await TriggerAllEventAsync(BuffEventType.ReceiveSkillData, skillData);
        return;
    }
    public static async Task ElementalReaction(ElementalReactionData data)
    {
        if (data.Receiver == null)
        {
            return;
        }
        Debug.Log("接收对方发送的技能数据");
        data.AddLog($"接收技能数据给{data.Receiver.name}");
        await TriggerAllEventAsync(BuffEventType.ReceiveSkillData, data);
        return;
    }
    

    #endregion
    #region 内部事件处理类型
    /// <summary>
    /// 指定Buff类型事件：触发框定范围内所有buff某事件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="eventType"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static async Task TriggerAllEventAsync<T>(BuffEventType eventType, T data) where T : GameEventData
    {
        foreach (var targetBuff in data.TargetBuffs)
        {
            //如果目标范围内buff含有触发事件
            if (targetBuff.HasEvent(BuffTriggerType.On, eventType))
            {
                data.exceBuff = targetBuff;
                await TriggerTargetEventAsync(eventType, data);
            }
        }
    }
    /// <summary>
    /// 指定Buff类型事件：触发特定buff某事件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="eventType"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static async Task TriggerTargetEventAsync<T>(BuffEventType eventType, T data) where T : GameEventData
    {

        //Debug.Log($"触发{(MoNiYuZhouBuffList.BufferName)targetBuff.id}的{eventType}事件");
        foreach (var buff in data.ListenerBuffs)
        {
            await buff.TriggerAsync(BuffTriggerType.Before, eventType, data);
        }
        await data.exceBuff.TriggerAsync(BuffTriggerType.On, eventType, data);
        // After触发
        foreach (var buff in data.ListenerBuffs)
        {
            await buff.TriggerAsync(BuffTriggerType.After, eventType, data);
        }
    }
    #endregion

}
