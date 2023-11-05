using System.Collections.Generic;
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

    public override ActionData GetSpecialSkillData()
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

    public override async Task EnemySkillAction()
    {
        Debug.Log("丘丘人使用了随机攻击");
        PlayAnimation(AnimationType.SpecialAttack);
        //调整摄像机
        await Task.Delay(1000);
        ActionBarManager.BasicActionCompleted();
    }
}
