using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static CameraTrack;

class CameraTrackManager : MonoBehaviour
{
    public List<CameraTrack> cameraTrackDatas;
#if UNITY_EDITOR
    [Button("设为镜头轨迹的触发对象")]
    public void SetTrigger()
    {
        ModelConfigSystem.Instance.triggerModel = gameObject;
        Log.Show("当前技能触发目标为{}，当前技能生效目标为{}");
    }

    [Button("设为镜头轨迹的目标对象")]
    public void SetTarget() => ModelConfigSystem.Instance.targetModel = gameObject;
    [Button("新增点位组")]
    public void AddPoint()
    {
        cameraTrackDatas.Add(new CameraTrack());
    }
    [Button("加载所有轨迹数据")]
    public void LoadAllTrack()
    {
        cameraTrackDatas = new();
        string currentTrackName = name;
        string path = $"{Directory.GetCurrentDirectory()}/Assets/GameResources/CameraTrack/{currentTrackName}";
        //不存在时加载默认模板
        if (!Directory.Exists(path))
        {
            path = $"{Directory.GetCurrentDirectory()}/Assets/GameResources/CameraTrack/{"Template"}";
        }
        new DirectoryInfo(path).GetFiles("*.json").ForEach(file =>
        {
            CameraTrack newTrack = new CameraTrack()
            {
                trackName = currentTrackName,
                trackType = Enum.Parse<TrackType>(Path.GetFileNameWithoutExtension(file.Name))
            };
            newTrack.LoadPointFile();
            cameraTrackDatas.Add(newTrack);
        });
    }
    [Button("播放指定轨迹")]
    public async void PlayTrack(TrackType trackType)
    {
        var trgetTrack = cameraTrackDatas.FirstOrDefault(track => track.trackType == trackType);
        await trgetTrack.Run(Camera.main, ModelConfigSystem.Instance.triggerModel, ModelConfigSystem.Instance.targetModel);
    }
    [Button("播放指定轨迹与技能")]
    public async void PlayTrackAndAnimation(TrackType trackType)
    {
        var trgetTrack = cameraTrackDatas.FirstOrDefault(track => track.trackType == trackType);
        _ = trgetTrack.Run(Camera.main, ModelConfigSystem.Instance.triggerModel, ModelConfigSystem.Instance.targetModel);
        switch (trackType)
        {
            case TrackType.ShowPose:
                break;
            case TrackType.AttackPose:
                GetComponent<Character>().WaitForSelectSkill();
                break;
            case TrackType.SkillPose:
                GetComponent<Character>().WaitForSelectSkill();
                break;
            case TrackType.BrustPose:
                GetComponent<Character>().WaitForBrustSkill();
                break;
            case TrackType.Attack:
                await GetComponent<Character>().AttackAction();
                break;
            case TrackType.Skill:
                await GetComponent<Character>().SkillAction();
                break;
            case TrackType.Brust:
                await GetComponent<Character>().BrustAction();
                break;
            case TrackType.Enemyskill_1:
                break;
            case TrackType.Enemyskill_2:
                break;
            case TrackType.Enemyskill_3:
                break;
            case TrackType.Enemyskill_4:
                break;
            case TrackType.Enemyskill_5:
                break;
            default:
                break;
        }
    }
    [Button("为摄像头添加轨迹")]
    public void AddTrack(TrackType trackType)
    {
        var trgetTrack = cameraTrackDatas.FirstOrDefault(track => track.trackType == trackType);
        if (trgetTrack == null)
        {
            Log.Show("检索不到对应轨迹", 2);
        }
        CameraSystem.AddCameraTrack(trgetTrack, ModelConfigSystem.Instance.triggerModel, ModelConfigSystem.Instance.targetModel);
    }

#endif

}
