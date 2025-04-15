using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillNameManager : MonoBehaviour
{
    public GameObject skillNameUi;
    Transform skillNameBackground => skillNameUi.transform.GetChild(0);
    TextMeshProUGUI skillNameUiText => skillNameBackground.GetChild(0).GetComponent<TextMeshProUGUI>();
    public static SkillNameManager Instance;
    // Start is called before the first frame update
    private void Awake() => Instance = this;

    [Button("≤‚ ‘")]
    public static async void Show(string skillName, bool isEnemySkill)
    {
        Instance.skillNameUi.SetActive(true);
        Instance.skillNameUiText.text = skillName;
        Instance.skillNameBackground.GetComponent<Image>().material.SetColor("_Color", isEnemySkill ? new Color(0.7f, 0, 0) : new Color(0.3f, 0.6f, 1) * 0.7f);
        await CustomThread.TimerAsync(0.2f, progress =>
        {
            Instance.skillNameBackground.transform.localPosition = new Vector3(0, -25 * (1 - progress), 0);
            Instance.skillNameUi.GetComponent<CanvasGroup>().alpha = progress;
            Instance.skillNameBackground.GetComponent<Image>().material.SetFloat("_Alpha", progress);
        });
        await Task.Delay(1000);
        await CustomThread.TimerAsync(0.6f, progress =>
        {
            Instance.skillNameUi.GetComponent<CanvasGroup>().alpha = (1 - progress);
            Instance.skillNameBackground.GetComponent<Image>().material.SetFloat("_Alpha", (1 - progress));
        });
        Instance.skillNameUi.SetActive(false);
    }
}
