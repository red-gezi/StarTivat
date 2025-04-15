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
        #region ��Ҫ������Ŀ
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
        //����֪ͨ
        public GameObject RestartNotice;
        //������ѡ�����
        public GameObject ServerSelect;
        #endregion


        //�����Զ����޸ķ�����ip
        //string serverIP = File.ReadAllLines("������Ϣ.txt")[1];
        MD5 md5 = new MD5CryptoServiceProvider();
        public static GameStartMode CurrentGameStartMode;
        string ServerTag => CurrentGameStartMode.ToString();

        bool isTestMode;

        void Start()
        {
            RestartNotice.transform.localScale = new Vector3(1, 0, 1);
            versionText.text = version;
            loadText.text = "У����Դ��";
            //����ƽ̨�������ļ��ж�����ģʽ
            if (Application.isEditor)
                CurrentGameStartMode = GameStartMode.Editor;
            else if (isTestMode)
                CurrentGameStartMode = GameStartMode.PC_Test;
            else
                CurrentGameStartMode = GameStartMode.PC_Release;
        }
        public async void StartGame()
        {
            //�ص�ѡ�����
            ServerSelect.SetActive(false);
            //���ºͼ���AB��
            await CheckAssetBundles();

        }
        //У�鱾���ļ�
        private async Task CheckAssetBundles()
        {
            loadText.text = "��ʼ����AB����Դ���";
            //���AB����Դ��ǩ
            loadText.text = "��ʼ�����ļ�";
            Debug.LogWarning("��ʼ�����ļ�" + System.DateTime.Now);

            string downLoadPath = $"{Directory.GetCurrentDirectory()}/Assetbundles/{ServerTag}/";
            Directory.CreateDirectory(downLoadPath);
            using (var httpClient = new HttpClient())
            {
                var responseMessage = await httpClient.GetAsync($"{serverDownloadUrl}/{ServerTag}/MD5.json");
                if (!responseMessage.IsSuccessStatusCode) { loadText.text = "MD5�ļ���ȡ����"; return; }
                var OnlieMD5FiIeDatas = await responseMessage.Content.ReadAsStringAsync();
                var Md5Dict = OnlieMD5FiIeDatas.ToObject<Dictionary<string, byte[]>>();
                Debug.Log("MD5�ļ��Ѽ������" + OnlieMD5FiIeDatas);
                loadText.text = "MD5�ļ��Ѽ�����ɣ�";
                //���º�������
                int downloadTaskCount = 0;
                //��ʼ����У�鲢���±���AB���ļ�
                foreach (var MD5FiIeData in Md5Dict)
                {
                    //��ǰУ��ı����ļ�
                    FileInfo localFile = new FileInfo(downLoadPath + MD5FiIeData.Key);
                    if (localFile.Exists && MD5FiIeData.Value.SequenceEqual(md5.ComputeHash(File.ReadAllBytes(localFile.FullName))))
                    {
                        loadText.text = MD5FiIeData.Key + "У��ɹ�����������";
                        Debug.LogWarning(MD5FiIeData.Key + "У��ɹ�����������");
                    }
                    else
                    {
                        loadText.text = MD5FiIeData.Key + "���°汾����ʼ��������";
                        Debug.LogWarning(MD5FiIeData.Key + "���°汾����ʼ��������");
                        await DownLoadFile(MD5FiIeData, localFile);
                        async Task DownLoadFile(KeyValuePair<string, byte[]> MD5FiIeData, FileInfo localFile)
                        {
                            loadText.text = $"��������:{MD5FiIeData.Key},���� {downloadTaskCount}/{Md5Dict.Count}";
                            using (WebClient webClient = new WebClient())
                            {
                                webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
                                Debug.LogWarning("�����ļ�" + $"{serverDownloadUrl}/{ServerTag}/{MD5FiIeData.Key}");
                                await webClient.DownloadFileTaskAsync(new System.Uri($"{serverDownloadUrl}/{ServerTag}/{MD5FiIeData.Key}"), localFile.FullName);
                                Debug.LogWarning(MD5FiIeData.Key + "�������");
                                Debug.LogWarning("���������ļ�" + localFile.Name + " " + System.DateTime.Now);
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
                Debug.LogWarning("ȫ��AB���������");
                loadText.text = "ȫ��AB���������";


                string localDllOrApkPath = "";
                string onlineDllOrApkPath = "";
                string onlineDllOrApk_MD5Path = "";

                //ָ���ȸ���������Դ����·��
                localDllOrApkPath = new DirectoryInfo(Directory.GetCurrentDirectory()).GetFiles("TouHouMachineLearningSummary.dll", SearchOption.AllDirectories).FirstOrDefault()?.FullName;
                //ָ���ȸ���������Դ����·��
                onlineDllOrApkPath = $"{serverDownloadUrl}/{ServerTag}_Dll/TouHouMachineLearningSummary.dll";
                onlineDllOrApk_MD5Path = $"{serverDownloadUrl}/{ServerTag}_Dll/MD5.json";

                responseMessage = await httpClient.GetAsync(onlineDllOrApk_MD5Path);
                if (!responseMessage.IsSuccessStatusCode) { Debug.LogError("dll����apk��md5�ļ�����ʧ��"); return; }
                var data = await responseMessage.Content.ReadAsByteArrayAsync();
                //������ֻ��ˣ����apk�����������dll����������������������
                if (data.SequenceEqual(md5.ComputeHash(File.ReadAllBytes(new FileInfo(localDllOrApkPath).FullName))))
                {
                    Debug.Log("�ļ��޸Ķ�");
                }
                else
                {
                    responseMessage = await httpClient.GetAsync(onlineDllOrApkPath);
                    if (!responseMessage.IsSuccessStatusCode) { Debug.LogError("�ļ�����ʧ��"); return; }
                    //������ص�dll����apk�ļ�
                    if (!Application.isEditor)
                    {
                        File.WriteAllBytes(localDllOrApkPath, await responseMessage.Content.ReadAsByteArrayAsync());
                        RestartGame();
                    }
                }
            }
            md5.Dispose();

            //����AB���������м��س���
            Debug.LogWarning("��ʼ��ʼ��AB��");
            AssetBundle.UnloadAllAssetBundles(true);
            loadText.text = "��Դ��У����ϣ���Ů������~~~~~";
            await AssetBundleManager.Init(ServerTag,true);
            //��ʾAB�����ؽ���
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
            Debug.LogWarning("AB��������ϣ��л�����������");
            SceneManager.LoadScene("1_LoginScene", LoadSceneMode.Single);
        }
        //����Ӧ��
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
            if (GUI.Button(new Rect(0, 150, 100, 50), "����"))
            {
                RestartGame();
            }
            if (GUI.Button(new Rect(0, 200, 100, 50), "�˳�"))
            {
                Application.Quit();

            }
        }
    }
}