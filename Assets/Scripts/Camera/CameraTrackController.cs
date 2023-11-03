using Sirenix.OdinInspector;
using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static UnityEditor.PlayerSettings;

public class CameraTrackManager : MonoBehaviour
{
    static Vector3 defaultPos;
    static Vector3 defaultEular;
    private void Update()
    {
        Vector3 biasEular = new Vector3(0, SelectManager.currentSelectTargets.FirstOrDefault().Rank, 0);
        Camera.main.transform.position = defaultPos;
        Camera.main.transform.eulerAngles =Quaternion.Lerp(Camera.main.transform.rotation,Quaternion.Euler(defaultEular+ biasEular), Time.deltaTime*5).eulerAngles ;
        //Camera.main.transform.LookAt(SelectManager.currentSelectTarget.FirstOrDefault()?.transform);
    }
    public static void Init(Vector3 pos)
    {
        defaultPos = pos;
    }
    /// <summary>
    /// 将摄像机设置到人物默认点位
    /// </summary>
    /// <param name="pos"></param>
    [Button("设置点位")]
    public static void SetDefalutCharaRank(int rank)
    {
        defaultPos = rank switch
        {
            1 => new Vector3(-2.5f, 1.4f, -2f),
            2 => new Vector3(2f, 1.4f, -2f),
            3 => new Vector3(2.5f, 1.4f, -2f),
            4 => new Vector3(4.5f, 1.4f, 0f),
            _ => Vector3.zero,
        };
        defaultEular = rank switch
        {
            1 => new Vector3(10, 370, 5),
            2 => new Vector3(10, 340, 5),
            3 => new Vector3(10, 340, 5),
            4 => new Vector3(10, 310, 5),
            _ => Vector3.zero,
        };
        Camera.main.transform.eulerAngles = defaultEular + new Vector3(0, 10, 0);
    }
}
class CameraData
{

}