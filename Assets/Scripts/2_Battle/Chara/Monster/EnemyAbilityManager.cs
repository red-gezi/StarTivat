using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerAbilityManager
{
    public Func<Task> AttackAction;
    public Func<Task> SkillAction;
    public Func<Task> BurstAction;
    public virtual async Task StrengthenAttackAction() { await Task.CompletedTask; }
    public virtual async Task StrengthenSkillAction() { await Task.CompletedTask; }
    public PlayerAbilityManager RegisterAttackAction(Func<Task> action) => (AttackAction = action).Return(this);
    public PlayerAbilityManager RegisterSkillAction(Func<Task> action) => (SkillAction = action).Return(this);
    public PlayerAbilityManager RegisterBurstAction(Func<Task> action) => (BurstAction = action).Return(this);
}
public class EnemyAbilityManager
{
    public class EnemyAbility
    {
        public int MaxCD { get; set; }
        public int CurrentCD { get; set; }
        public Func<Task> AbilityAction { get; set; }
        public Func<bool> Executable { get; set; }
        public void DecreaseCoolDown()
        {
            if (CurrentCD > 0)
            {
                CurrentCD--;
            }
        }
    }

    public List<EnemyAbility> EnemySkillList = new();
    public EnemyAbilityManager Register(Func<Task> abilityAction, int CD = 0, Func<bool> executable = null)
    {
        EnemySkillList.Add(new EnemyAbility()
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
