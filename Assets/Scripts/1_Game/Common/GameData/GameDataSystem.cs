using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.Playables;

public class GameDataSystem
{
    public static GameData CurrentGameData { get; set; } = new();
    public static void Delete() => File.Delete("save.json");
    public static void Save() => File.WriteAllText("save.json", CurrentGameData.ToJson());
    public static async void Load()
    {

        if (!File.Exists("save.json"))
        {
            CurrentGameData = new();
            CurrentGameData.CurrentOutBattleData = new();
            //此处应该根据存档数据设置基础流程
            SetBaseBuff<SU_BuffList>();
            //此处应该根据存档数据设置事件模式
            OccurrenceSystem.Activate(SU_OccurrenceList.Occurrences, SU_OccurrenceList.Occurrences);
            await RoomSystem.RebackInitRoom();
            Save();
        }
        else
        {
            CurrentGameData = File.ReadAllText("save.json").ToObject<GameData>();
            RoomSystem.RefreshRoomModel();
        }
    }

    public static GameData GetGameData() => CurrentGameData;
    public static RoomData GetLastRoomData() => CurrentGameData.CurrentRoomDatas.LastOrDefault();

    public static List<Buff> GetCurrentBuff() => CurrentGameData.CurrentOutBattleData.Buffs;
    internal static void SetBaseBuff<T>() where T : BaseBuffList
    {
        switch (typeof(T))
        {
            case Type _ when typeof(T) == typeof(SU_BuffList):
                CurrentGameData.BaseBuff = BuffSystem.GetBuff(SU_BuffName.基础流程);
                break;
            default:
                // 默认处理
                break;
        }
    }
    internal static Buff GetBaseBuff() => CurrentGameData.BaseBuff;
    internal static void AddRoomData(RoomData roomData)
    {
        CurrentGameData.CurrentRoomDatas.Add(roomData);
    }

    internal static List<TeamCharaData> GetTeamAppearanceList()
    {
        return CurrentGameData.TeamAppearanceList;
    }
    internal static void SetTeamAppearanceList(List<TeamCharaData> teamCharaDatas)
    {
        CurrentGameData.TeamAppearanceList = teamCharaDatas;
    }

    internal static int GetTeamAppearanceIndex()
    {
        return CurrentGameData.TeamAppearanceIndex;
    }

    internal static void SetTeamAppearanceIndex(int index)
    {
        CurrentGameData.TeamAppearanceIndex = index;
    }
}
