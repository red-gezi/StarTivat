using System.Collections.Generic;

public class RoomData : EventData
{
    //当前层数
    public int Layer { get; set; }
    //房间已完成,激活传送门
    public bool IsFinish { get; set; }
    //当前场地
    public SceneModelType CurrentSceneModel { get; set; }
    //当前房间类型
    public RoomType CurrentRoomType { get; set; }
    //当前房间标签
    public RoomTag CurrentRoomTag { get; set; }
    //传送门类型与状态
    public List<RoomType> Doors { get; set; }
    //事件类型与状态
    //敌人类型与状态
    public Dictionary<OutBattleEnemy, bool> Enemies { get; set; }
    //场景物体与存活状态
    public Dictionary<OutBattleEnemy, bool> Enemies { get; set; }
}
public class RoomConfigData
{
    //当前层数
    //当前场地
    public List<SceneModelType> SelectableSceneModel { get; set; }
    //当前房间类型
    public RoomType SelectableRoomType { get; set; }
    //当前房间标签
    public RoomTag SelectableRoomTag { get; set; }
    //出口类型
    public List<RoomType> OutDoorType { get; set; } = new();
    public List<int> DoorCount { get; set; } = new();
    public List<int> OccurrenceCount { get; set; } = new();
    public List<int> MonstertCount { get; set; } = new();
    public List<EnemyType> MonstertType { get; set; } = new();
    public List<int> ChestCount { get; set; } = new();
    public List<int> SceneObjectCount { get; set; } = new();
    public List<int> NPC { get; set; } = new();

}
