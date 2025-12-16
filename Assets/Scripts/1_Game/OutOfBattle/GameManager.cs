using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
//决定一场游戏的整体流程,在局外房间和局内战斗来回切换

public class GameManager :InstanceBehaviour<GameManager> 
{
    //定位点模型
    public GameObject pointPrefab;
    private void Awake() => Instance = this;
    private void Start() => Init();

    public async void Init()
    {
        //初始化热更资源包，测试阶段只加载少量包
        await AssetBundleManager.Init("", false, new List<string>() { "charaicon.gezi" });
        Debug.Log("游戏开始");
        //初始化buff数据
        BuffSystem.Init();
        //初始化事件数据
        OccurrenceSystem.Init();

        //删除游戏存档(临时)
        GameDataSystem.Delete();
        //加载游戏存档
        GameDataSystem.Load();
        //GameDataSystem.Save();
        //初始化buff数据
        //初始化角色池子与队伍信息
        TeamManager.InitCharaList();

        //OutBattleUIManager.Instance.InitCharaSelectCanves( OutBattleUIManager.CharaSelectCanvasMode.TeamCreat);
        //进入当前房间
        RoomManager.RefreshRoom();

        //初始化ui数据
        OutOfBattleUIManager.Instance.Init();
        SwitchOutOfBattleMode();
        //测试获得事件数据
        var buff = BuffSystem.GetBuff(SU_BuffName.基础流程);
        OccurrenceSystem.GetOccurrence(OccurrenceName.test1);
        OutOfBattleUIManager.Instance.OpenOccurrenceCanvas("1_2");
        //BattleGameEventManager.SendSkillData
        await GameEventSystem.BattleStart();
    }
    
    //public void SavePlayerPos(Transform transform)
    //{
    //    gameData.PlayerPos = transform.position.ToTuple();
    //    gameData.PlayerEular = transform.eulerAngles.ToTuple();
    //}
    public void SwitchOutOfBattleMode()
    {
        //关闭局内ui
        BattleUIManager.CloeUI();

        //开启局外ui
        OutOfBattleUIManager.ShowUI();
        //显示局外角色，设置模式
        OutOfBattleManager.Instance.playerController.SetActive(true);
        //角色返回房间
        RoomManager.ReturnRoom();
        //设置摄像机模式
        CameraManager.Instance.CurrentCameraMode = CameraMode.Free;
        //初始化房间

        //规则化角色视角
        PlayerManager.Instance.ResetCameraView();
    }
    public void SwitchInBattleMode(List<PlayerName> playerNames, OutOfBattleEnemyDatas enemyDatas)
    {
        //关闭局外ui
        OutOfBattleUIManager.CloeUI();
        //隐藏局外角色
        OutOfBattleManager.Instance.playerController.SetActive(false);
        //隐藏局外物体
        OutOfBattleManager.Instance.outBattleParent.gameObject.SetActive(false);

        //开启局内ui
        BattleUIManager.ShowUI();
        //开启战场角色
        InBattleManager.Instance.battleParent.gameObject.SetActive(true);
        //设置摄像机模式
        CameraManager.Instance.CurrentCameraMode = CameraMode.CameraTrack;
        //初始化对局信息
        _ = InBattleManager.Instance.Init(playerNames, enemyDatas.enemyDatas);
        //初始化对局
    }

}
