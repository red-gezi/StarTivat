using System.Collections.Generic;
using UnityEngine;

class CharaBoardManager : MonoBehaviour
{
    public static CharaBoardManager Instance;
    public GameObject charaBoardPrefab;
    static List<GameObject> charaBoardList;
    private void Awake() => Instance = this;
    //初始化人物信息
    public static void Init()
    {
        for (int i = charaBoardList.Count; i < BattleManager.PlayerList.Count; i++)
        {
            var newCharaBoard = Instantiate(Instance.charaBoardPrefab);

            charaBoardList.Add(newCharaBoard);
        }
        //隐藏所有角色面板
        charaBoardList.ForEach(board => board.SetActive(false));
        //刷新每个面板的数据
        for (int i = 0; i < BattleManager.PlayerList.Count; i++)
        {
            //开启需要数量的角色面板
            charaBoardList[i].SetActive(true);
            //设置角色图标
            //设置元素爆发图标
            //BattleManager.PlayerList[i]
        }
    }
    public static void Refresh()
    {
        //刷新每个面板的数据
        for (int i = 0; i < BattleManager.PlayerList.Count; i++)
        {
            //刷新角色血量
            //刷新角色盾量
            //刷新角色状态

            //设置元素爆发能量槽
        }
    }
    public void ClickBrust(GameObject charaBoard)
    {
        var index = charaBoardList.IndexOf(charaBoard);
       var chara= BattleManager.PlayerList[index];
        //校验是否可以触发
        ActionBarManager.AddAction(chara, ActionType.Brust, chara.WaitForBrustSkill);
    }
}