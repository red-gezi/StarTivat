using System.Collections.Generic;
using UnityEngine;
public class SkillData : EventData
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
    public float SkillHPMultiplier { get; set; }
    public float SkillDefMultiplier { get; set; }
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
