using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class OutOfBattleManager : InstanceBehaviour<OutOfBattleManager>
{
    //局外模型所处的父物体
    public Transform outBattleParent;
    public Transform enemiesParent;
    public GameObject playerController;
    public static List<Buff> GetCurrentBuff() => new(GameManager.gameData.CurrentOutBattleData.Buffs);
    //每局初始化一个新的
    public static void AddBuff(Buff buff)
    {
        GameManager.gameData.CurrentOutBattleData.Buffs.Add(buff);
    }
    public static void RemoveBuff(Buff buff)
    {
        GameManager.gameData.CurrentOutBattleData.Buffs.Remove(buff);
    }
    public static void ChangeGold(int count)
    {
        GameManager.gameData.CurrentOutBattleData.Gold += count;
    }

    internal void CreatEnemy(OutOfBattleEnemyDatas outOfBattleEnemyDatas)
    {
        //局外使用的敌人模型类型为首个敌人的模型
        var showModel = outOfBattleEnemyDatas.enemyDatas.FirstOrDefault();

        var charaName = showModel.CurrentEnemyName.ToString();
        GameObject charaModel = enemiesParent.Find(charaName).gameObject;
        GameObject chara = Instantiate(charaModel, outBattleParent);
        chara.transform.Find("OutOfBattle").GetComponent<OutOfBattleEnemyManager>().enemyDatas=outOfBattleEnemyDatas;
        chara.name = charaModel.name;
        chara.SetActive(true);
        Character charaScript = chara.GetComponent<Character>();
        charaScript.SwitchOutOfBattleMode();
        charaScript.model = chara;
        charaScript.IsEnemy = true;
    }
}
