using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenBreak : MonoBehaviour
{
    public Transform breakPoint;
    public Transform screenPanel;
    public Transform screenPanel_temp;
    public GameObject lightBackground;
    public float force;
    public float radius;
    public AudioClip breakStart;
    public AudioClip breakKeep;
    public AudioClip breakEnd;
    public static ScreenBreak Instance;
    AudioSource source;
    private void Awake()
    {
        Instance = this;
        source = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    async void Start()
    {
        Init();
        //Show();
    }
    public void Init()
    {
        screenPanel.gameObject.SetActive(false);
        lightBackground.gameObject.SetActive(false);
    }
    public async Task Show()
    {
        // 异步加载新场景
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
        //创造一个新实例用于破坏
        screenPanel_temp = Instantiate(screenPanel, screenPanel.transform.parent);
        ForachPiece(piece => piece.GetComponent<Rigidbody>().isKinematic = true);
        ForachPiece(piece => piece.GetComponent<Renderer>().material.SetTexture("_MainTex", Capture()));
        ForachPiece(piece => piece.GetComponent<Renderer>().material.SetFloat("_alpha", 1));
        source.clip = breakStart;
        source.loop = false;
        source.Play();
       SceneManager.LoadScene(2, LoadSceneMode.Single);

        // 等待场景加载完成
        //while (!asyncLoad.isDone)
        //{
        //    await Task.Yield(); // 使协程不阻塞主线程
        //    Debug.Log("进度" + asyncLoad.progress);
        //}
        lightBackground.gameObject.SetActive(true);
        screenPanel_temp.gameObject.SetActive(true);
        //加载新场景
        await Task.Delay(100);

        screenPanel.parent.GetComponent<Canvas>().worldCamera = Camera.main;
        ForachPiece(piece => piece.localEulerAngles += UnityEngine.Random.insideUnitSphere * 3);
        ForachPiece(piece => piece.GetComponent<Renderer>().material.SetFloat("_angel", MathF.Abs(Mathf.Sin(piece.transform.localEulerAngles.magnitude))));
        source.clip = breakKeep;
        source.loop = true ;
        source.Play();
        await Task.Delay(1000);

        ForachPiece(piece => piece.GetComponent<Rigidbody>().isKinematic = false);
        lightBackground.SetActive(false);
        ForachPiece(piece => piece.GetComponent<Rigidbody>().AddExplosionForce(force, breakPoint.position, radius));
        source.clip = breakEnd;
        source.loop = false;
        source.Play();
        await CustomThread.TimerAsync(1, process => ForachPiece(piece => piece.GetComponent<Renderer>().material.SetFloat("_alpha", 1 - process)));

        //清除资源
        lightBackground.gameObject.SetActive(false);
        screenPanel_temp.gameObject.SetActive(false);
        Destroy(screenPanel_temp.gameObject);

        void ForachPiece(Action<Transform> action)
        {
            foreach (Transform child in screenPanel_temp)
            {
                action(child);
            }
        }

        Texture2D Capture()
        {
            try
            {
                // 设置纹理的宽度和高度
                int width = Screen.width;
                int height = Screen.height;
                Texture2D CaptureTexture = new Texture2D(width, height, TextureFormat.RGB24, false);
                // 捕获相机视图
                RenderTexture activeRT = RenderTexture.active;
                RenderTexture tempRT = RenderTexture.GetTemporary(width, height, 0);
                // 将相机渲染到临时的 RenderTexture
                Camera.main.targetTexture = tempRT;
                RenderTexture.active = tempRT;
                Camera.main.Render();
                // 读取像素数据到 Texture2D
                CaptureTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                CaptureTexture.Apply();
                // 清理
                RenderTexture.active = activeRT;
                Camera.main.targetTexture = null;
                RenderTexture.ReleaseTemporary(tempRT);
                return CaptureTexture;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return null;
            }
        }
    }
}
