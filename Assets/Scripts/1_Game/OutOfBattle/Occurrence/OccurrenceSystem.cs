using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OccurrenceSystem
{


    public string GetName(OccurrenceData occurrence)
    {
        return "";
    }
    public string GetText(OccurrenceData occurrence)
    {
        return "";
    }
    public static List<OccurrenceData> GetRandomOccurrence(int count, params OccurrenceTag[] tags)
    {
        //游戏从存档获得当前事件列表与激活状态
        List<OccurrenceData> occurrences = GameManager.gameData.CurrentOccurrenceList;
        // 1. 筛选出包含目标tag的项
        var filtered = occurrences
            .Where(o => !o.isLock)
            .Where(o => tags.Any(tag => o.occurrenceTags.Contains(tag)))
            .ToList();

        if (filtered.Count == 0 || count <= 0)
        {
            return new List<OccurrenceData>();
        }

        // 2. 如果请求数量大于可用数量，返回全部
        if (count >= filtered.Count)
        {
            return new List<OccurrenceData>(filtered);
        }

        // 3. 计算总权重
        float totalWeight = filtered.Sum(o => o.weight);

        // 4. 加权随机选择
        var result = new List<OccurrenceData>();
        for (int i = 0; i < count; i++)
        {
            // 生成0到总权重之间的随机数
            float r = Random.value * totalWeight;

            // 找出随机数落在哪个项
            float sum = 0;
            for (int j = 0; j < filtered.Count; j++)
            {
                sum += filtered[j].weight;
                if (r <= sum)
                {
                    // 将选中的项加入结果并从候选列表中移除
                    result.Add(filtered[j]);
                    totalWeight -= filtered[j].weight;
                    filtered.RemoveAt(j);
                    break;
                }
            }
        }
        return result;
    }
}
