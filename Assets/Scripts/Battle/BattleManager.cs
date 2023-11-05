using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//管理场上人物、位置数据
public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    //登场双方战前配置
    public List<PlayerType> players;
    public List<EnemyType> enemies;
    //登场双方模型预制体
    public List<GameObject> playerPrefebs;
    public List<GameObject> enemyPrefebs;
    //登场双方集合
    public static List<Character> charaList = new();
    public static List<Character> PlayerList => charaList.Where(chara => !chara.IsEnemy).ToList();
    public static List<Character> EnemyList => charaList.Where(chara => chara.IsEnemy).ToList();
    //站位配置
    static float playerDistance = 2f;
    static float PlayerOffset => (PlayerList.Count - 1) * playerDistance / 2f;
    static float enemyDistance = 1.5f;
    static float EnemyOffset => (EnemyList.Count - 1) * enemyDistance / 2f;

    private void Awake() => Instance = this;
    private void Start()
    {
        //加载角色列表
        charaList.Clear();
        InitChara(players, enemies);
        //初始化摄像机
        CameraTrackManager.Init(Vector3.zero);
        //关闭角色选择图标
        SelectManager.Close();
        //关闭角色技能图标
        SkillManager.Close();
        //初始化行动条
        ActionBarManager.Init(charaList);
        //开始行动
        ActionBarManager.RunAction();

    }
    public void InitChara(List<PlayerType> playerList, List<EnemyType> enemyList)
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
            charaList.Add(charaScript);
        }
        RefreshCharaPos(0);
    }
    [Button("刷新位置")]
    //根据当前站位索引刷新场上人物位置
    public static void RefreshCharaPos(int rank)
    {
        //刷新玩家角色
        for (int i = 0; i < PlayerList.Count; i++)
        {
            GameObject chara = PlayerList[i].model;
            float x = i * playerDistance - PlayerOffset - ((rank - 1) * playerDistance);
            float z = rank == i ? 0 : -2;
            chara.transform.position = new Vector3(x, 0, z);
        }
        //刷新敌人角色
        for (int i = 0; i < EnemyList.Count; i++)
        {
            GameObject chara = EnemyList[i].model;
            float x = i * enemyDistance - EnemyOffset;
            chara.transform.position = new Vector3(x, 0, 6 + 0.5f * MathF.Cos(x));
            chara.transform.forward = PlayerList[rank].transform.position - chara.transform.position;
        }
    }

}
