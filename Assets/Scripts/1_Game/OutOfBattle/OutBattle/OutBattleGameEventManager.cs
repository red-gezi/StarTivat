using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class OutOfBattleGameEventManager
{
    ////////////////////////////////////////////////////////////////////////局外////////////////////////////////////////////////////////////////////////
    public static async Task EnterRoom(RoomData roomData)
    {
        Debug.Log("进入房间");
        roomData.BelongBuffList = GameManager.gameData.CurrentBuffList;
        roomData.ListenerBuffs = OutOfBattleManager.GetCurrentBuff();
        roomData.exceBuff = GameManager.BaseBuff;
        await GameEventManager.TriggerTargetEventAsync(BuffEventType.EnterRoom, roomData);
    }
    public static async Task DestoryObject(RoomData roomData)
    {
        Debug.Log("破坏物体");
        roomData.BelongBuffList = GameManager.gameData.CurrentBuffList;
        roomData.ListenerBuffs = OutOfBattleManager.GetCurrentBuff();
        roomData.exceBuff = GameManager.BaseBuff;
        await GameEventManager.TriggerTargetEventAsync(BuffEventType.DestoryObject, roomData);
    }
    //获得奇物,可指定id，-1代表随机
    public static async Task GetItem(IBaseBuffList buffList, List<int> buffIndex)
    {
        Debug.Log("调用指令获得物品");
        var data = new OutBattleEventData()
        {
            BelongBuffList = buffList,
            TargetBuffIndex = buffIndex,
            ListenerBuffs = OutOfBattleManager.GetCurrentBuff(),
            exceBuff = GameManager.BaseBuff,
        };
        await GameEventManager.TriggerTargetEventAsync(BuffEventType.ItemGain, data);
    }
    public static async void GetRandomItem(IBaseBuffList buffList, int num, List<BuffTag> buffTypes)
    {
        Debug.Log("调用指令获得物品");
        var data = new OutBattleEventData()
        {
            BelongBuffList = buffList,
            TargetBuffIndex = new List<int>(num),
            ListenerBuffs = OutOfBattleManager.GetCurrentBuff(),
            exceBuff = GameManager.BaseBuff,
        };
        await GameEventManager.TriggerTargetEventAsync(BuffEventType.ItemGain, data);
    }
    public static async Task GetGoldAsync(int num)
    {
        Debug.Log("调用指令获得金钱" + num);
        var data = new OutBattleEventData()
        {
            TargetValue = num,
            ListenerBuffs = OutOfBattleManager.GetCurrentBuff(),
            exceBuff = GameManager.BaseBuff,
        };
        await GameEventManager.TriggerTargetEventAsync(BuffEventType.GoldGain, data);
    }
}
