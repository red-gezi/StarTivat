using UnityEngine;

public class WallSystem : MonoBehaviour
{
    Material material;
    void Start() => material = transform.GetComponent<Renderer>().material;

    //玩家靠近时设置接触坐标
    void Update() => material.SetVector("_pos", PlayerSystem.Instance.transform.position);
}
