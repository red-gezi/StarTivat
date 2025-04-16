using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
class Qiuqiu : Character
{
    private async void Awake()
    {
        CharacterInit();
        RegisterCharaData(new CharaData()
        {
            CharaName = "丘丘人",
            BaseAttack = 1000,
            BaseDefense = 0.5f,
            BaseCriticalDamage = 50,
            MaxElementalEnergy = 100,
            MaxActionPoint = 60,
            EnergyRecharge = 100,
            PlayerElement = ElementType.Physical
        });
        //注册作为玩家角色技能
        RegisterAttackAction(EnemySkill1);
        RegisterSkillAction(EnemySkill2);
        RegisterBurstAction(EnemySkill2);
        AttackSkillName = "敲打";
        ElementalSkillName = "点燃火把";
        ElementalBurstName = "炎棒冲锋";

        EnemyAbilitys
            .Register(EnemySkill1, 0, () => true)
            .Register(EnemySkill2, 5, () => true);
        Task a = EnemySkill1();
    }
    public override SkillData BasicSkillData => new()
    {
        SkillIcon = basicSkillIcon,
        SkillPointChange = 1,
        SkillTags = { SkillTag.SingleTarget, SkillTag.BasicAttack },

        DefaultTargets = BattleManager.CurrentBattle.DifferentSideList(this).Take(1).ToList(),
        TargetIsEnemy = true,
        Sender = this,
    };
    public override SkillData SpecialSkillData => throw new System.NotImplementedException();
    public override SkillData BrustSkillData => throw new System.NotImplementedException();

    public override async Task AttackAction() => await EnemySkill1();
    public override async Task SkillAction() => await EnemySkill2();
    public override async Task BrustAction() => await EnemySkill2();
    public override async Task Entrance()
    {
        transform.GetChild(1).gameObject.SetActive(false);
        PlayAnimation(AnimationType.Walk);
        // 模型后移1米
        await CustomThread.TimerAsync(1, progress =>
        {
            transform.GetChild(0).localPosition = new Vector3(0, 0, progress - 1) * 2;
        });
        transform.GetChild(1).gameObject.SetActive(true);
        PlayAnimation(AnimationType.Idle);
    }
    public async Task EnemySkill1()
    {
        SkillNameManager.Show("敲打", IsEnemy);
        //设置摄像机点位
        //设置玩家角色模型朝向
        if (!SelectManager.CurrentSelectTargets.Any())
        {
            SelectManager.CurrentSelectTargets =BattleManager.CurrentBattle.charaList.Where(chara => !chara.IsEnemy).Take(1).ToList();
        }
        var target = SelectManager.CurrentSelectTargets.First();
        PlayAnimation(AnimationType.Walk);

        await CustomThread.TimerAsync(1, progress =>
        {
            transform.GetChild(0).localPosition = Matrix4x4.Rotate(transform.rotation).inverse * (target.ForwardPoint - transform.position) * progress;
            Debug.DrawLine(transform.position, target.ForwardPoint);
        });
        PlayAnimation(AnimationType.Attack);
        await Task.Delay(1000);
        PlayAnimation(AnimationType.Walk);

        await CustomThread.TimerAsync(0.5f, progress =>
        {
            transform.GetChild(0).localPosition = Matrix4x4.Rotate(transform.rotation).inverse * (target.ForwardPoint - transform.position) * (1 - progress);
            Debug.DrawLine(transform.position, target.ForwardPoint);
        });
        ActionBarManager.BasicActionCompleted();
    }
    public async Task EnemySkill2()
    {
        Debug.Log("丘丘人使用了着火");

        await Task.Delay(1000);
        ActionBarManager.BasicActionCompleted();

    }

}