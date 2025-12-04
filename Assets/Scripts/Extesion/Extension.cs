using Newtonsoft.Json;
using UnityEngine;

public static class Extension
{
    public static string ToJson(this object dataObject) => JsonConvert.SerializeObject(dataObject, Formatting.Indented, new JsonSerializerSettings
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        PreserveReferencesHandling = PreserveReferencesHandling.None
    });
    public static T ToObject<T>(this string target) => JsonConvert.DeserializeObject<T>(target);
    public static T Clone<T>(this T dataObject) => dataObject.ToJson().ToObject<T>();
    public static T To<T>(this object target) => target.ToJson().ToObject<T>();
    public static T Return<T>(this object _, T result) => result;
    public static (float x, float y, float z) ToTuple(this Vector3 vector) => (vector.x, vector.y, vector.z);
    public static Vector3 ToVector3(this (float x, float y, float z) tuple) => new Vector3(tuple.x, tuple.y, tuple.z);


}
