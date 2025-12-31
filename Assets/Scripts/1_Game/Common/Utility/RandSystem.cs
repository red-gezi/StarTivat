using System;
using System.Collections.Generic;
using System.Linq;


internal class RandSystem
{
    static Random Random = new Random();
    public static float GetValue()
    {
        return Random.Next();
    }
    public static List<T> GetRandomEnum<T>(Dictionary<T, float> weightedItems, int count)
    {
        // ... existing code ...
        if (weightedItems == null || weightedItems.Count == 0 || count <= 0)
            return new List<T>();
        // 计算总权重喵
        float totalWeight = weightedItems.Values.Sum();
        var results = new List<T>();
        var tempDict = new Dictionary<T, float>(weightedItems);

        for (int i = 0; i < count && tempDict.Count > 0; i++)
        {
            float randomPoint = UnityEngine.Random.Range(0f, totalWeight);
            foreach (var item in tempDict)
            {
                if (randomPoint < item.Value)
                {
                    results.Add(item.Key);
                    totalWeight -= item.Value;
                    tempDict.Remove(item.Key);
                    break;
                }
                randomPoint -= item.Value;
            }
        }
        return results;
    }
    public static List<T> GetRandomValues<T>(List<T> list, int count)
    {
        return list.OrderBy(rand => GetValue()).Take(count).ToList();
    }
    public static T GetRandomValue<T>(List<T> list)
    {
        return list.OrderBy(rand => GetValue()).FirstOrDefault();
    }
}
