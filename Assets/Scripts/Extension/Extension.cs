

using Newtonsoft.Json;

public static class Extension
{
    public static string ToJson(this object DataObject) => JsonConvert.SerializeObject(DataObject, Formatting.Indented);
    public static T ToObject<T>(this string target) => JsonConvert.DeserializeObject<T>(target);
    public static T To<T>(this object target) => target.ToJson().ToObject<T>();

}
