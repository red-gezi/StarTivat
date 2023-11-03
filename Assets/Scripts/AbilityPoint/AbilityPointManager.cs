using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityPointManager : MonoBehaviour
{
    public static AbilityPointManager Instance;
    public static int maxPoint = 5;
    public static int lastPoint = 0;
    public static int currentPoint = 0;
    public List<GameObject> pointIcon;
    public TextMeshProUGUI pointText;
    private void Awake()
    {
        Instance = this;
        Init();
    }
    public static void Init()
    {
        Instance.pointIcon.ForEach(icon =>
        {
            icon.GetComponent<Image>().material = new Material(icon.GetComponent<Image>().material);
            icon.transform.GetChild(0).gameObject.SetActive(false);
        });
        currentPoint = 3;
        lastPoint = 3;
        RefreshUI();
    }
    [Button("改变点数")]
    public static void ChangePoint(int point)
    {
        currentPoint += point;
        currentPoint = Math.Max(0, Math.Min(maxPoint, currentPoint));
        RefreshUI();
    }
    [Button("预测点数变动")]
    public static void PredictionChangePoint(int point)
    {
        if (point > 0)
        {
            //至少有一个点不满才能增加
            for (int i = 0; i < maxPoint; i++)
            {
                if (i >= currentPoint && i < currentPoint + point)
                {
                    var pointIconTransform = Instance.pointIcon[i].transform;
                    pointIconTransform.GetChild(0).gameObject.SetActive(true);
                    pointIconTransform.GetChild(0).GetComponent<Image>().material.SetFloat("_IsDecrease", 0);
                    RenderTexture.ReleaseTemporary(RenderTexture.active);
                }
                else
                {
                    var pointIconTransform = Instance.pointIcon[i].transform;
                    pointIconTransform.GetChild(0).gameObject.SetActive(false);
                }
            }
        }
        if (point < 0)
        {
            //至少有一个点才能减少
            for (int i = 1; i <= maxPoint; i++)
            {
                if ( i <= currentPoint && currentPoint + point < i)
                {
                    var pointIconTransform = Instance.pointIcon[i-1].transform;
                    pointIconTransform.GetChild(0).gameObject.SetActive(true);
                    pointIconTransform.GetChild(0).GetComponent<Image>().material.SetFloat("_IsDecrease", 1);
                    RenderTexture.ReleaseTemporary(RenderTexture.active);
                }
                else
                {
                    var pointIconTransform = Instance.pointIcon[i-1].transform;
                    pointIconTransform.GetChild(0).gameObject.SetActive(false);
                }
            }
        }
    }
    public static async void RefreshUI()
    {
        for (int i = 0; i < maxPoint; i++)
        {
            var iconImage = Instance.pointIcon[i].GetComponent<Image>();
            iconImage.material.SetFloat("_isFull", i < currentPoint ? 1 : 0);
        }

        if (lastPoint != currentPoint)
        {
            if (lastPoint > currentPoint)
            {
                for (int i = lastPoint; i > currentPoint; i--)
                {
                    var pointIconTransform = Instance.pointIcon[i - 1].transform;
                    pointIconTransform.GetChild(0).gameObject.SetActive(true);
                    pointIconTransform.GetChild(0).GetComponent<Image>().material.SetFloat("_IsDecrease", 1);
                    RenderTexture.ReleaseTemporary(RenderTexture.active);
                }
                Instance.pointText.text = $"<size=40><color=cyan>{currentPoint}</color></size>/";
                await Task.Delay(500);
                Instance.pointText.text = $"<size=40><color=white>{currentPoint}</color></size>/";
            }
            else
            {
                for (int i = lastPoint; i < currentPoint; i++)
                {
                    var pointIconTransform = Instance.pointIcon[i].transform;
                    pointIconTransform.GetChild(0).gameObject.SetActive(true);
                    pointIconTransform.GetChild(0).GetComponent<Image>().material.SetFloat("_IsDecrease", 0);
                    RenderTexture.ReleaseTemporary(RenderTexture.active);
                }
                Instance.pointText.text = $"<size=40><color=red>{currentPoint}</color></size>/";
                await Task.Delay(500);
                Instance.pointText.text = $"<size=40><color=white>{currentPoint}</color></size>/";
            }
            Instance.pointIcon.ForEach(icon =>
            {
                icon.transform.GetChild(0).gameObject.SetActive(false);
            });

            lastPoint = currentPoint;
        }

    }
}