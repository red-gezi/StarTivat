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
    public List<Character> charaList = new();

    private void Awake() => Instance = this;
    private void Start()
    {
        //初始化行动条
        charaList.Clear();
        InitChara(players, enemies);
        CameraTrackController.Init();
        //初始化行动条
        ActionBar.Init(charaList);
        //开始行动
        ActionBar.Run();
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
            charaScript.IsEnemy = true;
            charaList.Add(charaScript);
            //Type type = Type.GetType(charaName);
            //if (type != null)
            //{
            //    Character charaScript = (Character)Activator.CreateInstance(type);
            //    charaScript.IsEnemy = false;
            //    charaList.Add(charaScript);
            //}
            //else
            //{
            //    Debug.LogError("无法找到对应人物类" + charaName);
            //}
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
            //Type type = Type.GetType(charaName);
            //if (type != null)
            //{
            //    Character charaScript  = (Character)Activator.CreateInstance(type);
            //    charaScript.IsEnemy = true;
            //    charaList.Add(charaScript);
            //}
            //else
            //{
            //    Debug.LogError("无法找到对应人物类" + charaName);
            //}
        }
    }
}
