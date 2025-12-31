using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class CameraSystem : InstanceBehaviour<CameraSystem>
{
    //局外状态下摄像机的位置
    public Transform outBattleCameraPos;
    public Camera battleCamera;
    //局内状态下摄像机的轨迹
    public Queue<Func<Task>> CameraTracks { get; set; } = new();
    //摄像机结束运行时位于的点位
    private Vector3 cameraEndPoint;
    private Vector3 cameraEndEula;
    bool isBusy = false;
    [ShowInInspector]
    public CameraMode CurrentCameraMode { get; set; } = CameraMode.CameraTrack;

    public static void AddCameraTrack(CameraTrack cameraTrack, GameObject trigger, GameObject target)
    {
        Instance.CameraTracks.Enqueue(async () => await cameraTrack.Run(Instance.battleCamera,trigger, target));
    }
    private void Start()
    {
        battleCamera = Camera.main;
    }
    private async void Update()
    {
        switch (CurrentCameraMode)
        {
            case CameraMode.Free:
                battleCamera.transform.position = outBattleCameraPos.transform.position;
                battleCamera.transform.eulerAngles = outBattleCameraPos.transform.eulerAngles;
                break;
            case CameraMode.CameraTrack:
                if (isBusy)
                {
                    return;
                }
                if (CameraTracks.Any())
                {
                    isBusy = true;
                    await CameraTracks.Dequeue()();
                    isBusy = false;
                    cameraEndPoint = battleCamera.transform.position;
                    cameraEndEula = battleCamera.transform.eulerAngles;
                }
                else
                {
                    //摄像头微小晃动
                    Vector3 biasEular = Vector3.zero;
                    //叠加选中不同敌方索引时的角度偏置
                    int rank = SelectManager.CurrentSelectTargets.Any() ? SelectManager.CurrentSelectTargets.First().Rank : 0;
                    biasEular += new Vector3(0, rank, 0);
                    //叠加不同时间时的角度缓动偏置
                    biasEular += Vector3.up * Mathf.Sin(Time.time * 0.5f);
                    //叠加不同时间时的角度缓动偏置
                    battleCamera.transform.position = Vector3.Lerp(Camera.main.transform.position, cameraEndPoint, Time.deltaTime * 5);
                    battleCamera.transform.eulerAngles = Quaternion.Lerp(Camera.main.transform.rotation, Quaternion.Euler(cameraEndEula + biasEular), Time.deltaTime * 5).eulerAngles;
                }
                break;
            default:
                break;
        }
    }
}