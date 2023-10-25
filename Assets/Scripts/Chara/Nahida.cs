using System.Collections.Generic;

class Nahida : Character
{
    public override AtcionData GetBasicAttackActionData()
    {
        return new AtcionData()
        {
            ActionType = ActionType.SingleTarget,
        };
    }
    public override AtcionData GetSpecialSkillActionData()
    {
        return new AtcionData()
        {
            ActionType = ActionType.AreaOfEffect,
        };
    }
    public override AtcionData GetBrustSkillActionData()
    {
        return base.GetBrustSkillActionData();
    }
    public override void WaitForSelectSkill()
    {
        //获取技能信息图标
        //刷新图标
        //设置摄像机位置和角度
        //设置选择目标类型
        //玩家按键执行下个操作
        GetBasicAttackActionData();
        GetBasicAttackActionData();

    }
    public override void OnCharaDead()
    {

    }
}
