using UnityEngine;

public class ForwardToPlayer : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.forward = Vector3.Cross(Vector3.up, -Camera.main.transform.right);
    }
}
