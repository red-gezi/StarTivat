using System.Threading.Tasks;
using UnityEngine;

public class BattleGameEventManager
{
    ////////////////////////////////////////////////////////////////////////局内////////////////////////////////////////////////////////////////////////

    public static async Task<CharaData> GetCurrentCharaData(Character character)
    {
        Debug.Log("查看当前面板数值");
        var data = new CharaData()
        {
            //TargetBuffIndex = buffIndex,
            ListenerBuffs = OutBattleManager.GetCurrentBuff(),
        };
        await GameEventManager.TriggerAllEventAsync(BuffEventType.GetCurrentCharaData, data);
        return data;
    }
    public static async Task SendSkillData(SkillData skillData)
    {
        Debug.Log("发送技能数据给对面");
        skillData.ListenerBuffs = skillData.Sender.Buffs;
        skillData.exceBuff = GameManager.BaseBuff;
        skillData.AddLog($"发送技能数据给{skillData.Receiver.name}");
        await GameEventManager.TriggerAllEventAsync(BuffEventType.SendSkillData, skillData);
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
        await GameEventManager.TriggerAllEventAsync(BuffEventType.ReceiveSkillData, skillData);
        return;
    }
    public static async Task ElementalReaction(ElementalReactionData data)
    {

        await GameEventManager.TriggerTargetEventAsync(BuffEventType.ElementalReaction, data);
        return;
    }
    //通用的广播角色某个事件
    public static async Task BroadcastCharaEvent(BuffEventType eventType, SkillData skillData)
    {

        await GameEventManager.TriggerTargetEventAsync(eventType, skillData);
        return;
    }
}
