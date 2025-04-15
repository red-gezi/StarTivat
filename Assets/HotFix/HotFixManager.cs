using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Hotfix
{
    public enum GameStartMode
    {
        Editor,
        PC_Release,
        PC_Test,
        Android
    }
    public partial class HotFixManager : MonoBehaviour
    {
        #region 需要配置项目
        string projectName = "StartTivat";
        static string serverDownloadUrl = $"http://106.15.38.165:495/Download";
        #endregion
        #region UI
        public string version;
        public Text loadText;
        public Text processText;
        public Text versionText;

        public TMP_Dropdown serverSelect;
        public Slider slider;
        //重启通知
        public GameObject RestartNotice;
        //服务器选择界面
        public GameObject ServerSelect;
        #endregion


        //后期自定义修改服务器ip
        //string serverIP = File.ReadAllLines("敏感信息.txt")[1];
        MD5 md5 = new MD5CryptoServiceProvider();
        public static GameStartMode CurrentGameStartMode;
        string ServerTag => CurrentGameStartMode.ToString();

        bool isTestMode;

        void Start()
        {
            RestartNotice.transform.localScale = new Vector3(1, 0, 1);
            versionText.text = version;
            loadText.text = "校验资源包";
            //根据平台和配置文件判断启动模式
            if (Application.isEditor)
                CurrentGameStartMode = GameStartMode.Editor;
            else if (isTestMode)
                CurrentGameStartMode = GameStartMode.PC_Test;
            else
                CurrentGameStartMode = GameStartMode.PC_Release;
        }
        public async void StartGame()
        {
            //关掉选择界面
            ServerSelect.SetActive(false);
            //更新和加载AB包
            await CheckAssetBundles();

        }
        //校验本地文件
        private async Task CheckAssetBundles()
        {
            loadText.text = "开始本地AB包资源检测";
            //获得AB包来源标签
            loadText.text = "开始下载文件";
            Debug.LogWarning("开始下载文件" + System.DateTime.Now);

            string downLoadPath = $"{Directory.GetCurrentDirectory()}/Assetbundles/{ServerTag}/";
            Directory.CreateDirectory(downLoadPath);
            using (var httpClient = new HttpClient())
            {
                var responseMessage = await httpClient.GetAsync($"{serverDownloadUrl}/{ServerTag}/MD5.json");
                if (!responseMessage.IsSuccessStatusCode) { loadText.text = "MD5文件获取出错"; return; }
                var OnlieMD5FiIeDatas = await responseMessage.Content.ReadAsStringAsync();
                var Md5Dict = OnlieMD5FiIeDatas.ToObject<Dictionary<string, byte[]>>();
                Debug.Log("MD5文件已加载完成" + OnlieMD5FiIeDatas);
                loadText.text = "MD5文件已加载完成：";
                //已下好任务数
                int downloadTaskCount = 0;
                //开始遍历校验并更新本地AB包文件
                foreach (var MD5FiIeData in Md5Dict)
                {
                    //当前校验的本地文件
                    FileInfo localFile = new FileInfo(downLoadPath + MD5FiIeData.Key);
                    if (localFile.Exists && MD5FiIeData.Value.SequenceEqual(md5.ComputeHash(File.ReadAllBytes(localFile.FullName))))
                    {
                        loadText.text = MD5FiIeData.Key + "校验成功，无需下载";
                        Debug.LogWarning(MD5FiIeData.Key + "校验成功，无需下载");
                    }
                    else
                    {
                        loadText.text = MD5FiIeData.Key + "有新版本，开始重新下载";
                        Debug.LogWarning(MD5FiIeData.Key + "有新版本，开始重新下载");
                        await DownLoadFile(MD5FiIeData, localFile);
                        async Task DownLoadFile(KeyValuePair<string, byte[]> MD5FiIeData, FileInfo localFile)
                        {
                            loadText.text = $"正在下载:{MD5FiIeData.Key},进度 {downloadTaskCount}/{Md5Dict.Count}";
                            using (WebClient webClient = new WebClient())
                            {
                                webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
                                Debug.LogWarning("下载文件" + $"{serverDownloadUrl}/{ServerTag}/{MD5FiIeData.Key}");
                                await webClient.DownloadFileTaskAsync(new System.Uri($"{serverDownloadUrl}/{ServerTag}/{MD5FiIeData.Key}"), localFile.FullName);
                                Debug.LogWarning(MD5FiIeData.Key + "下载完成");
                                Debug.LogWarning("结束下载文件" + localFile.Name + " " + System.DateTime.Now);
                                void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
                                {
                                    processText.text = $"{e.BytesReceived / 1024 / 1024}MB/{e.TotalBytesToReceive / 1024 / 1024}MB";
                                    slider.value = e.BytesReceived * 1f / e.TotalBytesToReceive;
                                }
                            }
                        }
                    }
                    downloadTaskCount++;
                }
                Debug.LogWarning("全部AB包下载完成");
                loadText.text = "全部AB包下载完成";


                string localDllOrApkPath = "";
                string onlineDllOrApkPath = "";
                string onlineDllOrApk_MD5Path = "";

                //指定热更场景和资源本地路径
                localDllOrApkPath = new DirectoryInfo(Directory.GetCurrentDirectory()).GetFiles("TouHouMachineLearningSummary.dll", SearchOption.AllDirectories).FirstOrDefault()?.FullName;
                //指定热更场景和资源网络路径
                onlineDllOrApkPath = $"{serverDownloadUrl}/{ServerTag}_Dll/TouHouMachineLearningSummary.dll";
                onlineDllOrApk_MD5Path = $"{serverDownloadUrl}/{ServerTag}_Dll/MD5.json";

                responseMessage = await httpClient.GetAsync(onlineDllOrApk_MD5Path);
                if (!responseMessage.IsSuccessStatusCode) { Debug.LogError("dll或者apk的md5文件下载失败"); return; }
                var data = await responseMessage.Content.ReadAsByteArrayAsync();
                //如果是手机端，检查apk变更，否则检查dll变更，若发生变更，则重启
                if (data.SequenceEqual(md5.ComputeHash(File.ReadAllBytes(new FileInfo(localDllOrApkPath).FullName))))
                {
                    Debug.Log("文件无改动");
                }
                else
                {
                    responseMessage = await httpClient.GetAsync(onlineDllOrApkPath);
                    if (!responseMessage.IsSuccessStatusCode) { Debug.LogError("文件下载失败"); return; }
                    //保存相关的dll或者apk文件
                    if (!Application.isEditor)
                    {
                        File.WriteAllBytes(localDllOrApkPath, await responseMessage.Content.ReadAsByteArrayAsync());
                        RestartGame();
                    }
                }
            }
            md5.Dispose();

            //加载AB包，并从中加载场景
            Debug.LogWarning("开始初始化AB包");
            AssetBundle.UnloadAllAssetBundles(true);
            loadText.text = "资源包校验完毕，少女加载中~~~~~";
            await AssetBundleManager.Init(ServerTag,true);
            //显示AB包加载进度
            while (true)
            {
                (int currentLoadABCouat, int totalLoadABCouat) process = AssetBundleManager.GetLoadProcess();
                slider.value = process.currentLoadABCouat * 1.0f / process.totalLoadABCouat;
                processText.text = $"{process.currentLoadABCouat}/{process.totalLoadABCouat}";
                if (process.currentLoadABCouat == process.totalLoadABCouat)
                {
                    break;
                }
                await Task.Delay(50);
            }
            Debug.LogWarning("AB包加载完毕，切换场景。。。");
            SceneManager.LoadScene("1_LoginScene", LoadSceneMode.Single);
        }
        //重启应用
        public void RestartGame()
        {
            var game = new DirectoryInfo(Directory.GetCurrentDirectory()).GetFiles("TouHouMachineLearningSummary.exe", SearchOption.AllDirectories).FirstOrDefault();
            if (game != null)
            {
                System.Diagnostics.Process.Start(game.FullName);
            }
            Application.Quit();
        }
        private void OnGUI()
        {
            if (GUI.Button(new Rect(0, 150, 100, 50), "重启"))
            {
                RestartGame();
            }
            if (GUI.Button(new Rect(0, 200, 100, 50), "退出"))
            {
                Application.Quit();

            }
        }
    }
}