using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class BuffTest : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        OutBattleManager.Init();
        var chara = GetComponent<Character>();
        BattleManager.CurrentBattle = new BattleManager();
        BattleManager.CurrentBattle.charaList.Add(chara);
        MoNiYuZhouBuffList.Init();
        //���һ����ҷ�������
        await Test3();
    }
    private async void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            var targets = new List<Buff>()
            {
                MoNiYuZhouBuffList.BuffList.GetBuff((int)MoNiYuZhouBuffList.BufferName.ף��_������),
                MoNiYuZhouBuffList.BuffList.GetBuff((int)MoNiYuZhouBuffList.BufferName.ף��_ƻ����),
                MoNiYuZhouBuffList.BuffList.GetBuff((int)MoNiYuZhouBuffList.BufferName.ף��_����),
            };
            var buff = await OutBattleUIManager.Instance.OpenBlessingSelection(targets);
            Debug.Log(buff.buffName);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            var targets = new List<Buff>()
            {
                MoNiYuZhouBuffList.BuffList.GetBuff((int)MoNiYuZhouBuffList.BufferName.ף��_ƻ����),
                MoNiYuZhouBuffList.BuffList.GetBuff((int)MoNiYuZhouBuffList.BufferName.ף��_����),
            };
            var buff = await OutBattleUIManager.Instance.OpenBlessingSelection(targets);
            Debug.Log(buff.buffName);

        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            var targets = new List<Buff>()
            {
                MoNiYuZhouBuffList.BuffList.GetBuff((int)MoNiYuZhouBuffList.BufferName.ף��_����),
            };
            var buff = await OutBattleUIManager.Instance.OpenBlessingSelection(targets);
            Debug.Log(buff.buffName);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            var targets = new List<Buff>()
            {
                MoNiYuZhouBuffList.BuffList.GetBuff((int)MoNiYuZhouBuffList.BufferName.����_����֮Ե),
                MoNiYuZhouBuffList.BuffList.GetBuff((int)MoNiYuZhouBuffList.BufferName.����_ԭʯ),
                MoNiYuZhouBuffList.BuffList.GetBuff((int)MoNiYuZhouBuffList.BufferName.����_����֮Ե),
            };
            var buff = await OutBattleUIManager.Instance.OpenCurioSelectionAsync(targets);
            Debug.Log(buff.buffName);
        }
    }
    private static async Task Test3()
    {

        var targets = new List<Buff>()
        {
             MoNiYuZhouBuffList.BuffList.GetBuff((int)MoNiYuZhouBuffList.BufferName.����_����֮Ե),
             MoNiYuZhouBuffList.BuffList.GetBuff((int)MoNiYuZhouBuffList.BufferName.����_ԭʯ),
             MoNiYuZhouBuffList.BuffList.GetBuff((int)MoNiYuZhouBuffList.BufferName.����_����֮Ե),
        };
       await OutBattleUIManager.Instance.OpenCurioSelectionAsync(targets);
    }
    private static async Task Test2()
    {
        await GameEventManager.GetItem(MoNiYuZhouBuffList.BuffList, new List<int> { (int)MoNiYuZhouBuffList.BufferName.��ý�Ǯʱ����Ǯ����50���50�����Ƿ�� });
        Debug.Log("������buff��" + OutBattleManager.CurrentOutBattleInfo.Buffs.Select(buff => ((MoNiYuZhouBuffList.BufferName)buff.id).ToString()).ToJson());
        Debug.Log("�����ܽ����" + OutBattleManager.CurrentOutBattleInfo.Gold);
        Debug.Log("#########################1############################");
        //���10��
        await GameEventManager.GetGoldAsync(10);
        Debug.Log("������buff��" + OutBattleManager.CurrentOutBattleInfo.Buffs.Select(buff => ((MoNiYuZhouBuffList.BufferName)buff.id).ToString()).ToJson());
        Debug.Log("�����ܽ����" + OutBattleManager.CurrentOutBattleInfo.Gold);
        Debug.Log("#########################2############################");
        //���100��
        await GameEventManager.GetGoldAsync(100);
        Debug.Log("������buff��" + OutBattleManager.CurrentOutBattleInfo.Buffs.Select(buff => ((MoNiYuZhouBuffList.BufferName)buff.id).ToString()).ToJson());
        Debug.Log("�����ܽ����" + OutBattleManager.CurrentOutBattleInfo.Gold);
        Debug.Log("#########################3############################");


    }
    private static async Task Test1()
    {
        var targets = new List<int>
        {
            (int)MoNiYuZhouBuffList.BufferName.���1����ҷ�������
        };
        await GameEventManager.GetItem(MoNiYuZhouBuffList.BuffList, targets);
        Debug.Log("������buff��" + OutBattleManager.CurrentOutBattleInfo.Buffs.Select(buff => ((MoNiYuZhouBuffList.BufferName)buff.id).ToString()).ToJson());
        Debug.Log("�����ܽ����" + OutBattleManager.CurrentOutBattleInfo.Gold);
        Debug.Log("#########################1############################");
        //���100���
        await GameEventManager.GetGoldAsync(100);
        Debug.Log("������buff��" + OutBattleManager.CurrentOutBattleInfo.Buffs.Select(buff => ((MoNiYuZhouBuffList.BufferName)buff.id).ToString()).ToJson());
        Debug.Log("�����ܽ����" + OutBattleManager.CurrentOutBattleInfo.Gold);
        Debug.Log("#########################2############################");
        //���ÿ�λ���������30���d��Ч��
        targets = new List<int>
        {
            (int)MoNiYuZhouBuffList.BufferName.ÿ�λ���������30���
        };
        await GameEventManager.GetItem(MoNiYuZhouBuffList.BuffList, targets);
        Debug.Log("������buff��" + OutBattleManager.CurrentOutBattleInfo.Buffs.Select(buff => ((MoNiYuZhouBuffList.BufferName)buff.id).ToString()).ToJson());
        Debug.Log("�����ܽ����" + OutBattleManager.CurrentOutBattleInfo.Gold);
        Debug.Log("#########################3############################");
        ////���һ���װ�����
        targets = new List<int>
        {
            (int)MoNiYuZhouBuffList.BufferName.������
        };
        await GameEventManager.GetItem(MoNiYuZhouBuffList.BuffList, targets);
        Debug.Log("������buff��" + OutBattleManager.CurrentOutBattleInfo.Buffs.Select(buff => ((MoNiYuZhouBuffList.BufferName)buff.id).ToString()).ToJson());
        Debug.Log("�����ܽ����" + OutBattleManager.CurrentOutBattleInfo.Gold);
        Debug.Log("#########################4############################");
    }
}
