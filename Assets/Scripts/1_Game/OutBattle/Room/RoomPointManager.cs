using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class RoomPointManager : MonoBehaviour
{
    public Transform roompoints;
    //按标签获得一组点位
    [Button("获得点位")]
    public List<Transform> GetPoints(RoomPointsType roomPointsType)
    {
        return roompoints
                .Cast<Transform>()
                .FirstOrDefault(t =>
                    t.TryGetComponent<RoomPointTag>(out var tag) &&
                    tag.roomPointsType == roomPointsType)?
                .Cast<Transform>()
                .ToList() ?? new List<Transform>();
    }
}
//foreach (Transform roompoint in roompoints.transform)
//{
//    if (roompoint.GetComponent<RoomPointTag>().roomPointsType == roomPointsType)
//    {
//        List<Transform> results = new();
//        foreach (Transform point in roompoint)
//        {
//            results.Add(point);
//        }
//        return results;
//    }
//}
//return null;