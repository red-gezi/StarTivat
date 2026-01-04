using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class TestSystem : InstanceBehaviour<TestSystem>
{
    [Title("是否从AB包加载数据")]
    public bool loadConfigDataFromAB;
    [Title("事件")]
    [LabelText("自动触发事件")]
    public bool autoTriggerOccurrence;
    public string occurrenceTag;
    [Button("触发事件")]
    public async void TriggerOccurrence()
    {
        OccurrenceSystem.Init();
        var occurence = OccurrenceSystem.GetOccurrence(occurrenceTag);
        PlayerSystem.Instance.SetCameraLockState(true);
        OutOfBattleUISystem.Instance.OpenOccurrenceCanvas(occurence);
        await OccurrenceSystem.Run(occurence.Data);
        OutOfBattleUISystem.Instance.CloseOccurrenceCanvas();
        PlayerSystem.Instance.SetCameraLockState(false);
    }

    // Update is called once per frame
    private void Start()
    {
        if (autoTriggerOccurrence)
        {
            TriggerOccurrence();
        }
    }
}