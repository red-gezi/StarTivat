using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class BuffSystem
{
    private static List<BuffData> AllBuffData { get; set; } = new();

    public static void Init()
    {
        //加载所有事件数据
#if UNITY_EDITOR
        //从本地加载
        //AllBuffData = File.ReadAllText("E:\\UnityProject\\StarTivat\\Assets\\GameResources\\GameData\\Buff.json").ToObject<List<BuffData>>();
#else
         //从AB包加载
         AssetBundleManager.Load<TextAsset>("GameData", "Occurrence.json");
#endif
        SU_BuffList.Init();
        BuffCore.AddBuffList(typeof(SU_BuffName), SU_BuffList.Buffs);
    }
    public static BuffData GetData(string tag)
    {
        return AllBuffData.FirstOrDefault(buff => buff.Tag == tag);
    }
    public static Buff GetBuff<T>(T buffName) where T : Enum
    {
        return BuffCore.GetBuff(buffName);
    }
    //public static List<Occurrence> GetRandomOccurrence(int count, params OccurrenceTag[] tags)
    //{
    //    //游戏从存档获得当前事件列表与激活状态
    //    List<Occurrence> occurrences = GameDataSystem.GetGameData().CurrentOccurrenceList;
    //    // 1. 筛选出包含目标tag的项
    //    var filtered = occurrences
    //        .Where(o => !o.isLock)
    //        .Where(o => tags.Any(tag => o.occurrenceTags.Contains(tag)))
    //        .ToList();

    //    if (filtered.Count == 0 || count <= 0)
    //    {
    //        return new List<Occurrence>();
    //    }

    //    // 2. 如果请求数量大于可用数量，返回全部
    //    if (count >= filtered.Count)
    //    {
    //        return new List<Occurrence>(filtered);
    //    }

    //    // 3. 计算总权重
    //    float totalWeight = filtered.Sum(o => o.weight);

    //    // 4. 加权随机选择
    //    var result = new List<Occurrence>();
    //    for (int i = 0; i < count; i++)
    //    {
    //        // 生成0到总权重之间的随机数
    //        float r = UnityEngine.Random.value * totalWeight;

    //        // 找出随机数落在哪个项
    //        float sum = 0;
    //        for (int j = 0; j < filtered.Count; j++)
    //        {
    //            sum += filtered[j].weight;
    //            if (r <= sum)
    //            {
    //                // 将选中的项加入结果并从候选列表中移除
    //                result.Add(filtered[j]);
    //                totalWeight -= filtered[j].weight;
    //                filtered.RemoveAt(j);
    //                break;
    //            }
    //        }
    //    }
    //    return result;
    //}

}
