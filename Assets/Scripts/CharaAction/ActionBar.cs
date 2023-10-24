using System;
using System.Collections.Generic;
using System.Linq;

class ActionBar
{
    static List<CharaAction> charaActions = new();
    internal static void Init(List<Character> charaList)
    {
        charaActions.Clear();
        charaActions.AddRange(charaList.Select(chara => new CharaAction(chara)));
    }
    /// <summary>
    /// 排序行动队列，触发首个目标
    /// </summary>
    public static void Run()
    {

    }
    class CharaAction
    {
        //行动格主体
        Character character;
        bool isTemp = false;
        string name = "";
        int basicActionValue = 100;
        int currentActionValue = 150;
        int showActionValue = 100;
        Action BasicAction;
        List<Action> ExternAction;
        List<Action> RunAction = new();

        public CharaAction(Character character)
        {
            this.character = character;
        }

        public void Run()
        {
            //如果有普通行动，执行普通行动
            //如果没有普通行动，执行额外回合行动

            //如果额外回合执行完普通
            //如果存在依附主体，行动者buff回合减一，并重置冷却
            //否则直接消除该回合

        }
        //首先插入普通行动
        public void AddAction(SkillType skillType)
        {
            //如果当前主体是怪物，将其下移，并插入我方人物
            //如果当前主体是我方，将其右移
            //追加攻击、反击、亡语加到左侧，大招加到右侧
        }
    }
    //新增一名角色
    public void AddChara(ActionType actionType)
    {

    }
    //追加行为
    //
    public void AddAction(ActionType actionType)
    {

    }
}