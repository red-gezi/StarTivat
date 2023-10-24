using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class Character : IChara
{
    public GameObject model;
    public int HealthPoints { get; set; } // 生命值
    public int Attack { get; set; } // 攻击力
    public int Defense { get; set; } // 防御力
    public int ElementalMastery { get; set; } // 元素精通
    public float ElementalDamageBonus { get; set; } // 元素伤害加成
    public float EnergyRecharge { get; set; } // 元素充能效率
    public float CriticalRate { get; set; } // 暴击率
    public float CriticalDamage { get; set; } // 暴击伤害
    public float HealingBonus { get; set; } // 治疗加成
    public float PhysicalDamageBonus { get; set; } // 属性伤害加成
    public float ElementalEnergy { get; set; } // 元素能量
    public string ElementalSkill { get; set; } // 元素战技
    public string ElementalBurst { get; set; } // 元素爆发
    public virtual AtcionData GetBasicAttackActionData() { return null; }
    public virtual AtcionData GetSpecialSkillActionData() { return null; }
    public virtual AtcionData GetBrustSkillActionData() { return null; }
    public virtual List<AtcionData> GetEnemySkillActionData() { return null; }


    public virtual void OnCharaLightHit() { }
    public virtual void OnCharaHeavyHurt() { }
    public virtual void OnEnemyHit() { }
    public virtual void OnCharaDead() { }
    public virtual void OnEnemyDead() { }
    public virtual void OnCharaRevived() { }
    public virtual void OnEnemyRevived() { }

}
class Ability
{

}
