using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public abstract class Character:MonoBehaviour
{
    public GameObject model;
    public GameObject largeLock;
    public GameObject smallLock;
    public bool IsEnemy { get; set; }
    public int HealthPoints { get; set; } // 生命值
    public int MaxHealthPoints { get; set; } // 生命值上限
    public int Attack { get; set; } // 攻击力
    public int Defense { get; set; } // 防御力
    public int CurrentActionPoint { get; set; } // 当前剩余行动值
    public int MaxActionPoint { get; set; } // 行动力基准值
    public int ElementalMastery { get; set; } // 元素精通
    public float ElementalDamageBonus { get; set; } // 元素伤害加成
    public float EnergyRecharge { get; set; } // 元素充能效率
    public float CriticalRate { get; set; } // 暴击率
    public float CriticalDamage { get; set; } // 暴击伤害
    public float HealingBonus { get; set; } // 治疗加成
    public float ElementalEnergy { get; set; } // 元素能量
    public float MaxElementalEnergy { get; set; } // 元素能量上限
    public string ElementalSkill { get; set; } // 元素战技
    public string ElementalBurst { get; set; } // 元素爆发

    public bool IsBasicActionEnd = false;

    public Sprite BasicAttackIcon;
    public Sprite SpecialSkillIcon;
    public Sprite BrustSkillIcon;


    public abstract ActionData GetBasicAttackSkillData();
    public abstract ActionData GetSpecialSkillData();
    public abstract ActionData GetBrustSkillData();
    public abstract List<ActionData> GetEnemySkillActionData();

    public abstract void WaitForSelectSkill();
    public abstract void WaitForBrustSkill();

    public abstract Task BasicAttackAction();
    public abstract Task SpecialSkillAction();
    public abstract Task BrustSkillAction();

    public abstract void OnCharaLightHit();
    public abstract void OnCharaHeavyHurt();
    public virtual void OnEnemyHit() { }
    public virtual void OnCharaDead() { }
    public virtual void OnEnemyDead() { }
    public virtual void OnCharaRevived() { }
    public virtual void OnEnemyRevived() { }
    ///////////////////////////计算公式//////////////////////////////
    /// <summary>
    /// 输入伤害倍率和目标，根据当前玩家的攻击力、暴击、爆伤、对方防御力和双方的状态、buff等决定伤害
    /// </summary>
    /// <param name="DamageMultipler"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    float CalculateHitPoints(int DamageMultipler, Character target)
    {
        bool isCritical = Random.Range(0f, 100f) > CriticalRate;
        float point =
            Attack
            * ((100 + CriticalDamage) * 0.01f)
            * ((100 - target.Defense) * 0.01f);
        return point;
    }
    private void OnMouseDown()
    {
        Debug.Log("点击了" + name);
        SelectManager.CharaClick(this);
    }
}