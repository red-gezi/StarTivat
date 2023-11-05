using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//���������λ������
public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    //�ǳ�˫��սǰ����
    public List<PlayerType> players;
    public List<EnemyType> enemies;
    //�ǳ�˫��ģ��Ԥ����
    public List<GameObject> playerPrefebs;
    public List<GameObject> enemyPrefebs;
    //�ǳ�˫������
    public static List<Character> charaList = new();
    public static List<Character> PlayerList => charaList.Where(chara => !chara.IsEnemy).ToList();
    public static List<Character> EnemyList => charaList.Where(chara => chara.IsEnemy).ToList();
    //վλ����
    static float playerDistance = 2f;
    static float PlayerOffset => (PlayerList.Count - 1) * playerDistance / 2f;
    static float enemyDistance = 1.5f;
    static float EnemyOffset => (EnemyList.Count - 1) * enemyDistance / 2f;

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
        //�������ô��쳡������
        for (int i = 0; i < playerList.Count; i++)
        {
            var charaName = playerList[i].ToString();
            GameObject charaModel = playerPrefebs.FirstOrDefault(prefeb => prefeb.name == playerList[i].ToString());
            GameObject chara = Instantiate(charaModel, charaModel.transform.parent);
            chara.name = charaModel.name + $"վλ:{i + 1}";
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
            chara.name = charaModel.name + $"վλ:{i + 1}";
            chara.SetActive(true);
            Character charaScript = chara.GetComponent<Character>();
            charaScript.model = chara;
            charaScript.IsEnemy = true;
            charaList.Add(charaScript);
        }
        RefreshCharaPos(0);
    }
    [Button("ˢ��λ��")]
    //���ݵ�ǰվλ����ˢ�³�������λ��
    public static void RefreshCharaPos(int rank)
    {
        //ˢ����ҽ�ɫ
        for (int i = 0; i < PlayerList.Count; i++)
        {
            GameObject chara = PlayerList[i].model;
            float x = i * playerDistance - PlayerOffset - ((rank - 1) * playerDistance);
            float z = rank == i ? 0 : -2;
            chara.transform.position = new Vector3(x, 0, z);
        }
        //ˢ�µ��˽�ɫ
        for (int i = 0; i < EnemyList.Count; i++)
        {
            GameObject chara = EnemyList[i].model;
            float x = i * enemyDistance - EnemyOffset;
            chara.transform.position = new Vector3(x, 0, 6 + 0.5f * MathF.Cos(x));
            chara.transform.forward = PlayerList[rank].transform.position - chara.transform.position;
        }
    }

}
