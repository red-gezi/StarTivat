using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

class CameraTrackManager : MonoBehaviour
{
    //测试时的触发者和目标
    public GameObject TestTrigger;
    public GameObject TestTarget;

    public List<CameraTrack> cameraTrackDatas;
    [Button("加载所有轨迹数据")]
    public void LoadAllTrack()
    {
        cameraTrackDatas = new();
        string currentTrackName = name;
        string path = $"{Directory.GetCurrentDirectory()}/Assets/GameResource/CameraTrack/{currentTrackName}";
        //不存在时加载默认模板
        if (!Directory.Exists(path))
        {
            path = $"{Directory.GetCurrentDirectory()}/Assets/GameResource/CameraTrack/{"Template"}";
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
        trgetTrack.Run(TestTrigger, TestTarget);
    }
}
