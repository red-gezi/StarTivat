using System;
using System.Collections.Generic;
using UnityEngine;

public class EventData
{
    //事件发送者
    public Character Sender { get; set; }
    //事件发送者
    public Character Receiver { get; set; }
    /// <summary>
    /// 总的事件执行目标的buff
    /// </summary>
    public List<Buff> ExceBuffs { get; set; }
    /// <summary>
    /// 当前事件执行的buff
    /// </summary>
    public Buff ExceBuff { get; set; }

    /// <summary>
    /// 可能会与buff产生连锁触发的buff列表
    /// </summary>
    public List<Buff> ListenerBuffs { get; set; }
    //当前buff所属的系列
    public List<Buff> BelongBuffList { get; set; }
    /// <summary>
    /// 当前执行中的buff本身
    /// </summary>
    public Buff ThisBuff { get; set; }


    //运行日志
    List<string> Logs { get; set; } = new();
    public void ShowLog() => Debug.Log(Logs.ToJson());
    public void AddLog(string Text)
    {
        Logs.Add(Text);
        ShowLog();
    }
   
}
