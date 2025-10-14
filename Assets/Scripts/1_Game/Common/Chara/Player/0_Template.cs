using System.Threading.Tasks;
//角色模板类
public class Template : Character
{
    public override SkillData BasicSkillData => throw new System.NotImplementedException();

    public override SkillData SpecialSkillData => throw new System.NotImplementedException();

    public override SkillData BrustSkillData => throw new System.NotImplementedException();

    public override Task AttackAction()
    {
        throw new System.NotImplementedException();
    }

    public override Task BrustAction()
    {
        throw new System.NotImplementedException();
    }

    public override Task SkillAction()
    {
        throw new System.NotImplementedException();
    }
}