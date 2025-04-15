using UnityEngine;

public class LockControl : MonoBehaviour
{
    void Update()
    {
        transform.forward = -Camera.main.transform.forward;
    }
}
