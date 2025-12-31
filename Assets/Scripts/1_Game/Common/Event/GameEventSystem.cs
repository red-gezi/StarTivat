using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class GameEventSystem
{
    public static async Task<T> Test<T>(BuffEventType buffEventType, T data) where T : EventData
    {
        Log.Show("测试开始");
        await GameEventCore.TriggerEventAsync(buffEventType, data);
        return data;
    }
    ////////////////////////////////////////////////////////////////////////局外////////////////////////////////////////////////////////////////////////
    public static async Task EnterRoom(RoomData roomData)
    {
        //roomData.BelongBuffList = GameDataSystem.GetGameData().CurrentBuffList;
        //roomData.ListenerBuffs = OutOfBattleManager.GetCurrentBuff();
        Debug.Log("进入房间");
        roomData.ListenerBuffs = GameDataSystem.GetCurrentBuff();
        roomData.ExceBuff = GameDataSystem.GetBaseBuff();
        await GameEventCore.TriggerEventAsync(BuffEventType.EnterRoom, roomData);
    }
    public static async Task DestoryObject(RoomData roomData)
    {
        Debug.Log("破坏物体");
        //roomData.BelongBuffList = GameDataSystem.GetGameData().CurrentBuffList;
        roomData.ListenerBuffs = OutOfBattleSystem.GetCurrentBuff();
        roomData.ExceBuff = GameDataSystem.GetBaseBuff();
        await GameEventCore.TriggerEventAsync(BuffEventType.DestoryObject, roomData);
    }
    //获得奇物,可指定id，-1代表随机
    public static async Task GetItem(params object[] buffIndex)
    {
        Debug.Log("调用指令获得物品");
        var data = new OutBattleEventData()
        {
            //BelongBuffList = buffList,
            //TargetBuffIndex = buffIndex,
            TargetItem = buffIndex.ToList(),
            ListenerBuffs = OutOfBattleSystem.GetCurrentBuff(),
            ExceBuff = GameDataSystem.GetBaseBuff(),
        };
        await GameEventCore.TriggerEventAsync(BuffEventType.ItemGain, data);
    }
    public static async void GetRandomItem(BaseBuffList buffList, int num, List<BuffTag> buffTypes)
    {
        Debug.Log("调用指令获得物品");
        var data = new OutBattleEventData()
        {
            //BelongBuffList = buffList,
            TargetBuffIndex = new List<int>(num),
            ListenerBuffs = OutOfBattleSystem.GetCurrentBuff(),
            ExceBuff = GameDataSystem.GetBaseBuff(),
        };
        await GameEventCore.TriggerEventAsync(BuffEventType.ItemGain, data);
    }
    public static async Task GetGoldAsync(int num)
    {
        Debug.Log("调用指令获得金钱" + num);
        var data = new OutBattleEventData()
        {
            TargetValue = num,
            ListenerBuffs = OutOfBattleSystem.GetCurrentBuff(),
            ExceBuff = GameDataSystem.GetBaseBuff(),
            TargetItem = new() { SU_BuffName.基础流程, SU_BuffName.获得金币翻倍, Chara_BuffName.人物天赋1 }
        };
        await GameEventCore.TriggerEventAsync(BuffEventType.GoldGain, data);
    }
    ////////////////////////////////////////////////////////////////////////局内////////////////////////////////////////////////////////////////////////
    public static async Task<InBattleEventData> BattleStart()
    {
        Log.Show("开始");
        var data = new InBattleEventData()
        {
            ListenerBuffs = InBattleSystem.GetAllInBattleBuffs(),
            ExceBuff = InBattleSystem.GetBaseBuff(),

        };
        await GameEventCore.TriggerEventAsync(BuffEventType.BattleStart, data);
        return data;
    }



    public static async Task<CharaData> GetCurrentCharaData(Character character)
    {
        Debug.Log("查看当前面板数值");
        var data = new CharaData()
        {
            //TargetBuffIndex = buffIndex,
            Target = character,
            ListenerBuffs = character.GetCurrentBuffs(),
            ExceBuff = InBattleSystem.GetBaseBuff(),
        };
        await GameEventCore.TriggerEventAsync(BuffEventType.GetCurrentCharaData, data);
        return data;
    }
    public static async Task SendSkillData(SkillData skillData)
    {
        Debug.Log("发送技能数据给对面");
        skillData.ListenerBuffs = skillData.Sender.GetCurrentBuffs();
        skillData.ExceBuff = InBattleSystem.GetBaseBuff();
        skillData.AddLog($"发送技能数据给{skillData.Receiver.name}");
        await GameEventCore.TriggerEventAsync(BuffEventType.SendSkillData, skillData);
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
        await GameEventCore.TriggerEventAsync(BuffEventType.ReceiveSkillData, skillData);
        return;
    }
    public static async Task ElementalReaction(ElementalReactionData data)
    {

        await GameEventCore.TriggerEventAsync(BuffEventType.ElementalReaction, data);
        return;
    }
    //通用的广播角色某个事件
    public static async Task BroadcastCharaEvent(BuffEventType eventType, SkillData skillData)
    {

        await GameEventCore.TriggerEventAsync(eventType, skillData);
        return;
    }
}
