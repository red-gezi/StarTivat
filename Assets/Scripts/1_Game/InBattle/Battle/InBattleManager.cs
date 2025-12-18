using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
//管理场上人物、位置数据
public class InBattleSystem : InstanceBehaviour<InBattleSystem>
{
    //登场双方战前配置
    public List<PlayerName> players;
    public List<EnemyName> enemies;


    //局内模型生成时的父物体
    public Transform battleParent;

    //所有角色的模型数据

    public Transform playerParent;
    public Transform enemiesParent;
    //登场双方集合
    public List<Character> charaList = new();
    public List<Character> PlayerList => charaList.Where(chara => !chara.IsEnemy).ToList();
    public List<Character> EnemyList => charaList.Where(chara => chara.IsEnemy).ToList();
    public List<Character> SameSideList(Character target) => charaList.Where(chara => !(chara.IsEnemy ^ target.IsEnemy)).ToList();
    public List<Character> DifferentSideList(Character target) => charaList.Where(chara => chara.IsEnemy ^ target.IsEnemy).ToList();
    public static Buff GetBaseBuff() => GameDataSystem.GetBaseBuff();
    //全局buff列表
    public List<Buff> GoblePlayerBuffs = new();
    public List<Buff> GobleEnemyBuffs = new();
    public static List<Buff> GetAllPlayerInBattleBuffs() => Instance.PlayerList.SelectMany(player => player.GetCurrentBuffs()).Distinct().ToList();
    public static List<Buff> GetAllEnemyInBattleBuffs() => Instance.PlayerList.SelectMany(enemy => enemy.GetCurrentBuffs()).Distinct().ToList();
    public static List<Buff> GetAllInBattleBuffs() => GetAllPlayerInBattleBuffs().Concat(GetAllEnemyInBattleBuffs()).Distinct().ToList();

    //站位配置
    static float playerDistance = 2f;
    static float PlayerOffset => (Instance.PlayerList.Count - 1) * playerDistance / 2f;
    static float enemyDistance = 1.5f;
    static float EnemyOffset => (Instance.EnemyList.Count - 1) * enemyDistance / 2f;

   

    public async Task Init(List<PlayerName> playerNames, List<EnemyData> enemyData)
    {
        ClearAllChara();
        //初始化双方角色
        CreatPlayer(playerNames);
        CreatEnemy(enemyData);

        //关闭角色选择图标
        SelectManager.Close();
        //关闭角色技能图标
        SkillManager.Close();
        //初始化人物信息版
        CharaBoardManager.Init();
        //初始化行动条
        ActionBarManager.Init(charaList);
        //初始技能点
        SkillPointManager.Init();
        //初始化摄像机,环绕敌人
        await BattleCameraManager.BattleStartAround(EnemyList);
        //通知战斗开始

        //BroadcastManager.BroadcastEvent(Character., new CharaEvent());
        //激活行动条
        ActionBarManager.RunAction();
    }


    public void ClearAllChara()
    {
        //清空角色列表
        charaList.Clear();
        //销毁角色模型
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
    public void CreatPlayer(List<PlayerName> playerList)
    {
        charaList.Clear();
        //根据配置创造场上人物
        for (int i = 0; i < playerList.Count; i++)
        {
            var charaName = playerList[i].ToString();
            GameObject charaModel = playerParent.Find(charaName).gameObject;
            GameObject chara = Instantiate(charaModel, battleParent);
            //角色设为局内模式
            //chara.name = charaModel.name + $"站位:{i + 1}";
            chara.name = charaModel.name;
            chara.SetActive(true);
            Character charaScript = chara.GetComponent<Character>();
            charaScript.model = chara;
            charaScript.IsEnemy = false;
            //charaScript.RefreshElementsUI();
            charaList.Add(charaScript);
        }

        //计算场上人物默认位置
        RefreshPlayerPos(0);
        //敌人坐标延后，做个出厂动画

    }
    private void CreatEnemy(List<EnemyData> enemyDatas)
    {
        for (int i = 0; i < enemyDatas.Count; i++)
        {
            var charaName = enemyDatas[i].CurrentEnemyName.ToString();
            GameObject charaModel = enemiesParent.Find(charaName).gameObject;
            GameObject chara = Instantiate(charaModel, battleParent);
            //chara.name = charaModel.name + $"站位:{i + 1}";
            chara.name = charaModel.name;
            chara.SetActive(true);
            Character charaScript = chara.GetComponent<Character>();
            charaScript.SwitchInBattleMode();
            charaScript.model = chara;
            charaScript.IsEnemy = true;
            charaScript.RefreshElementsUIAsync();
            charaList.Add(charaScript);
        }
        for (int i = 0; i < EnemyList.Count; i++)
        {
            _ = EnemyList[i].Entrance();
        }
        RefreshEnemyPos(0);
    }

    /// <summary>
    /// 根据当前站位索引刷新场上人物位置
    /// </summary>
    /// <param name="rank"></param>
    [Button("刷新玩家当前角色位置")]
    public void RefreshPlayerPos(int rank)
    {
        //刷新玩家角色位置
        for (int i = 0; i < PlayerList.Count; i++)
        {
            GameObject chara = PlayerList[i].model;
            float x = i * playerDistance - PlayerOffset - ((rank - 1) * playerDistance);
            float z = rank == i ? 0 : -2;
            chara.transform.localPosition = new Vector3(x, 0, z);
        }
    }
    public void RefreshEnemyPos(int rank)
    {
        //刷新敌人角色位置
        for (int i = 0; i < EnemyList.Count; i++)
        {
            GameObject chara = EnemyList[i].model;
            float x = i * enemyDistance - EnemyOffset;
            chara.transform.localPosition = new Vector3(x, 0, 6 + 0.5f * MathF.Cos(x));
            chara.transform.forward = PlayerList[rank].transform.position - chara.transform.position;
        }
    }
    //重构
    public void RefreshCharaPosNew(int rank)
    {
        //玩家位置4个相似位置 敌人位置 玩家身前
        //刷新玩家角色位置
        for (int i = 0; i < PlayerList.Count; i++)
        {
            GameObject chara = PlayerList[i].model;
            float x = i * playerDistance - PlayerOffset - ((rank - 1) * playerDistance);
            float z = rank == i ? 0 : -2;
            chara.transform.position = new Vector3(x, 0, z);
        }
        //刷新敌人角色位置
        for (int i = 0; i < EnemyList.Count; i++)
        {
            GameObject chara = EnemyList[i].model;
            float x = i * enemyDistance - EnemyOffset;
            chara.transform.position = new Vector3(x, 0, 6 + 0.5f * MathF.Cos(x));
            chara.transform.forward = PlayerList[rank].transform.position - chara.transform.position;
        }
    }
}
