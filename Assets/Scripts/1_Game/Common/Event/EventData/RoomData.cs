using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomData : EventData
{
    //当前层数
    public int CurrentLayer { get; set; }
    public int MaxLayer { get; set; }
    //当前房间基础配置信息
    //public RoomConfigData BaseRoomConfigData { get; set; }
    //房间已完成,激活传送门
    public Dictionary<string, bool> OccurenceState { get; set; } = new();
    public List<bool> EnemyState { get; set; } = new();
    public bool IsFinish { get; set; }
    //当前场地
    public SceneModelType CurrentSceneModel { get; set; }

    public RoomType CurrentRoomType { get; set; }
    //当前房间标签
    public RoomTag CurrentRoomTag { get; set; }
    //当前房间的传送门类型与激活状态
    //public List<RoomType> Doors { get; set; } = new();
    //应该生成的传送门数量
    public int TargetDoorCount { get; set; } = new();
    public List<string> OccurrenceTag { get; set; } = new();

    //不同类型出口传送门随机的权重,为初始模板,可能会因为奇物进行改变
    public Dictionary<RoomType, float> DoorTypeWeight { get; set; } = new()
    {
        { RoomType.InitRoom, 0f },
        { RoomType.StartRoom, 0f },
        { RoomType.BossRoom, 0f },

        { RoomType.BattleRoom, 1f },
        { RoomType.EncounterRoom, 1f },
        { RoomType.EliteRoom, 0.2f },

        { RoomType.OccurrenceRoom, 1f },
        { RoomType.RewardRoom, 0.5f },
        { RoomType.ShopRoom, 0.2f },
        { RoomType.GameRoom, 0.2f },
        { RoomType.MiracleRoom, 0.2f },
    };
    //当前房间类型
    //传送门对应的下个房间配置信息
    public List<RoomConfigData> RoomConfigDataFromDoor { get; set; } = new();
    //事件类型与状态
    //敌人类型与状态
    public List<OutOfBattleEnemyDatas> Enemies { get; set; } = new();
    //场景物体与存活状态
    public Dictionary<SceneObjectData, bool> sceneObjects { get; set; } = new();
    //先根据基础配置参数构建数据，场景会被确认下来，传送门参数随机，之后会通过进入新场景事件二次修改
    public RoomData(int layer, RoomConfigData baseRoomConfigData)
    {
        CurrentLayer = layer;
        //暂时设置最大层数
        MaxLayer = 12;
        //BaseRoomConfigData = baseRoomConfigData;

        //Enemies = BaseRoomConfigData.outOfBattleEnemyDatas;
        /////////////////////////////////////////////////////根据配置信息构造房间实例数据/////////////////////////////////////////////////////
        //获得当前房间数据,用于选定新房间的场地模型不与当前重复
        RoomData currentRoomData = GameDataSystem.GetLastRoomData();
        if (currentRoomData == null)
        {
            Debug.LogError("当前无房间信息");
            CurrentSceneModel = RandSystem.GetRandomValues(baseRoomConfigData.SelectableSceneModel, 1)[0];
        }
        else
        {
            SceneModelType currentSceneModel = currentRoomData.CurrentSceneModel;
            // 获取可用场景列表(排除当前场景)
            var availableScenes = new List<SceneModelType>(baseRoomConfigData.SelectableSceneModel);
            availableScenes.Remove(currentSceneModel);
            // 如果没有可用场景就保留所有场景
            if (!availableScenes.Any())
            {
                availableScenes = new List<SceneModelType>(baseRoomConfigData.SelectableSceneModel);
            }
            // 随机选择一个场景
            CurrentSceneModel = RandSystem.GetRandomValues(availableScenes, 1)[0];
        }
        Debug.Log("当前选中房间为" + CurrentSceneModel.ToString());
        //随机当前传送门出口数量
        TargetDoorCount = RandSystem.GetRandomValue(baseRoomConfigData.DoorCount);
        //随机当前房间tag
        CurrentRoomTag = RandSystem.GetRandomValue(baseRoomConfigData.SelectableEnemyTag);
        switch (CurrentRoomTag)
        {
            case RoomTag.EnemyCount1:
                Enemies.Add(EnemyConfigData.GetRandomEnemyConfigData(EnemyConfigDataType.Enemy1Wave));
                break;
            case RoomTag.EnemyCount2:
                Enemies.Add(EnemyConfigData.GetRandomEnemyConfigData(EnemyConfigDataType.Enemy1Wave));
                Enemies.Add(EnemyConfigData.GetRandomEnemyConfigData(EnemyConfigDataType.Enemy1Wave));
                break;
            case RoomTag.EnemyCount3:
                Enemies.Add(EnemyConfigData.GetRandomEnemyConfigData(EnemyConfigDataType.Enemy1Wave));
                Enemies.Add(EnemyConfigData.GetRandomEnemyConfigData(EnemyConfigDataType.Enemy1Wave));
                Enemies.Add(EnemyConfigData.GetRandomEnemyConfigData(EnemyConfigDataType.Enemy1Wave));
                break;
            case RoomTag.Intensify:
                break;
            default: break;
        }

    }
}
