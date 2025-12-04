using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
public class OccurrenceData
{
    public int index;
    public List<OccurrenceTag> occurrenceTags = new();
    public List<Task> occurrenceTask = new();
    public float weight = 1;
    public bool isLock = false;
    public Sprite cardFace = null;
    public OccurrenceData()
    {
    }
    public OccurrenceData RegisterName<T>(T occurrenceName) where T : Enum
    {
        index = Convert.ToInt32(occurrenceName);
        return this;
    }
    public OccurrenceData RegisterTag(params OccurrenceTag[] tags)
    {
        occurrenceTags = tags.ToList();
        return this;
    }
    //注册生效条件
    public OccurrenceData RegisterFilter(Func<OccurrenceData,bool> a)
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
    public OccurrenceData RegisterWeight(float weight)
    {
        this.weight = weight;
        return this;
    }
    //添加选项后的行为
    public OccurrenceData RegisterOption(string flag, Task task)
    {
        occurrenceTask.Add(task);
        return this;
    }
    //添加选项后的行为
    public OccurrenceData RegisterStory(string flag)
    {
        //occurrenceTask.Add(task);
        return this;
    }
    public OccurrenceData RegisterAction(string tag,Func<Task> task)
    {
        //occurrenceTask.Add(task);
        return this;
    }
}