using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class ActionBarManager : MonoBehaviour
{
    static List<CharaActionTurn> charaActions = new();
    internal static void Init(List<Character> charaList)
    {
        charaActions.Clear();
        charaActions.AddRange(charaList.Select(chara => new CharaActionTurn(chara)));
        int minActionPoint = charaActions.Min(ca => ca.CurrentActionValue);
        charaActions.ForEach(x => x.CurrentActionValue -= minActionPoint);
    }
    /// <summary>
    /// 排序行动队列，触发首个目标
    /// </summary>
    public static void RunAction()
    {
        Debug.LogWarning("重新计算行动队列");
        int minActionPoint = charaActions.Min(ca => ca.CurrentActionValue);
        charaActions.ForEach(x => x.CurrentActionValue -= minActionPoint);
        charaActions = charaActions.OrderBy(x => x.CurrentActionValue).ToList();
        //刷新行动条




        //执行行动条
        charaActions.First().RunAction();
    }
    //新增一名角色/衍生物进入行动序列
    public void AddChara(CharaActionTurn chara)//廷加一个基础行动，一般为衍生物
    {
        //charaActions.AddRange(charaList.Select(chara => new CharaAction(chara)));
    }
    //插入某个角色某个类型的某个行动
    public void AddAction(Character chara, ActionType actionType, Action action)
    {
        Debug.LogWarning("追加角色行动");
        charaActions.First().AddAction(actionType, new CharaActionTurn.CharaAction(chara, action));
    }
    public static void BasicActionCompleted()
    {
        Debug.LogWarning("当前行动回合已完成基础行动");
        charaActions.First().BasicActionCompleted = true;
        RunAction();
    }
    public class CharaActionTurn
    {
        //行动格主体
        public Character character;
        public bool isTemp = false;
        public bool BasicActionCompleted = false;
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
            Debug.Log($"当前队列{charaActions.Select(action => $"{ action.character.gameObject.name}:剩余行动力：{ action.CurrentActionValue}/总行动力：{action.BasicActionValue}").ToJson()}");
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
            else if (!BasicActionCompleted)
            {
                Debug.Log("进行了主回合操作");

                BasicAction.skillAction();
                BasicActionCompleted = true;
            }
            //如果额外回合，爆发回合都为空，且普通行动执行完，则结束该人物回合
            else
            {
                Debug.Log("操作都已执行完毕，跳过回合");
                EndAction();
            }
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
            BasicActionCompleted = false;
            CurrentActionValue = BasicActionValue;
            ActionBarManager.RunAction();
        }
        public class CharaAction
        {
            public Action skillAction;
            public Sprite charaSprite;

            public CharaAction(Character character, Action action)
            {
                charaSprite = character.actionBarIcon;
                skillAction = action;
            }
        }
    }

}