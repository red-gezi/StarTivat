using System.Collections.Generic;
using System.Threading.Tasks;

class Nahida : Character
{
    //获得基础攻击的一些数据，如消耗/回复技能点数，类型，生效对象，镜头控制数据等
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
        return new AtcionData()
        {
            ActionType = ActionType.AreaOfEffect,
        };
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

    public override Task BasicAttackAction()
    {
        throw new System.NotImplementedException();
    }

    public override List<AtcionData> GetEnemySkillActionData()
    {
        throw new System.NotImplementedException();
    }

    public override void WaitForBrustSkill()
    {
        throw new System.NotImplementedException();
    }

    public override Task SpecialSkillAction()
    {
        throw new System.NotImplementedException();
    }

    public override Task GetBrustSkillAction()
    {
        throw new System.NotImplementedException();
    }

    public override void OnCharaLightHit()
    {
        throw new System.NotImplementedException();
    }

    public override void OnCharaHeavyHurt()
    {
        throw new System.NotImplementedException();
    }
}
