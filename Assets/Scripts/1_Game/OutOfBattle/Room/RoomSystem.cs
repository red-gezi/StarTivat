using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public partial class RoomSystem : InstanceBehaviour<RoomSystem>
{
    /////////////////////////////////////////////////////场景/////////////////////////////////////////////////////
    public Transform screenModelRoot;
    [Button("切换场景")]
    public static void SwitchSceneModel(SceneModelType screenModel)
    {
        Debug.Log("切换场景为" + screenModel.ToString());
        foreach (Transform model in Instance.screenModelRoot)
        {
            model.gameObject.SetActive(model.name == screenModel.ToString());
        }
    }
    //根据房间信息刷新场地模型
    public static void RefreshRoomModel()
    {
        //清空之前的配置


        var currentRoomData = GameDataSystem.GetLastRoomData();
        //切换地图
        SwitchSceneModel(currentRoomData.CurrentSceneModel);
        //生成房间道具
        //生成怪物
        var EnemyPoints = currentRoomData.Enemies.Count() switch
        {
            0 => new List<Transform>(),
            1 => RoomPointSystem.Instance.GetPoints(RoomPointsType.EnemyPoint1),
            2 => RoomPointSystem.Instance.GetPoints(RoomPointsType.EnemyPoint2),
            3 => RoomPointSystem.Instance.GetPoints(RoomPointsType.EnemyPoint3),
            _ => null
        };
        if (EnemyPoints == null)
        {
            Log.Show("敌人点位数量异常" + currentRoomData.Enemies.Count(), 2);
        }
        else
        {
            for (int i = 0; i < EnemyPoints.Count; i++)
            {
                currentRoomData.Enemies[i].bornPos = EnemyPoints[i].position;
                OutOfBattleSystem.Instance.CreatEnemy(currentRoomData.Enemies[i]);
            }
        }

        switch (currentRoomData.OccurrenceTag.Count)
        {
            //生成事件
            case 0:
                // 三个标签，激活所有模型
                OccurrenceSystem.TurnOff(Instance.Occurrences[0]);
                OccurrenceSystem.TurnOff(Instance.Occurrences[1]);
                OccurrenceSystem.TurnOff(Instance.Occurrences[2]);
                break;
            case 1:
                // 只有一个标签，激活第一个模型
                OccurrenceSystem.TurnOn(Instance.Occurrences[0], currentRoomData.OccurrenceTag[0]);
                OccurrenceSystem.TurnOff(Instance.Occurrences[1]);
                OccurrenceSystem.TurnOff(Instance.Occurrences[2]);
                break;

            case 2:
                // 两个标签，激活第2、3个模型
                OccurrenceSystem.TurnOff(Instance.Occurrences[0]);
                OccurrenceSystem.TurnOn(Instance.Occurrences[1], currentRoomData.OccurrenceTag[0]);
                OccurrenceSystem.TurnOn(Instance.Occurrences[2], currentRoomData.OccurrenceTag[1]);
                break;

            case 3:
                // 三个标签，激活所有模型
                OccurrenceSystem.TurnOn(Instance.Occurrences[0], currentRoomData.OccurrenceTag[0]);
                OccurrenceSystem.TurnOn(Instance.Occurrences[1], currentRoomData.OccurrenceTag[1]);
                OccurrenceSystem.TurnOn(Instance.Occurrences[2], currentRoomData.OccurrenceTag[2]);
                break;
        }
        currentRoomData.OccurenceState.Clear();
        currentRoomData.OccurrenceTag.ForEach(tag => currentRoomData.OccurenceState[tag] = false);
        //生成传送门(开启指定数量的门,并对门初始化)
        switch (currentRoomData.RoomConfigDataFromDoor.Count)
        {
            case 0:
                {
                    HideDoor(0);
                    HideDoor(1);
                    HideDoor(2);
                    break;
                }
            case 1:
                {
                    ShowDoor(0, currentRoomData.RoomConfigDataFromDoor[0]);
                    HideDoor(1);
                    HideDoor(2);
                    break;
                }
            case 2:
                {
                    HideDoor(0);
                    ShowDoor(1, currentRoomData.RoomConfigDataFromDoor[0]);
                    ShowDoor(2, currentRoomData.RoomConfigDataFromDoor[1]);
                    break;
                }
            case 3:
                {
                    ShowDoor(0, currentRoomData.RoomConfigDataFromDoor[0]);
                    ShowDoor(1, currentRoomData.RoomConfigDataFromDoor[1]);
                    ShowDoor(2, currentRoomData.RoomConfigDataFromDoor[2]);
                    break;
                }
            default: Debug.LogError($"传送门数量错误,当前数量{currentRoomData.RoomConfigDataFromDoor.Count},请纠正"); break;
        }
        //关闭存档中已完成的门
        //检测房间状态，已完成时激活传送门
        CheckRoomFinishState();
        //初始化角色位置
        Transform BirthPoint = RoomPointSystem.Instance.GetPoints(RoomPointsType.BirthPoint).FirstOrDefault();
        PlayerSystem.Instance.transform.position = BirthPoint.position;
        PlayerSystem.Instance.transform.eulerAngles = BirthPoint.eulerAngles;
        //触发镜头初始化效果
    }
    /////////////////////////////////////////////////////房间/////////////////////////////////////////////////////
    public List<Sprite> roomIcons;
    public static Texture2D GetRoomIcon(RoomType roomType)
    {
        var icon = roomType switch
        {
            RoomType.VoidRoom => Instance.roomIcons.FirstOrDefault(icon => icon.name == "空"),
            RoomType.InitRoom => Instance.roomIcons.FirstOrDefault(icon => icon.name == "空"),
            RoomType.StartRoom => Instance.roomIcons.FirstOrDefault(icon => icon.name == "空"),
            RoomType.BattleRoom => Instance.roomIcons.FirstOrDefault(icon => icon.name == "战斗"),
            RoomType.EncounterRoom => Instance.roomIcons.FirstOrDefault(icon => icon.name == "遭遇"),
            RoomType.EliteRoom => Instance.roomIcons.FirstOrDefault(icon => icon.name == "遭遇"),
            RoomType.BossRoom => Instance.roomIcons.FirstOrDefault(icon => icon.name == "首领"),
            RoomType.OccurrenceRoom => Instance.roomIcons.FirstOrDefault(icon => icon.name == "事件"),
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
            CurrentRoomType = RoomType.OccurrenceRoom,
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
        RoomData newRoomData = new RoomData(layer, roomConfigData);
        await GameEventSystem.EnterRoom(newRoomData);
        //指定房间测试代码
        newRoomData.CurrentRoomType = RoomType.OccurrenceRoom;
        newRoomData.CurrentRoomTag = RoomTag.Double;

        FillRoomData(newRoomData);
        //添加房间并保存
        AddRoom(newRoomData);
        RefreshRoomModel();
    }
    /// <summary>
    /// 根据配置规则填充房间数据
    /// </summary>
    /// <param name="roomData"></param>
    /// <returns></returns>
    public static void FillRoomData(RoomData roomData)
    {
        ////如果不存在强制指定的？
        //if (roomData.CurrentRoomTag != RoomTag.None)
        //{

        //}
        /////////////////////////////////////////////////////根据房间参数构造怪物数据/////////////////////////////////////////////////////
        //根据敌人数量标签随机敌人数量
        //roomData.CurrentRoomTag
        /////////////////////////////////////////////////////根据房间参数构造事件数据/////////////////////////////////////////////////////
        switch (roomData.CurrentRoomType)
        {
            case RoomType.InitRoom:
                break;
            case RoomType.StartRoom:
                break;
            case RoomType.BattleRoom:
                break;
            case RoomType.EncounterRoom:
                break;
            case RoomType.EliteRoom:
                break;
            case RoomType.BossRoom:
                break;
            case RoomType.OccurrenceRoom:
                List<Occurrence> targetOccurrences = new();
                int occurrenceCount = roomData.CurrentRoomTag switch
                {
                    RoomTag.One => 1,
                    RoomTag.Double => 2,
                    RoomTag.Select => 3,
                    _ => 0
                };

                if (occurrenceCount > 0)
                {
                    targetOccurrences = OccurrenceSystem.GetRandomOccurrence(occurrenceCount, OccurrenceTag.Occurrence);
                    targetOccurrences.ForEach(occurrence =>
                    {
                        roomData.OccurrenceTag.Add(occurrence.Data.Tag);
                    });
                }
                else
                {
                    Log.Show("填充房间事件遇到无效标签，请检查", 2);
                }
                break;
            case RoomType.RewardRoom:
                break;
            case RoomType.ShopRoom:
                break;
            case RoomType.GameRoom:
                break;
            case RoomType.MiracleRoom:
                break;
            case RoomType.RestRoom:
                break;
            default:
                break;
        }
        /////////////////////////////////////////////////////根据房间参数构造传送门数据/////////////////////////////////////////////////////

        roomData.TargetDoorCount = UnityEngine.Random.Range(1, 4);
        //若传送门数据不存在,则根据配置文件生成
        //移除当前已指定的类型
        roomData.RoomConfigDataFromDoor.ForEach(door =>
        {
            roomData.DoorTypeWeight.Remove(door.CurrentRoomType);
        });
        int doorCreatCount = roomData.TargetDoorCount - roomData.RoomConfigDataFromDoor.Count;
        var doorCreatType = RandSystem.GetRandomEnum(roomData.DoorTypeWeight, doorCreatCount);
        doorCreatType.ForEach(type =>
        {
            Debug.Log($"添加{type}传送门");
            RoomConfigData newRoomConfigData = GetRoomConfig(type);
            roomData.RoomConfigDataFromDoor.Add(newRoomConfigData);
        });
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


    //返回初始大厅
    public static async Task RebackInitRoom()
    {
        GameDataSystem.GetGameData().CurrentRoomDatas = new();
        await EnterRoom(GetRoomConfig(RoomType.InitRoom));
    }
    //战斗结束,返回房间原位
    internal static void ReturnRoom()
    {
        //throw new NotImplementedException();
    }
    /////////////////////////////////////////////////////事件/////////////////////////////////////////////////////
    public List<GameObject> Occurrences;
    internal static void FinishOccurrence(GameObject gameObject, string tag)
    {
        //数据层完成房间
        var roomData = GameDataSystem.GetLastRoomData();
        switch (roomData.CurrentRoomTag)
        {
            case RoomTag.None:
            case RoomTag.One:
            case RoomTag.Double:
                //完成指定事件
                if (roomData.OccurenceState.ContainsKey(tag))
                {
                    roomData.OccurenceState[tag] = true;
                    OccurrenceSystem.TurnOff(gameObject);
                }
                else
                {
                    Log.Show("房间事件tag中不包含当前tag,请检查", 2);
                }
                break;
            case RoomTag.Select:
                //完成所有事件
                roomData.OccurenceState.Keys.ToList().ForEach(key => roomData.OccurenceState[tag] = true);
                for (int i = 0; i < 3; i++)
                {
                    OccurrenceSystem.TurnOff(Instance.Occurrences[i]);
                }
                break;
            default:
                Log.Show("当前房间不应当包含事件", 2);
                break;
        }
        CheckRoomFinishState();
        //存档
        GameDataSystem.Save();
    }

    private static void CheckRoomFinishState()
    {
        //如果所有事件/敌人都完成（后续追加游戏等）
        var roomData = GameDataSystem.GetLastRoomData();
        if (roomData.OccurenceState.All(state => state.Value) && roomData.EnemyState.All(state => state))
        {
            //房间设为完成
            roomData.IsFinish = true;
            //开启传送门
            Instance.doors.ForEach(door => door.GetComponent<DoorSystem>().TurnOn());
        }
    }

    /////////////////////////////////////////////////////传送门/////////////////////////////////////////////////////
    public List<GameObject> doors;
    public static void HideDoor(int index)
    {
        Instance.doors[index].gameObject.SetActive(false);
    }
    public static void ShowDoor(int index, RoomConfigData roomConfigData)
    {
        Instance.doors[index].SetActive(true);
        Instance.doors[index].GetComponent<DoorSystem>().InitDoor(roomConfigData);
    }
}