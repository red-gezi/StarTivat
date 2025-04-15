#if UNITY_EDITOR
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

namespace Hotfix
{
    public class EditorExtension : MonoBehaviour
    {
        //服务器路径
        //static string serverIP = @"http://106.15.38.165:495";
        static string serverIP = @"http://127.0.0.1:233";
        //游戏热更资源放置路径
        static string HotfixAssetPath = @"Assets\GameResource";
        //游戏AB包资源路径
        static string ABPath = @"AssetsBundles";

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

        /////////////////////////////////////////////////////////////////工具///////////////////////////////////////////////////////////////////////////////////////////
        [MenuItem("StarTivat/Tools/打开服务端", false, 1)]
        static void StartServer() => System.Diagnostics.Process.Start(@"HotfixServer\bin\Debug\net6.0\HotFixServer.exe");
        [MenuItem("StarTivat/Tools/打开游戏客户端", false, 2)]
        static void StartClient() => System.Diagnostics.Process.Start(@"Pc\TouHouMachineLearningSummary.exe");
        [MenuItem("StarTivat/Tools/打开数据表格（云端）", false, 50)]
        static void OpenCloudXls() => System.Diagnostics.Process.Start(@"https://kdocs.cn/l/cfS6F51QxqGd");
        [MenuItem("StarTivat/Tools/打开数据表格", false, 51)]
        static void OpenXls() => System.Diagnostics.Process.Start(@"Assets\GameResources\GameData\GameData.xlsx");
        [MenuItem("StarTivat/Tools/打开表格数据实时同步工具", false, 52)]
        static void UpdateXls() => System.Diagnostics.Process.Start(@"OtherSolution\xls检测更新\bin\Debug\net6.0\xls检测更新.exe");
        /////////////////////////////////////////////////////////////////场景///////////////////////////////////////////////////////////////////////////////////////////
        [MenuItem("StarTivat/Scene/载入初始化场景", priority = 150)]
        static void LoadInitScene() => System.Diagnostics.Process.Start(@"Assets\Scenes\-1_InitScene.unity");
        [MenuItem("StarTivat/Scene/载入热更场景", priority = 151)]
        static void LoadHotFixScene() => System.Diagnostics.Process.Start(@"Assets\Scenes\0_HotfixScene.unity");
        [MenuItem("StarTivat/Scene/载入登录场景", priority = 152)]
        static void LoadLoginScene() => System.Diagnostics.Process.Start(@"Assets\Scenes\1_LoginScene.unity");
        [MenuItem("StarTivat/Scene/载入对战场景", priority = 153)]
        static void LoaBattleScene() => System.Diagnostics.Process.Start(@"Assets\Scenes\2_BattleScene.unity");
        /////////////////////////////////////////////////////////////////项目配置///////////////////////////////////////////////////////////////////////////////////////////
        [MenuItem("StarTivat/Config/切换当前卡牌使用线上版本（确保debug完要切回来）", priority = 1)]
        static void ChangeToOnlineCardScript()
        {
            var targetFile = new FileInfo("Assets\\Script\\9_MixedScene\\CardSpace\\GameCard.asmdef1");
            if (targetFile.Exists)
            {
                targetFile.MoveTo("Assets\\Script\\9_MixedScene\\CardSpace\\GameCard.asmdef");
                AssetDatabase.Refresh();
            }
        }
        [MenuItem("StarTivat/Config/切换当前卡牌使用本地版本（可以查看更多debug细节）", priority = 2)]
        static void ChangeToLoaclCardScript()
        {
            var targetFile = new FileInfo("Assets\\Script\\9_MixedScene\\CardSpace\\GameCard.asmdef");
            if (targetFile.Exists)
            {
                targetFile.MoveTo("Assets\\Script\\9_MixedScene\\CardSpace\\GameCard.asmdef1");
                AssetDatabase.Refresh();
            }
        }
        /////////////////////////////////////////////////////////////////发布（服务端）///////////////////////////////////////////////////////////////////////////////////////////
        [MenuItem("StarTivat/Public/发布当前服务器到正式环境", false, 0)]
        static async void UpdateServer()
        {
            var VersionsHub = new HubConnectionBuilder().WithUrl($"{serverIP}/VersionsHub").Build();
            await VersionsHub.StartAsync();
            var result = await VersionsHub.InvokeAsync<string>("UpdateServer", File.ReadAllBytes(@"OtherSolution\Server\bin\Debug\net6.0\Server.dll"), CommandPassword);
            Debug.LogWarning("上传结果" + result);
            await VersionsHub.StopAsync();
        }
        /////////////////////////////////////////////////////////////////发布卡牌版本///////////////////////////////////////////////////////////////////////////////////////////
        [MenuItem("StarTivat/Public/发布代码版本到测试版", false, 100)]
        static void UpdateCardToTest() => UpdateCard("Test");

        [MenuItem("StarTivat/Public/发布代码版本到正式版", false, 101)]
        static void UpdateCardToRelease() => UpdateCard("Release");
        private static void UpdateCard(string tag)
        {
            var gameCardAssembly = new DirectoryInfo(@"Library\ScriptAssemblies").GetFiles("GameCard*.dll").FirstOrDefault();

            if (gameCardAssembly != null)
            {
                //_ = Command.NetCommand.UploadCardConfigsAsync(cardConfig, drawAbleList, CommandPassword);
            }
            else
            {
                Debug.LogError("检索不到卡牌dll文件");
            }
        }
        /////////////////////////////////////////////////////////////////发布热更新资源///////////////////////////////////////////////////////////////////////////////////////////
        [MenuItem("StarTivat/Public/清空AB包标签", priority = 150)]
        static void ClearABTags() => ClearAssetBundlesTags();
        [MenuItem("StarTivat/Public/生成测试AB包本地AB包资源", priority = 150)]
        static void BuildAssetBundlesToEditor() => BuildAssetBundles();
        [MenuItem("StarTivat/Public/发布电脑游戏热更资源为测试版", priority = 151)]
        static void BuildDAssetBundlesToTest() => BuildAssetBundles("PC_Test");
        [MenuItem("StarTivat/Public/发布电脑游戏热更资源为正式版", priority = 152)]
        static void BuildAssetBundlesToRelease() => BuildAssetBundles("PC_Release");
        //[MenuItem("StarTivat/Public/发布安卓端游戏热更资源为正式版", priority = 153)]
        //static void BuildAssetBundlesToAndroid() => BuildAssetBundles("Android");
        private static void ClearAssetBundlesTags()
        {
            //清空标签
            new DirectoryInfo("Assets").GetDirectories().ToList()
                .ForEach(dire =>
                {
                    dire.GetFiles("*.*", SearchOption.AllDirectories)
                            .Where(file => file.Extension != ".meta" && file.Extension != ".cs")
                            .ToList()
                            .ForEach(file =>
                            {
                                string path = file.FullName.Replace(Directory.GetCurrentDirectory() + @"\", "");
                                try
                                {

                                    AssetImporter.GetAtPath(path).assetBundleName = $"";
                                }
                                catch (Exception e)
                                {
                                    Debug.LogError(path + e.Message);
                                }
                            });
                });
        }
        //仅打包AB包
        private static void BuildAssetBundles()
        {
            //打标签
            new DirectoryInfo(HotfixAssetPath).GetDirectories().ToList()
                .ForEach(dire =>
                {
                    dire.GetFiles("*.*", SearchOption.AllDirectories)
                            .Where(file => file.Extension != ".meta")
                            .ToList()
                            .ForEach(file =>
                            {
                                string path = file.FullName.Replace(Directory.GetCurrentDirectory() + @"\", "");
                                AssetImporter.GetAtPath(path).assetBundleName = $"{dire.Name}.gezi";
                            });
                });
            Debug.LogWarning("标签修改完毕，开始打包");
            //将AB打包到AB文件夹下，并上传
            string outputPath = Directory.GetCurrentDirectory() + $@"\AB\PC_Test";
            Directory.CreateDirectory(outputPath);
            BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
        }
        //打包AB包并上传
        private static async void BuildAssetBundles(string tag)
        {
            //打标签
            new DirectoryInfo(HotfixAssetPath).GetDirectories().ToList()
                .ForEach(dire =>
                {
                    dire.GetFiles("*.*", SearchOption.AllDirectories)
                            .Where(file => file.Extension != ".meta")
                            .ToList()
                            .ForEach(file =>
                            {
                                string path = file.FullName.Replace(Directory.GetCurrentDirectory() + @"\", "");
                                AssetImporter.GetAtPath(path).assetBundleName = $"{dire.Name}.gezi";
                            });
                });
            Debug.LogWarning("标签修改完毕，开始打包");
            //将AB打包到AB文件夹下，并上传
            string outputPath = Directory.GetCurrentDirectory() + $@"\AB\{tag}";
            Directory.CreateDirectory(outputPath);

            BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.None, tag == "Android" ? BuildTarget.Android : BuildTarget.StandaloneWindows64);
            Debug.LogWarning($"{tag}打包完毕");
            Debug.LogWarning("开始生成MD5值校验文件");

            //创建md5文件
            MD5 md5 = new MD5CryptoServiceProvider();
            Dictionary<string, byte[]> MD5s = new();
            new DirectoryInfo(outputPath).GetFiles("*.*").ToList().ForEach(file =>
            {
                //只上传代码和gezi文件
                if (file.Extension == ".gezi" || file.Extension == ".dll")
                {
                    byte[] result = md5.ComputeHash(File.ReadAllBytes(file.FullName));
                    MD5s[file.Name] = result;
                }
            });
            File.WriteAllText(outputPath + @"\MD5.json", MD5s.ToJson());
            var localMD5Dict = MD5s;
            Debug.LogWarning("MD5值校验生成完毕,开始上传文件");
            var hotFixHub = new HubConnectionBuilder().WithUrl($"{serverIP}/HotFixHub").Build();
            hotFixHub.ServerTimeout = new TimeSpan(0, 5, 0);
            await hotFixHub.StartAsync();
            string result = "";
            string OnlieMD5FiIeDatas = "{}";
            try
            {
                //OnlieMD5FiIeDatas = webClient.DownloadString(@$"{serverIP}/Download/{tag}/MD5.json");
                OnlieMD5FiIeDatas = await hotFixHub.InvokeAsync<string>("GetAssetBundlesMD5", tag);
            }
            catch (Exception e)
            {
                Debug.LogError("无法下载网络上MD5.json文件" + e.Message);
            }
            var onlineMD5Dict = OnlieMD5FiIeDatas.ToObject<Dictionary<string, byte[]>>();
            //上传AB包

            foreach (var item in localMD5Dict)
            {
                //如果文件不存在或者md5值不相等才上传
                if (!onlineMD5Dict.ContainsKey(item.Key) || !onlineMD5Dict[item.Key].SequenceEqual(item.Value))
                {
                    Debug.LogWarning(item.Key + "开始传输");
                    result = await hotFixHub.InvokeAsync<string>("UploadAssetBundles", @$"AssetBundles/{tag}/{item.Key}", File.ReadAllBytes(@$"AB/{tag}/{item.Key}"), CommandPassword);
                    Debug.LogWarning(item.Key + "传输" + result);
                }
                else
                {
                    Debug.LogWarning(item.Key + "无更改，无需上传");
                }
            }
            //传输完成后上传AB包MD5文件
            result = await hotFixHub.InvokeAsync<string>("UploadAssetBundles", @$"AssetBundles/{tag}/MD5.json", File.ReadAllBytes(@$"AB/{tag}/MD5.json"), CommandPassword);
            Debug.LogWarning($"{tag}的MD5.json的传输结果为{result}");

            Debug.LogWarning("TouHouMachineLearningSummary.dll开始传输");
            //result = await touhouHub.InvokeAsync<string>("UploadAssetBundles", @$"AssetBundles/DllOrAPK/{tag}/TouHouMachineLearningSummary.dll", File.ReadAllBytes($@"{Directory.GetCurrentDirectory()}/Library/ScriptAssemblies/TouHouMachineLearningSummary.dll"), CommandPassword);
            Debug.LogWarning("TouHouMachineLearningSummary.dll传输" + result);

            byte[] dllMd5 = md5.ComputeHash(File.ReadAllBytes($@"{Directory.GetCurrentDirectory()}/Library/ScriptAssemblies/TouHouMachineLearningSummary.dll"));
            //result = await touhouHub.InvokeAsync<string>("UploadAssetBundles", @$"AssetBundles/DllOrAPK/{tag}/MD5.json", dllMd5, CommandPassword);
            Debug.LogWarning("TouHouMachineLearningSummary.dll的MD5码更新" + result);
            await hotFixHub.StopAsync();
            md5.Dispose();
        }
    }
}
#endif
