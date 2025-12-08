using System.Collections.Generic;

public class GameData
{
    //局外数据
    public OutOfBattleData CurrentOutBattleData { get; set; }
    //房间数据
    public List<RoomData> CurrentRoomDatas { get; set; } = new();
    //当前层数
    public int CurrentLayer => CurrentRoomDatas.Count;
    #region 队伍数据
    //队伍所有成员数据
    public List<TeamCharaData> TeamCharaPool { get; set; } = new();
    //选择出战人物时的固定四位列表
    public TeamCharaData[] TempTeamAppearanceList { get; set; } = new TeamCharaData[4];
    //队伍登场成员数据
    public List<TeamCharaData> TeamAppearanceList { get; set; } = new();
    //当前登场的人物索引
    public int TeamAppearanceIndex { get; set; } = new();
    //固定的下载对象
    public TeamCharaData DownloadChara { get; set; }
    #endregion

    public GameConfigData CurrentGameConfigData { get; set; }
    //当前模式选择的buff列表数据
    public IBaseBuffList CurrentBuffList { get; set; }
    //当前模式选择的事件列表数据,包含状态
    public List<Occurrence> CurrentOccurrenceList { get; set; }
}
