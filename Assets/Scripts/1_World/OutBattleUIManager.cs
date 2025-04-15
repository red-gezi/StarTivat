using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OutBattleUIManager : MonoBehaviour
{
    public static OutBattleUIManager Instance;

    public GameObject BlessingSelectionCanve;
    public GameObject BlessingAcquisitionCanve;
    public GameObject CurioSelectionCanve;
    public GameObject CurioAcquisitionCanve;
    public List<Sprite> Icons;
    public List<Sprite> curioIcons;
    bool isSelectionOver;
    int SelectionIndex;
    private void Awake()
    {
        Instance = this;
        InitBlessingSelection();
        InitCurioSelection();
    }
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

    // ==================== 选择道具界面 ====================
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

    // ==================== 获得祝福界面 ====================
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

    // ==================== 获得道具界面 ====================
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
}
