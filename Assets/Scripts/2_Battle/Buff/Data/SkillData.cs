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
    public Character Target { get; set; }
    public List<Buff> TargetBuffs { get; set; }

    public List<Character> DefaultTargets { get; set; }
    //锁定目标无法更改
    public bool IsLockTarget {  get; set; }
    //生效目标是否是敌人
    public bool TargetIsEnemy { get; set; }
}
