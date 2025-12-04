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
    public List<RoomTag> SelectableEnemyTag { get; set; } = new();
    public List<RoomTag> SelectableSignTag { get; set; } = new();
    //出口类型
    public List<int> DoorCount { get; set; } = new();
    public List<int> OccurrenceCount { get; set; } = new();
    public List<int> MonstertCount { get; set; } = new();
    //public List<EnemyName> MonstertType { get; set; } = new();
    public List<OutOfBattleEnemyDatas> outOfBattleEnemyDatas { get; set; } = new();
    public List<int> ChestCount { get; set; } = new();
    public List<int> SceneObjectCount { get; set; } = new();
    public List<int> NPC { get; set; } = new();
    public RoomConfigData SetEnemyTag(params RoomTag[] roomTags)
    {
        SelectableEnemyTag.Clear();
        SelectableEnemyTag.AddRange(roomTags.ToList());
        return this;
    }
    public RoomConfigData SetSignTag(params RoomTag[] roomTags)
    {
        SelectableSignTag.Clear();
        SelectableSignTag.AddRange(roomTags.ToList());
        return this;
    }
}
