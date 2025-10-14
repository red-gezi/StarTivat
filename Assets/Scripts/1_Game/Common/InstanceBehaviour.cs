using UnityEngine;

public class InstanceBehaviour<T> : MonoBehaviour where T : InstanceBehaviour<T>
{
    public static T Instance;
    private void Awake() => Instance = this as T;
}