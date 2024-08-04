using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
class Traveler : Character
{
    private void Awake()
    {
        name = "旅行者";
        MaxActionPoint = 50;
        PlayerElement = ElementType.Anemo;

    }
    //获得基础攻击的一些数据，如消耗/回复技能点数，类型，生效对象，镜头控制数据等
    public override ActionData BasicAttackSkillData => new ActionData()
    {
        CurrentSkillType = SkillType.SingleTarget,
        SkillIcon = basicAttackIcon,
        SkillPointChange = 1,
        CurrentActionType = ActionType.BasicAttack,
        DefaultTargets = BattleManager.EnemyList.Take(1).ToList(),
        TargetIsEnemy = true,
        Sender = this,
    };
    public override ActionData SpecialSkillData => new ActionData()
    {
        CurrentSkillType = SkillType.Diffusion,
        SkillIcon = specialSkillIcon,
        SkillPointChange = -1,
        CurrentActionType = ActionType.SpecialSkill,
        DefaultTargets = BattleManager.EnemyList.Skip(2).Take(1).ToList(),
        TargetIsEnemy = true,
        Sender = this,
    };
    public override ActionData BrustSkillData => new ActionData()
    {
        CurrentSkillType = SkillType.SingleTarget,
        SkillIcon = brustSkillIcon,
        BrustCharaIcon = largeCharaIcon,
        SkillPointChange = 0,
        CurrentActionType = ActionType.Brust,
        DefaultTargets = BattleManager.EnemyList.Skip(2).Take(1).ToList(),
        TargetIsEnemy = true,
        Sender = this,
    };
    public override async Task AttackAction()
    {
        Debug.Log(name + "进行普通攻击");
        //播放动作
        PlayAnimation(AnimationType.Attack_Pose);
        //调整摄像机
        await CalculateHitPointsAsync(200, ElementType.Anemo, 2, SelectManager.CurrentSelectTargets);
        await Task.Delay(1000);
        ActionBarManager.BasicActionCompleted();
    }

    public override async Task SkillAction()
    {
        Debug.Log(name + "使用了元素战技");
        PlayAnimation(AnimationType.Skill_Pose);
        //调整摄像机
        await Task.Delay(1000);
        await CalculateHitPointsAsync(200, ElementType.Anemo, 2, SelectManager.CurrentSelectTargets);
        ActionBarManager.BasicActionCompleted();
    }
    public override async Task BrustAction()
    {
        Debug.Log(name + "使用了元素爆发");
        PlayAnimation(AnimationType.Burst_Pose);
        await CalculateHitPointsAsync(200, ElementType.Anemo, 2, SelectManager.CurrentSelectTargets);
        //调整摄像机
        await Task.Delay(1000);
        ActionBarManager.ActiveActionCompleted();
    }
    public override async Task EnemySkillAction()
    {
        Debug.Log(name + "作为敌人进行攻击");
        await Task.Delay(1000);
        ActionBarManager.BasicActionCompleted();
    }
}
