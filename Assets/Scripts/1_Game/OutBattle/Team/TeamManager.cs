using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TeamManager
{
    public static List<TeamCharaData> AllCharaData = new()
    {
        // 火元素
        new TeamCharaData(CharaName.Amber, "安柏"),
        new TeamCharaData(CharaName.Bennett, "班尼特"),
        new TeamCharaData(CharaName.Xiangling, "香菱"),
        new TeamCharaData(CharaName.Xinyan, "辛焱"),
        new TeamCharaData(CharaName.Yanfei, "烟绯"),
        new TeamCharaData(CharaName.Thoma, "托马"),
        new TeamCharaData(CharaName.Chevreuse, "夏沃蕾"),
        new TeamCharaData(CharaName.Lynette, "琳妮特"),
        new TeamCharaData(CharaName.Diluc, "迪卢克"),
        new TeamCharaData(CharaName.Klee, "可莉"),
        new TeamCharaData(CharaName.HuTao, "胡桃"),
        new TeamCharaData(CharaName.Yoimiya, "宵宫"),
        new TeamCharaData(CharaName.Dehya, "迪希雅"),
        new TeamCharaData(CharaName.Arlecchino, "阿蕾奇诺"),
        new TeamCharaData(CharaName.Lyney, "林尼"),

        // 水元素
        new TeamCharaData(CharaName.Barbara, "芭芭拉"),
        new TeamCharaData(CharaName.Xingqiu, "行秋"),
        new TeamCharaData(CharaName.Mona, "莫娜"),
        new TeamCharaData(CharaName.Tartaglia, "达达利亚"),
        new TeamCharaData(CharaName.Yelan, "夜兰"),
        new TeamCharaData(CharaName.Kokomi, "珊瑚宫心海"),
        new TeamCharaData(CharaName.Ayato, "神里绫人"),
        new TeamCharaData(CharaName.Nilou, "妮露"),
        new TeamCharaData(CharaName.Furina, "芙宁娜"),
        new TeamCharaData(CharaName.Emilie, "艾梅莉埃"),
        new TeamCharaData(CharaName.Sigewinne, "希格雯"),
        new TeamCharaData(CharaName.Neuvillette, "那维莱特"),
        new TeamCharaData(CharaName.Mualani, "玛拉妮"),

        // 风元素
        new TeamCharaData(CharaName.Sucrose, "砂糖"),
        new TeamCharaData(CharaName.Gaming, "嘉明"),
        new TeamCharaData(CharaName.Sayu, "早柚"),
        new TeamCharaData(CharaName.Heizou, "鹿野院平藏"),
        new TeamCharaData(CharaName.Faruzan, "珐露珊"),
        new TeamCharaData(CharaName.Jean, "琴"),
        new TeamCharaData(CharaName.Venti, "温迪"),
        new TeamCharaData(CharaName.Xiao, "魈"),
        new TeamCharaData(CharaName.Xianyun, "闲云"),
        new TeamCharaData(CharaName.Kazuha, "枫原万叶"),
        new TeamCharaData(CharaName.Wanderer, "流浪者"),
        new TeamCharaData(CharaName.Xilonen, "希诺宁"),
        new TeamCharaData(CharaName.Chasca, "恰斯卡"),

        // 雷元素
        new TeamCharaData(CharaName.Lisa, "丽莎"),
        new TeamCharaData(CharaName.Razor, "雷泽"),
        new TeamCharaData(CharaName.Fischl, "菲谢尔"),
        new TeamCharaData(CharaName.Sara, "九条裟罗"),
        new TeamCharaData(CharaName.KukiShinobu, "久岐忍"),
        new TeamCharaData(CharaName.Kirara, "绮良良"),
        new TeamCharaData(CharaName.Dori, "多莉"),
        new TeamCharaData(CharaName.Sethos, "赛索斯"),
        new TeamCharaData(CharaName.Ororon, "欧洛伦"),

        // 草元素
        new TeamCharaData(CharaName.Yaoyao, "瑶瑶"),
        new TeamCharaData(CharaName.Collei, "柯莱"),
        new TeamCharaData(CharaName.Kaveh, "卡维"),
        new TeamCharaData(CharaName.Tighnari, "提纳里"),
        new TeamCharaData(CharaName.Cyno, "赛诺"),
        new TeamCharaData(CharaName.Nahida, "纳西妲"),
        new TeamCharaData(CharaName.Alhaitham, "艾尔海森"),

        // 冰元素
        new TeamCharaData(CharaName.Kaeya, "凯亚"),
        new TeamCharaData(CharaName.Diona, "迪奥娜"),
        new TeamCharaData(CharaName.Rosaria, "罗莎莉亚"),
        new TeamCharaData(CharaName.Chongyun, "重云"),
        new TeamCharaData(CharaName.Layla, "莱依拉"),
        new TeamCharaData(CharaName.Charlotte, "夏洛蒂"),
        new TeamCharaData(CharaName.Freminet, "菲米尼"),
        new TeamCharaData(CharaName.Mika, "米卡"),
        new TeamCharaData(CharaName.Qiqi, "七七"),
        new TeamCharaData(CharaName.Ganyu, "甘雨"),
        new TeamCharaData(CharaName.Shenhe, "申鹤"),
        new TeamCharaData(CharaName.Wriothesley, "莱欧斯利"),

        // 岩元素
        new TeamCharaData(CharaName.Noelle, "诺艾尔"),
        new TeamCharaData(CharaName.Ningguang, "凝光"),
        new TeamCharaData(CharaName.YunJin, "云堇"),
        new TeamCharaData(CharaName.Gorou, "五郎"),
        new TeamCharaData(CharaName.Candace, "坎蒂丝"),
        new TeamCharaData(CharaName.Kachina, "卡齐娜"),
        new TeamCharaData(CharaName.Albedo, "阿贝多"),
        new TeamCharaData(CharaName.Zhongli, "钟离"),
        new TeamCharaData(CharaName.Itto, "荒泷一斗"),
        new TeamCharaData(CharaName.Chiori, "千织"),
        new TeamCharaData(CharaName.Navia, "娜维娅"),
        //new TeamCharaData (CharaName.Nahida,"纳西妲") ,
        //new TeamCharaData (CharaName.Lisa,"丽莎") ,
        //new TeamCharaData (CharaName.Amber,"安博") ,
        new TeamCharaData (CharaName.Qiuqiu,"丘丘人") ,
    };
    private static TeamCharaData GetCharaData(CharaName charaName)
    {
        return AllCharaData.FirstOrDefault(data => data.CharaNameType == charaName);
    }
    public static void InitCharaList()
    {
        //角色总表排序
        AllCharaData = AllCharaData
            .OrderBy(chara => chara.CharaNameType)
            .OrderByDescending(chara => ((int)chara.CharaNameType) / 1000)
            .ToList();
        //如果角色池没角色，默认加入旅行者
        if (!GameManager.gameData.TeamCharaPool.Any())
        {
            Debug.LogWarning("角色池子无角色，默认加入一个");
            GameManager.gameData.TeamCharaPool.Add(GetCharaData(CharaName.Nahida));
            GameManager.gameData.TeamCharaPool.Add(GetCharaData(CharaName.Amber));
            GameManager.gameData.TeamCharaPool.Add(GetCharaData(CharaName.Lisa));
        }
        if (!GameManager.gameData.TeamAppearanceList.Any())
        {
            Debug.LogWarning("出战队列无角色，默认加入一个");
            GameManager.gameData.TeamAppearanceList.Add(GetCharaData(CharaName.Nahida));
            GameManager.gameData.TeamAppearanceIndex = 1;
        }
        //GameManager.gameData.TeamAppearanceList.Add(GetCharaData(CharaName.Amber));
        //GameManager.gameData.TeamAppearanceList.Add(GetCharaData(CharaName.Lisa));

        SwitchChara(GameManager.gameData.TeamAppearanceIndex);

        //OutBattleUIManager.Instance.RefreshTeamAppearanceList();

    }
    public static void SwitchChara(int index)
    {
        var teamList = GameManager.gameData.TeamAppearanceList;
        if (index > 0 && index <= teamList.Count)
        {
            PlayerManager.Instance.SwitchChara(teamList[index - 1].CharaNameType);
            GameManager.gameData.TeamAppearanceIndex = index;
        }
        else
        {
            Debug.LogWarning("切换索引超出范围");
        }
        OutBattleUIManager.Instance.RefreshTeamAppearanceList();
    }
    #region 总角色列表
    #endregion
    #region 可选角色池列表
    public static void AddCharaIntoTeamPool(CharaName charaName)
    {
        var item = GetCharaData(charaName);
        if (item == null)
        {
            Debug.LogWarning("获取角色模板数据失败，请检查" + charaName);
            return;
        }
        if (GameManager.gameData.TeamCharaPool.Any(chara => chara.CharaNameType == charaName))
        {
            Debug.LogError("队伍池已存在该人物，不做处理");
            return;
        }
        GameManager.gameData.TeamCharaPool.Add(item);
        OutBattleUIManager.Instance.RefreshTeamAppearanceList();
        Debug.Log(GameManager.gameData.TeamCharaPool.ToJson());
    }
    //将指定角色移除队伍列表
    public static void RemoveCharaFromTeamPool(CharaName charaName)
    {
        var item = GameManager.gameData.TeamAppearanceList.FirstOrDefault(data => data.CharaNameType == charaName);
        if (item == null) return;
        GameManager.gameData.TeamCharaPool.Remove(item);
        OutBattleUIManager.Instance.RefreshTeamAppearanceList();
    }

    internal static void RemoveAllFromTeamPool()
    {
        throw new NotImplementedException();
    }
    #endregion
    #region 临时出战列表
    public static void AddCharaIntoTempTeamAppearanceList(CharaName charaName)
    {
        var targetChara = GameManager.gameData.TeamCharaPool.FirstOrDefault(chara => chara.CharaNameType == charaName);
        if (targetChara == null)
        {
            Debug.LogError("队伍池不存在存在该人物，不做处理");
            return;
        }
        for (int i = 0; i < GameManager.gameData.TempTeamAppearanceList.Length; i++)
        {
            if (GameManager.gameData.TempTeamAppearanceList[i] == null)
            {
                Debug.Log($"加入人物{charaName}到位置{i}");
                GameManager.gameData.TempTeamAppearanceList[i] = targetChara;
                OutBattleUIManager.Instance.RefreshTempTeamAppearanceList();
                OutBattleUIManager.Instance.RefreshCharaList();
                return;
            }
        }
        Debug.LogWarning("临时队伍列表已满，无法加入人物");
    }
    public static void RemoveCharaFromTempTeamAppearanceList(CharaName charaName)
    {
        for (int i = 0; i < GameManager.gameData.TempTeamAppearanceList.Length; i++)
        {
            if (GameManager.gameData.TempTeamAppearanceList[i]?.CharaNameType == charaName)
            {
                Debug.Log($"移除人物{charaName}从位置{i}");
                GameManager.gameData.TempTeamAppearanceList[i] = null;
                OutBattleUIManager.Instance.RefreshTempTeamAppearanceList();
                OutBattleUIManager.Instance.RefreshCharaList();
                return;
            }
        }
        Debug.LogWarning("检索不到移除目标");
    }
    #endregion
    #region 出战列表
    public static void AddCharaIntoTeamAppearanceList(CharaName charaName)
    {

    }
    internal static void RemoveCharaFromTeamAppearanceList(CharaName charaName)
    {
        throw new NotImplementedException();
    }
    internal static void RemoveAllFromTeamAppearanceList()
    {
        throw new NotImplementedException();
    }
    //设置指定角色出战
    public static void SetTeamAppearanceList(List<CharaName> charaNameList)
    {
        GameManager.gameData.TeamAppearanceIndex = 1;
        GameManager.gameData.TeamAppearanceList = charaNameList
            .Select(charaName => GameManager.gameData.TeamCharaPool.FirstOrDefault(chara => chara.CharaNameType == charaName))
            .ToList();
        OutBattleUIManager.Instance.RefreshTeamAppearanceList();
    }
    //设置下载角色
    public static void SetDownloadChara(CharaName charaName)
    {
        GameManager.gameData.DownloadChara = GameManager.gameData.TeamCharaPool.FirstOrDefault(chara => chara.CharaNameType == charaName);
        OutBattleUIManager.Instance.RefreshTeamAppearanceList();
    }
    public static void RemoveDownloadChara()
    {
        GameManager.gameData.DownloadChara = null;
       OutBattleUIManager.Instance.RefreshTeamAppearanceList();
    }
    #endregion







}

public class TeamCharaData
{
    public CharaName CharaNameType { get; set; }
    public Dictionary<string, string> ShowCharaName { get; set; } = new() { };
    private float healthPercentage = 1f;
    //角色稀有度
    public bool IsGold { get; set; }
    public float HealthPercentage
    {
        get => healthPercentage;
        set => healthPercentage = Math.Clamp(value, 0.01f, 1f);
    }
    public bool IsDead { get; set; } = false;


    public TeamCharaData(CharaName charaNameType, string showCharaName)

    {
        CharaNameType = charaNameType;
        ShowCharaName["ch"] = showCharaName;
        IsGold = ((int)charaNameType)>=5000;
    }
}