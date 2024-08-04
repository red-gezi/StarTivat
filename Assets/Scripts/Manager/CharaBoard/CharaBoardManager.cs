using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

class CharaBoardManager : MonoBehaviour
{
    public static CharaBoardManager Instance;
    public GameObject charaBoardPrefab;
    static List<GameObject> charaBoardList = new();
    private void Awake() => Instance = this;
    //初始化人物信息
    public static void Init()
    {

        for (int i = charaBoardList.Count; i < BattleManager.PlayerList.Count; i++)
        {
            var newCharaBoard = Instantiate(Instance.charaBoardPrefab, Instance.charaBoardPrefab.transform.parent);

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
            charaBoardList[i].transform.GetChild(0).GetComponent<Image>().sprite = BattleManager.PlayerList[i].miniCharaIcon;
            //设置元素爆发图标
            charaBoardList[i].transform.GetChild(1).GetComponent<Image>().sprite = BattleManager.PlayerList[i].brustSkillIcon;
            //设置元素爆发图标边缘颜色
            charaBoardList[i].transform.GetChild(1).GetComponent<Image>().material = new Material(charaBoardList[i].transform.GetChild(1).GetComponent<Image>().material);
            charaBoardList[i].transform.GetChild(1).GetComponent<Image>().material.color = BattleManager.PlayerList[i].PlayerElement switch
            {
                ElementType.Anemo => Color.green,
                ElementType.Pyro => Color.red,
                ElementType.Hydro => Color.blue,
                ElementType.Electro => new Color(1, 0, 1),
                ElementType.Cryo => Color.cyan,
                ElementType.Geo => Color.yellow,
                ElementType.Herb => Color.green,
                _ => Color.white
            } * 3;

        }
    }
    public static void Refresh()
    {
        //刷新每个面板的数据
        for (int i = 0; i < BattleManager.PlayerList.Count; i++)
        {

            //刷新角色血量
            charaBoardList[i].transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = BattleManager.PlayerList[i].CurrentHealthPoints.ToString();
            //刷新角色盾量
            //刷新角色状态

            //设置元素爆发能量槽
        }
    }
    public void ClickBrust(GameObject charaBoard)
    {
        var index = charaBoardList.IndexOf(charaBoard);
        var chara = BattleManager.PlayerList[index];
        Debug.Log("点击了" + chara.name);
        //校验是否可以触发
        ActionBarManager.AddAction(chara, ActionType.Brust, chara.WaitForBrustSkill);
    }
}