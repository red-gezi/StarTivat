﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

class ActionBarManager : MonoBehaviour
{
    static ActionBarManager Instance { get; set; }
    public GameObject actionIconPrefab;
    public GameObject actionBar;
    static List<GameObject> actionIcons = new();
    static List<CharaActionTurn> charaActions = new();
    private void Awake() => Instance = this;
    internal static void Init(List<Character> charaList)
    {
        ////清空控件条
        //actionIcons.ForEach(Destroy);
        //actionIcons.Clear();

        charaActions.Clear();
        charaActions.AddRange(charaList.Select(chara => new CharaActionTurn(chara)));
        int minActionPoint = charaActions.Min(ca => ca.CurrentActionValue);
        charaActions.ForEach(x => x.CurrentActionValue -= minActionPoint);
    }
    /// <summary>
    /// 排序行动队列，触发首个目标
    /// </summary>
    public static void RunAction() => charaActions.First().RunAction();

    private static async void RefreshActionBar(bool isNeedRefreshRank=false)
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
            var newIcon = Instantiate(Instance.actionIconPrefab, Instance.actionIconPrefab.transform.parent);
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
            
            //设置回合多操作
            //计算当前回合操作数
            int actionCount= charaActions[i].ExternActions.Count()+ charaActions[i].BrustActions.Count()+ charaActions[i].BasicActionState==2?0:1;
            //设置操作控件

            //设置图标
            currentActionIcon.transform.GetChild(1).GetChild(0).GetComponent<Image>().sprite = charaActions[i].character.charaIcon;


        }
        if (isNeedRefreshRank)
        {
            //刷新行动条
            for (int i = 0; i < 65; i++)
            {
                Instance.actionBar.GetComponent<GridLayoutGroup>().padding = new RectOffset(0, 0, 65 - i, 0);
                LayoutRebuilder.MarkLayoutForRebuild(Instance.transform as RectTransform);
                await Task.Delay(2);
            }
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
        charaActions.First().AddAction(actionType, new CharaActionTurn.CharaAction(chara, action));
        RefreshActionBar();
    }
    public static void BasicActionCompleted()
    {
        Debug.LogWarning("当前行动回合已完成基础行动");
        charaActions.First().BasicActionState = 2;
        RunAction();
    }
    public class CharaActionTurn
    {
        //行动格主体
        public Character character;
        public bool isTemp = false;
        //0是未执行，1是执行中，2是执行完成
        public int BasicActionState = 0;
        public string name = "";
        public int BasicActionValue { get; set; } = 0;
        public int CurrentActionValue { get; set; } = 0;
        //额外回合
        public Stack<CharaAction> ExternActions { get; set; } = new();
        //基础回合
        public CharaAction BasicAction => new CharaAction(character, character.WaitForSelectSkill);
        //大招插入回合
        public Queue<CharaAction> BrustActions { get; set; } = new();
        public CharaActionTurn(Character character)
        {
            this.character = character;
            BasicActionValue = character.MaxActionPoint;
            CurrentActionValue = BasicActionValue;
        }
        public void RunAction()
        {
            Debug.Log($"判定当前玩家{character.name}行动,额外回合数|{ExternActions.Count},大招回合数{BrustActions.Count}");
            Debug.Log($"当前队列{charaActions.Select(action => $"{action.character.gameObject.name}:剩余行动力：{action.CurrentActionValue}/总行动力：{action.BasicActionValue}").ToJson()}");
            //是否需要改变顺序
            bool isNeedRefreshRank = false;
            //如果额外回合有无行动，则按顺序执行额外回合的
            if (ExternActions.Any())
            {
                Debug.Log("进行了额外回合操作");
                ExternActions.Pop().skillAction();
            }
            //如果爆发回合有行动，则按顺序执行爆发回合的
            else if (BrustActions.Any())
            {
                Debug.Log("进行了爆发回合操作");

                BrustActions.Dequeue().skillAction();
            }
            //如果额外回合，爆发回合都为空，且普通行动未执行完，则执行该回合基础行动
            else if (BasicActionState==0)
            {
                Debug.Log("进行了主回合操作");
                BasicActionState = 1;
                BasicAction.skillAction();
            }
            //如果额外回合，爆发回合都为空，且普通行动执行完，则结束该人物回合
            else
            {
                isNeedRefreshRank = true;
                Debug.Log("操作都已执行完毕，跳过回合");
                EndAction();
            }
            //刷新行动条UI
            RefreshActionBar(isNeedRefreshRank);
        }
        //首先插入普通行动
        public void AddAction(ActionType skillType, CharaActionTurn.CharaAction action)
        {
            switch (skillType)
            {
                case ActionType.Brust:
                    BrustActions.Enqueue(action);
                    break;
                case ActionType.CounterAttack:
                    ExternActions.Push(action);
                    break;
                case ActionType.AdditionalAttack:
                    ExternActions.Push(action);
                    break;
                case ActionType.ExtraAttack:
                    ExternActions.Push(action);
                    break;
                default:
                    Debug.LogError("非法添加");
                    break;
            }
            //刷新
        }
        //结束当前回合
        public void EndAction()
        {
            //如果存在依附主体，行动者buff回合减一，并重置行动值
            //否则直接消除该回合
            //如果所属角色存活，重置点数
            BasicActionState = 0;
            CurrentActionValue = BasicActionValue;
            ActionBarManager.RunAction();
        }
        public class CharaAction
        {
            public Action skillAction;
            public Sprite charaSprite;

            public CharaAction(Character character, Action action)
            {
                charaSprite = character.charaIcon;
                skillAction = action;
            }
        }
    }

}