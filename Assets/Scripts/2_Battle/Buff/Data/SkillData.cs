using System.Collections.Generic;
using UnityEngine;
public class SkillData : GameEventData
{


    public Sprite SkillIcon { get; set; }
    public string SkillNmae { get; set; }
    public List<SkillTag> SkillTags { get; set; } = new();
    public Sprite BrustCharaIcon { get; set; }
    public int SkillPointChange { get; set; }
    public Character Sender { get; set; }
    public Character Receiver { get; set; }
    public CharaData CurrentCharaData { get; set; }
    public List<Character> DefaultTargets { get; set; }
    public int TargetMultiple { get; set; }
    public int DiffusionMultiple { get; set; }
    public float SkillAktMultiplier { get; set; }
    //锁定目标无法更改
    public bool IsLockTarget { get; set; }
    //是否是扩散目标
    public bool IsDiffusionTarget { get; set; }
    //生效目标是否是敌人
    public bool TargetIsEnemy { get; set; }
    public SkillData Clone() => (SkillData)MemberwiseClone();
    public SkillData RedirectTarget(Character newCharacter) => (Receiver = newCharacter, this).Item2;
    public ElementType SkillElement { get; set; }
    public int TurnsRemaining { get; set; }

}
public class ElementalReactionData : GameEventData
{
    public int Point { get; set; }
    public bool IsCritical { get; set; }
    public int TurnsRemaining { get; set; }
    public ReactionType CurrentReactionType { get; set; }

    public ElementalReactionData(int point, bool isCritical, ElementType pyro, int turnsRemaining, Character target, ReactionType currentReactionType)
    {
        Point = point;
        IsCritical = isCritical;
        TurnsRemaining = turnsRemaining;
        CurrentReactionType = currentReactionType;
    }

}
