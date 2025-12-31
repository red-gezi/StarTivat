using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class OccurrenceSystem
{
    //整个游戏的事件文本数据集合，包括未激活模式的
    private static List<OccurrenceData> AllOccurrenceData { get; set; } = new();
    //初始化,依次装载特定模式下事件的数据，可以在游戏过程中被修改
    private static Dictionary<Type, List<Occurrence>> AllOccurrences { get; set; } = new();
    //当前游戏模式下激活的事件对象集合，来自游戏存档
    private static List<Occurrence> CurrentModeOccurrences => GameDataSystem.CurrentGameData.CurrentOccurrenceList;
    public static void Init()
    {
        //加载所有事件数据
        if (GameFlowSystem.Instance.loadConfigDataFromAB)
        {
            AllOccurrenceData = AssetBundleSystem.Load<TextAsset>("GameData", "Occurrence.json").text.ToObject<List<OccurrenceData>>();
        }
        else
        {
            AllOccurrenceData = File.ReadAllText("E:\\UnityProject\\StarTivat\\Assets\\GameResources\\GameData\\Occurrence.json").ToObject<List<OccurrenceData>>();
        }
        //加载所有系列事件列表，有增加的话在这里补充
        SU_OccurrenceList.Init();
        AllOccurrences.Add(typeof(OccurrenceName), SU_OccurrenceList.Occurrences);
    }
    /// <summary>
    /// 激活指定模式的事件系列，写入游戏存档
    /// </summary>
    public static void Activate(params List<Occurrence>[] occurrences)
    {
        CurrentModeOccurrences.Clear();
        foreach (var occurrence in occurrences.SelectMany(x => x))
        {
            CurrentModeOccurrences.Add(occurrence.DeepClone());
        }
        GameDataSystem.Save();
    }
    public static OccurrenceData GetData(string tag)
    {
        OccurrenceData occurrenceData = AllOccurrenceData.FirstOrDefault(occurrence => occurrence.Tag == tag);
        if (occurrenceData == null)
        {
            Log.Show($"无法找到{tag}对应事件文本");
        }
        return occurrenceData;
    }
    public static async Task Run(OccurrenceData occurrenceData)
    {
        //解析剧本
        var node = DialogueSystem.Parse(occurrenceData.ShowDialogue);
        await DialogueSystem.RunAsync(node);
    }
    /// <summary>
    /// 通过tag获得特定事件
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public static Occurrence GetOccurrence(string tag)
    {
        //返回事件本身，方便全局处理
        Occurrence targetOccurrence = AllOccurrences.SelectMany(x => x.Value).FirstOrDefault(x => x.Data?.Tag == tag);
        if (targetOccurrence == null)
        {
            Log.Show("总事件列表不包含该事件tag,请确认是否有注册");
        }
        return targetOccurrence;
    }
    /// <summary>
    /// 通过事件枚举获得特定事件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="occurrenceName"></param>
    /// <returns></returns>
    public static Occurrence GetOccurrence<T>(T occurrenceName) where T : Enum
    {
        int ID = Convert.ToInt32(occurrenceName);
        if (!AllOccurrences.ContainsKey(typeof(T)))
        {
            Log.Show("总事件列表不包含该事件枚举类型,请在上方代码注册");
            return null;
        }
        var currentOccurrenceList = AllOccurrences[typeof(T)];
        //return currentOccurrenceList.FirstOrDefault(occurrence => occurrence.ID == ID).Clone();
        //返回事件本身，方便全局处理
        return currentOccurrenceList.FirstOrDefault(occurrence => occurrence.ID == ID);
    }
    public static List<Occurrence> GetRandomOccurrence(int count, params OccurrenceTag[] tags)
    {
        // 1. 从存档获得当前模式事件列表筛选出包含目标tag的项
        var filtered = CurrentModeOccurrences
            .Where(o => !o.IsLock)
            .Where(o => tags.Any(tag => o.OccurrenceTags.Contains(tag)))
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
        float totalWeight = filtered.Sum(o => o.Weight);

        // 4. 加权随机选择
        var result = new List<Occurrence>();
        for (int i = 0; i < count; i++)
        {
            // 生成0到总权重之间的随机数
            float r = UnityEngine.Random.value * totalWeight;

            // 找出随机数落在哪个项
            float sum = 0;
            for (int j = 0; j < filtered.Count; j++)
            {
                sum += filtered[j].Weight;
                if (r <= sum)
                {
                    // 将选中的项加入结果并从候选列表中移除
                    result.Add(filtered[j]);
                    totalWeight -= filtered[j].Weight;
                    filtered.RemoveAt(j);
                    break;
                }
            }
        }
        return result;
    }
    public static async void TurnOn(GameObject gameObject, string tag)
    {
        gameObject.SetActive(true);
        RefreshOccurrenceModel(gameObject, tag);
    }
    public static async void TurnOff(GameObject gameObject, bool immediate = false)
    {
        if (!immediate)
        {
            gameObject.SetActive(true);
            Transform particle = gameObject.transform.Find("Particle").transform;
            Material material = gameObject.GetComponent<Renderer>().material;
            //播放事件物体消失特效
            await CustomThread.TimerAsync(0.8f, progress =>
            {
                particle.localPosition = Vector3.Lerp(new(0, -0.5f, 0), new(0, 0.5f, 0), progress);
                material.SetFloat("_Progress", 1 - progress);
            });
            material.SetFloat("_Progress", 1);
            particle.localPosition = new(0, -0.5f, 0);
        }
        gameObject.SetActive(false);
    }
    public static void RefreshOccurrenceModel(GameObject gameObject, string tag)
    {
        var occurrence = GetOccurrence(tag);
        gameObject.GetComponentInChildren<TextMeshProUGUI>().text = occurrence.Data.ShowName;
        gameObject.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", occurrence.GetOccurrenceImage());
        var sideColor = occurrence.Data.SideColor switch
        {
            "pink" => new Color(0.5f, 0.2f, 0.2f),
            "red" => new Color(1f, 0.2f, 0.2f),
            "blue" => new Color(0.2f, 0.2f, 1f),
            "green" => new Color(0.2f, 1f, 0.2f),
            "gold" => new Color(0.2f, 1f, 1f),
            _ => Color.white
        };
        gameObject.GetComponent<MeshRenderer>().material.SetColor("_SideColor", sideColor);
        gameObject.GetComponent<InteractiveSystem>().Event.RemoveAllListeners();
        //添加事件交互
        gameObject.GetComponent<InteractiveSystem>().Event.AddListener(async () =>
        {
           
            var occurence = GetOccurrence(tag);
            PlayerSystem.Instance.SetCameraLockState(true);
            OutOfBattleUISystem.Instance.OpenOccurrenceCanvas(occurence);
            await Run(occurence.Data);
            OutOfBattleUISystem.Instance.CloseOccurrenceCanvas();
            PlayerSystem.Instance.SetCameraLockState(false);
            //通知房间该游戏事件已完成(需要广播?)
            RoomSystem.FinishOccurrence(gameObject, tag);
        });
    }
}
