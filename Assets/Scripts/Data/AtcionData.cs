using System.Collections.Generic;
using UnityEngine;

public class ActionData
{
    public SkillType CurrentSkillType { get; set; }
    public ActionType CurrentActionType { get; set; }
    public Sprite Icon { get; set; }
    public int AbilityPointChange { get; set; }
    public Character Sender { get; set; }
    public List<Character> DefaultTargets { get; set; }
    //生效目标是否是敌人
    public bool IsTargetEnemy { get; set; }
}