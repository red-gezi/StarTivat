using System;
using UnityEngine;
enum CharaIconType
{

}
[Obsolete("·ÏÆú")]

public class CharaIconManager : MonoBehaviour
{
    private void Update()
    {
        transform.forward = -Camera.main.transform.forward;
    }
}
