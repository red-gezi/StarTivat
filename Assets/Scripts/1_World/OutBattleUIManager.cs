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
                //����buff���������Ӧ��������
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
                    // Ӧ���޸�
                    layout.padding = newPadding;
                });
                //����buff���������Ӧ��������
                for (int i = 0; i < 3; i++)
                {


                    var target = item.GetChild(i).gameObject;
                    if (i < buffs.Count)
                    {
                        target.SetActive(true);
                        //���ò���
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
            // Ӧ���޸�
            layout.padding = newPadding;
        });
        BlessingSelectionCanve.SetActive(false);
        isSelectionOver = true;
        SelectionIndex = item.GetSiblingIndex();
    }

    // ==================== ѡ����߽��� ====================
    public void InitCurioSelection()
    {
        foreach (Transform item in CurioSelectionCanve.transform)
        {
            if (item.name == "Content")
            {
                //����buff���������Ӧ��������
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
                    // Ӧ���޸�
                    layout.padding = newPadding;
                });
                //����buff���������Ӧ��������
                for (int i = 0; i < 3; i++)
                {
                    var target = item.GetChild(i).gameObject;
                    if (i < buffs.Count)
                    {
                        target.SetActive(true);
                        //���ò���
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
            // Ӧ���޸�
            layout.padding = newPadding;
        });
        CurioSelectionCanve.SetActive(false);
        isSelectionOver = true;
        SelectionIndex = item.GetSiblingIndex();
    }

    // ==================== ���ף������ ====================
    public void OpenBlessingAcquisition(/* �ɴ�����ף�� Blessing newBlessing */)
    {
        // TODO: ��ʾ���ף������
        // 1. չʾף��ͼ�������
        // 2. ������Ч����
        // 3. ���ü�����ť
    }

    public void CloseBlessingAcquisition()
    {
        // TODO: �رջ��ף������
        // 1. ֹͣ���ж���
        // 2. ����UI״̬
        // 3. ���������ص�
    }

    // ==================== ��õ��߽��� ====================
    public void OpenCurioAcquisition(/* �ɴ����µ��� Item newItem */)
    {
        // TODO: ��ʾ��õ��߽���
        // 1. չʾ����3Dģ��
        // 2. ��ʾ���ԶԱ����
        // 3. ��ʼ��ʹ��/������ť
    }

    public void CloseCurioAcquisition(bool confirmSelection)
    {
        // TODO: ����ѡ��رս���
        // 1. ����ȷ��/ȡ���߼�
        // 2. ���±�������
        // 3. ִ�н���رչ���
    }
}
