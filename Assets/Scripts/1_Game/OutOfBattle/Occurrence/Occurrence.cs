using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
public class Occurrence
{
    public int ID;
    public List<OccurrenceTag> occurrenceTags = new();
    public List<Task> occurrenceTask = new();
    public float weight = 1;
    public bool isLock = false;
    public Sprite cardFace = null;
    public string text = "";
    public Occurrence()
    {
    }
    public Occurrence RegisterName<T>(T occurrenceName) where T : Enum
    {
        ID = Convert.ToInt32(occurrenceName);
        return this;
    }
    public Occurrence RegisterTag(params OccurrenceTag[] tags)
    {
        occurrenceTags = tags.ToList();
        return this;
    }
    //注册生效条件
    public Occurrence RegisterFilter(Func<Occurrence, bool> a)
    {
        RegisterFilter(x => x.isLock);
        //occurrenceTask = a;
        return this;
    }
    ////public OccurrenceData RegisterTypes(params OccurrenceType[] types)
    ////{
    ////    occurrenceTags = types.ToList();
    ////    return this;
    ////}
    public Occurrence RegisterWeight(float weight)
    {
        this.weight = weight;
        return this;
    }
    //添加选项后的行为
    public Occurrence RegisterOption(string flag, Task task)
    {
        occurrenceTask.Add(task);
        return this;
    }
    //从数据表格获得数据
    public Occurrence RegisterData(string tag)
    {
        OccurrenceSystem.GetData(tag);

        //occurrenceTask.Add(task);
        return this;
    }
    public Occurrence RegisterAction(string tag, Func<Task> task)
    {
        //occurrenceTask.Add(task);
        return this;
    }
}