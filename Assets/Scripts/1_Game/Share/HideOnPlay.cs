using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideOnPlay : MonoBehaviour
{
    void Start()
    {
        //GetComponent<MeshRenderer>().enabled = false;
        gameObject.SetActive(false);
    }
}
