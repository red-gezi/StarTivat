using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;

public class CameraTrackManager : MonoBehaviour
{
    //static Vector3 defaultPos = new(0.45f, 1.6f, -2.3f);
    //static Vector3 brustPos = new(0.55f, 0.55f, -2.7f);

    //static Vector3 defaultEular;
    //private void Update()
    //{
    //    int rank = SelectManager.CurrentSelectTargets.Any() ? SelectManager.CurrentSelectTargets.First().Rank : 0;
    //    Vector3 biasEular = new Vector3(0, rank, 0);
    //    //Camera.main.transform.position = defaultPos;
    //    Camera.main.transform.eulerAngles = Quaternion.Lerp(Camera.main.transform.rotation, Quaternion.Euler(defaultEular + biasEular), Time.deltaTime * 5).eulerAngles;
    //    //Camera.main.transform.LookAt(SelectManager.currentSelectTarget.FirstOrDefault()?.transform);
    //}
    public static Transform targetCameraPoint;
    private void Update()
    {
        Vector3 biasEular = Vector3.zero;
        //叠加选中不同敌方索引时的角度偏置
        int rank = SelectManager.CurrentSelectTargets.Any() ? SelectManager.CurrentSelectTargets.First().Rank : 0;
        biasEular += new Vector3(0, rank, 0);
        //叠加不同时间时的角度缓动偏置
        biasEular += Vector3.up * Mathf.Sin(Time.time*0.5f);
        //叠加不同时间时的角度缓动偏置


        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetCameraPoint.transform.position, Time.deltaTime * 5);
        Camera.main.transform.eulerAngles = Quaternion.Lerp(Camera.main.transform.rotation, Quaternion.Euler(targetCameraPoint.eulerAngles + biasEular), Time.deltaTime * 5).eulerAngles;
        //Camera.main.transform.LookAt(SelectManager.currentSelectTarget.FirstOrDefault()?.transform);
    }
    public static void Init(Vector3 pos)
    {
        //defaultPos = pos;
    }
    /// <summary>
    /// 将摄像机设置到人物角色默认点位
    /// </summary>
    /// <param name="pos"></param>
    //public static void SetDefalutCharaRank(int rank)
    //{
    //    BattleManager.RefreshCharaPos(rank);
    //    Camera.main.transform.position = defaultPos;
    //    defaultEular = new Vector3(11, 350, 2);
    //    Camera.main.transform.eulerAngles = defaultEular + new Vector3(0, 10, 0);
    //}
    //public static void SetBrustCharaRank(int rank)
    //{
    //    BattleManager.RefreshCharaPos(rank);
    //    Camera.main.transform.position = brustPos;
    //    defaultEular = new Vector3(-6, 350, 2);
    //    Camera.main.transform.eulerAngles = defaultEular + new Vector3(0, 10, 0);
    //}

    [Button("设置待机点位")]
    public static void SetIdlePose(Character character)
    {
        //重置人物位置
        BattleManager.RefreshCharaPos(character.Rank);
        targetCameraPoint = character.Idle_Pose;
    }
    [Button("设置普攻点位")]
    public static void SetAttackPose(Character character)
    {
        //重置人物位置
        BattleManager.RefreshCharaPos(character.Rank);
        targetCameraPoint = character.Attack_Pose;

    }
    [Button("设置战技点位")]
    public static void SetSkillPose(Character character)
    {
        //重置人物位置
        BattleManager.RefreshCharaPos(character.Rank);
        targetCameraPoint = character.Skill_Pose;
    }
    [Button("设置战技点位")]
    public static void SetBrustPose(Character character)
    {
        //重置人物位置
        BattleManager.RefreshCharaPos(character.Rank);
        targetCameraPoint = character.Brust_Pose;
    }
}