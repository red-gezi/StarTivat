class Nahida : Character
{
    public override AtcionData GetBasicAttackActionData()
    {
        return new AtcionData()
        {
            ActionType = ActionType.SingleTarget,
        };
    }
    public override AtcionData GetSpecialSkillActionData()
    {
        return new AtcionData()
        {
            ActionType = ActionType.AreaOfEffect,
        };
    }
    public override AtcionData GetBrustSkillActionData()
    {
        return base.GetBrustSkillActionData();
    }
    public override void OnCharaDead()
    {

    }
}
