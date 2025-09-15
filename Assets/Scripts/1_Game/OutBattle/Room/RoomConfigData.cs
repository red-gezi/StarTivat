using System.Collections.Generic;
using System.Linq;

public class RoomConfigData
{
    //当前房间类型
    public RoomType CurrentRoomType { get; set; }
    //当前层数
    //可选的场地
    public List<SceneModelType> SelectableSceneModel { get; set; }
    //当前房间标签
    public List<List<RoomTag>> SelectableRoomTag { get; set; } = new List<List<RoomTag>>();
    //出口类型
    public List<int> DoorCount { get; set; } = new();
    public List<int> OccurrenceCount { get; set; } = new();
    public List<int> MonstertCount { get; set; } = new();
    public List<EnemyType> MonstertType { get; set; } = new();
    //public List<EnemyType> MonstertType { get; set; } = new();
    public List<int> ChestCount { get; set; } = new();
    public List<int> SceneObjectCount { get; set; } = new();
    public List<int> NPC { get; set; } = new();
    public RoomConfigData AddTagGroup(params RoomTag[] roomTags)
    {
        SelectableRoomTag.Add(roomTags.ToList());
        return this;
    }
}
