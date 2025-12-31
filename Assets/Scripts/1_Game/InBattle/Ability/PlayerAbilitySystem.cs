using System;
using System.Threading.Tasks;

public class PlayerAbilitySystem
{
    public Func<Task> AttackAction;
    public Func<Task> SkillAction;
    public Func<Task> BurstAction;
    public virtual async Task StrengthenAttackAction() { await Task.CompletedTask; }
    public virtual async Task StrengthenSkillAction() { await Task.CompletedTask; }

}
