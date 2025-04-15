
using Server;
using MongoDB.Bson;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Channels;
using TouhouMachineLearningSummary.GameEnum;
using System.Data.Common;
using Newtonsoft.Json.Linq;

public class HotFixHub : Hub
{
    public override Task OnConnectedAsync()
    {
        Console.WriteLine("一个用户登录了" + Context.ConnectionId);
        return base.OnConnectedAsync();
    }
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine("一个用户登出了" + Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
    //////////////////////////////////////////////卡牌配置////////////////////////////////////////////////////////////////////

    //////////////////////////////////////////////上传AB包////////////////////////////////////////////////////////////////////
    static string CommandPassword
    {
        get
        {
            if (!File.Exists("password.txt"))
            {
                File.WriteAllLines("password.txt", new string[] { "1234" });
            }
            return File.ReadAllLines("password.txt")[0];
        }
    }
    public string UploadAssetBundles(string path, byte[] fileData, string commandPassword)
    {
        if (CommandPassword == commandPassword)
        {
            Directory.CreateDirectory(new FileInfo(path).DirectoryName);
            Console.WriteLine("接收到" + path + "——开始写入，长度为" + fileData.Length);
            File.WriteAllBytes(path, fileData);
            return "AB包更新成功";
        }
        return "指令密码输入错误，服务器拒绝修改";
    }
    public string GetAssetBundlesMD5(string tag)
    {
        if (File.Exists(@"Tag\Md5.txt"))
        {
            var MD5s = File.ReadAllText(@$"AssetBundles/{tag}/MD5.json");
            return MD5s;
        }
        return "{}";
    }

    internal static void Init()
    {

    }
}