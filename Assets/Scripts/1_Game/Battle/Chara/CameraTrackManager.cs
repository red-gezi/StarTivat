using System.Collections.Generic;
using System.IO;
using System.Linq;

class CameraTrackManager
{
    public static List<CameraTrack> CameraTrackDatas { get; set; }

    public static void Save()
    {
        File.WriteAllText("CameraTrack.json", CameraTrackDatas.ToJson());
    }
    public static void Load()
    {
        CameraTrackDatas = File.ReadAllText("CameraTrack.json").ToObject<List<CameraTrack>>();

    }
    public static void GetTrackData(string charaName)
    {
        CameraTrackDatas.FirstOrDefault(data => data.trackName == charaName);
    }
}
