using NUnit.Framework;
using System.IO;
using UnityEngine.Playables;
public class GameDataCore
{
    //删除非法存档(测试)
    //public static void Delete()
    //{
    //    File.Delete("save.json");
    //}
    //public static void Save(GameData gameData)
    //{
    //    File.WriteAllText("save.json", gameData.ToJson());
    //}
    //public static async void Load(GameData gameData)
    //{
    //    if (!File.Exists("save.json"))
    //    {
    //        gameData = new();
    //        gameData.CurrentOutBattleData = new();
    //        var s = GameDataSystem.CurrentGameData.CurrentOutBattleData;
    //        await RoomManager.RebackInitRoom();
    //        Save(gameData);
    //    }
    //    else
    //    {
    //        gameData = File.ReadAllText("save.json").ToObject<GameData>();
    //    }
    //}

}