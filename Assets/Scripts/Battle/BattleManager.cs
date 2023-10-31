using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    public List<PlayerType> players;
    public List<EnemyType> enemies;
    public List<GameObject> playerPrefebs;
    public List<GameObject> enemyPrefebs;
    //当前战场上的角色集合
    public static List<Character> charaList = new();

    private void Awake() => Instance = this;
    private void Start()
    {
        //加载角色列表
        charaList.Clear();
        InitChara(players, enemies);
        //初始化摄像机
        CameraTrackController.Init();
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
        var playerDistance = 3;
        var playerOffset = (playerList.Count - 1) * playerDistance / 2f;
        var enemyDistance = 1.5f;
        var enemyOffset = (enemyList.Count - 1) * enemyDistance / 2f;
        for (int i = 0; i < playerList.Count; i++)
        {
            var charaName = playerList[i].ToString();
            GameObject charaModel = playerPrefebs.FirstOrDefault(prefeb => prefeb.name == playerList[i].ToString());
            GameObject chara = Instantiate(charaModel, charaModel.transform.parent);
            chara.SetActive(true);
            chara.transform.position = new Vector3(i * playerDistance - playerOffset, 0, 0);
            Character charaScript = chara.GetComponent<Character>();
            charaScript.IsEnemy = false;
            charaList.Add(charaScript);
        }
        for (int i = 0; i < enemyList.Count; i++)
        {
            var charaName = enemyList[i].ToString();
            GameObject charaModel = enemyPrefebs.FirstOrDefault(prefeb => prefeb.name == charaName);
            GameObject chara = Instantiate(charaModel, charaModel.transform.parent);
            chara.SetActive(true);
            chara.transform.position = new Vector3(i * enemyDistance - enemyOffset, 0, 5);
            Character charaScript = chara.GetComponent<Character>();
            charaScript.IsEnemy = true;
            charaList.Add(charaScript);
        }
    }
}
