using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Battle : MonoBehaviour
{
    public List<PlayerType> players;
    public List<EnemyType> enemies;
    public List<GameObject> playerPrefebs;
    [ShowInInspector]
    public List<GameObject> enemyPrefebs;

    private void Start()
    {
        InitChara(players, enemies);
    }
    public void InitChara(List<PlayerType> playerList, List<EnemyType> enemyList)
    {
        var playerDistance = 3;
        var playerOffset = (playerList.Count - 1) * playerDistance / 2f;
        var enemyDistance = 2;
        var enemyOffset = (enemyList.Count - 1) * enemyDistance / 2f;
        for (int i = 0; i < playerList.Count; i++)
        {
            GameObject charaModel = playerPrefebs.FirstOrDefault(prefeb => prefeb.name == playerList[i].ToString());
            GameObject chara = Instantiate(charaModel, charaModel.transform.parent);
            chara.SetActive(true);
            chara.transform.position = new Vector3(i * playerDistance - playerOffset, 0, 0);
        }
        for (int i = 0; i < enemyList.Count; i++)
        {
            GameObject charaModel = enemyPrefebs.FirstOrDefault(prefeb => prefeb.name == enemyList[i].ToString());

            GameObject chara = Instantiate(charaModel, charaModel.transform.parent);
            chara.SetActive(true);
            chara.transform.position = new Vector3(i * enemyDistance - enemyOffset, 0, 5);
        }
    }
}
public enum PlayerType
{
    Nahida
}
public enum EnemyType
{
    Qiuqiu,
    Cube,
    Sphere
}