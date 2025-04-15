using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class Buff
{
    public int id;
    public int timer;
    public int layers;
    public int rank;
    //祝福专享
    public ElementType element;
    //奇物专项
    public CurioType curio;
    public string buffName;
    public string buffAbility;
    //执行顺序权重，越大的越后
    public float weight;
    // 生命周期事件
    public Buff(int id) => this.id = id;
    /// <summary>
    /// 创造祝福类型buff
    /// </summary>
    /// <param name="id"></param>
    /// <param name="element"></param>
    /// <param name="rank"></param>
    /// <param name="buffName"></param>
    /// <param name="buffAbility"></param>
    public Buff(int id, ElementType element, int rank, string buffName, string buffAbility)
    {
        this.id = id;
        this.element = element;
        this.rank = rank;
        this.buffName = buffName;
        this.buffAbility = buffAbility;

    }
    public Buff(int id, CurioType curio, int rank, string buffName, string buffAbility)
    {
        this.id = id;
        this.curio = curio;
        this.rank = rank;
        this.buffName = buffName;
        this.buffAbility = buffAbility;
    }
    public Buff Clone() => (Buff)MemberwiseClone();

    Dictionary<(BuffTriggerType, BuffEventType), Delegate> BufferEvents = new();
    public Func<T, Task> GetEvent<T>(BuffTriggerType triggerType, BuffEventType eventType)
    {
        return (Func<T, Task>)(BufferEvents.ContainsKey((triggerType, eventType)) ? BufferEvents[(triggerType, eventType)] : null);
    }
    public Buff Register<T>(BuffTriggerType triggerType, BuffEventType eventType, Func<T, Task> handler) where T : GameEventData
    {
        BufferEvents[(triggerType, eventType)] = handler;
        return this;
    }
    public async Task TriggerAsync<T>(BuffTriggerType triggerType, BuffEventType eventType, T data) where T : GameEventData
    {
        var buffEvent = GetEvent<T>(triggerType, eventType);
        if (buffEvent == null)
        {
            //Debug.LogError($"当前buff不存在{triggerType}—{eventType}事件");
        }
        else
        {
            Debug.LogWarning($"当前buff成功触发{triggerType}—{eventType}事件");
            await buffEvent?.Invoke(data);
        }
    }
}
//public event Action OnApply;        // 当Buff被施加时
//public event Action OnExpire;       // 当Buff失效时
//public event Action OnStackChange;  // 堆叠数变化时
//                                    // 战斗事件

//public event Action<CharaData> OnPlayerBaseValueChange;
//public event Action<CharaData> OnEnemyBaseValueChange;

//public event Func<SkillData, Task> BeforeSkillCast;    // 施放技能前
//public event Func<SkillData, Task> OnSkillCast;        // 施放技能前
//public event Func<SkillData, Task> AfterSkillCast;     // 施放技能后

//public event Action<SkillData> BeforeTakeDamage;   // 受到伤害前
//public event Action<SkillData> OnTakeDamage;       // 受到伤害时
//public event Action<SkillData> AfterTakeDamage;    // 受到伤害后

//public event Action<SkillData> OnTurnStart;        // 回合开始时
//public event Action<SkillData> OnTurnEnd;          // 回合结束时
//public event Action<OutBattleEventData> OnGetItem;          // 获得道具
