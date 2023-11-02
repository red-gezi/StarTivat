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
        CameraTrackManager.Init(Vector3.zero);
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
        var playerDistance = 2f;
        var playerOffset = (playerList.Count - 1) * playerDistance / 2f;
        var enemyDistance = 1.5f;
        var enemyOffset = (enemyList.Count - 1) * enemyDistance / 2f;
        for (int i = 0; i < playerList.Count; i++)
        {
            var charaName = playerList[i].ToString();
            GameObject charaModel = playerPrefebs.FirstOrDefault(prefeb => prefeb.name == playerList[i].ToString());
            GameObject chara = Instantiate(charaModel, charaModel.transform.parent);
            chara.name = charaModel.name+ $"վλ:{i + 1}";
            chara.SetActive(true);
            float x = i * playerDistance - playerOffset;
            chara.transform.position = new Vector3(x, 0, -0.5f * MathF.Cos(x));
            Character charaScript = chara.GetComponent<Character>();
            charaScript.IsEnemy = false;
            charaList.Add(charaScript);
        }
        for (int i = 0; i < enemyList.Count; i++)
        {
            var charaName = enemyList[i].ToString();
            GameObject charaModel = enemyPrefebs.FirstOrDefault(prefeb => prefeb.name == charaName);
            GameObject chara = Instantiate(charaModel, charaModel.transform.parent);
            chara.name = charaModel.name + $"վλ:{i + 1}";
            chara.SetActive(true);
            float x = i * enemyDistance - enemyOffset;
            chara.transform.position = new Vector3(x, 0, 5 + 0.5f * MathF.Cos(x));
            Character charaScript = chara.GetComponent<Character>();
            charaScript.IsEnemy = true;
            charaList.Add(charaScript);
        }
    }
}
