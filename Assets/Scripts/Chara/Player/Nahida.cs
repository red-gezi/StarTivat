using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
class Nahida : Character
{
    //初始赋值人物数据
    private void Awake()
    {
        //name = "纳西妲";
        BaseAttack = 1000;
        BaseDefense = 0.5f;
        MaxElementalEnergy = 100;
        MaxActionPoint = 40;
        EnergyRecharge = 100;
        ElementalSkillName = "拍照";
        ElementalBurstName = "房子";
        PlayerElement = ElementType.Herb;
    }
    //配置技能的基础数据，如消耗/回复技能点数，类型，生效对象，镜头控制数据等
    public override ActionData BasicAttackSkillData => new ActionData()
    {
        CurrentSkillType = SkillType.SingleTarget,
        SkillIcon = basicAttackIcon,
        SkillPointChange = 1,
        CurrentActionType = ActionType.BasicAttack,
        DefaultTargets = BattleManager.charaList.Where(chara => chara.IsEnemy).Take(1).ToList(),
        TargetIsEnemy = true,
        Sender = this,
    };
    public override ActionData SpecialSkillData => new ActionData()
    {
        CurrentSkillType = SkillType.AreaOfEffect,
        SkillIcon = specialSkillIcon,
        SkillPointChange = -1,
        CurrentActionType = ActionType.SpecialSkill,
        DefaultTargets = BattleManager.charaList.Where(chara => chara.IsEnemy).ToList(),
        TargetIsEnemy = true,
        Sender = this,
    };
    public override ActionData BrustSkillData => new ActionData()
    {
        CurrentSkillType = SkillType.AreaOfEffect,
        SkillIcon = brustSkillIcon,
        BrustCharaIcon = largeCharaIcon,
        SkillPointChange = 0,
        CurrentActionType = ActionType.Brust,
        DefaultTargets = BattleManager.charaList.Where(chara => chara.IsEnemy).Skip(2).Take(1).ToList(),
        TargetIsEnemy = true,
        Sender = this,
    };
    public override async Task AttackAction()
    {
        Debug.Log(name + "进行普通攻击");
        AbilityPointManager.ChangePoint(BasicAttackSkillData.SkillPointChange);
        //播放动作
        PlayAnimation(AnimationType.Attack_Pose);
        //调整摄像机
        //CameraTrackManager.
        await CalculateHitPointsAsync(200, ElementType.Herb, 2, SelectManager.CurrentSelectTargets);
        await Task.Delay(1000);
        ActionBarManager.BasicActionCompleted();
    }

    public override async Task SkillAction()
    {
        Debug.Log(name + "使用了元素战技");
        AbilityPointManager.ChangePoint(SpecialSkillData.SkillPointChange);
        PlayAnimation(AnimationType.Skill_Pose);
        //调整摄像机
        await CalculateHitPointsAsync(200, ElementType.Herb, 2, SelectManager.CurrentSelectTargets);
        await Task.Delay(1000);
        ActionBarManager.BasicActionCompleted();
    }

    public override async Task BrustAction()
    {
        Debug.Log(name + "使用了元素爆发");
        AbilityPointManager.ChangePoint(BrustSkillData.SkillPointChange);
        PlayAnimation(AnimationType.Skill_Pose);
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
