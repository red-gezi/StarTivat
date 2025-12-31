using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using static MMD4MecanimImporterImpl.MMDModel;
using static UnityEngine.ParticleSystem;

public class DoorSystem : MonoBehaviour
{
    public AnimationCurve curve;
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
            RoomType.OccurrenceRoom => EventColor,
            RoomType.BattleRoom => BattleColor,
            RoomType.EncounterRoom => BattleColor,
            RoomType.RewardRoom => EventColor,
            RoomType.ShopRoom => EventColor,
            RoomType.GameRoom => EventColor,
            RoomType.MiracleRoom => EventColor,
            RoomType.RestRoom => Color.green,
            _ => Color.gray
        };
        GetComponent<Renderer>().material.SetTexture("_Icon", RoomSystem.GetRoomIcon(CurrentRoomConfigData.CurrentRoomType));

        //GetComponent<Renderer>().material.SetTexture("_Icon", RoomSystem.GetRoomIcon(RoomConfigData.CurrentRoomType));
        TurnOff();
    }
    [Button("开启传送门")]
    public async void TurnOn()
    {
        //放大，显示ui和图标
        await CustomThread.TimerAsync(0.2f, progress =>
        {
            transform.localScale = Vector3.Lerp(new(1f, 0.7f, 1f), new(2.5f, 2.5f, 1f), curve.Evaluate(progress));
        });
        //设置图标
        //GetComponent<Renderer>().material.SetFloat("_ShowIcon", 1);
        GetComponent<Renderer>().material.SetTexture("_Icon", RoomSystem.GetRoomIcon(CurrentRoomConfigData.CurrentRoomType));
        //设置文字
        transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = CurrentRoomConfigData.CurrentRoomType switch
        {
            RoomType.InitRoom => "大厅",
            RoomType.StartRoom => "起始房",
            RoomType.EliteRoom => "精英",
            RoomType.BossRoom => "首领",
            RoomType.OccurrenceRoom => "事件",
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
    [Button("关闭传送门")]
    public async void TurnOff()
    {
        //缩小，去掉ui和图标
        GetComponent<Renderer>().material.SetTexture("_Icon", RoomSystem.GetRoomIcon(RoomType.VoidRoom));
        transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
        await CustomThread.TimerAsync(0.2f, progress =>
        {
            transform.localScale = Vector3.Lerp(new(2.5f, 2.5f, 1f), new(1f, 0.7f, 1f), curve.Evaluate(progress));
        });
    }
    public async void GoToNextRoom()
    {
        await RoomSystem.EnterRoom(CurrentRoomConfigData);
    }
}
