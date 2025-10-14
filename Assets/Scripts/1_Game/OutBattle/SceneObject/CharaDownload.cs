using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class CharaDownload : MonoBehaviour
{
    public GameObject fate;
    Vector3 startPos;
    private void Awake()
    {
        startPos = fate.transform.position;
    }
    private void Update()
    {
        fate.transform.Rotate(Vector3.up, 45 * Time.deltaTime,Space.Self);
        fate.transform.Rotate(Vector3.up, 60 * Time.deltaTime, Space.World);
        fate.transform.position = startPos + Vector3.up * Mathf.Sin(Time.time)*0.07f;
    }
}
