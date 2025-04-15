using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        biasEular += Vector3.up * Mathf.Sin(Time.time * 0.5f);
        //叠加不同时间时的角度缓动偏置
        if (targetCameraPoint != null)
        {
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetCameraPoint.transform.position, Time.deltaTime * 5);
            Camera.main.transform.eulerAngles = Quaternion.Lerp(Camera.main.transform.rotation, Quaternion.Euler(targetCameraPoint.eulerAngles + biasEular), Time.deltaTime * 5).eulerAngles;
        }
    }
    public static async Task BattleStartAround(List<Character> enemyList)
    {
        //计算敌人中心点
        var childrenPositions = enemyList.Select(chara => chara.transform.position);

        float centerX = childrenPositions.Average(pos => pos.x);
        float centerY = childrenPositions.Average(pos => pos.y);
        float centerZ = childrenPositions.Average(pos => pos.z);
        var targetPoint= new Vector3(centerX, centerY, centerZ);
        await CustomThread.TimerAsync(1, progress =>
        {
            Camera.main.transform.position = targetPoint + new Vector3(Mathf.Cos((progress*0.4f+1.2f)*Mathf.PI)*5, 3, Mathf.Sin((progress * 0.4f + 1.2f) * Mathf.PI) *5);
            Camera.main.transform.LookAt(targetPoint);
        });
    }
    [Button("设置待机点位")]
    public static void SetIdlePose(Character character)
    {
        //重置人物位置
        BattleManager.CurrentBattle.RefreshCharaPos(character.Rank);
        targetCameraPoint = character.Idle_Pose;
    }
    [Button("设置普攻点位")]
    public static void SetAttackPose(Character character)
    {
        //重置人物位置
        BattleManager.CurrentBattle.RefreshCharaPos(character.Rank);
        targetCameraPoint = character.Attack_Pose;

    }
    [Button("设置战技点位")]
    public static void SetSkillPose(Character character)
    {
        //重置人物位置
        BattleManager.CurrentBattle.RefreshCharaPos(character.Rank);
        targetCameraPoint = character.Skill_Pose;
    }
    [Button("设置战技点位")]
    public static void SetBrustPose(Character character)
    {
        //重置人物位置
        BattleManager.CurrentBattle.RefreshCharaPos(character.Rank);
        targetCameraPoint = character.Brust_Pose;
    }
    [Button("设置战技点位")]
    public static void SetAttackPos(Character character)
    {
        //重置人物位置
        BattleManager.CurrentBattle.RefreshCharaPos(character.Rank);
        targetCameraPoint = character.Brust_Pose;
    }
}