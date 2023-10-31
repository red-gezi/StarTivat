using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
class Traveler : Character
{
    private void Awake()
    {
        MaxActionPoint = 50;
    }
    //获得基础攻击的一些数据，如消耗/回复技能点数，类型，生效对象，镜头控制数据等
    public override ActionData GetBasicAttackSkillData() => new ActionData()
    {
        CurrentSkillType = SkillType.SingleTarget,
        Icon = basicAttackIcon,
        AbilityPointChange = 1,
        CurrentActionType = ActionType.BasicAttack,
        DefaultTargets = BattleManager.charaList.Where( chara=>chara.IsEnemy).Take(1).ToList(),
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
        //播放动作
        PlayAnimation(AnimationType.BasicAttack);
        //调整摄像机
        //
        await Task.Delay(1000);
        ActionBarManager.BasicActionCompleted();
    }

    public override async Task SpecialSkillAction()
    {
        Debug.Log("纳西妲使用了元素战技");
        PlayAnimation(AnimationType.SpecialAttack);
        //调整摄像机
        await Task.Delay(1000);
        ActionBarManager.BasicActionCompleted();
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

    public override void PlayAnimation(AnimationType animationType)
    {
        animator.CrossFade(animationType.ToString(),0.2f);
    }
    public override void PlayAudio(AnimationType animationType)
    {
        audioSource.clip = null;
        audioSource.Play();
    }
}
