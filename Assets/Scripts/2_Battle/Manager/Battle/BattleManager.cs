using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
//管理场上人物、位置数据
public class BattleManager : MonoBehaviour
{
    public static BattleManager CurrentBattle;
    //登场双方战前配置
    public List<PlayerName> players;
    public List<EnemyName> enemies;
    //登场双方模型预制体
    public List<GameObject> playerPrefebs;
    public List<GameObject> enemyPrefebs;
    //登场双方集合
    public List<Character> charaList = new();
    public List<Character> PlayerList => charaList.Where(chara => !chara.IsEnemy).ToList();
    public List<Character> EnemyList => charaList.Where(chara => chara.IsEnemy).ToList();
    public List<Character> SameSideList(Character target) => charaList.Where(chara => !(chara.IsEnemy ^ target.IsEnemy)).ToList();
    public List<Character> DifferentSideList(Character target) => charaList.Where(chara => chara.IsEnemy ^ target.IsEnemy).ToList();
    //全局buff列表
    public List<Buff> GoblePlayerBuffs = new();
    public List<Buff> GobleEnemyBuffs = new();
    //站位配置
    static float playerDistance = 2f;
    static float PlayerOffset => (CurrentBattle.PlayerList.Count - 1) * playerDistance / 2f;
    static float enemyDistance = 1.5f;
    static float EnemyOffset => (CurrentBattle.EnemyList.Count - 1) * enemyDistance / 2f;

    private void Awake() => CurrentBattle = this;
    private async void Start()
    {
        await Init();
    }

    private async Task Init()
    {
        //清空角色列表
        charaList.Clear();
        //初始化双方角色
        InitChara(players, enemies);

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
        await CameraTrackManager.BattleStartAround(EnemyList);
        //通知战斗开始

        //BroadcastManager.BroadcastEvent(Character., new CharaEvent());
        //激活行动条
        ActionBarManager.RunAction();
    }

    public async void InitChara(List<PlayerName> playerList, List<EnemyName> enemyList)
    {
        charaList.Clear();
        //根据配置创造场上人物
        for (int i = 0; i < playerList.Count; i++)
        {
            var charaName = playerList[i].ToString();
            GameObject charaModel = playerPrefebs.FirstOrDefault(prefeb => prefeb.name == playerList[i].ToString());
            GameObject chara = Instantiate(charaModel, charaModel.transform.parent);
            chara.name = charaModel.name + $"站位:{i + 1}";
            chara.SetActive(true);
            Character charaScript = chara.GetComponent<Character>();
            charaScript.model = chara;
            charaScript.IsEnemy = false;
            //charaScript.RefreshElementsUI();
            charaList.Add(charaScript);
        }
        for (int i = 0; i < enemyList.Count; i++)
        {
            var charaName = enemyList[i].ToString();
            GameObject charaModel = enemyPrefebs.FirstOrDefault(prefeb => prefeb.name == charaName);
            GameObject chara = Instantiate(charaModel, charaModel.transform.parent);
            chara.name = charaModel.name + $"站位:{i + 1}";
            chara.SetActive(true);
            Character charaScript = chara.GetComponent<Character>();
            charaScript.model = chara;
            charaScript.IsEnemy = true;
            charaScript.RefreshElementsUIAsync();
            charaList.Add(charaScript);
        }
        //计算场上人物默认位置
        RefreshCharaPos(0);
        //敌人坐标延后，做个出厂动画
        for (int i = 0; i < EnemyList.Count; i++)
        {
            _ = EnemyList[i].Entrance();
        }
    }
    /// <summary>
    /// 根据当前站位索引刷新场上人物位置
    /// </summary>
    /// <param name="rank"></param>
    [Button("刷新位置")]
    public void RefreshCharaPos(int rank)
    {
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
