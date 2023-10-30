using System.Collections.Generic;
using UnityEngine;

public class ActionData
{
    public SkillType skillType;
    public ActionType actionType;
    public Sprite icon;
    public int abilityPointChange;
    public Character sender;
    public List<Character> DefaultTargets;
    public bool isTargetBelongPlayer;
}