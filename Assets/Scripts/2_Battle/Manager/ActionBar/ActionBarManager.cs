using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

partial class ActionBarManager : MonoBehaviour
{
    static ActionBarManager Instance { get; set; }
    public GameObject mainTurnIconPrefab;
    public GameObject childTurnIconPrefab;
    public GameObject actionBar;
    static List<GameObject> actionIcons = new();
    static List<CharaActionTurn> charaActions = new();
    public static CharaActionTurn CurrentCharaActions => charaActions.FirstOrDefault();
    private void Awake() => Instance = this;
    internal static void Init(List<Character> charaList)
    {
        ////清空行动条
        charaActions.Clear();
        charaActions.AddRange(charaList.Select(chara => new CharaActionTurn(chara)));
        int minActionPoint = charaActions.Min(ca => ca.CurrentActionValue);
        charaActions.ForEach(x => x.CurrentActionValue -= minActionPoint);
        RefreshActionBar();
    }
    /// <summary>
    /// 排序行动队列，触发首个目标
    /// </summary>
    public static void RunAction() => charaActions.First().RunAction();

    private static async void RefreshActionBar(bool isNeedRefreshRank = false)
    {
        Debug.LogWarning("重新计算行动队列");
        int minActionPoint = charaActions.Min(ca => ca.CurrentActionValue);
        charaActions.ForEach(x => x.CurrentActionValue -= minActionPoint);
        charaActions = charaActions.OrderBy(x => x.CurrentActionValue).ToList();


        int currentActionCount = charaActions.Count();
        int currentActionIconCount = actionIcons.Count();
        //创建数量不足的项
        for (int i = currentActionIconCount; i < currentActionCount; i++)
        {
            var newIcon = Instantiate(Instance.mainTurnIconPrefab, Instance.mainTurnIconPrefab.transform.parent);
            newIcon.name = charaActions[i].character.name;
            actionIcons.Add(newIcon);
        }
        //修改每个项可见性和外观
        for (int i = 0; i < actionIcons.Count(); i++)
        {
            GameObject currentActionIcon = actionIcons[i];
            currentActionIcon.SetActive(i <= currentActionCount);

            //设置敌我标识
            currentActionIcon.transform.GetChild(2).GetComponent<Image>().color = charaActions[i].character.IsEnemy ? Color.red : Color.cyan;
            //设置行动值
            currentActionIcon.transform.GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = charaActions[i].CurrentActionValue.ToString();

            /////////////////////////////////////////设置回合的子回合操作////////////////////
            //计算当前回合的子回合数量
            int childTurnCount = charaActions[i].PassiveActions.Count() + charaActions[i].ActiveActions.Count() + (charaActions[i].BasicActionState == 2 ? 0 : 1) - 1;

            //补足缺少的控件
            for (int j = currentActionIcon.transform.GetChild(4).childCount; j < childTurnCount; j++)
            {
                Instantiate(Instance.childTurnIconPrefab, currentActionIcon.transform.GetChild(4));
            }

            //设置可见性
            for (int j = 0; j < currentActionIcon.transform.GetChild(4).childCount; j++)
            {
                if (j < childTurnCount)
                {
                    currentActionIcon.transform.GetChild(4).GetChild(j).gameObject.SetActive(true);
                }
                else
                {
                    currentActionIcon.transform.GetChild(4).GetChild(j).gameObject.SetActive(false);
                }
            }
            //.GetChild(0)
            //设置主回合图标
            //currentActionIcon.transform.GetChild(1).GetChild(0).GetComponent<Image>().sprite = charaActions[i].character.charaIcon;
            try
            {
                currentActionIcon.transform.GetChild(1).GetChild(0).GetComponent<Image>().sprite = charaActions[i].GetActionListInfo().FirstOrDefault().charaSprite;

            }
            catch (Exception)
            {
                var ss = charaActions;
                var s = charaActions[i].GetActionListInfo();
            }
            //设置子回合图标
            for (int j = 1; j < charaActions[i].GetActionListInfo().Count; j++)
            {
                currentActionIcon.transform.GetChild(4).GetChild(j - 1).GetChild(1).GetChild(0).GetComponent<Image>().sprite = charaActions[i].GetActionListInfo()[j].charaSprite;

            }
            //设置子回合图标
        }
        if (isNeedRefreshRank)
        {
            await CustomThread.TimerAsync(0.5f, (progress) =>
            {
                Instance.actionBar.GetComponent<GridLayoutGroup>().padding = new RectOffset(0, 0, (int)(65 * (1 - progress)), 0);
                LayoutRebuilder.MarkLayoutForRebuild(Instance.transform as RectTransform);
            });
            //刷新行动条
            //for (int i = 0; i < 65; i++)
            //{
            //    Instance.actionBar.GetComponent<GridLayoutGroup>().padding = new RectOffset(0, 0, 65 - i, 0);
            //    LayoutRebuilder.MarkLayoutForRebuild(Instance.transform as RectTransform);
            //    await Task.Delay(2);
            //}
        }
    }

    //新增一名角色/衍生物进入行动序列
    public static void AddChara(CharaActionTurn chara)//廷加一个基础行动，一般为衍生物
    {
        //charaActions.AddRange(charaList.Select(chara => new CharaAction(chara)));
        RefreshActionBar();
    }
    //插入某个角色某个类型的某个行动
    public static void AddAction(Character chara, ActionType actionType, Action action)
    {
        Debug.LogWarning("追加角色行动");
        var currentRunAction = charaActions.FirstOrDefault()?.GetActionListInfo().FirstOrDefault();
        charaActions.First().AddAction(actionType, new CharaActionTurn.CharaAction(chara, action));
        RefreshActionBar();
        //判断该回合的首个回合主体是否发生变化，若发生了则重新触发
        if (currentRunAction != charaActions.FirstOrDefault()?.GetActionListInfo().FirstOrDefault())
        {
            Debug.Log("重新触发回合主体");
            charaActions.First().RunAction();
        }
    }
    public static void BasicActionStart()
    {
        Debug.LogWarning("当前行动回合基础行动执行中");
        charaActions.First().BasicActionState = 1;
    }
    /// <summary>
    /// 当前主体回合处理完毕
    /// </summary>
    public static void BasicActionCompleted()
    {
        Debug.LogWarning("当前行动回合基础行动执完毕");
        charaActions.First().BasicActionState = 2;
        RefreshActionBar(false);
        RunAction();
    }
    /// <summary>
    /// 当前被动响应回合处理完毕
    /// </summary>
    public static void PassiveActionCompleted()
    {
        Debug.LogWarning("当前行动响应回合行动执完毕");
        charaActions.First().PassiveActions.Remove(charaActions.First().CurrentExceAction);
        RefreshActionBar();
        RunAction();
    }
    /// <summary>
    /// 当前主动响应回合处理完毕
    /// </summary>
    public static void ActiveActionCompleted()
    {
        Debug.LogWarning("当前行动回合爆发行动执完毕");
        charaActions.First().ActiveActions.Remove(charaActions.First().CurrentExceAction);
        RefreshActionBar();
        RunAction();
    }

}