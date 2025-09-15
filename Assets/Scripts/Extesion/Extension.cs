using Newtonsoft.Json;
using UnityEngine;

public static class Extension
{
    public static string ToJson(this object DataObject) => JsonConvert.SerializeObject(DataObject, Formatting.Indented);
    public static T ToObject<T>(this string target) => JsonConvert.DeserializeObject<T>(target);
    public static T To<T>(this object target) => target.ToJson().ToObject<T>();
    public static T Return<T>(this object _, T result) => result;
    public static (float x, float y, float z) ToTuple(this Vector3 vector)
    {
        return (vector.x, vector.y, vector.z);
    }

    public static Vector3 ToVector3(this (float x, float y, float z) tuple)
    {
        return new Vector3(tuple.x, tuple.y, tuple.z);
    }


}
