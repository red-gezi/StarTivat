using Codice.Client.Common;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class RoomManager : MonoBehaviour
{
    //RoomManager Instance;
    //private void Awake() => Instance = this;
    //固定房间模板
    static List<RoomConfigData> roomConfigs = new()
    {
        new RoomConfigData()
            {
                SelectableRoomType = RoomType.InitRoom,
                SelectableRoomTag = RoomTag.None,
                SelectableSceneModel = new (){SceneModelType.教令院 },
            },
        new RoomConfigData()
            {
                SelectableRoomType = RoomType.EliteRoom,
                SelectableRoomTag = RoomTag.None,
                SelectableSceneModel = new (){SceneModelType.椛染之庭, SceneModelType.西风教堂, SceneModelType.西风骑士团},
            },
        new RoomConfigData()
            {
                SelectableRoomType = RoomType.BoosRoom,
                SelectableRoomTag = RoomTag.None,
                SelectableSceneModel = new (){ SceneModelType.椛染之庭},
                DoorCount=new (){1},
                MonstertCount=new (){1},

            },
    };
    //重置当前房间为初始大厅
    public static void ResetRoomConfigData()
    {
        GameManager.gameData.RoomDatas = new();
        EnterRoom(0, RoomType.InitRoom);
    }
    //获得第n层特定类型随机房间数据
    public static void AddRoom(int layer, RoomType roomType)
    {
        //根据类型随机获得指定类型下某个房间配置
        RoomConfigData RoomConfigData = roomConfigs
              .Where(room => room.SelectableRoomType == roomType)
              .OrderBy(x => UnityEngine.Random.Range(0, 1f))
              .FirstOrDefault();
        //根据层数来决定房间的传送门
        //RoomConfigData RoomConfigData = layer switch
        //{
        //    //初始房间
        //    0 => GetRandomRoom(RoomType.InitRoom),
        //    //精英房间
        //    7 => GetRandomRoom(RoomType.EliteRoom),
        //    //boss房间
        //    13 => GetRandomRoom(RoomType.BoosRoom),
        //    //随机房间
        //    _ => new RoomConfigData(),
        //};
        //根据配置文件生成房间信息
        GameManager.gameData.RoomDatas.Add(RoomConfigData);
        //保存改动后数据
        GameManager.Save();
    }
    //根据房间信息刷新场地模型
    public static void RefreshRoom()
    {
        var currentRoom = GameManager.gameData.RoomDatas.Last();
        //配置地图
        SceneModelManager.Instance.SwitchScreenModel(currentRoom.CurrentSceneModel);
        //配置房间道具
        //配置怪物
        //配置门
        switch (currentRoom.OutDoorType.Count)
        {
            case 1:




                break;
            default: Debug.LogError($"传送门数量错误,当前数量{currentRoom.OutDoorType.Count},请纠正"); break;
        }
        //触发事件
    }
    public static void EnterRoom(int layer, RoomType roomType)
    {
        AddRoom(layer, roomType);
        RefreshRoom();
    }
    //战斗结束,返回房间原位
    internal static void ReturnRoom()
    {
        throw new NotImplementedException();
    }
}
