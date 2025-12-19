using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
public partial class EnemyAbilitySystem
{

    public List<EnemyAbilityData> EnemySkillList = new();
    public EnemyAbilitySystem Register(Func<Task> abilityAction, int CD = 0, Func<bool> executable = null)
    {
        EnemySkillList.Add(new EnemyAbilityData()
        {
            MaxCD = CD,
            CurrentCD = CD,
            AbilityAction = abilityAction,
            Executable = executable ?? (() => true),
        });
        return this;
    }
    public async Task Run()
    {
        try
        {
            var targetSkill = EnemySkillList.LastOrDefault(skill => skill.CurrentCD == 0 && skill.Executable());
            if (targetSkill == null)
            {
                Debug.Log("无可触发技能");
            }
            EnemySkillList.ForEach(skill => skill.DecreaseCoolDown());
            targetSkill.CurrentCD = targetSkill.MaxCD;
            await targetSkill.AbilityAction();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

    }
}
