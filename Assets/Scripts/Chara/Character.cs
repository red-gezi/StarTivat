using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public abstract class Character : MonoBehaviour
{
    [HideInInspector]
    //角色类对应的模型
    public GameObject model;
    //////////////////////////////////////////////////角色图标////////////////////////////////////////////////////////////////////////////
    //锁定图标
    [Header("锁定图标")]
    public GameObject largeLock;
    public GameObject smallLock;
    //技能图标
    [Header("技能图标")]
    public Sprite basicAttackIcon;
    public Sprite specialSkillIcon;
    public Sprite brustSkillIcon;
    //在行动条与角色面板上的图标
    public Sprite charaIcon;
    
    //////////////////////////////////////////////////角色相关信息////////////////////////////////////////////////////////////////////////////
    public bool IsEnemy { get; set; }//是否是敌人
    //角色在自身排位置
    public int Rank => BattleManager.charaList.Where(chara => chara.IsEnemy == IsEnemy).ToList().IndexOf(this);
    //角色左侧的角色，可能为null
    public Character Left => BattleManager.charaList.FirstOrDefault(chara => chara.IsEnemy == IsEnemy && chara.Rank == Rank - 1);
    //角色右侧的角色，可能为null
    public Character Right => BattleManager.charaList.FirstOrDefault(chara => chara.IsEnemy == IsEnemy && chara.Rank == Rank + 1);
    //动画控制器
    public Animator animator => GetComponent<Animator>();
    //声音控制器
    public AudioSource audioSource => GetComponent<AudioSource>();
    //点击模型触发选中效果
    private void OnMouseDown() => SelectManager.CharaClick(this);
    //////////////////////////////////////////////////角色基础属性////////////////////////////////////////////////////////////////////////////
    public int MaxHealthPoints { get; set; } // 生命值上限
    public int CurrentHealthPoints { get; set; } // 当前生命值
    public int Attack { get; set; } // 基础攻击力
    public int Defense { get; set; } // 减伤比例
    public int MaxActionPoint { get; set; } //行动力基准值
    public float ElementalMastery { get; set; } // 元素反应加成
    public float ElementalDamageBonus { get; set; } // 元素伤害加成
    public float EnergyRecharge { get; set; } // 元素充能效率
    public float CriticalRate { get; set; } // 暴击率
    public float CriticalDamage { get; set; } // 暴击伤害
    public float HealingBonus { get; set; } // 治疗加成
    public float MaxElementalEnergy { get; set; } // 元素能量上限
    public float CurrentElementalEnergy { get; set; } // 当前元素能量
    public string ElementalSkillName { get; set; } // 元素战技
    public string ElementalBurstName { get; set; } // 元素爆发
    public ElementType PlayerElement { get; set; }
    //////////////////////////////////////////////////角色技能的相关配置信息////////////////////////////////////////////////////////////////////////////
    public abstract ActionData GetBasicAttackSkillData();
    public abstract ActionData GetSpecialSkillData();
    public abstract ActionData GetBrustSkillData();
    //////////////////////////////////////////////////角色技能的具体流程////////////////////////////////////////////////////////////////////////////
    public abstract Task BasicAttackAction();
    public abstract Task SpecialSkillAction();
    public abstract Task BrustSkillAction();
    public abstract Task EnemySkillAction();
    //////////////////////////////////////////////////等待技能选择////////////////////////////////////////////////////////////////////////////
    public virtual void WaitForSelectSkill()
    {
        if (IsEnemy)
        {
            EnemySkillAction();
        }
        else
        {
            SkillManager.ShowBasicAndSpecialSkill(GetBasicAttackSkillData(), GetSpecialSkillData());
        }
    }
    public abstract void WaitForBrustSkill();

    public virtual void PlayAnimation(AnimationType animationType)
    {
        animator.CrossFade(animationType.ToString(), 0.2f);
    }
    public virtual void PlayAudio(AnimationType animationType)
    {
        audioSource.clip = null;
        audioSource.Play();
    }
    //////////////////////////////////////////////////游戏事件相应////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// 当玩家受到攻击时
    /// </summary>
    public virtual void OnCharaHit(int point)
    {
        point = (int)(point * ((100 - Defense) * 0.01f));
        //判定护盾
        //跳盾量减少
        //判定血量
        //跳数字
        if (point > MaxActionPoint * 0.2f)
        {

        }
        else
        {

        }
    }
    /// <summary>
    /// 当敌人受到攻击时
    /// </summary>
    public virtual void OnEnemyHit() { }
    public virtual void OnCharaDead() { }
    public virtual void OnEnemyDead() { }
    public virtual void OnCharaRevived() { }
    public virtual void OnEnemyRevived() { }
    ///////////////////////////计算公式//////////////////////////////
    /// <summary>
    /// 输入伤害倍率和目标，根据当前玩家的攻击力、暴击、爆伤、buff、debuff等决定伤害
    /// </summary>
    /// <param name="DamageMultipler"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public float CalculateHitPoints(int DamageMultipler, Character target)
    {
        bool isCritical = Random.Range(0f, 100f) > CriticalRate;
        float point = Attack * ((100 + CriticalDamage) * 0.01f);
        return point;
    }
}