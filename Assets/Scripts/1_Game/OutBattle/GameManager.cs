using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
//决定一场游戏的整体流程,在局外房间和局内战斗来回切换

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    //整个游戏的存档
    [ShowInInspector]
    public static GameData gameData;
    //当前游戏模式的基本buff,包含游戏的基础流程
    public static Buff BaseBuff { get; set; }
    //当前应用的游戏模式下各种奇物祝福的buff列表

    //定位点模型
    public GameObject pointPrefab;
    private void Awake() => Instance = this;
    private void Start() => Init();

    public async void Init()
    {
        await AssetBundleManager.Init("", false,new List<string>() { "charaicon.gezi" });
        Debug.Log("游戏开始");
        //删除游戏存档(临时)
        Delete();
        //加载游戏存档
        Load();
        //Save();
        //初始化buff数据
        //初始化角色池子与队伍信息
        TeamManager.InitCharaList();
        
        //OutBattleUIManager.Instance.InitCharaSelectCanves( OutBattleUIManager.CharaSelectCanvasMode.TeamCreat);
        //进入当前房间
        RoomManager.RefreshRoom();
    }
    //初始化模拟宇宙buff列表
    public static void InitBuffList()
    {
        SimulatedUniverseBuffList.Init();
        BaseBuff = SimulatedUniverseBuffList.BuffList.GetBuff((int)SimulatedUniverseBuffList.BufferName.基础流程);
        gameData.CurrentBuffList = SimulatedUniverseBuffList.BuffList;
    }
    //public void SavePlayerPos(Transform transform)
    //{
    //    gameData.PlayerPos = transform.position.ToTuple();
    //    gameData.PlayerEular = transform.eulerAngles.ToTuple();
    //}
    public void SwitchOutBattleMode()
    {
        //开启局外ui
        OutBattleUIManager.ShowUI();
        //关闭局内ui
        BattleUIManager.CloeUI();
        //角色返回房间
        RoomManager.ReturnRoom();
        //初始化房间
        //设置角色

        //规则化角色视角
        PlayerManager.Instance.ResetCamera();
    }
    public void SwitchBattleMode(OutBattleEnemy enemyDatas)
    {
        //开启局内ui
        BattleUIManager.ShowUI();
        //关闭局外ui
        OutBattleUIManager.CloeUI();
        //隐藏局外角色
        //开启战场角色
        //加载对局信息
        BattleManager.Instance.Init(enemyDatas.enemyDatas);
        //初始化对局
    }
    //删除非法存档(测试)
    public static void Delete()
    {
        File.Delete("save.json");
    }
    public static void Save()
    {
        File.WriteAllText("save.json", gameData.ToJson());
    }
    public static async void Load()
    {
        if (!File.Exists("save.json"))
        {
            gameData = new();
            gameData.CurrentOutBattleData = new();
            InitBuffList();

            await RoomManager.ResetRoom();
            Save();
        }
        else
        {
            gameData = File.ReadAllText("save.json").ToObject<GameData>();
        }
    }
}
