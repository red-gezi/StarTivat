using UnityEngine;

public class InstanceBehaviour<T> : MonoBehaviour where T : InstanceBehaviour<T>
{
    public static T Instance;
    private void Awake()
    {
        Instance = this as T;
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