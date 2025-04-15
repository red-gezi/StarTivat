using System.Collections.Generic;
using UnityEngine;
public class CharaData : GameEventData
{
    public string CharaName { get; set; }
    public Character Sender { get; set; }
    public Character Target { get; set; }
    //public float TotalHP => BaseAttack + BaseAttack * AttackPercentageBonus + AttackFlatBonus;//总生命
    public float BaseHP { get; set; } // 基础生命值上限
    public float HPPercentageBonus { get; set; }//生命百分比加成
    public float HPFlatBonus { get; set; }//生命固定值加成
    public float MaxHP => BaseHP + BaseHP * HPPercentageBonus + HPFlatBonus;// 生命值上限
    public float CurrentHealthPoints { get; set; } // 当前生命值

    public float BaseAttack { get; set; } // 基础攻击力
    public float AttackPercentageBonus { get; set; }//攻击力百分比加成
    public float AttackFlatBonus { get; set; }//基础攻击力固定值加成
    public float TotalAttack => BaseAttack + BaseAttack * AttackPercentageBonus + AttackFlatBonus;//总攻击力

    public float BaseDefense { get; set; } // 基础防御减伤比例
    public int BaseDefenseBonus { get; set; }//防御减伤加成
    public int TotalDefenseBonus { get; set; }//经过增益减益后的实际防御减伤加成

    public float CurrentDefense { get; set; } // 当前减伤比例
    public int MaxActionPoint { get; set; } //行动力基准值
    public float ElementalMastery { get; set; } // 元素反应加成
    public float ElementalDamageBonus { get; set; } // 元素伤害加成
    public float EnergyRecharge { get; set; } // 元素充能效率
    public float CriticalRate { get; set; } = 50f; // 暴击率
    public float BaseCriticalDamage { get; set; } // 暴击伤害
    public float HealingBonus { get; set; } // 治疗加成
    public float MaxElementalEnergy { get; set; } // 元素能量上限
    public float CurrentElementalEnergy { get; set; } // 当前元素能量

    public ElementType PlayerElement { get; set; }
    public Color PlayerColor => PlayerElement switch
    {
        ElementType.Anemo => new Color(0, 1, 0.4f),
        ElementType.Pyro => new Color(0.9f, 0.25f, 0),
        ElementType.Hydro => new Color(0f, 0.35f, 0.9f),
        ElementType.Electro => new Color(0.3f, 0, 0.75f),
        ElementType.Cryo => throw new System.NotImplementedException(),
        ElementType.Geo => throw new System.NotImplementedException(),
        ElementType.Herb => new Color(0, 1f, 0),
        _ => new Color(1, 1, 1),
    };

    public List<Buff> TargetBuffs { get; set; }
    public List<string> Logs { get; set; }

    //应用buff中的数值加成
    public CharaData GetCurrentCharaData(List<Buff> buffs)
    {
        buffs.ForEach(async buff => await buff.TriggerAsync(BuffTriggerType.On, BuffEventType.GetCurrentCharaData, this));
        return this;
    }
}
