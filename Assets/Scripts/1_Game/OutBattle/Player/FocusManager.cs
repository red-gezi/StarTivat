using UnityEngine;
using UnityEngine.Events;

public class FocusManager : MonoBehaviour
{
    ////////////////////////////////////////////////玩家注释标识//////////////////////////////
    public GameObject focusIcon;
    //被注视的焦点权重
    public float focusWeight;
    
    public float distance;
    public float direDot;
    public UnityEvent hitEvent; 
    //刷新标识位置和状态
    private void Start()
    {
        PlayerManager.Instance.focusTargetList.Add(this);
        CloseFocusIcon();
    }

    private void OnDestroy() => PlayerManager.Instance.focusTargetList.Remove(this);
    private void Update() => RefreshFocusWeight();
    private void RefreshFocusWeight()
    {
        distance = Vector3.Distance(transform.position, PlayerManager.Instance.transform.position);
        direDot = Vector3.Dot((transform.position - PlayerManager.Instance.transform.position).normalized, Camera.main.transform.forward);
        if (distance > 10 || direDot < 0.5)
        {
            focusWeight = 0;
        }
        else
        {
            focusWeight = (10-distance) * direDot * direDot;
        }
    }
    public async void ShowFocusIcon()
    {
        focusIcon.SetActive(true);
        await CustomThread.TimerAsync(0.1f, progress =>
        {
            focusIcon.transform.GetChild(0).localScale = Vector3.one * Mathf.Lerp(2.5f, 1, progress);
        });
    }
    public void CloseFocusIcon()
    {
        focusIcon.SetActive(false);
    }
    public void OnHit()
    {
        hitEvent?.Invoke();
    }
}