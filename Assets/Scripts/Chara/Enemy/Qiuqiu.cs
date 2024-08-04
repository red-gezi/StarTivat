using System.Threading.Tasks;
using UnityEngine;

class Qiuqiu : Character
{
    private void Awake()
    {
        CharacterInit("丘丘人", 70, ElementType.Pyro, "兔兔伯爵", "箭如雨下");
    }
    public override Task AttackAction()
    {
        throw new System.NotImplementedException();
    }

    public override Task BrustAction()
    {
        throw new System.NotImplementedException();
    }

    public override ActionData BasicAttackSkillData => throw new System.NotImplementedException();

    public override ActionData BrustSkillData => throw new System.NotImplementedException();

    public override ActionData SpecialSkillData => throw new System.NotImplementedException();

    public override Task SkillAction()
    {
        throw new System.NotImplementedException();
    }

    public override async Task EnemySkillAction()
    {
        Debug.Log("丘丘人使用了随机攻击");
        PlayAnimation(AnimationType.Skill_Pose);
        //调整摄像机
        await Task.Delay(1000);
        ActionBarManager.BasicActionCompleted();
    }
}
