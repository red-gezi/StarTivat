using TMPro;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    Color BattleColor = new Color(5, 0.5f, 0, 1);
    Color EventColor = new Color(0, 0.8f, 5, 1);
    Color EndColor = new Color(1.5f, 1.5f, 1.5f);
    // Start is called before the first frame update
    RoomConfigData CurrentRoomConfigData;
    //房间初始化时配置门信息
    public void InitDoor(RoomConfigData RoomConfigData)
    {
        //初始化数据
        CurrentRoomConfigData = RoomConfigData;
        //设置颜色
        GetComponent<Renderer>().material.color = RoomConfigData.CurrentRoomType switch
        {
            RoomType.InitRoom => EndColor,
            RoomType.StartRoom => Color.green,
            RoomType.EliteRoom => BattleColor,
            RoomType.BossRoom => BattleColor,
            RoomType.EventRoom => EventColor,
            RoomType.BattleRoom => BattleColor,
            RoomType.EncounterRoom => BattleColor,
            RoomType.RewardRoom => EventColor,
            RoomType.ShopRoom => EventColor,
            RoomType.GameRoom => EventColor,
            RoomType.MiracleRoom => EventColor,
            RoomType.RestRoom => Color.green,
            _ => Color.gray
        };
        //设置图标
        GetComponent<Renderer>().material.SetTexture("_Icon", RoomSystem.GetRoomIcon(RoomConfigData.CurrentRoomType));
        //设置文字
        transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = RoomConfigData.CurrentRoomType switch
        {
            RoomType.InitRoom => "大厅",
            RoomType.StartRoom => "起始房",
            RoomType.EliteRoom => "精英",
            RoomType.BossRoom => "首领",
            RoomType.EventRoom => "事件",
            RoomType.BattleRoom => "战斗",
            RoomType.EncounterRoom => "遭遇",
            RoomType.RewardRoom => "奖励",
            RoomType.ShopRoom => "商店",
            RoomType.GameRoom => "游戏",
            RoomType.MiracleRoom => "奇遇",
            RoomType.RestRoom => "休息",
            _ => "虚无房"
        };
    }
    public async void GoToNextRoom()
    {
        await RoomSystem.EnterRoom(CurrentRoomConfigData);
    }
}
