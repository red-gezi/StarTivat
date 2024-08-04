using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

partial class ActionBarManager
{
    public class CharaActionTurn
    {
        //行动格主体
        public Character character;
        public bool isTemp = false;
        //0是未执行，1是执行中，2是执行完成
        public int BasicActionState = 0;
        public string name = "";
        //设置行动值
        public int BasicActionValue { get; set; } = 0;
        public int CurrentActionValue { get; set; } = 0;

        //当前正在执行的行动，执行完后从队列中移除
        public CharaAction CurrentExceAction { get; set; } = null;
        //被动触发回合（反击，追加攻击，额外回合，复活，亡语等）
        public List<CharaAction> PassiveActions { get; set; } = new();
        //主动触发回合（元素爆发，命途回想等）
        public List<CharaAction> ActiveActions { get; set; } = new();
        //基础主体回合
        public CharaAction BasicAction => new CharaAction(character, character.WaitForSelectSkill);

        public CharaActionTurn(Character character)
        {
            this.character = character;
            BasicActionValue = character.MaxActionPoint;
            CurrentActionValue = BasicActionValue;
        }

        public void RunAction()
        {
            Debug.Log($"判定当前玩家{character.name}行动,额外回合数|{PassiveActions.Count},大招回合数{ActiveActions.Count}");
            Debug.Log($"当前队列{charaActions.Select(action => $"{action.character.gameObject.name}:剩余行动力：{action.CurrentActionValue}/总行动力：{action.BasicActionValue}").ToJson()}");
            //是否需要改变顺序
            //如果额外回合有无行动，则按顺序执行额外回合的
            if (PassiveActions.Any())
            {
                Debug.Log("进行了额外回合操作");
                CurrentExceAction = PassiveActions.First();
                CurrentExceAction.skillAction();
            }
            //如果爆发回合有行动，则按顺序执行爆发回合的
            else if (ActiveActions.Any())
            {
                Debug.Log("进行了爆发回合操作");
                CurrentExceAction = ActiveActions.Last();
                CurrentExceAction.skillAction();
            }
            //如果额外回合，爆发回合都为空，且普通行动未执行完，则执行该回合基础行动
            else if (BasicActionState == 0)
            {
                Debug.Log("等待角色选择技能");
                BasicAction.skillAction();
            }
            //如果额外回合，爆发回合都为空，且普通行动执行完，则结束该人物回合
            else
            {
                Debug.Log("操作都已执行完毕，跳过回合");
                EndAction();
            }
            //刷新行动条UI

        }
        //首先插入普通行动
        public void AddAction(ActionType skillType, CharaAction action)
        {
            switch (skillType)
            {
                case ActionType.Brust:
                    ActiveActions.Add(action);
                    break;
                case ActionType.CounterAttack:
                    PassiveActions.Insert(0, action);
                    break;
                case ActionType.AdditionalAttack:
                    PassiveActions.Insert(0, action);
                    break;
                case ActionType.ExtraAttack:
                    PassiveActions.Insert(0, action);
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
            RefreshActionBar(true);
            //最上方插入一个假的，高度缓慢缩小消失
            ActionBarManager.RunAction();
        }
        public class CharaAction
        {
            public Action skillAction;
            public Sprite charaSprite;

            public CharaAction(Character character, Action action)
            {
                charaSprite = character.miniCharaIcon;
                skillAction = action;
            }
        }
        public List<CharaAction> GetActionListInfo()
        {
            var list = new List<CharaAction>();
            list.AddRange(PassiveActions.ToArray().Reverse());
            switch (BasicActionState)
            {
                case (0):
                    {
                        list.AddRange(ActiveActions.ToArray());
                        list.Add(BasicAction);
                        break;
                    }
                case (1):
                    {
                        list.Add(BasicAction);
                        list.AddRange(ActiveActions.ToArray());
                        break;
                    }
                case (2):
                    {
                        list.AddRange(ActiveActions.ToArray());
                        break;
                    };
                default:
                    break;
            }
            //Debug.Log("载入当前回合所有信息" + list.Select(x=>x.charaSprite.name).ToJson());
            return list;
        }
    }

}