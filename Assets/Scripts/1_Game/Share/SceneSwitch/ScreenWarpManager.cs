using Sirenix.OdinInspector;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
public class ScreenWarpManager : MonoBehaviour
{
    public static ScreenWarpManager Instance { get; set; }
    public Material material;
    public void Awake() => Instance = this;
    // Update is called once per frame
    [Button("进入战斗")]
    public static async Task ShowScreen()
    {
        _ = CustomThread.TimerAsync(0.6f, (progress) =>
        {
            Instance.material.SetFloat("_progress1", progress * 0.7f);
        });
        //await Task.Delay(250);
        await CustomThread.TimerAsync(0.8f, (progress) =>
        {
            Instance.material.SetFloat("_progress2", Mathf.Pow(progress, 10) * 2f);
        });
        await Task.Delay(2000);
        await CustomThread.TimerAsync(0.5f, (progress) =>
        {
            Instance.material.SetFloat("_progress3", progress);
        });
    }

    public static async Task CloseScreen()
    {
        Instance.material.SetFloat("_progress1", 0);
        Instance.material.SetFloat("_progress2", 0);
        await CustomThread.TimerAsync(0.2f, (progress) =>
        {
            Instance.material.SetFloat("_progress3", 1 - progress);
        });
        ResetScreen();
    }

    [Button("重置屏幕")]
    async static void ResetScreen()
    {
        Instance.material.SetFloat("_progress1", 0);
        Instance.material.SetFloat("_progress2", 0);
        Instance.material.SetFloat("_progress3", 0);
    }
}
