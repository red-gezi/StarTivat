﻿using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

class Qiuqiu : Character
{
    private void Awake()
    {
        MaxActionPoint = 70;
    }
    public override Task BasicAttackAction()
    {
        throw new System.NotImplementedException();
    }

    public override Task BrustSkillAction()
    {
        throw new System.NotImplementedException();
    }

    public override ActionData GetBasicAttackSkillData()
    {
        throw new System.NotImplementedException();
    }

    public override ActionData GetBrustSkillData()
    {
        throw new System.NotImplementedException();
    }

    public override List<ActionData> GetEnemySkillActionData()
    {
        throw new System.NotImplementedException();
    }

    public override ActionData GetSpecialSkillData()
    {
        throw new System.NotImplementedException();
    }

    public override void OnCharaHeavyHurt()
    {
        throw new System.NotImplementedException();
    }

    public override void OnCharaLightHit()
    {
        throw new System.NotImplementedException();
    }

    public override void PlayAnimation(AnimationType animationType)
    {
        throw new System.NotImplementedException();
    }

    public override void PlayAudio(AnimationType animationType)
    {
        throw new System.NotImplementedException();
    }

    public override Task SpecialSkillAction()
    {
        throw new System.NotImplementedException();
    }

    public override void WaitForBrustSkill()
    {
        throw new System.NotImplementedException();
    }

    public override void WaitForSelectSkill()
    {
        Debug.Log("丘丘人随机攻击");
        ActionBarManager.BasicActionCompleted();
    }
}