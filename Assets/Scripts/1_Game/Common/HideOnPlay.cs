using UnityEngine;

public class HideOnPlay : MonoBehaviour
{
    void Awake()
    {
        //GetComponent<MeshRenderer>().enabled = false;
        gameObject.SetActive(false);
    }
}
