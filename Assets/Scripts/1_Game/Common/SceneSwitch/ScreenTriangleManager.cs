using Sirenix.OdinInspector;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ScreenTriangleManager : MonoBehaviour
{
    public static ScreenTriangleManager Instance { get; set; }
    public Material material;
    public void Awake() => Instance = this;
    private void Start() => ResetScreen();
    // Update is called once per frame
    [Button("开启屏幕切换(进入模拟宇宙)")]
    public static async Task ShowScreen()
    {
        Instance.material.SetInt("_IsClose", 0);
        _ = CustomThread.TimerAsync(1f, (progress) =>
        {
            Instance.material.SetFloat("_Lerp", Mathf.Pow(progress, 3));
        });
        await Task.Delay(1000);
        await CustomThread.TimerAsync(0.8f, (progress) =>
        {
            Instance.material.SetFloat("_Progress", Mathf.Pow(progress, 3));
        });
        Instance.material.SetInt("_IsBlack", 1);
        Instance.material.SetInt("_IsClose", 1);
        await CustomThread.TimerAsync(0.8f, (progress) =>
        {
            Instance.material.SetFloat("_Progress", 1 - Mathf.Pow(progress, 2));
        });
    }
    [Button("关闭屏幕切换(进入模拟宇宙)")]
    public static async Task CloseScreen()
    {
        Instance.material.SetInt("_IsClose", 0);
        Instance.material.SetInt("_Lerp", 0);
        await CustomThread.TimerAsync(1.6f, (progress) =>
        {
            Instance.material.SetFloat("_Progress", Mathf.Pow(progress, 3));
        });
        Instance.material.SetInt("_IsBlack", 0);
        Instance.material.SetInt("_IsClose", 1);
        await CustomThread.TimerAsync(0.8f, (progress) =>
        {
            Instance.material.SetFloat("_Progress", 1 - Mathf.Pow(progress, 2));
        });
        ResetScreen();
    }
    [Button("重置屏幕")]
    static void ResetScreen()
    {
        Instance.material.SetFloat("_Progress", 0);
        Instance.material.SetInt("_IsClose", 0);
        Instance.material.SetInt("_Lerp", 0);
        Instance.material.SetInt("_IsBlack", 0);
    }
}
