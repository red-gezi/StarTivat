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
        charaActions.First().Run();
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
        Action BasicAction=>character.WaitForSelectSkill;
        //额外回合
        List<Action> externActions = new();
        //大招插入回合
        List<Action> brustActions = new();
        List<Action> RunAction = new();

        public CharaAction(Character character)
        {
            this.character = character;
        }

        public void Run()
        {
            //如果额外回合有无行动，则按顺序执行额外回合的
            //判断普通回合是否触发完
            //如果没有触发普通回合，优先按顺序执行大招回合的
            //大招为空时执行普通回合

            //如果普通回合触发完，按顺序执行大招回合的
            //如果额外回合和大招队列都为空，切主行动触发完，执行end
        }
        //首先插入普通行动
        public void AddAction(SkillType skillType)
        {
            //如果当前主体是怪物，将其下移，并插入我方人物
            //如果当前主体是我方，将其右移
            //追加攻击、反击、亡语加到左侧，大招加到右侧
        }
        //结束当前回合
        public void EndAction(SkillType skillType)
        {
            //如果存在依附主体，行动者buff回合减一，并重置行动值
            //否则直接消除该回合
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