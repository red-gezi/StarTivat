using UnityEngine;
using UnityEngine.Events;

public class InteractiveManager : MonoBehaviour
{
    public bool CanTrigger { get; set; } = true;
    public UnityEvent Event;
    public string InteractiveTag;
    public void Interactive()
    {
        Debug.Log("触发事件!" + gameObject.name);
        Event.Invoke();
    }
    private void OnDestroy()
    {
        CheckManager.Instance.RemoveInteractObject(this);
    }
    public void CloseTrigger()
    {
        CanTrigger = false;
        CheckManager.Instance.RemoveInteractObject(this);
    }
}