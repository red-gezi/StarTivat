using System.Linq;
using System.Threading.Tasks;
public class GameEventCore
{
    #region 对外开放事件接口
    public static async Task TriggerEventAsync<T>(BuffEventType eventType, T data) where T : EventData
    {
        if (data.ExceBuff != null)
        {
            await TriggerTargetEventAsync(eventType, data);
        }
        else if (data.ExceBuffs.Any())
        {
           await TriggerAllEventAsync(eventType, data);
        }
        else
        {
            Log.Show("未设置事件触发目标buff");
        }
    }
    #endregion
    #region 内部事件处理类型
    /// <summary>
    /// 指定Buff类型事件：触发框定范围内所有buff某事件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="eventType"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    private static async Task TriggerAllEventAsync<T>(BuffEventType eventType, T data) where T : EventData
    {
        foreach (var targetBuff in data.ExceBuffs)
        {
            //如果目标范围内buff含有触发事件
            if (targetBuff.HasEvent(BuffTriggerType.On, eventType))
            {
                data.ExceBuff = targetBuff;
                await TriggerTargetEventAsync(eventType, data);
            }
        }
    }
    /// <summary>
    /// 指定Buff类型事件：触发特定buff某事件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="eventType"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    private static async Task TriggerTargetEventAsync<T>(BuffEventType eventType, T data) where T : EventData
    {

        //Debug.Log($"触发{(MoNiYuZhouBuffList.BufferName)targetBuff.id}的{eventType}事件");
        foreach (var buff in data.ListenerBuffs)
        {
            await buff.TriggerAsync(BuffTriggerType.Before, eventType, data);
        }
        await data.ExceBuff.TriggerAsync(BuffTriggerType.On, eventType, data);
        // After触发
        foreach (var buff in data.ListenerBuffs)
        {
            await buff.TriggerAsync(BuffTriggerType.After, eventType, data);
        }
    }
    #endregion

}
