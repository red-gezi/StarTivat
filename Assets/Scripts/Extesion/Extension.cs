using Newtonsoft.Json;
using System.IO;
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
    public static Texture2D FileToTexture(this string filePath)
    {
        if (File.Exists(filePath))
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(2, 2); // 创建一个初始大小的Texture2D
            texture.LoadImage(fileData); // 将图片数据加载到Texture2D中
            return texture;
        }
        else
        {
            Debug.LogError("文件不存在: " + filePath);
            return null;
        }
    }
    public static Sprite ToSprite(this Texture2D texture)
    {
        // 检查纹理是否为可读状态
        if (!texture.isReadable)
        {
            Debug.LogWarning("Texture2D is not readable. Make sure 'Read/Write Enabled' is checked in import settings.");
        }

        // 创建Sprite，使用默认参数
        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),  // 中心点作为锚点
            100.0f                   // 默认像素密度
        );

        return sprite;
    }
}
