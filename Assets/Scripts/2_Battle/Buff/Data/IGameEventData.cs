using System.Collections.Generic;
using UnityEngine;

public class GameEventData 
{
    //必选信息
    public Character Sender { get; set; }
    public Character Target { get; set; }
    //当前事件执行的buff
    public Buff exceBuff { get; set; }

    //可能会与buff产生连锁触发的buff列表
    public List<Buff> ListenerBuffs { get; set; }

    //作为目标的buff
    public List<Buff> TargetBuffs { get; set; }
    //运行日志
    List<string> Logs { get; set; } = new();
    public void ShowLog() => Debug.Log(Logs.ToJson());
    public void AddLog(string Text)
    {
        Logs.Add(Text);
        ShowLog();
    }
    //可选信息
}
