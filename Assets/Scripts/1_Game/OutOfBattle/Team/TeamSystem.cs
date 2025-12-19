using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TeamSystem
{
    public static List<TeamCharaData> AllCharaData = new()
    {
        // 火元素
        new TeamCharaData(PlayerName.Amber, "安柏"),
        new TeamCharaData(PlayerName.Bennett, "班尼特"),
        new TeamCharaData(PlayerName.Xiangling, "香菱"),
        new TeamCharaData(PlayerName.Xinyan, "辛焱"),
        new TeamCharaData(PlayerName.Yanfei, "烟绯"),
        new TeamCharaData(PlayerName.Thoma, "托马"),
        new TeamCharaData(PlayerName.Chevreuse, "夏沃蕾"),
        new TeamCharaData(PlayerName.Lynette, "琳妮特"),
        new TeamCharaData(PlayerName.Diluc, "迪卢克"),
        new TeamCharaData(PlayerName.Klee, "可莉"),
        new TeamCharaData(PlayerName.HuTao, "胡桃"),
        new TeamCharaData(PlayerName.Yoimiya, "宵宫"),
        new TeamCharaData(PlayerName.Dehya, "迪希雅"),
        new TeamCharaData(PlayerName.Arlecchino, "阿蕾奇诺"),
        new TeamCharaData(PlayerName.Lyney, "林尼"),

        // 水元素
        new TeamCharaData(PlayerName.Barbara, "芭芭拉"),
        new TeamCharaData(PlayerName.Xingqiu, "行秋"),
        new TeamCharaData(PlayerName.Mona, "莫娜"),
        new TeamCharaData(PlayerName.Tartaglia, "达达利亚"),
        new TeamCharaData(PlayerName.Yelan, "夜兰"),
        new TeamCharaData(PlayerName.Kokomi, "珊瑚宫心海"),
        new TeamCharaData(PlayerName.Ayato, "神里绫人"),
        new TeamCharaData(PlayerName.Nilou, "妮露"),
        new TeamCharaData(PlayerName.Furina, "芙宁娜"),
        new TeamCharaData(PlayerName.Emilie, "艾梅莉埃"),
        new TeamCharaData(PlayerName.Sigewinne, "希格雯"),
        new TeamCharaData(PlayerName.Neuvillette, "那维莱特"),
        new TeamCharaData(PlayerName.Mualani, "玛拉妮"),

        // 风元素
        new TeamCharaData(PlayerName.Sucrose, "砂糖"),
        new TeamCharaData(PlayerName.Gaming, "嘉明"),
        new TeamCharaData(PlayerName.Sayu, "早柚"),
        new TeamCharaData(PlayerName.Heizou, "鹿野院平藏"),
        new TeamCharaData(PlayerName.Faruzan, "珐露珊"),
        new TeamCharaData(PlayerName.Jean, "琴"),
        new TeamCharaData(PlayerName.Venti, "温迪"),
        new TeamCharaData(PlayerName.Xiao, "魈"),
        new TeamCharaData(PlayerName.Xianyun, "闲云"),
        new TeamCharaData(PlayerName.Kazuha, "枫原万叶"),
        new TeamCharaData(PlayerName.Wanderer, "流浪者"),
        new TeamCharaData(PlayerName.Xilonen, "希诺宁"),
        new TeamCharaData(PlayerName.Chasca, "恰斯卡"),

        // 雷元素
        new TeamCharaData(PlayerName.Lisa, "丽莎"),
        new TeamCharaData(PlayerName.Razor, "雷泽"),
        new TeamCharaData(PlayerName.Fischl, "菲谢尔"),
        new TeamCharaData(PlayerName.Sara, "九条裟罗"),
        new TeamCharaData(PlayerName.KukiShinobu, "久岐忍"),
        new TeamCharaData(PlayerName.Kirara, "绮良良"),
        new TeamCharaData(PlayerName.Dori, "多莉"),
        new TeamCharaData(PlayerName.Sethos, "赛索斯"),
        new TeamCharaData(PlayerName.Ororon, "欧洛伦"),

        // 草元素
        new TeamCharaData(PlayerName.Yaoyao, "瑶瑶"),
        new TeamCharaData(PlayerName.Collei, "柯莱"),
        new TeamCharaData(PlayerName.Kaveh, "卡维"),
        new TeamCharaData(PlayerName.Tighnari, "提纳里"),
        new TeamCharaData(PlayerName.Cyno, "赛诺"),
        new TeamCharaData(PlayerName.Nahida, "纳西妲"),
        new TeamCharaData(PlayerName.Alhaitham, "艾尔海森"),

        // 冰元素
        new TeamCharaData(PlayerName.Kaeya, "凯亚"),
        new TeamCharaData(PlayerName.Diona, "迪奥娜"),
        new TeamCharaData(PlayerName.Rosaria, "罗莎莉亚"),
        new TeamCharaData(PlayerName.Chongyun, "重云"),
        new TeamCharaData(PlayerName.Layla, "莱依拉"),
        new TeamCharaData(PlayerName.Charlotte, "夏洛蒂"),
        new TeamCharaData(PlayerName.Freminet, "菲米尼"),
        new TeamCharaData(PlayerName.Mika, "米卡"),
        new TeamCharaData(PlayerName.Qiqi, "七七"),
        new TeamCharaData(PlayerName.Ganyu, "甘雨"),
        new TeamCharaData(PlayerName.Shenhe, "申鹤"),
        new TeamCharaData(PlayerName.Wriothesley, "莱欧斯利"),

        // 岩元素
        new TeamCharaData(PlayerName.Noelle, "诺艾尔"),
        new TeamCharaData(PlayerName.Ningguang, "凝光"),
        new TeamCharaData(PlayerName.YunJin, "云堇"),
        new TeamCharaData(PlayerName.Gorou, "五郎"),
        new TeamCharaData(PlayerName.Candace, "坎蒂丝"),
        new TeamCharaData(PlayerName.Kachina, "卡齐娜"),
        new TeamCharaData(PlayerName.Albedo, "阿贝多"),
        new TeamCharaData(PlayerName.Zhongli, "钟离"),
        new TeamCharaData(PlayerName.Itto, "荒泷一斗"),
        new TeamCharaData(PlayerName.Chiori, "千织"),
        new TeamCharaData(PlayerName.Navia, "娜维娅"),
        //new TeamCharaData (CharaName.Nahida,"纳西妲") ,
        //new TeamCharaData (CharaName.Lisa,"丽莎") ,
        //new TeamCharaData (CharaName.Amber,"安博") ,
        new TeamCharaData (PlayerName.Qiuqiu,"丘丘人") ,
    };
    private static TeamCharaData GetCharaData(PlayerName charaName)
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
        if (!GameDataSystem.GetGameData().TeamCharaPool.Any())
        {
            Debug.LogWarning("角色池子无角色，默认加入一个");
            GameDataSystem.GetGameData().TeamCharaPool.Add(GetCharaData(PlayerName.Nahida));
            GameDataSystem.GetGameData().TeamCharaPool.Add(GetCharaData(PlayerName.Amber));
            GameDataSystem.GetGameData().TeamCharaPool.Add(GetCharaData(PlayerName.Lisa));
        }
        if (!GameDataSystem.GetGameData().TeamAppearanceList.Any())
        {
            Debug.LogWarning("出战队列无角色，默认加入一个");
            GameDataSystem.GetTeamAppearanceList().Add(GetCharaData(PlayerName.Nahida));
            GameDataSystem.SetTeamAppearanceIndex(1);
        }
        //GameDataSystem.GetGameData().TeamAppearanceList.Add(GetCharaData(CharaName.Amber));
        //GameDataSystem.GetGameData().TeamAppearanceList.Add(GetCharaData(CharaName.Lisa));

        SwitchChara(GameDataSystem.GetGameData().TeamAppearanceIndex);

        //OutBattleUIManager.Instance.RefreshTeamAppearanceList();

    }
    public static void SwitchChara(int index)
    {
        var teamList = GameDataSystem.GetTeamAppearanceList();
        if (index > 0 && index <= teamList.Count)
        {
            PlayerManager.Instance.SwitchChara(teamList[index - 1].CharaNameType);
            GameDataSystem.SetTeamAppearanceIndex(index);
        }
        else
        {
            Debug.LogWarning("切换索引超出范围");
        }
        OutOfBattleUIManager.Instance.RefreshTeamAppearanceList();
    }
    #region 总角色列表
    #endregion
    #region 可选角色池列表
    public static void AddCharaIntoTeamPool(PlayerName charaName)
    {
        var item = GetCharaData(charaName);
        if (item == null)
        {
            Debug.LogWarning("获取角色模板数据失败，请检查" + charaName);
            return;
        }
        if (GameDataSystem.GetGameData().TeamCharaPool.Any(chara => chara.CharaNameType == charaName))
        {
            Debug.LogError("队伍池已存在该人物，不做处理");
            return;
        }
        GameDataSystem.GetGameData().TeamCharaPool.Add(item);
        OutOfBattleUIManager.Instance.RefreshTeamAppearanceList();
        Debug.Log(GameDataSystem.GetGameData().TeamCharaPool.ToJson());
    }
    //将指定角色移除队伍列表
    public static void RemoveCharaFromTeamPool(PlayerName charaName)
    {
        var item = GameDataSystem.GetTeamAppearanceList().FirstOrDefault(data => data.CharaNameType == charaName);
        if (item == null) return;
        GameDataSystem.GetGameData().TeamCharaPool.Remove(item);
        OutOfBattleUIManager.Instance.RefreshTeamAppearanceList();
    }

    internal static void RemoveAllFromTeamPool()
    {
        throw new NotImplementedException();
    }
    #endregion
    #region 临时出战列表
    public static void AddCharaIntoTempTeamAppearanceList(PlayerName charaName)
    {
        var targetChara = GameDataSystem.GetGameData().TeamCharaPool.FirstOrDefault(chara => chara.CharaNameType == charaName);
        if (targetChara == null)
        {
            Debug.LogError("队伍池不存在存在该人物，不做处理");
            return;
        }
        for (int i = 0; i < GameDataSystem.GetGameData().TempTeamAppearanceList.Length; i++)
        {
            if (GameDataSystem.GetGameData().TempTeamAppearanceList[i] == null)
            {
                Debug.Log($"加入人物{charaName}到位置{i}");
                GameDataSystem.GetGameData().TempTeamAppearanceList[i] = targetChara;
                OutOfBattleUIManager.Instance.RefreshTempTeamAppearanceList();
                OutOfBattleUIManager.Instance.RefreshCharaList();
                return;
            }
        }
        Debug.LogWarning("临时队伍列表已满，无法加入人物");
    }
    public static void RemoveCharaFromTempTeamAppearanceList(PlayerName charaName)
    {
        for (int i = 0; i < GameDataSystem.GetGameData().TempTeamAppearanceList.Length; i++)
        {
            if (GameDataSystem.GetGameData().TempTeamAppearanceList[i]?.CharaNameType == charaName)
            {
                Debug.Log($"移除人物{charaName}从位置{i}");
                GameDataSystem.GetGameData().TempTeamAppearanceList[i] = null;
                OutOfBattleUIManager.Instance.RefreshTempTeamAppearanceList();
                OutOfBattleUIManager.Instance.RefreshCharaList();
                return;
            }
        }
        Debug.LogWarning("检索不到移除目标");
    }
    #endregion
    #region 出战列表
    public static void AddCharaIntoTeamAppearanceList(PlayerName charaName)
    {

    }
    internal static void RemoveCharaFromTeamAppearanceList(PlayerName charaName)
    {
        throw new NotImplementedException();
    }
    internal static void RemoveAllFromTeamAppearanceList()
    {
        throw new NotImplementedException();
    }
    //设置指定角色出战
    public static void SetTeamAppearanceList(List<PlayerName> charaNameList)
    {
        GameDataSystem.GetGameData().TeamAppearanceIndex = 1;
        GameDataSystem.SetTeamAppearanceList(charaNameList
             .Select(charaName => GameDataSystem.GetGameData().TeamCharaPool.FirstOrDefault(chara => chara.CharaNameType == charaName))
             .ToList());
        OutOfBattleUIManager.Instance.RefreshTeamAppearanceList();
    }
    //设置下载角色
    public static void SetDownloadChara(PlayerName charaName)
    {
        GameDataSystem.GetGameData().DownloadChara = GameDataSystem.GetGameData().TeamCharaPool.FirstOrDefault(chara => chara.CharaNameType == charaName);
        OutOfBattleUIManager.Instance.RefreshTeamAppearanceList();
    }
    public static void RemoveDownloadChara()
    {
        GameDataSystem.GetGameData().DownloadChara = null;
        OutOfBattleUIManager.Instance.RefreshTeamAppearanceList();
    }
    #endregion







}
