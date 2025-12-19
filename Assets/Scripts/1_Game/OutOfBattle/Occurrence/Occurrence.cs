using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
public class Occurrence
{
    public int ID { get; set; }
    public List<OccurrenceTag> OccurrenceTags { get; set; } = new();
    public Dictionary<string, Func<Task>> OccurrenceTask { get; set; } = new();
    public float Weight { get; set; } = 1;
    public bool IsLock { get; set; } = false;
    public OccurrenceData Data { get; set; }
    public Dictionary<string, int> Flags;
    private Sprite image;
    public Occurrence()
    {
    }
    public Sprite GetOccurrenceImage()
    {
        if (Data == null)
        {
            return null;
        }
        return image ??= AssetBundleSystem.Load<Sprite>("OccurrenceImage", Data.ImageName);
    }
    public Occurrence RegisterName<T>(T occurrenceName) where T : Enum
    {
        ID = Convert.ToInt32(occurrenceName);
        return this;
    }
    public Occurrence RegisterTag(params OccurrenceTag[] tags)
    {
        OccurrenceTags = tags.ToList();
        return this;
    }
    //注册生效条件
    public Occurrence RegisterFilter(Func<Occurrence, bool> a)
    {
        RegisterFilter(x => x.IsLock);
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
        this.Weight = weight;
        return this;
    }
    //添加选项后的行为
    //public Occurrence RegisterOption(string flag, Task task)
    //{
    //    OccurrenceTask.Add(task);
    //    return this;
    //}
    //从数据表格获得数据
    public Occurrence RegisterData(string tag)
    {
        Data = OccurrenceSystem.GetData(tag);
        return this;
    }
    public Occurrence RegisterAction(string tag, Func<Task> task)
    {
        OccurrenceTask[tag] = task;
        return this;
    }
    public Occurrence SetLock()
    {
        IsLock = true;
        return this;
    }
    public Occurrence SetUnLock()
    {
        IsLock = false;
        return this;
    }
}