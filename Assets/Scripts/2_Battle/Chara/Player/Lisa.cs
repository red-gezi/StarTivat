using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
class Lisa : Character
{
    //初始赋值人物数据
    private void Awake()
    {
        CharacterInit();
        RegisterCharaData(new CharaData()
        {
            CharaName = "丽莎",
            BaseAttack = 800,
            BaseDefense = 0.6f,
            BaseCriticalDamage = 150,
            MaxElementalEnergy = 100,
            MaxActionPoint = 60,
            EnergyRecharge = 120,
            PlayerElement = ElementType.Electro
        });
        //注册作为玩家角色技能
        RegisterAttackAction(AttackAction);
        RegisterSkillAction(SkillAction);
        RegisterBurstAction(BrustAction);
        //注册作为敌人角色技能
    }
    //配置技能的基础数据，如消耗/回复技能点数，类型，生效对象，镜头控制数据等
    public override SkillData BasicSkillData => new SkillData()
    {
        SkillIcon = basicSkillIcon,
        SkillPointChange = 1,
        SkillTags = { SkillTag.SingleTarget, SkillTag.BasicAttack },
        DefaultTargets = BattleManager.CurrentBattle.charaList.Where(chara => chara.IsEnemy).Take(1).ToList(),
        TargetIsEnemy = true,
        Sender = this,
    };
    public override SkillData SpecialSkillData => new SkillData()
    {
        SkillIcon = specialSkillIcon,
        SkillPointChange = -1,
        SkillTags = { SkillTag.AreaOfEffect, SkillTag.SpecialSkill },
        DefaultTargets = BattleManager.CurrentBattle.charaList.Where(chara => chara.IsEnemy).ToList(),
        TargetIsEnemy = true,
        Sender = this,
    };
    public override SkillData BrustSkillData => new SkillData()
    {
        SkillIcon = brustSkillIcon,
        BrustCharaIcon = largeCharaIcon,
        SkillPointChange = 0,
        SkillTags = { SkillTag.AreaOfEffect, SkillTag.Brust },
        DefaultTargets = BattleManager.CurrentBattle.charaList.Where(chara => chara.IsEnemy).Skip(2).Take(1).ToList(),
        TargetIsEnemy = true,
        Sender = this,
    };
    public override async Task AttackAction()
    {
        Debug.Log(name + "进行普通攻击");
        SkillPointManager.ChangePoint(BasicSkillData.SkillPointChange);
        //播放动作
        PlayAnimation(AnimationType.Attack_Pose);
        //调整摄像机
        //
        await CalculateHitPointsAsync(200, ElementType.Electro, 2, SelectManager.CurrentSelectTargets);
        await Task.Delay(1000);
        ActionBarManager.BasicActionCompleted();
    }

    public override async Task SkillAction()
    {
        Debug.Log(name + "使用了元素战技");
        SkillPointManager.ChangePoint(SpecialSkillData.SkillPointChange);
        PlayAnimation(AnimationType.Skill_Pose);
        //调整摄像机
        await CalculateHitPointsAsync(200, ElementType.Electro, 2, SelectManager.CurrentSelectTargets);
        await Task.Delay(1000);
        ActionBarManager.BasicActionCompleted();
    }

    public override async Task BrustAction()
    {
        Debug.Log(name + "使用了元素爆发");
        PlayAnimation(AnimationType.Skill_Pose);
        //调整摄像机
        await Task.Delay(1000);
        ActionBarManager.ActiveActionCompleted();
    }
    //public override async Task EnemySkillAction()
    //{
    //    Debug.Log(name + "作为敌人进行攻击");
    //    await Task.Delay(1000);
    //    ActionBarManager.BasicActionCompleted();
    //}
}
