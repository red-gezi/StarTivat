using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static MMD4MecanimData;

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
        public TrackPointType trackkPointType;
        public float duration;
        public AnimationCurve moveCurve;
        public AnimationCurve roationCurve;
        public CameraTrackPoint(Vector3 pos, Quaternion quaternion)
        {
            this.pos = pos;
            this.quat = quaternion;
            moveCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 1) });
            roationCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 1) });
        }
#if UNITY_EDITOR
        [Button("更新当前点位")]
        public void UpdatePoint()
        {
            switch (trackkPointType)
            {
                case TrackPointType.RelativeToTrigger:
                    pos = SceneView.lastActiveSceneView.camera.transform.position- ModelConfigManager.Instance.triggerModel.transform.position;
                    break;
                case TrackPointType.RelativeToTarget:
                    pos = SceneView.lastActiveSceneView.camera.transform.position - ModelConfigManager.Instance.targetModel.transform.position;
                    break;
                case TrackPointType.FixedPosition:
                    pos = SceneView.lastActiveSceneView.camera.transform.position;
                    break;
                default:
                    break;
            }
            quat = SceneView.lastActiveSceneView.camera.transform.rotation;
        }
#endif

    }
#if UNITY_EDITOR

    [Button("新增当前点位")]
    public void AddPoint()
    {
        var pos = SceneView.lastActiveSceneView.camera.transform.position;
        var rotation = SceneView.lastActiveSceneView.camera.transform.rotation;
        points.Add(new CameraTrackPoint(pos, rotation));
    }
    [Button("保存点位文件")]
    public void SavePointFile()
    {
        string path = $"{Directory.GetCurrentDirectory()}/Assets/GameResources/CameraTrack/{trackName}";
        string filePath = path + $"/{trackType.ToString()}.json";
        Directory.CreateDirectory(path);
        Log.Show("保存轨迹数据到" + path);
        File.WriteAllText(filePath, this.ToJson());
    }
    [Button("加载点位文件")]
    public void LoadPointFile()
    {
        string path = $"{Directory.GetCurrentDirectory()}/Assets/GameResources/CameraTrack/{trackName}";
        string filePath = $"{path}/{trackType.ToString()}.json";

        if (File.Exists(filePath))
        {
            Log.Show("加载轨迹数据从" + filePath);
            points = File.ReadAllText(filePath).ToObject<CameraTrack>().points;
        }
        else
        {
            Log.Show("点位文件不存在: " + filePath);
        }
    }
#endif
    internal async Task Run(Camera camera, GameObject trigger, GameObject target)
    {
        for (int i = 0; i < points.Count; i++)
        {
            var pointData = points[i];
            var startPoint = camera.transform.position;
            var startQuat = camera.transform.rotation;
            await CustomThread.TimerAsync(pointData.duration, (progress) =>
            {
                switch (pointData.trackkPointType)
                {
                    case TrackPointType.RelativeToTrigger:
                        camera.transform.position = Vector3.Lerp(startPoint, pointData.pos + trigger.transform.position, pointData.moveCurve.Evaluate(progress));
                        break;
                    case TrackPointType.RelativeToTarget:
                        camera.transform.position = Vector3.Lerp(startPoint, pointData.pos + target.transform.position, pointData.moveCurve.Evaluate(progress));
                        break;
                    case TrackPointType.FixedPosition:
                        camera.transform.position = Vector3.Lerp(startPoint, pointData.pos, pointData.moveCurve.Evaluate(progress));
                        break;
                    default:
                        break;
                }
                camera.transform.rotation = Quaternion.Lerp(startQuat, pointData.quat, pointData.roationCurve.Evaluate(progress));

            });

        }

    }
}
