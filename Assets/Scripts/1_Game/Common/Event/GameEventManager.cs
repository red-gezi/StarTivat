using System.Threading.Tasks;
public class GameEventManager
{
    #region 对外开放事件接口



    #endregion
    #region 内部事件处理类型
    /// <summary>
    /// 指定Buff类型事件：触发框定范围内所有buff某事件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="eventType"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static async Task TriggerAllEventAsync<T>(BuffEventType eventType, T data) where T : EventData
    {
        foreach (var targetBuff in data.TargetBuffs)
        {
            //如果目标范围内buff含有触发事件
            if (targetBuff.HasEvent(BuffTriggerType.On, eventType))
            {
                data.exceBuff = targetBuff;
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
    public static async Task TriggerTargetEventAsync<T>(BuffEventType eventType, T data) where T : EventData
    {

        //Debug.Log($"触发{(MoNiYuZhouBuffList.BufferName)targetBuff.id}的{eventType}事件");
        foreach (var buff in data.ListenerBuffs)
        {
            await buff.TriggerAsync(BuffTriggerType.Before, eventType, data);
        }
        await data.exceBuff.TriggerAsync(BuffTriggerType.On, eventType, data);
        // After触发
        foreach (var buff in data.ListenerBuffs)
        {
            await buff.TriggerAsync(BuffTriggerType.After, eventType, data);
        }
    }
    #endregion

}
