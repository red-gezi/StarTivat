using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public partial class RoomSystem : InstanceBehaviour<RoomSystem>
{
    public List<Sprite> roomIcons;
    public static Texture2D GetRoomIcon(RoomType roomType)
    {
        var icon = roomType switch
        {
            RoomType.InitRoom => Instance.roomIcons.FirstOrDefault(icon => icon.name == "空"),
            RoomType.StartRoom => Instance.roomIcons.FirstOrDefault(icon => icon.name == "空"),
            RoomType.BattleRoom => Instance.roomIcons.FirstOrDefault(icon => icon.name == "战斗"),
            RoomType.EncounterRoom => Instance.roomIcons.FirstOrDefault(icon => icon.name == "遭遇"),
            RoomType.EliteRoom => Instance.roomIcons.FirstOrDefault(icon => icon.name == "遭遇"),
            RoomType.BossRoom => Instance.roomIcons.FirstOrDefault(icon => icon.name == "首领"),
            RoomType.EventRoom => Instance.roomIcons.FirstOrDefault(icon => icon.name == "事件"),
            RoomType.RewardRoom => Instance.roomIcons.FirstOrDefault(icon => icon.name == "奖励"),
            RoomType.ShopRoom => Instance.roomIcons.FirstOrDefault(icon => icon.name == "商店"),
            RoomType.GameRoom => Instance.roomIcons.FirstOrDefault(icon => icon.name == "游戏"),
            RoomType.MiracleRoom => Instance.roomIcons.FirstOrDefault(icon => icon.name == "空"),
            RoomType.RestRoom => Instance.roomIcons.FirstOrDefault(icon => icon.name == "修整"),
            _ => null, // 默认情况返回 null
        };
        if (icon == null)
        {
            Log.Show($"无法找到{roomType}房间图标", 2);
        }
        return icon?.texture;
    }
    //固定的房间模板
    static List<RoomConfigData> roomConfigs = new()
    {
        new RoomConfigData()
        {
            CurrentRoomType = RoomType.InitRoom,
            SelectableSceneModel = new (){SceneModelType.教令院 },
            DoorCount=new (){0},
        }
        .SetEnemyTag(RoomTag.None, RoomTag.Double,RoomTag.Select,RoomTag.Intensify),
        new RoomConfigData()
        {
            CurrentRoomType = RoomType.BattleRoom,
            SelectableSceneModel = new (){SceneModelType.椛染之庭, SceneModelType.西风教堂, SceneModelType.西风骑士团},
            DoorCount=new (){2,3},
        }
        .SetEnemyTag( RoomTag.EnemyCount1,RoomTag.EnemyCount2,RoomTag.EnemyCount3),
        new RoomConfigData()
        {
            CurrentRoomType = RoomType.EventRoom,
            SelectableSceneModel = new (){SceneModelType.椛染之庭, SceneModelType.西风教堂, SceneModelType.西风骑士团},
            DoorCount=new (){2,3},
            MonstertCount=new (){1},
        }
        .SetEnemyTag(RoomTag.None, RoomTag.Double,RoomTag.Select,RoomTag.Intensify),
        new RoomConfigData()
        {
            CurrentRoomType = RoomType.EliteRoom,
            SelectableSceneModel = new (){SceneModelType.椛染之庭, SceneModelType.西风教堂, SceneModelType.西风骑士团},
            DoorCount=new (){1,2,3},
        }
        .SetEnemyTag(RoomTag.One),
        new RoomConfigData()
        {
            CurrentRoomType = RoomType.BossRoom,
            SelectableSceneModel = new (){ SceneModelType.椛染之庭},
            DoorCount=new (){2},
            MonstertCount=new (){1},
        }
        .SetEnemyTag(RoomTag.One),
          // 新增StartRoom配置
    new RoomConfigData()
    {
        CurrentRoomType = RoomType.StartRoom,
        SelectableSceneModel = new (){SceneModelType.教令院},
        DoorCount=new (){1,2},
    }
    .SetEnemyTag(RoomTag.Select, RoomTag.Intensify),
    
    // 新增EncounterRoom配置
    new RoomConfigData()
    {
        CurrentRoomType = RoomType.EncounterRoom,
        SelectableSceneModel = new (){SceneModelType.椛染之庭, SceneModelType.西风教堂},
        DoorCount=new (){2,3},
    }
    .SetEnemyTag(RoomTag.EnemyCount1, RoomTag.Select),
    
    // 新增RewardRoom配置
    new RoomConfigData()
    {
        CurrentRoomType = RoomType.RewardRoom,
        SelectableSceneModel = new (){SceneModelType.教令院, SceneModelType.西风骑士团},
        DoorCount=new (){1},
    }
    .SetEnemyTag(RoomTag.None, RoomTag.Double),
    
    // 新增ShopRoom配置
    new RoomConfigData()
    {
        CurrentRoomType = RoomType.ShopRoom,
        SelectableSceneModel = new (){SceneModelType.教令院},
        DoorCount=new (){1},
    }
    .SetEnemyTag(RoomTag.None, RoomTag.Select),
    
    // 新增GameRoom配置
    new RoomConfigData()
    {
        CurrentRoomType = RoomType.GameRoom,
        SelectableSceneModel = new (){SceneModelType.椛染之庭},
        DoorCount=new (){1,2},
    }
    .SetEnemyTag(RoomTag.Intensify, RoomTag.Double),
    
    // 新增MiracleRoom配置
    new RoomConfigData()
    {
        CurrentRoomType = RoomType.MiracleRoom,
        SelectableSceneModel = new (){SceneModelType.西风教堂},
        DoorCount=new (){1},
    }
    .SetEnemyTag(RoomTag.None, RoomTag.Select),
    
    // 新增RestRoom配置
    new RoomConfigData()
    {
        CurrentRoomType = RoomType.RestRoom,
        SelectableSceneModel = new (){SceneModelType.教令院, SceneModelType.西风骑士团},
        DoorCount=new (){1},
    }
    .SetEnemyTag(RoomTag.None, RoomTag.Double)
    };

    //获得特定类型随机房间数据
    public static RoomConfigData GetRoomConfig(RoomType roomType)
    {
        //随机拿个指定房间信息的深拷贝
        RoomConfigData targetRoomConfigData = roomConfigs
              .Where(room => room.CurrentRoomType == roomType)
              .OrderBy(x => UnityEngine.Random.Range(0, 1f))
              .FirstOrDefault()?.Clone();
        if (targetRoomConfigData == null)
        {
            Debug.LogError($"无法找到{roomType}类型的房间,请补充");
        }
        return targetRoomConfigData;
    }
    public static async Task EnterRoom(RoomConfigData roomConfigData)
    {
        //获取当前层数
        int layer = GameDataSystem.GetGameData().CurrentLayer;
        Log.Show($"进入第{layer}层房间", 0);
        await GameEventSystem.EnterRoom(new RoomData(layer, roomConfigData));
        RefreshRoomModel();
    }
    public static RoomData ReConfigRoomData(RoomData roomData)
    {
        ////如果不存在强制指定的？
        //if (roomData.CurrentRoomTag != RoomTag.None)
        //{

        //}
        /////////////////////////////////////////////////////根据房间参数构造怪物数据/////////////////////////////////////////////////////
        //根据敌人数量标签随机敌人数量
        //roomData.CurrentRoomTag
        /////////////////////////////////////////////////////根据房间参数构造传送门数据/////////////////////////////////////////////////////

        roomData.TargetDoorCount = UnityEngine.Random.Range(1, 4);
        //若传送门数据不存在,则根据配置文件生成
        //移除当前已指定的类型
        roomData.RoomConfigDataFromDoor.ForEach(door =>
        {
            roomData.DoorTypeWeight.Remove(door.CurrentRoomType);
        });
        int doorCreatCount = roomData.TargetDoorCount - roomData.RoomConfigDataFromDoor.Count;
        var doorCreatType = RandManager.GetRandomEnum(roomData.DoorTypeWeight, doorCreatCount);
        doorCreatType.ForEach(type =>
        {
            Debug.Log($"添加{type}传送门");
            RoomConfigData newRoomConfigData = GetRoomConfig(type);
            roomData.RoomConfigDataFromDoor.Add(newRoomConfigData);
        });



        return roomData;
    }
    public static void AddRoom(RoomData roomData)
    {
        //将当前房间数据添加到游戏数据中
        Debug.Log("将当前房间数据添加到游戏数据中");
        Debug.Log(roomData.ToJson());
        GameDataSystem.AddRoomData(roomData);
        //保存改动后游戏数据
        GameDataSystem.Save();
    }
    //战斗结束,返回房间原位
    internal static void ReturnRoom()
    {
        //throw new NotImplementedException();
    }
    //根据房间信息刷新场地模型
    public static void RefreshRoomModel()
    {
        //清空之前的配置


        var currentRoom = GameDataSystem.GetGameData().CurrentRoomDatas.Last();
        //切换地图
        SceneObjectManager.Instance.SwitchSceneModel(currentRoom.CurrentSceneModel);
        //生成房间道具
        //生成怪物
        var EnemyPoints = currentRoom.Enemies.Count() switch
        {
            0 => new List<Transform>(),
            1 => RoomPointSystem.Instance.GetPoints(RoomPointsType.EnemyPoint1),
            2 => RoomPointSystem.Instance.GetPoints(RoomPointsType.EnemyPoint2),
            3 => RoomPointSystem.Instance.GetPoints(RoomPointsType.EnemyPoint3),
            _ => null
        };
        if (EnemyPoints == null)
        {
            Log.Show("敌人点位数量异常" + currentRoom.Enemies.Count(), 2);

        }
        else
        {
            for (int i = 0; i < EnemyPoints.Count; i++)
            {
                currentRoom.Enemies[i].bornPos = EnemyPoints[i].position;
                OutOfBattleManager.Instance.CreatEnemy(currentRoom.Enemies[i]);
            }
        }
        //生成事件

        //生成传送门(开启指定数量的门,并对门初始化)
        switch (currentRoom.RoomConfigDataFromDoor.Count)
        {
            case 0:
                {
                    SceneObjectManager.Instance.sceneDoor1.gameObject.SetActive(false);
                    SceneObjectManager.Instance.sceneDoor2.gameObject.SetActive(false);
                    SceneObjectManager.Instance.sceneDoor3.gameObject.SetActive(false);
                    break;
                }
            case 1:
                {
                    SceneObjectManager.Instance.sceneDoor1.gameObject.SetActive(true);
                    SceneObjectManager.Instance.sceneDoor2.gameObject.SetActive(false);
                    SceneObjectManager.Instance.sceneDoor3.gameObject.SetActive(false);
                    SceneObjectManager.Instance.sceneDoor1.GetComponent<DoorManager>().InitDoor(currentRoom.RoomConfigDataFromDoor[0]);
                    break;
                }
            case 2:
                {
                    SceneObjectManager.Instance.sceneDoor1.gameObject.SetActive(false);
                    SceneObjectManager.Instance.sceneDoor2.gameObject.SetActive(true);
                    SceneObjectManager.Instance.sceneDoor3.gameObject.SetActive(true);
                    SceneObjectManager.Instance.sceneDoor2.GetComponent<DoorManager>().InitDoor(currentRoom.RoomConfigDataFromDoor[0]);
                    SceneObjectManager.Instance.sceneDoor3.GetComponent<DoorManager>().InitDoor(currentRoom.RoomConfigDataFromDoor[1]);
                    break;
                }
            case 3:
                {
                    SceneObjectManager.Instance.sceneDoor1.gameObject.SetActive(true);
                    SceneObjectManager.Instance.sceneDoor2.gameObject.SetActive(true);
                    SceneObjectManager.Instance.sceneDoor3.gameObject.SetActive(true);
                    SceneObjectManager.Instance.sceneDoor1.GetComponent<DoorManager>().InitDoor(currentRoom.RoomConfigDataFromDoor[0]);
                    SceneObjectManager.Instance.sceneDoor2.GetComponent<DoorManager>().InitDoor(currentRoom.RoomConfigDataFromDoor[1]);
                    SceneObjectManager.Instance.sceneDoor3.GetComponent<DoorManager>().InitDoor(currentRoom.RoomConfigDataFromDoor[2]);
                    break;
                }
            default: Debug.LogError($"传送门数量错误,当前数量{currentRoom.RoomConfigDataFromDoor.Count},请纠正"); break;
        }
        //初始化角色位置
        Transform BirthPoint = RoomPointSystem.Instance.GetPoints(RoomPointsType.BirthPoint).FirstOrDefault();
        PlayerManager.Instance.transform.position = BirthPoint.position;
        PlayerManager.Instance.transform.eulerAngles = BirthPoint.eulerAngles;
        //触发镜头初始化效果
    }
    //返回初始大厅
    public static async Task RebackInitRoom()
    {
        GameDataSystem.GetGameData().CurrentRoomDatas = new();
        await EnterRoom(GetRoomConfig(RoomType.InitRoom));
    }
}
