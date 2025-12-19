using System;
using System.Threading.Tasks;
public class EnemyAbilityData
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
