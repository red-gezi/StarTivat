using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
public enum OccurrenceName
{
    test1
}
public enum OccurrenceTag
{
    Encounter,   // 遭遇
    Reward,      // 奖励
    Occurrence,  // 事件
    Positive,    // 正面
    Negative,    // 负面
    Subtle,      // 微妙
    Combat,      // 战斗
    Money,       // 金钱
    Curiosity,   // 奇物
    Ingredient   // 食材
}
public enum OccurrenceType
{

}
public interface IBaseOccurrenceList
{
    //public Buff GetOccurrence(int bufferId) => null;
    public Buff GetOccurrence<T>(T occurrence) => null;
}
public class OccurrenceList : IBaseOccurrenceList
{
    public static List<Occurrence> occurrenceList = new()
    {
        new Occurrence(OccurrenceName.test1)
            .RegisterTag( OccurrenceTag.Occurrence, OccurrenceTag.Positive)
    };
}
public class OccurrenceManager
{


    public string GetName(Occurrence occurrence)
    {
        return "";
    }
    public string GetText(Occurrence occurrence)
    {
        return "";
    }
    public static List<Occurrence> GetRandomOccurrence(int count, params OccurrenceTag[] tags)
    {
        //游戏从存档获得当前事件列表与激活状态
        List<Occurrence> occurrences = GameManager.gameData.CurrentOccurrenceList;
        // 1. 筛选出包含目标tag的项
        var filtered = occurrences
            .Where(o => !o.isLock)
            .Where(o => tags.Any(tag => o.occurrenceTags.Contains(tag)))
            .ToList();

        if (filtered.Count == 0 || count <= 0)
        {
            return new List<Occurrence>();
        }

        // 2. 如果请求数量大于可用数量，返回全部
        if (count >= filtered.Count)
        {
            return new List<Occurrence>(filtered);
        }

        // 3. 计算总权重
        float totalWeight = filtered.Sum(o => o.weight);

        // 4. 加权随机选择
        var result = new List<Occurrence>();
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
public class Occurrence
{
    public int index;
    public List<OccurrenceTag> occurrenceTags = new();
    public List<Task> occurrenceTask = new();
    public float weight = 1;
    public bool isLock = false;
    public Occurrence(OccurrenceName occurrenceName)
    {
        this.index = (int)occurrenceName;
        //加载数据

    }

    public Occurrence RegisterTag(params OccurrenceTag[] tags)
    {
        occurrenceTags = tags.ToList();
        return this;
    }
    public Occurrence RegisterTypes(params OccurrenceTag[] tags)
    {
        occurrenceTags = tags.ToList();
        return this;
    }
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
}