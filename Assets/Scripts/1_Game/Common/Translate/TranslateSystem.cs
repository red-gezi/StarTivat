using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class TranslateSystem : InstanceBehaviour<TestSystem>
{
    public static string CurrentLanguage { get; set; } = "Ch";
    public static string GetRoomTypeName(RoomType roomType)
    {
        return CurrentLanguage switch
        {
            "Ch" => roomType switch
            {
                RoomType.VoidRoom => "空房间",
                RoomType.InitRoom => "局外初始房",
                RoomType.StartRoom => "起点",
                RoomType.BattleRoom => "战斗",
                RoomType.EncounterRoom => "遭遇",
                RoomType.EliteRoom => "精英",
                RoomType.BossRoom => "首领",
                RoomType.OccurrenceRoom => "事件",
                RoomType.RewardRoom => "奖励",
                RoomType.ShopRoom => "商店",
                RoomType.GameRoom => "游戏",
                RoomType.MiracleRoom => "奇遇",
                RoomType.RestRoom => "休息",
                _ => roomType.ToString()
            },
            "En" => roomType switch
            {
                RoomType.VoidRoom => "Void Room",
                RoomType.InitRoom => "Init Room",
                RoomType.StartRoom => "Start Room",
                RoomType.BattleRoom => "Battle Room",
                RoomType.EncounterRoom => "Encounter Room",
                RoomType.EliteRoom => "Elite Room",
                RoomType.BossRoom => "Boss Room",
                RoomType.OccurrenceRoom => "Occurrence Room",
                RoomType.RewardRoom => "Reward Room",
                RoomType.ShopRoom => "Shop Room",
                RoomType.GameRoom => "Game Room",
                RoomType.MiracleRoom => "Miracle Room",
                RoomType.RestRoom => "Rest Room",
                _ => roomType.ToString()
            },
            _ => roomType.ToString()
        };
    }

}