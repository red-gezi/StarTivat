using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [ShowInInspector]
    public Color BattleColor;
    public Color EventColor;
    public Color EndColor = new Color(1.5f, 1.5f, 1.5f);
    // Start is called before the first frame update
    RoomConfigData RoomConfigData;
    //房间初始化时配置门信息
    public void InitDoor(RoomConfigData RoomConfigData)
    {
        //初始化数据
        this.RoomConfigData = RoomConfigData;
        //颜色
        //图标
    }
    public async void GoToNextRoom()
    {
        await GameEventManager.EnterRoom(RoomConfigData);

    }
}
