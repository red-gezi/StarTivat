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
        //获得一个金币翻倍奇物
        await Test3();
    }
    private async void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            var targets = new List<Buff>()
            {
                MoNiYuZhouBuffList.BuffList.GetBuff((int)MoNiYuZhouBuffList.BufferName.祝福_妙论派),
                MoNiYuZhouBuffList.BuffList.GetBuff((int)MoNiYuZhouBuffList.BufferName.祝福_苹果酿),
                MoNiYuZhouBuffList.BuffList.GetBuff((int)MoNiYuZhouBuffList.BufferName.祝福_琥珀),
            };
            var buff = await OutBattleUIManager.Instance.OpenBlessingSelection(targets);
            Debug.Log(buff.buffName);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            var targets = new List<Buff>()
            {
                MoNiYuZhouBuffList.BuffList.GetBuff((int)MoNiYuZhouBuffList.BufferName.祝福_苹果酿),
                MoNiYuZhouBuffList.BuffList.GetBuff((int)MoNiYuZhouBuffList.BufferName.祝福_琥珀),
            };
            var buff = await OutBattleUIManager.Instance.OpenBlessingSelection(targets);
            Debug.Log(buff.buffName);

        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            var targets = new List<Buff>()
            {
                MoNiYuZhouBuffList.BuffList.GetBuff((int)MoNiYuZhouBuffList.BufferName.祝福_琥珀),
            };
            var buff = await OutBattleUIManager.Instance.OpenBlessingSelection(targets);
            Debug.Log(buff.buffName);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            var targets = new List<Buff>()
            {
                MoNiYuZhouBuffList.BuffList.GetBuff((int)MoNiYuZhouBuffList.BufferName.奇物_相遇之缘),
                MoNiYuZhouBuffList.BuffList.GetBuff((int)MoNiYuZhouBuffList.BufferName.奇物_原石),
                MoNiYuZhouBuffList.BuffList.GetBuff((int)MoNiYuZhouBuffList.BufferName.奇物_纠缠之缘),
            };
            var buff = await OutBattleUIManager.Instance.OpenCurioSelectionAsync(targets);
            Debug.Log(buff.buffName);
        }
    }
    private static async Task Test3()
    {

        var targets = new List<Buff>()
        {
             MoNiYuZhouBuffList.BuffList.GetBuff((int)MoNiYuZhouBuffList.BufferName.奇物_相遇之缘),
             MoNiYuZhouBuffList.BuffList.GetBuff((int)MoNiYuZhouBuffList.BufferName.奇物_原石),
             MoNiYuZhouBuffList.BuffList.GetBuff((int)MoNiYuZhouBuffList.BufferName.奇物_纠缠之缘),
        };
       await OutBattleUIManager.Instance.OpenCurioSelectionAsync(targets);
    }
    private static async Task Test2()
    {
        await GameEventManager.GetItem(MoNiYuZhouBuffList.BuffList, new List<int> { (int)MoNiYuZhouBuffList.BufferName.获得金钱时若金钱大于50则减50并获得欠条 });
        Debug.Log("人物总buff有" + OutBattleManager.CurrentOutBattleInfo.Buffs.Select(buff => ((MoNiYuZhouBuffList.BufferName)buff.id).ToString()).ToJson());
        Debug.Log("人物总金币有" + OutBattleManager.CurrentOutBattleInfo.Gold);
        Debug.Log("#########################1############################");
        //获得10块
        await GameEventManager.GetGoldAsync(10);
        Debug.Log("人物总buff有" + OutBattleManager.CurrentOutBattleInfo.Buffs.Select(buff => ((MoNiYuZhouBuffList.BufferName)buff.id).ToString()).ToJson());
        Debug.Log("人物总金币有" + OutBattleManager.CurrentOutBattleInfo.Gold);
        Debug.Log("#########################2############################");
        //获得100块
        await GameEventManager.GetGoldAsync(100);
        Debug.Log("人物总buff有" + OutBattleManager.CurrentOutBattleInfo.Buffs.Select(buff => ((MoNiYuZhouBuffList.BufferName)buff.id).ToString()).ToJson());
        Debug.Log("人物总金币有" + OutBattleManager.CurrentOutBattleInfo.Gold);
        Debug.Log("#########################3############################");


    }
    private static async Task Test1()
    {
        var targets = new List<int>
        {
            (int)MoNiYuZhouBuffList.BufferName.获得1个金币翻倍奇物
        };
        await GameEventManager.GetItem(MoNiYuZhouBuffList.BuffList, targets);
        Debug.Log("人物总buff有" + OutBattleManager.CurrentOutBattleInfo.Buffs.Select(buff => ((MoNiYuZhouBuffList.BufferName)buff.id).ToString()).ToJson());
        Debug.Log("人物总金币有" + OutBattleManager.CurrentOutBattleInfo.Gold);
        Debug.Log("#########################1############################");
        //获得100金币
        await GameEventManager.GetGoldAsync(100);
        Debug.Log("人物总buff有" + OutBattleManager.CurrentOutBattleInfo.Buffs.Select(buff => ((MoNiYuZhouBuffList.BufferName)buff.id).ToString()).ToJson());
        Debug.Log("人物总金币有" + OutBattleManager.CurrentOutBattleInfo.Gold);
        Debug.Log("#########################2############################");
        //获得每次获得奇物后获得30金币d的效果
        targets = new List<int>
        {
            (int)MoNiYuZhouBuffList.BufferName.每次获得奇物后获得30金币
        };
        await GameEventManager.GetItem(MoNiYuZhouBuffList.BuffList, targets);
        Debug.Log("人物总buff有" + OutBattleManager.CurrentOutBattleInfo.Buffs.Select(buff => ((MoNiYuZhouBuffList.BufferName)buff.id).ToString()).ToJson());
        Debug.Log("人物总金币有" + OutBattleManager.CurrentOutBattleInfo.Gold);
        Debug.Log("#########################3############################");
        ////获得一个白板奇物
        targets = new List<int>
        {
            (int)MoNiYuZhouBuffList.BufferName.空奇物
        };
        await GameEventManager.GetItem(MoNiYuZhouBuffList.BuffList, targets);
        Debug.Log("人物总buff有" + OutBattleManager.CurrentOutBattleInfo.Buffs.Select(buff => ((MoNiYuZhouBuffList.BufferName)buff.id).ToString()).ToJson());
        Debug.Log("人物总金币有" + OutBattleManager.CurrentOutBattleInfo.Gold);
        Debug.Log("#########################4############################");
    }
}
