using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestOcc : MonoBehaviour
{
    [Button("≤‚ ‘")]
    public void Test()
    {
        OccurrenceSystem.TurnOff(gameObject);
    }
}
