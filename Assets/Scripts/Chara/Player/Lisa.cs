using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
class Lisa : Character
{
    //初始赋值人物数据
    private void Awake()
    {
        MaxElementalEnergy = 100;
        MaxActionPoint = 60;
        EnergyRecharge = 100;
        ElementalSkillName = "拍照";
        ElementalBurstName = "房子";
        PlayerElement = ElementType.Electro;

    }
    //配置技能的基础数据，如消耗/回复技能点数，类型，生效对象，镜头控制数据等
    public override ActionData GetBasicAttackSkillData() => new ActionData()
    {
        CurrentSkillType = SkillType.SingleTarget,
        Icon = basicAttackIcon,
        AbilityPointChange = 1,
        CurrentActionType = ActionType.BasicAttack,
        DefaultTargets = BattleManager.charaList.Where(chara => chara.IsEnemy).Take(1).ToList(),
        IsTargetEnemy = true,
        Sender = this,
    };
    public override ActionData GetSpecialSkillData() => new ActionData()
    {
        CurrentSkillType = SkillType.AreaOfEffect,
        Icon = specialSkillIcon,
        AbilityPointChange = -1,
        CurrentActionType = ActionType.SpecialSkill,
        DefaultTargets = BattleManager.charaList.Where(chara => chara.IsEnemy).ToList(),
        IsTargetEnemy = true,
        Sender = this,
    };
    public override ActionData GetBrustSkillData() => new ActionData()
    {
        CurrentSkillType = SkillType.AreaOfEffect,
        Icon = brustSkillIcon,
        AbilityPointChange = 0,
        CurrentActionType = ActionType.Brust,
        DefaultTargets = BattleManager.charaList.Where(chara => chara.IsEnemy).Skip(2).Take(1).ToList(),
        IsTargetEnemy = true,
        Sender = this,
    };
    public override void WaitForBrustSkill()
    {
        throw new System.NotImplementedException();
    }

    public override async Task BasicAttackAction()
    {
        Debug.Log("纳西妲进行普通攻击");
        AbilityPointManager.ChangePoint(GetBasicAttackSkillData().AbilityPointChange);
        //播放动作
        PlayAnimation(AnimationType.BasicAttack);
        //调整摄像机
        //
        await Task.Delay(1000);
        CalculateHitPoints(200, SelectManager.currentSelectTarget);

        ActionBarManager.BasicActionCompleted();
    }

    public override async Task SpecialSkillAction()
    {
        Debug.Log("纳西妲使用了元素战技");
        AbilityPointManager.ChangePoint(GetSpecialSkillData().AbilityPointChange);
        PlayAnimation(AnimationType.SpecialAttack);
        //调整摄像机
        await Task.Delay(1000);
        ActionBarManager.BasicActionCompleted();
    }

    public override async Task BrustSkillAction()
    {
        Debug.Log("纳西妲使用了元素爆发");
        AbilityPointManager.ChangePoint(GetSpecialSkillData().AbilityPointChange);
        PlayAnimation(AnimationType.SpecialAttack);
        //调整摄像机
        await Task.Delay(1000);
        ActionBarManager.BasicActionCompleted();
    }
    public override async Task EnemySkillAction()
    {
        Debug.Log("纳西妲作为敌人进行攻击");
        await Task.Delay(1000);
        ActionBarManager.BasicActionCompleted();
    }
}
