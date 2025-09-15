using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CameraTrack
{
    //轨迹名
    public string trackName;
    public TrackType trackType;
    public List<CameraTrackPoint> points;
    [Serializable]
    public class CameraTrackPoint
    {
        public Vector3 pos;
        public Quaternion quat;
        public int delay;
        public TrackType trackType;

        public CameraTrackPoint(Vector3 pos, Quaternion quaternion)
        {
            this.pos = pos;
            this.quat = quaternion;
        }
#if UNITY_EDITOR
        [Button("更新当前点位")]
        public void UpdatePoint()
        {
            //if (trackType== TrackType.)
            //{

            //}
            pos = SceneView.lastActiveSceneView.camera.transform.position;
            quat = SceneView.lastActiveSceneView.camera.transform.rotation;
        }
#endif

    }
#if UNITY_EDITOR

    [Button("新增当前点位")]
    public void AddPoint()
    {
        var pos = SceneView.lastActiveSceneView.camera.transform.position;
        var quat = SceneView.lastActiveSceneView.camera.transform.rotation;
        points.Add(new CameraTrackPoint(pos, quat));
    }
    [Button("播放摄像机")]
    public void PlayCamera()
    {

    }
    [Button("保存点位文件")]
    public void SavePointFile()
    {

    }
    [Button("加载点位文件")]
    public void LoadPointFile()
    {

    }
#endif
}
