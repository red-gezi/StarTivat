using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
public class Buff
{
    public int ID { get; set; }
    public int timer;
    public int layers;
    public int rank;
    //祝福专享
    public ElementType element;
    //奇物专项
    public CurioType curio;
    public List<BuffTag> tags = new();
    public string buffName;
    public string buffAbility;
    //执行顺序权重，越大的越后
    public float weight;
    //奇物的配表数据
    BuffData Data { get; set; }
    public Dictionary<string, object> Flags { get; set; } = new();
    // 生命周期事件
    public Buff()
    {
    }
    public Buff Clone() => (Buff)MemberwiseClone();
    public Buff RegisterName<T>(T occurrenceName) where T : Enum
    {
        ID = Convert.ToInt32(occurrenceName);
        return this;
    }

    Dictionary<(BuffTriggerType, BuffEventType), Delegate> BufferEvents = new();
    public Func<T, Task> GetEvent<T>(BuffTriggerType triggerType, BuffEventType eventType)
    {
        var targetEvent = (BufferEvents.ContainsKey((triggerType, eventType)) ? BufferEvents[(triggerType, eventType)] : null);
        // 添加类型检查
        if (targetEvent != null)
        {
            if (targetEvent is Func<T, Task>)
            {
                return (Func<T, Task>)targetEvent;
            }
            else
            {
                Log.Show($"事件 {triggerType} - {eventType}.的数据包类型出错 ，当前为 {typeof(T)}, 期望为 {targetEvent?.GetType()}", 2);
            }
        }
        return null;
    }
    public Buff RegisterTag(params BuffTag[] tags)
    {
        this.tags = tags.ToList();
        return this;
    }
    public Buff RegisterBless(ElementType element, int rank, string buffName, string buffAbility)
    {
        this.element = element;
        this.rank = rank;
        this.buffName = buffName;
        this.buffAbility = buffAbility;
        return this;
    }
    public Buff RegisterCurio(CurioType curio, int rank, string buffName, string buffAbility)
    {
        this.curio = curio;
        this.rank = rank;
        switch (rank)
        {
            case 1: tags.Add(BuffTag.rank1); break;
            case 2: tags.Add(BuffTag.rank2); break;
            case 3: tags.Add(BuffTag.rank3); break;
            default: break;
        }
        this.buffName = buffName;
        this.buffAbility = buffAbility;
        return this;
    }
    public Buff RegisterEvent<T>(BuffTriggerType triggerType, BuffEventType eventType, Func<T, Task> handler) where T : EventData
    {
        BufferEvents[(triggerType, eventType)] = handler;
        return this;
    }

    public bool HasEvent(BuffTriggerType triggerType, BuffEventType eventType)
    {
        return BufferEvents.ContainsKey((triggerType, eventType));
    }
    public async Task TriggerAsync<T>(BuffTriggerType triggerType, BuffEventType eventType, T data) where T : EventData
    {
        var buffEvent = GetEvent<T>(triggerType, eventType);
        if (buffEvent == null)
        {
            Log.Show($"当前buff不存在{triggerType}—{eventType}事件", 1);
        }
        else
        {
            Log.Show($"当前buff成功触发{triggerType}—{eventType}事件");
            data.ThisBuff = this;
            await buffEvent?.Invoke(data);
        }
    }
    public T GetFlag<T>(string key, T defaultValue = default(T))
    {
        if (Flags.ContainsKey(key) && Flags[key] is T)
        {
            return (T)Flags[key];
        }
        return defaultValue;
    }
    public void SetFlag(string key, object value)
    {
        Flags[key] = value;
    }
}
