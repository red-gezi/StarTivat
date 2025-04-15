using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class InteractionController : MonoBehaviour
{
    public Entry delegates;

    private void OnMouseDown() => delegates?.callback.Invoke(null);
}
