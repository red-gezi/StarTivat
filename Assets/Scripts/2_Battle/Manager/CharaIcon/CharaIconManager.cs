using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
enum CharaIconType
{

}
public class CharaIconManager : MonoBehaviour
{
    private void Update()
    {
        transform.forward = -Camera.main.transform.forward;
    }
}
