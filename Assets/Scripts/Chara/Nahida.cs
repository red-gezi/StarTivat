using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Threading.Tasks;

class Nahida : Character
{
    [Button("test")]
    public void test()
    {
        SkillManager.ShowBasicAndSpecialSkill(GetBasicAttackSkillData(), GetSpecialSkillData());
    }
    //获得基础攻击的一些数据，如消耗/回复技能点数，类型，生效对象，镜头控制数据等
    public override ActionData GetBasicAttackSkillData()
    {
        return new ActionData()
        {
            skill = SkillType.SingleTarget,
            icon = BasicAttackIcon,
        };
    }
    public override ActionData GetSpecialSkillData()
    {
        return new ActionData()
        {
            skill = SkillType.AreaOfEffect,
            icon = SpecialSkillIcon,
        };
    }
    public override ActionData GetBrustSkillData()
    {
        return new ActionData()
        {
            skill = SkillType.AreaOfEffect,
            icon = BrustSkillIcon,
        };
    }
    public override void WaitForSelectSkill()
    {
        //获取技能信息图标
        //刷新图标
        //设置摄像机位置和角度
        //设置选择目标类型
        //玩家按键执行下个操作
        GetBasicAttackSkillData();
        GetBasicAttackSkillData();

    }
    public override void OnCharaDead()
    {

    }

    public override Task BasicAttackAction()
    {
        throw new System.NotImplementedException();
    }

    public override List<ActionData> GetEnemySkillActionData()
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
