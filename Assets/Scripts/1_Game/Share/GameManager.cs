using System.Collections.Generic;
using System.IO;
using UnityEngine;
//决定一场游戏的整体流程,在局外房间和局内战斗来回切换
public  class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    //整个游戏的存档
    public static GameData gameData;
    //当前游戏模式的基本buff,包含游戏的基础流程
    public static Buff baseBuff;
    //当前应用的游戏模式下各种奇物祝福的buff列表
    public static IBaseBuffList CurrentBuffList;
    //定位点模型
    public GameObject pointPrefab;
    private void Awake() => Instance = this;
    private void Start() => Init();

    public void Init()
    {
        Debug.Log("游戏开始");
        //加载游戏存档
        Load();
        Save();
        //初始化buff数据
        InitBuffList();
        //进入当前房间
        //RoomManager.EnterRoom(gameData);
    }
    //初始化模拟宇宙buff列表
    public void InitBuffList()
    {
        MoNiYuZhouBuffList.Init();
        baseBuff = MoNiYuZhouBuffList.BuffList.GetBuff((int)MoNiYuZhouBuffList.BufferName.基础流程);
        CurrentBuffList = MoNiYuZhouBuffList.BuffList;
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
        //加载对局信息
        BattleManager.Instance.Init(enemyDatas.enemyDatas);
        //初始化对局
    }
    public static void Save()
    {
        File.WriteAllText("save.json", gameData.ToJson());
    }
    public static void Load()
    {
        if (!File.Exists("save.json"))
        {
            gameData = new();
            RoomManager.ResetRoomConfigData();
            Save();
        }
        else
        {
            gameData = File.ReadAllText("save.json").ToObject<GameData>();
        }
    }
}
