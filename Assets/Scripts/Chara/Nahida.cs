using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
class Nahida : Character
{
    //获得基础攻击的一些数据，如消耗/回复技能点数，类型，生效对象，镜头控制数据等
    public override ActionData GetBasicAttackSkillData() => new ActionData()
    {
        skillType = SkillType.SingleTarget,
        icon = BasicAttackIcon,
        abilityPointChange = 1,
        actionType = ActionType.BasicAttack,
        DefaultTargets = BattleManager.charaList.Where( chara=>chara.IsEnemy).Take(1).ToList(),
        isEnemyTarget = true,
        sender = this,
    };
    public override ActionData GetSpecialSkillData() => new ActionData()
    {
        skillType = SkillType.AreaOfEffect,
        icon = SpecialSkillIcon,
        abilityPointChange = -1,
        actionType = ActionType.SpecialSkill,
        DefaultTargets = BattleManager.charaList.Where(chara => chara.IsEnemy).ToList(),
        isEnemyTarget = true,
        sender = this,
    };
    public override ActionData GetBrustSkillData() => new ActionData()
    {
        skillType = SkillType.AreaOfEffect,
        icon = BrustSkillIcon,
        abilityPointChange = 0,
        actionType = ActionType.Brust,
        DefaultTargets = BattleManager.charaList.Where(chara => chara.IsEnemy).Skip(2).Take(1).ToList(),
        isEnemyTarget = true,
        sender = this,
    };
    public override void WaitForSelectSkill()
    {
        //获取技能信息图标
        //刷新图标
        //设置摄像机位置和角度
        //设置选择目标类型
        //玩家按键执行下个操作
        SkillManager.ShowBasicAndSpecialSkill(GetBasicAttackSkillData(), GetSpecialSkillData());
    }
    public override void OnCharaDead()
    {

    }


    public override List<ActionData> GetEnemySkillActionData()
    {
        throw new System.NotImplementedException();
    }

    public override void WaitForBrustSkill()
    {
        throw new System.NotImplementedException();
    }

    public override async Task BasicAttackAction()
    {
        Debug.Log("纳西妲进行普通攻击");
        await Task.Delay(1000);
        ActionBar.Run();
    }

    public override async Task SpecialSkillAction()
    {
        Debug.Log("纳西妲使用了元素战技");
        await Task.Delay(1000);
        ActionBar.Run();
    }

    public override Task BrustSkillAction()
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
