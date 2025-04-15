public enum SkillTag
{
    //技能目标
    SingleTarget,
    Diffusion,
    AreaOfEffect,
    Support,
    Hindrance,
    Healing,

    //技能回合种类
    BasicAttack,
    SpecialSkill,
    Brust,
    /// <summary>
    /// 反击
    /// </summary>
    CounterAttack,
    /// <summary>
    /// 追加攻击
    /// </summary>
    AdditionalAttack,
    /// <summary>
    /// 额外回合攻击
    /// </summary>
    ExtraAttack,
}