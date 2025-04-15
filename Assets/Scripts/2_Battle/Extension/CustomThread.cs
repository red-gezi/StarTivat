using System;
using System.Threading.Tasks;

public class CustomThread
{
    /// <summary>
    /// 定时任务模块
    /// </summary>
    public static async Task TimerAsync(float stopTime, Action<float> runAction = null, Action stopAction = null)
    {
        int currentMs = 0;
        int stopMs = (int)(stopTime * 1000);
        while (currentMs <= stopMs)
        {
            //Debug.Log("当前" + (currentMs));
            //如果任务瞬间停止则进度直接返回100%，否则返回百分比
            runAction(stopTime == 0 ? 1 : currentMs * 1f / stopMs);
            currentMs += 50;
            await Task.Delay(50);
        }
        if (stopAction!=null)
        {
            stopAction();
        }
        //Debug.Log("结束打印"+( time - DateTime.Now));
    }
}