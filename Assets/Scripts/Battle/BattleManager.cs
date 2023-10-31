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
    //��ǰս���ϵĽ�ɫ����
    public static List<Character> charaList = new();

    private void Awake() => Instance = this;
    private void Start()
    {
        //���ؽ�ɫ�б�
        charaList.Clear();
        InitChara(players, enemies);
        //��ʼ�������
        CameraTrackController.Init();
        //�رս�ɫѡ��ͼ��
        SelectManager.Close();
        //�رս�ɫ����ͼ��
        SkillManager.Close();
        //��ʼ���ж���
        ActionBarManager.Init(charaList);
        //��ʼ�ж�
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
