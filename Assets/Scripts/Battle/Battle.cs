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
    public List<GameObject> enemyPrefebs;
    List<Character> charaList = new();


    private void Start()
    {
        //��ʼ���ж���
        charaList.Clear();
        InitChara(players, enemies);
        CameraTrackController.Init();
        //��ʼ���ж���
        ActionBar.Init(charaList);
        //��ʼ�ж�
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
            Type type = Type.GetType(charaName);
            if (type != null)
            {
                Character charaScript = (Character)Activator.CreateInstance(type);
                charaList.Add(charaScript);
            }
            else
            {
                Debug.LogError("�޷��ҵ���Ӧ������" + charaName);
            }
        }
        for (int i = 0; i < enemyList.Count; i++)
        {
            var charaName = enemyList[i].ToString();
            GameObject charaModel = enemyPrefebs.FirstOrDefault(prefeb => prefeb.name == charaName);
            GameObject chara = Instantiate(charaModel, charaModel.transform.parent);
            chara.SetActive(true);
            chara.transform.position = new Vector3(i * enemyDistance - enemyOffset, 0, 5);
            Type type = Type.GetType(charaName);
            if (type != null)
            {
                Character charaScript = (Character)Activator.CreateInstance(type);
                charaList.Add(charaScript);
            }
            else
            {
                Debug.LogError("�޷��ҵ���Ӧ������" + charaName);
            }
        }
    }
}
