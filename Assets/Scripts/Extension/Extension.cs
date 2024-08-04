

using Newtonsoft.Json;

public static class Extension
{
    public static string ToJson(this object DataObject) => JsonConvert.SerializeObject(DataObject, Formatting.Indented);
    public static T ToObject<T>(this string target) => JsonConvert.DeserializeObject<T>(target);
    public static T To<T>(this object target) => target.ToJson().ToObject<T>();
    public static System.Numerics.Vector3 ToN(this UnityEngine.Vector3 v) => new System.Numerics.Vector3(v.x, v.y, v.z);
    public static UnityEngine.Vector3 ToU(this System.Numerics.Vector3 v) => new UnityEngine.Vector3(v.X, v.Y, v.Z);

}
