using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OutOfBattleUISystem : InstanceBehaviour<OutOfBattleUISystem>
{

    //private void Start() => InitOutBattleUIManager();
    private void Update()
    {
        if (true)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchChara(1);
            if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchChara(2);
            if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchChara(3);
            if (Input.GetKeyDown(KeyCode.Alpha4)) SwitchChara(4);
        }
    }
    //初始化所有局外ui的状态
    public static void Init()
    {
        Instance.InitBlessingSelection();
        Instance.InitCurioSelection();
        Instance.InitCharaSelect();
        //关闭各种界面
        Instance.CloseOccurrenceCanvas();
    }
    // ==================== 整体ui界面 ====================
    public GameObject UI;
    internal static void ShowUI() => Instance.UI.SetActive(true);
    internal static void CloeUI() => Instance.UI.SetActive(false);
    public GameObject BlessingSelectionCanve;
    public GameObject BlessingAcquisitionCanve;
    public GameObject CurioSelectionCanve;
    public GameObject CurioAcquisitionCanve;
    public List<Sprite> Icons;
    public List<Sprite> curioIcons;
    bool isSelectionOver;
    int SelectionIndex;
    // ==================== 选择祝福界面 ====================
    #region 选择祝福
    public void InitBlessingSelection()
    {
        foreach (Transform item in BlessingSelectionCanve.transform)
        {
            if (item.name == "Content")
            {
                //根据buff数量构造对应的子物体
                for (int i = 0; i < 3; i++)
                {
                    var image = item.transform.GetChild(i).GetChild(0).GetComponent<Image>();
                    image.material = new Material(image.material);
                }
            }
        }
    }

    public async Task<Buff> OpenBlessingSelection(List<Buff> buffs)
    {
        BlessingSelectionCanve.SetActive(true);
        isSelectionOver = false;
        //BlessingSelectionCanve.transform.GetChild("");
        foreach (Transform item in BlessingSelectionCanve.transform)
        {
            if (item.name == "Content")
            {
                var layout = item.GetComponent<HorizontalLayoutGroup>();
                _ = CustomThread.TimerAsync(0.1f, progress =>
                {
                    var newPadding = new RectOffset(
                        layout.padding.left,
                        layout.padding.right,
                        (int)(-100 + (1 - progress) * 70),
                        layout.padding.bottom
                    );
                    // 应用修改
                    layout.padding = newPadding;
                });
                //根据buff数量构造对应的子物体
                for (int i = 0; i < 3; i++)
                {


                    var target = item.GetChild(i).gameObject;
                    if (i < buffs.Count)
                    {
                        target.SetActive(true);
                        //设置参数
                        var targetColor = buffs[i].rank switch
                        {
                            1 => new Color(1.5f, 1.5f, 1.5f),
                            2 => new Color(0, 0.65f, 2),
                            3 => new Color(2, 0.8f, 0),
                            _ => new Color(1, 1, 0),
                        };
                        target.transform.GetChild(0).GetComponent<Image>().material.SetColor("_Color", targetColor);
                        target.transform.GetChild(1).GetComponent<Image>().sprite = Icons.FirstOrDefault(icon => icon.name == buffs[i].element.ToString());
                        target.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = buffs[i].buffName;
                        target.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = buffs[i].buffAbility;
                    }
                    else
                    {
                        target.SetActive(false);
                    }
                }
            }
        }
        while (!isSelectionOver)
        {
            await Task.Delay(50);
        }
        return buffs[SelectionIndex];
    }

    public async void CloseBlessingSelection(Transform item)
    {
        var layout = item.parent.GetComponent<HorizontalLayoutGroup>();
        await CustomThread.TimerAsync(0.1f, progress =>
        {
            var newPadding = new RectOffset(
                layout.padding.left,
                layout.padding.right,
                (int)(-100 - progress * 30),
                layout.padding.bottom
            );
            // 应用修改
            layout.padding = newPadding;
        });
        BlessingSelectionCanve.SetActive(false);
        isSelectionOver = true;
        SelectionIndex = item.GetSiblingIndex();
    }
    #endregion
    // ==================== 选择道具界面 ====================
    #region 选择道具
    public void InitCurioSelection()
    {
        foreach (Transform item in CurioSelectionCanve.transform)
        {
            if (item.name == "Content")
            {
                //根据buff数量构造对应的子物体
                for (int i = 0; i < 3; i++)
                {
                    var image = item.transform.GetChild(i).GetChild(0).GetComponent<Image>();
                    image.material = new Material(image.material);
                }
            }
        }
    }
    public async Task<Buff> OpenCurioSelectionAsync(List<Buff> buffs)
    {
        CurioSelectionCanve.SetActive(true);
        isSelectionOver = false;
        foreach (Transform item in CurioSelectionCanve.transform)
        {
            if (item.name == "Content")
            {
                var layout = item.GetComponent<HorizontalLayoutGroup>();
                _ = CustomThread.TimerAsync(0.1f, progress =>
                {
                    var newPadding = new RectOffset(
                        layout.padding.left,
                        layout.padding.right,
                        (int)(-100 + (1 - progress) * 70),
                        layout.padding.bottom
                    );
                    // 应用修改
                    layout.padding = newPadding;
                });
                //根据buff数量构造对应的子物体
                for (int i = 0; i < 3; i++)
                {
                    var target = item.GetChild(i).gameObject;
                    if (i < buffs.Count)
                    {
                        target.SetActive(true);
                        //设置参数
                        var targetColor = buffs[i].rank switch
                        {
                            1 => new Color(2, 2, 2),
                            2 => new Color(0.2f, 1, 3),
                            3 => new Color(3, 1.5f, 0.2f),
                            _ => new Color(1, 1, 0),
                        };
                        target.transform.GetChild(0).GetComponent<Image>().material.SetColor("_Color", targetColor);
                        target.transform.GetChild(1).GetComponent<Image>().sprite = curioIcons.FirstOrDefault(icon => icon.name == buffs[i].curio.ToString());
                        target.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = buffs[i].buffName;
                        target.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = buffs[i].buffAbility;
                    }
                    else
                    {
                        target.SetActive(false);
                    }
                }
            }
        }
        while (!isSelectionOver)
        {
            await Task.Delay(50);
        }
        return buffs[SelectionIndex];
    }

    public async void CloseCurioSelection(Transform item)
    {
        var layout = item.parent.GetComponent<HorizontalLayoutGroup>();
        await CustomThread.TimerAsync(0.1f, progress =>
        {
            var newPadding = new RectOffset(
                layout.padding.left,
                layout.padding.right,
                (int)(-100 - progress * 30),
                layout.padding.bottom
            );
            // 应用修改
            layout.padding = newPadding;
        });
        CurioSelectionCanve.SetActive(false);
        isSelectionOver = true;
        SelectionIndex = item.GetSiblingIndex();


    }
    #endregion
    // ==================== 获得祝福界面 ====================
    #region 获得祝福
    public void OpenBlessingAcquisition(/* 可传递新祝福 Blessing newBlessing */)
    {
        // TODO: 显示获得祝福界面
        // 1. 展示祝福图标和描述
        // 2. 播放特效动画
        // 3. 启用继续按钮
    }

    public void CloseBlessingAcquisition()
    {
        // TODO: 关闭获得祝福界面
        // 1. 停止所有动画
        // 2. 重置UI状态
        // 3. 触发后续回调
    }
    #endregion
    // ==================== 获得道具界面 ====================
    #region 获得道具
    public void OpenCurioAcquisition(/* 可传递新道具 Item newItem */)
    {
        // TODO: 显示获得道具界面
        // 1. 展示道具3D模型
        // 2. 显示属性对比面板
        // 3. 初始化使用/丢弃按钮
    }

    public void CloseCurioAcquisition(bool confirmSelection)
    {
        // TODO: 根据选择关闭界面
        // 1. 处理确认/取消逻辑
        // 2. 更新背包数据
        // 3. 执行界面关闭过渡
    }
    #endregion
    // ==================== 选择人物界面 ====================
    #region 选择人物
    public enum CharaSelectCanvasMode
    {
        TeamCreat,
        TeamSwap,
        CharacterDownload,
        CharacterRevive,
        AttributeModification,
    }
    [Header("人物选择界面")]
    public GameObject ConfirmButton;
    public GameObject CloseButton;
    public GameObject CharaSelectCanvas;
    public Transform TeamPoolContent;
    public Transform TempTeamAppearanceContent;
    CharaSelectCanvasMode currentCharaSelectInitMode;
    List<TeamCharaData> targetCharaDatas;
    public List<Sprite> elements;
    //临时角色队列的文字
    public TextMeshProUGUI TempAvartListText;
    public void InitCharaSelect()
    {
        CloseCharaSelectCanvas();
    }
    public void OpenCharaSelectTeamCreatCanvas()
    {
        CharaSelectCanvas.SetActive(true);
        OpenCharaSelectCanves(CharaSelectCanvasMode.TeamCreat);
        //展开动画
    }
    public void OpenCharaSelectTeamSwapCanvas()
    {
        CharaSelectCanvas.SetActive(true);
        OpenCharaSelectCanves(CharaSelectCanvasMode.TeamSwap);
        //展开动画
    }
    public void OpenCharaSelectCharacterDownloadCanvas()
    {
        CharaSelectCanvas.SetActive(true);
        OpenCharaSelectCanves(CharaSelectCanvasMode.CharacterDownload);
        //展开动画
    }
    public void OpenCharaSelectCharacterReviveCanvas()
    {
        CharaSelectCanvas.SetActive(true);
        OpenCharaSelectCanves(CharaSelectCanvasMode.CharacterRevive);
        //展开动画
    }
    public void CloseCharaSelectCanvas()
    {
        CharaSelectCanvas.SetActive(false);
        //收起动画
    }
    //初始化人物选择列表，有多种模式
    //游戏进入模式，从TeamManager.AllCharaData获取全人物模板数据,选择至多4个组队
    //队伍换人模式，从GameDataSystem.GetGameData().TeamCharaPool;获得角色池人物数据
    //人物下载模式，从TeamManager.AllCharaData获取全人物模板数据,剔除已有人物，选择一个加入队伍池子
    //人物复活模式，从GameDataSystem.GetGameData().TeamCharaPool,剔除未死亡人物，选择一个复活
    //属性变更模式，从TeamManager.AllCharaData获取全人物模板数据，选择一个进行修改
    private void OpenCharaSelectCanves(CharaSelectCanvasMode charaPoolInitMode)
    {
        //初始化按钮
        ConfirmButton.GetComponent<Button>().onClick.RemoveAllListeners();
        CloseButton.GetComponent<Button>().onClick.RemoveAllListeners();
        CloseButton.GetComponent<Button>().onClick.AddListener(CloseCharaSelectCanvas);
        //开启所有界面
        //设置四个人员框
        foreach (Transform item in TempTeamAppearanceContent)
        {
            item.gameObject.SetActive(true);
        }
        switch (currentCharaSelectInitMode)
        {
            //队伍创建
            case CharaSelectCanvasMode.TeamCreat:
                TempAvartListText.text = "当前队伍";
                ConfirmButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    List<PlayerName> charaNameList = GameDataSystem.GetGameData().TempTeamAppearanceList
                                                                            .Where(chara => chara != null)
                                                                            .Select(chara => chara.CharaNameType)
                                                                            .ToList();
                    TeamSystem.SetTeamAppearanceList(charaNameList);
                    CloseCharaSelectCanvas();
                });
                break;
            //队员更换
            case CharaSelectCanvasMode.TeamSwap:
                TempAvartListText.text = "当前队伍";

                ConfirmButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    List<PlayerName> charaNameList = GameDataSystem.GetGameData().TempTeamAppearanceList
                                                                            .Where(chara => chara != null)
                                                                            .Select(chara => chara.CharaNameType)
                                                                            .ToList();
                    TeamSystem.SetTeamAppearanceList(charaNameList);
                    CloseCharaSelectCanvas();
                });
                break;
            //队员下载
            case CharaSelectCanvasMode.CharacterDownload:
                TempAvartListText.text = "下载角色";
                //关掉后三个人员框
                for (int i = 1; i < TempTeamAppearanceContent.childCount; i++)
                {
                    TempTeamAppearanceContent.GetChild(i).gameObject.SetActive(false);
                }
                ConfirmButton.GetComponent<Button>().onClick.AddListener(() =>
                {

                    if (GameDataSystem.GetGameData().DownloadChara != null)
                    {
                        GameDataSystem.GetGameData().TeamCharaPool.Add(GameDataSystem.GetGameData().DownloadChara);
                    }
                    CloseCharaSelectCanvas();
                });
                break;
            //队员复活(改成吃东西)
            case CharaSelectCanvasMode.CharacterRevive:
                break;
        }
        //初始化角色列表UI
        currentCharaSelectInitMode = charaPoolInitMode;
        targetCharaDatas = currentCharaSelectInitMode switch
        {
            CharaSelectCanvasMode.TeamCreat => TeamSystem.AllCharaData,
            CharaSelectCanvasMode.TeamSwap => GameDataSystem.GetGameData().TeamCharaPool,
            CharaSelectCanvasMode.CharacterDownload => TeamSystem.AllCharaData.Where(chara => !GameDataSystem.GetGameData().TeamCharaPool.Select(teamChara => teamChara.CharaNameType).ToList().Contains(chara.CharaNameType)).ToList(),
            CharaSelectCanvasMode.CharacterRevive => GameDataSystem.GetGameData().TeamCharaPool.Where(chara => chara.IsDead).ToList(),
            _ => throw new NotImplementedException(),
        };
        //初始化角色池UI
        var itemTemplate = TeamPoolContent.GetChild(0);
        for (int i = TeamPoolContent.childCount; i < targetCharaDatas.Count; i++)
        {
            Instantiate(itemTemplate, itemTemplate.parent);
        }
        foreach (Transform item in TeamPoolContent)
        {
            item.gameObject.SetActive(false);
        }
        //刷新指定数量的人物条目数据
        for (int i = 0; i < targetCharaDatas.Count; i++)
        {
            int rank = i;
            Transform item = TeamPoolContent.GetChild(i);
            item.gameObject.SetActive(true);
            item.GetComponent<Button>().onClick.RemoveAllListeners();
            item.Find("Mask").Find("Icon").GetComponent<Image>().sprite = AssetBundleSystem.Load<Sprite>("CharaIcon", targetCharaDatas[i].CharaNameType.ToString());
            item.Find("Name").GetComponent<Text>().text = targetCharaDatas[i].ShowCharaName["ch"];
            // 设置选中框
            int index = Array.FindIndex(GameDataSystem.GetGameData().TempTeamAppearanceList,
                chara => chara?.CharaNameType == targetCharaDatas[i].CharaNameType);
            bool isActive = index != -1;
            item.Find("Select").gameObject.SetActive(isActive);
            item.Find("Mask").Find("Index").gameObject.SetActive(isActive);
            item.Find("Mask").Find("Index").GetChild(0).GetComponent<Text>().text = (index + 1).ToString();
            //设置元素
            int elementIndex = ((int)targetCharaDatas[i].CharaNameType) % 1000 / 100;
            item.Find("Mask").Find("Element").GetComponent<Image>().sprite = elements[elementIndex];
            //设置背景颜色
            item.Find("Mask").Find("Bg_G").gameObject.SetActive(targetCharaDatas[i].IsGold);
            item.Find("Mask").Find("Bg_P").gameObject.SetActive(!targetCharaDatas[i].IsGold);
            switch (currentCharaSelectInitMode)
            {
                case CharaSelectCanvasMode.TeamCreat:

                    //设置点击事件
                    item.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        if (!GameDataSystem.GetGameData().TempTeamAppearanceList.Contains(targetCharaDatas[rank]))
                        {
                            AddCharaIntoTeamPool(targetCharaDatas[rank].CharaNameType);
                            AddCharaIntoTempTeamAppearanceList(targetCharaDatas[rank].CharaNameType);
                        }
                        else
                        {
                            RemoveCharaFromTeamPool(targetCharaDatas[rank].CharaNameType);
                            RemoveCharaFromTempTeamAppearanceList(targetCharaDatas[rank].CharaNameType);
                        }
                    });
                    break;
                case CharaSelectCanvasMode.TeamSwap:
                    //设置点击事件-将角色列表人物加入到出战列表中/从出战列表移除
                    item.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        if (!GameDataSystem.GetGameData().TempTeamAppearanceList.Contains(targetCharaDatas[rank]))
                        {
                            AddCharaIntoTempTeamAppearanceList(targetCharaDatas[rank].CharaNameType);
                        }
                        else
                        {
                            RemoveCharaFromTempTeamAppearanceList(targetCharaDatas[rank].CharaNameType);
                        }
                    });
                    break;
                //设置点击事件-将角色列表人物加入到下载目标中
                case CharaSelectCanvasMode.CharacterDownload:
                    item.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        if (GameDataSystem.GetGameData().DownloadChara == null)
                        {
                            SetDownloadChara(targetCharaDatas[rank].CharaNameType);
                        }
                        else
                        {
                            RemoveDownloadChara();
                        }
                    });
                    break;
                case CharaSelectCanvasMode.CharacterRevive:
                    break;
                default:
                    break;
            }
        }
        //刷新临时队伍ui
        RefreshTempTeamAppearanceList();
        //刷新属性配置ui
    }
    public void AddCharaIntoTeamPool(PlayerName charaName) => TeamSystem.AddCharaIntoTeamPool(charaName);
    public void AddCharaIntoTempTeamAppearanceList(PlayerName charaName) => TeamSystem.AddCharaIntoTempTeamAppearanceList(charaName);
    public void SetDownloadChara(PlayerName charaName) => TeamSystem.SetDownloadChara(charaName);

    [Button("刷新角色池列表ui")]
    public void RefreshCharaList()
    {
        //刷新指定数量的人物条目数据
        for (int i = 0; i < targetCharaDatas.Count; i++)
        {
            int rank = i;
            Transform item = TeamPoolContent.GetChild(i);
            // 设置选中框
            int index = Array.FindIndex(GameDataSystem.GetGameData().TempTeamAppearanceList,
                chara => chara?.CharaNameType == targetCharaDatas[i].CharaNameType);
            bool isActive = index != -1;
            item.Find("Select").gameObject.SetActive(isActive);
            item.Find("Mask").Find("Index").gameObject.SetActive(isActive);
            item.Find("Mask").Find("Index").GetChild(0).GetComponent<Text>().text = (index + 1).ToString();
            switch (currentCharaSelectInitMode)
            {
                case CharaSelectCanvasMode.TeamCreat:

                    break;
                case CharaSelectCanvasMode.TeamSwap:
                    break;
                case CharaSelectCanvasMode.CharacterDownload:
                    break;
                case CharaSelectCanvasMode.CharacterRevive:
                    break;
                default:
                    break;
            }
            //item6.GetComponent<Button>().onClick.AddListener(() => SelectCharaVoiceListOnConfig(rank));
        }
    }
    [Button("刷新临时出战列表ui")]

    public void RefreshTempTeamAppearanceList()
    {
        for (int i = 0; i < 4; i++)
        {
            var item = TempTeamAppearanceContent.GetChild(i);
            item.GetComponent<Button>().onClick.RemoveAllListeners();
            var icon = item.Find("Mask").Find("Icon");
            var charaData = GameDataSystem.GetGameData().TempTeamAppearanceList[i];
            icon.gameObject.SetActive(charaData != null);
            int rank = i;
            if (charaData != null)
            {
                icon.GetComponent<Image>().sprite = AssetBundleSystem.Load<Sprite>("CharaIcon", charaData.CharaNameType.ToString());
                item.GetComponent<Button>().onClick.AddListener(() =>
                {
                    var charaData = GameDataSystem.GetGameData().TempTeamAppearanceList[rank];
                    RemoveCharaFromTempTeamAppearanceList(charaData.CharaNameType);
                    RemoveCharaFromTeamPool(charaData.CharaNameType);

                });
            }
        }
    }

    private void RemoveCharaFromTempTeamAppearanceList(PlayerName charaName)
    {
        TeamSystem.RemoveCharaFromTempTeamAppearanceList(charaName);
    }
    public void RemoveAllFromTeamPool() => TeamSystem.RemoveAllFromTeamPool();
    // public void RemoveCharaFromTeamPool(CharaName charaName) => TeamManager.RemoveCharaFromTeamPool(charaName);

    [Button("队伍池移除人物")]
    public void RemoveCharaFromTeamPool(PlayerName charaName) => TeamSystem.RemoveCharaFromTeamPool(charaName);
    public void RemoveFromTeamAppearanceList(PlayerName charaName) => TeamSystem.RemoveCharaFromTeamAppearanceList(charaName);
    public void RemoveAllFromTeamAppearanceList() => TeamSystem.RemoveAllFromTeamAppearanceList();

    public void RemoveDownloadChara() => TeamSystem.RemoveDownloadChara();

    public void SetCurrentChara(CharaData chara)
    {
        Debug.Log("按钮点击" + chara.ToString());
        //如果游戏池子存在，则
        //CharaConfigManager.Instance.SelectModel(chara);
        CloseCharaSelectCanvas();
    }
    #endregion
    // ==================== 食物烹饪界面 ====================
    #region 食物烹饪
    #endregion
    // ==================== 队伍与出战人物界面 ==============
    #region 出战人物
    public Transform TeamAvatarContent;
    [Button("设置出战人物")]
    public void SetTeamAppearanceList(List<PlayerName> charaNameList) => TeamSystem.SetTeamAppearanceList(charaNameList);
    [Button("队伍切换人物")]
    public void SwitchChara(int index) => TeamSystem.SwitchChara(index);
    [Button("刷新出战人物ui")]
    public void RefreshTeamAppearanceList()
    {
        var datas = GameDataSystem.GetTeamAppearanceList();
        for (int i = 0; i < 4; i++)
        {
            Transform item = TeamAvatarContent.GetChild(i);
            item.gameObject.SetActive(i < datas.Count);
            if (i >= datas.Count)
            {
                continue;
            }
            //TeamAvatarContent.GetChild(i).Find("Hp").GetComponent<Text>().text = datas[i].ShowCharaName["ch"];
            item.Find("Name").GetComponent<Text>().text = datas[i].ShowCharaName["ch"];
            item.Find("Mask").GetChild(0).GetComponent<Image>().sprite = AssetBundleSystem.Load<Sprite>("CharaIcon", datas[i].CharaNameType.ToString());
            item.Find("Number").GetChild(0).GetComponent<Text>().text = i.ToString();
            ;
            item.Find("bg_w").gameObject.SetActive(i + 1 == GameDataSystem.GetTeamAppearanceIndex());
            item.Find("bg_b").gameObject.SetActive(i + 1 != GameDataSystem.GetTeamAppearanceIndex());
        }
    }
    #endregion
    // ==================== 道具获得提示界面 ================
    #region 道具获得
    [Header("道具获得提示")]
    public GameObject GetItemPrefab;

    public enum ItemType
    {
        Mora,
        Cecelia,
    }
    [Button("通知道具获得")]
    internal void NoticeItemGet(ItemType itemType, int count)
    {
        var newItem = Instantiate(GetItemPrefab, GetItemPrefab.transform.transform.parent);
        newItem.SetActive(true);
        Destroy(newItem, 3);
    }
    #endregion
    // ==================== 事件界面 ========================
    #region 事件
    [Header("事件")]
    //事件总界面
    public Transform occurrenceCanvas;
    //聊天
    public Transform occurrenceChatContent;
    //下一步
    public Transform stepCanvas;
    //选项
    public Transform optionContent;
    //奖励界面
    public Transform rewardContent;
    //string occurrenceTag;
    //界面
    [Button("开启事件页面")]
    public void OpenOccurrenceCanvas(Occurrence occurrence)
    {
        //occurrenceTag = tag;
        Show(occurrenceCanvas);
        //清空对话ui
        DestoryContentItem(occurrenceChatContent);
        //获得事件数据

        //设置事件UI
        Transform transform1 = occurrenceCanvas.Find("Title/OccurrenceName");
        transform1.GetComponent<Text>().text = occurrence.Data.ShowName;
        occurrenceCanvas.Find("Image/OccurrenceImage").GetComponent<Image>().sprite = occurrence.GetOccurrenceImage().ToSprite();
        occurrenceCanvas.Find("Image/OccurrenceSide").GetComponent<Image>().color = occurrence.Data.SideColor switch
        {
            "pink" => new Color(0.5f, 0.2f, 0.2f),
            "red" => new Color(1f, 0.2f, 0.2f),
            "blue" => new Color(0.2f, 0.2f, 1f),
            "green" => new Color(0.2f, 1f, 0.2f),
            "gold" => new Color(0.2f, 1f, 1f),
            _ => Color.white
        };
    }
    [Button("关闭事件页面")]
    public void CloseOccurrenceCanvas() => Hide(occurrenceCanvas);

    ///////////////////////////////////对话
    [Button("添加事件对话")]
    public async void AddOccurrenceChat(string speaker, string text)
    {
        var newItem = CreatContentItemByFirstItem(occurrenceChatContent);
        newItem.GetComponentsInChildren<Text>()[0].text = speaker;
        newItem.GetComponentsInChildren<Text>()[1].text = text;
        //滑条滚到最底
        var scrollRect = occurrenceChatContent.parent.parent.GetComponent<ScrollRect>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);
        float currentVerticalNormalizedPosition = scrollRect.verticalNormalizedPosition;
        await CustomThread.TimerAsync(0.2f, (progress) =>
        {
            scrollRect.verticalNormalizedPosition = Mathf.Lerp(currentVerticalNormalizedPosition, 0, progress * progress);
        });
    }
    //需要连同滚动条一起隐藏
    public void OpenOccurrenceChatContent() => Show(occurrenceChatContent.parent.parent);
    public void CloseOccurrenceChatContent() => Hide(occurrenceChatContent.parent.parent);
    //执行下个对话
    public void StepChat() => DialogueSystem.Step();
    public void OpenStepCanves() => Show(stepCanvas);
    public void CloseStepCanves() => Hide(stepCanvas);
    ///////////////////////////////////选项
    //设置选项
    public void AddOccurrenceOption(string text)
    {
        var item = CreatContentItemByFirstItem(optionContent);
        item.GetComponentInChildren<Text>().text = text;
    }
    //选择选项
    public void SelectOccurrenceOption(Transform option)
    {
        int index = option.GetSiblingIndex();
        DialogueSystem.Select(index);
    }
    [Button("开启选项组件")]
    public void OpenOccurrenceOptionContent()
    {
        Show(optionContent);
        DestoryContentItem(optionContent);
    }
    [Button("关闭选项组件")]
    public void CloseOccurrenceOptionContent() => Hide(optionContent);
    ///////////////////////////////////奖励
    //选择奖励
    public void AddReward(string text)
    {
        var item = CreatContentItemByFirstItem(rewardContent);
        item.GetComponentInChildren<Text>().text = text;
    }
    public void SelectReward(Transform option)
    {
        int index = option.GetSiblingIndex();
        DialogueSystem.Select(index);
    }
    [Button("开启奖励组件")]
    public void OpenRewardContent()
    {
        Show(rewardContent);
        DestoryContentItem(rewardContent);
    }
    [Button("关闭奖励组件")]
    public void CloseRewardContent() => Hide(rewardContent);
    ///////////////////////////////////房间通告
    public Transform roomNoticeContent;
    public async void OpenRoomNotice(RoomType roomType)
    {
        Show(roomNoticeContent);
        var canvasGroup = roomNoticeContent.GetComponent<CanvasGroup>();
        //更换icon
        RoomData roomData = GameDataSystem.GetLastRoomData();
        var icon = RoomSystem.GetRoomIcon(roomType);
        roomNoticeContent.Find("Notice/RoomIcon").GetComponent<Image>().material.SetTexture("_MainTex", icon);
        roomNoticeContent.GetComponentInChildren<Text>().text = $"{TranslateSystem.GetRoomTypeName(roomType)}:{roomData.CurrentLayer}/{roomData.MaxLayer}";
        await CustomThread.TimerAsync(0.2f, (progress) =>
        {
            canvasGroup.alpha = progress;
        });
        await Task.Delay(1000);
        await CustomThread.TimerAsync(0.2f, (progress) =>
        {
            canvasGroup.alpha = 1 - progress;
        });
    }
    public void CloseRoomNotice()
    {
        Hide(roomNoticeContent);
    }
    #endregion
    // ==================== 常用UI管理函数 ==================
    #region 常用UI管理函数
    public void Show(Transform ui) => ui.gameObject.SetActive(true);
    public void Hide(Transform ui) => ui.gameObject.SetActive(false);
    private GameObject CreatContentItemByFirstItem(Transform content)
    {
        var firstItemTemplate = content.GetChild(0).gameObject;
        GameObject newItem = Instantiate(firstItemTemplate, content);
        newItem.SetActive(true);
        return newItem;
    }
    private void DestoryContentItem(Transform content)
    {
        for (int i = content.childCount - 1; i > 0; i--)
        {
            DestroyImmediate(content.GetChild(i).gameObject);
        }
    }
    #endregion
}