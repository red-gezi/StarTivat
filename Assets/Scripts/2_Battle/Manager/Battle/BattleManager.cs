using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
//���������λ������
public class BattleManager : MonoBehaviour
{
    public static BattleManager CurrentBattle;
    //�ǳ�˫��սǰ����
    public List<PlayerType> players;
    public List<EnemyType> enemies;
    //�ǳ�˫��ģ��Ԥ����
    public List<GameObject> playerPrefebs;
    public List<GameObject> enemyPrefebs;
    //�ǳ�˫������
    public List<Character> charaList = new();
    public List<Character> PlayerList => charaList.Where(chara => !chara.IsEnemy).ToList();
    public List<Character> EnemyList => charaList.Where(chara => chara.IsEnemy).ToList();
    public List<Character> SameSideList(Character target) => charaList.Where(chara => !(chara.IsEnemy ^ target.IsEnemy)).ToList();
    public List<Character> DifferentSideList(Character target) => charaList.Where(chara => chara.IsEnemy ^ target.IsEnemy).ToList();
    //ȫ��buff�б�
    public List<Buff> GoblePlayerBuffs = new();
    public List<Buff> GobleEnemyBuffs = new();
    //վλ����
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
        //��ս�ɫ�б�
        charaList.Clear();
        //��ʼ��˫����ɫ
        InitChara(players, enemies);

        //�رս�ɫѡ��ͼ��
        SelectManager.Close();
        //�رս�ɫ����ͼ��
        SkillManager.Close();
        //��ʼ��������Ϣ��
        CharaBoardManager.Init();
        //��ʼ���ж���
        ActionBarManager.Init(charaList);
        //��ʼ���ܵ�
        SkillPointManager.Init();
        //��ʼ�������,���Ƶ���
        await CameraTrackManager.BattleStartAround(EnemyList);
        //֪ͨս����ʼ

        //BroadcastManager.BroadcastEvent(Character., new CharaEvent());
        //�����ж���
        ActionBarManager.RunAction();
    }

    public async void InitChara(List<PlayerType> playerList, List<EnemyType> enemyList)
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
            //charaScript.RefreshElementsUI();
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
            charaScript.RefreshElementsUIAsync();
            charaList.Add(charaScript);
        }
        //���㳡������Ĭ��λ��
        RefreshCharaPos(0);
        //���������Ӻ�������������
        for (int i = 0; i < EnemyList.Count; i++)
        {
            _ = EnemyList[i].Entrance();
        }
    }
    /// <summary>
    /// ���ݵ�ǰվλ����ˢ�³�������λ��
    /// </summary>
    /// <param name="rank"></param>
    [Button("ˢ��λ��")]
    public void RefreshCharaPos(int rank)
    {
        //ˢ����ҽ�ɫλ��
        for (int i = 0; i < PlayerList.Count; i++)
        {
            GameObject chara = PlayerList[i].model;
            float x = i * playerDistance - PlayerOffset - ((rank - 1) * playerDistance);
            float z = rank == i ? 0 : -2;
            chara.transform.position = new Vector3(x, 0, z);
        }
        //ˢ�µ��˽�ɫλ��
        for (int i = 0; i < EnemyList.Count; i++)
        {
            GameObject chara = EnemyList[i].model;
            float x = i * enemyDistance - EnemyOffset;
            chara.transform.position = new Vector3(x, 0, 6 + 0.5f * MathF.Cos(x));
            chara.transform.forward = PlayerList[rank].transform.position - chara.transform.position;
        }
    }
}
