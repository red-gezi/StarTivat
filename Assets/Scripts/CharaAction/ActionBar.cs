using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class ActionBar
{
    static List<CharaAction> charaActions = new();
    internal static void Init(List<Character> charaList)
    {

        charaActions.Clear();
        charaActions.AddRange(charaList.Select(chara => new CharaAction(chara)));
        int minActionPoint = charaActions.Min(ca => ca.CurrentActionValue);
        charaActions.ForEach(x => x.CurrentActionValue -= minActionPoint);
    }
    /// <summary>
    /// 排序行动队列，触发首个目标
    /// </summary>
    public static void Run()
    {
        Debug.LogWarning("重整行动条");
        int minActionPoint = charaActions.Min(ca => ca.CurrentActionValue);
        charaActions.ForEach(x => x.CurrentActionValue -= minActionPoint);
        charaActions=charaActions.OrderBy(x => x.CurrentActionValue).ToList();
        charaActions.First().Run();
    }
    class CharaAction
    {
        //行动格主体
        public Character character;
        public bool isTemp = false;
        public bool BasicActionAlreadlyExce = false;
        public string name = "";
        public int basicActionValue = 100;
        public int CurrentActionValue { get; set; } = 150;
        public int showActionValue = 100;
        //额外回合
        public Stack<Action> ExternActions { get; set; } = new();
        //基础回合
        public Action BasicAction => character.WaitForSelectSkill;
        //大招插入回合
        public Queue<Action> BrustActions { get; set; } = new();
        //public List<Action> RunAction = new();

        public CharaAction(Character character)
        {
            this.character = character;
            CurrentActionValue = character.CurrentActionPoint;
        }
        public void Run()
        {
            if (ExternActions.Any())
            {
                ExternActions.Pop()();
            }
            else
            {
                //如果爆发回合有行动，则按顺序执行爆发回合的
                if (BrustActions.Any())
                {
                    BrustActions.Dequeue()();
                }
                else
                {
                    if (BasicActionAlreadlyExce)
                    {
                        
                        EndAction();
                    }
                    else
                    {
                        BasicAction();
                        BasicActionAlreadlyExce = true;
                    }
                }
            }
            //如果额外回合有无行动，则按顺序执行额外回合的
            if (ExternActions.Any())
            {
                ExternActions.Pop()();
            }
            //如果爆发回合有行动，则按顺序执行爆发回合的
            else if (BrustActions.Any())
            {
                BrustActions.Dequeue()();
            }
            //如果额外回合，爆发回合都为空，且普通行动未执行完，则执行该回合基础行动
            else if (!BasicActionAlreadlyExce)
            {
                BasicAction();
                BasicActionAlreadlyExce = true;
            }
            //如果额外回合，爆发回合都为空，且普通行动执行完，则结束该人物回合
            else
            {
                EndAction();
            }
        }
        //首先插入普通行动
        public void AddAction(ActionType skillType ,Action)
        {
            switch (skillType)
            {
                case ActionType.None:
                    break;
                case ActionType.BasicAttack:
                    break;
                case ActionType.SpecialSkill:
                    break;
                case ActionType.Brust:
                    break;
                case ActionType.CounterAttack:
                    break;
                case ActionType.AdditionalAttack:
                    break;
                case ActionType.ExtraAttack:
                    break;
            }
        }
        //结束当前回合
        public void EndAction()
        {
            //如果存在依附主体，行动者buff回合减一，并重置行动值
            //否则直接消除该回合
        }
    }
    //新增一名角色
    public void AddChara(SkillType skillType)
    {

    }
    //追加行为
    //
    public void AddAction(SkillType skillType)
    {

    }
}