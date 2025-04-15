using Sirenix.OdinInspector;
using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject trainModel;
    public GameObject sceneModel;
    public GameObject roadRoot;
    public GameObject roadPrefab;
    [Header("门")]
    public GameObject doorPrefab;
    public GameObject door_L;
    public GameObject door_R;
    public GameObject doorPlane_L;
    public GameObject doorPlane_R;
    [Header("Logo")]
    bool IsLoadFinish = false;
    bool IsQuit;
    public GameObject logoPlane;
    public GameObject loadingPlane;

    public GameObject EnterUi;
    public float stopPos;
    public Transform cameraPos1;
    public Transform cameraPos2;
    public enum MenuState
    {
        WaitLoad,//等待加载，列车运行
        LoadOver,//加载完成，列车减速，生成门
        WaitEnter,//列车停止，等待进入
        None,//不做处理
    }
    MenuState CurrentMenuState { get; set; } = MenuState.WaitLoad;
    async void Start()
    {
        Camera.main.transform.localPosition = cameraPos1.localPosition;
        Camera.main.transform.eulerAngles = cameraPos1.eulerAngles;
        logoPlane.SetActive(true);
        _ = CustomThread.TimerAsync(1f, progress =>
        {
            logoPlane.GetComponent<CanvasGroup>().alpha = 1 - progress;
        });
        await Task.Delay(5000);
        LoadOver();
    }
    float value = 0;
    int index = 0;
    // Update is called once per frame
    void FixedUpdate()
    {
        switch (CurrentMenuState)
        {
            case MenuState.WaitLoad:
                roadRoot.transform.Translate(Vector3.forward * Time.fixedDeltaTime * 10);
                sceneModel.transform.localPosition += (Vector3.forward * Time.fixedDeltaTime * 10);
                value += Time.fixedDeltaTime;
                if (value > 2)
                {
                    if (IsLoadFinish)
                    {
                        _ = CreatDoor(index);
                        CurrentMenuState = MenuState.LoadOver;
                    }
                    else
                    {
                        _ = CreatRoad(index);
                        value = 0;
                        index++;
                    }
                }
                break;
            case MenuState.LoadOver:
                //减速
                _ = CustomThread.TimerAsync(0.5f, progress =>
                {
                    Camera.main.transform.localPosition = Vector3.Lerp(cameraPos1.localPosition, cameraPos2.localPosition, progress);
                    Camera.main.transform.eulerAngles = Quaternion.Lerp(cameraPos1.rotation, cameraPos2.rotation, progress).eulerAngles;
                });
                _ = CustomThread.TimerAsync(5f, progress =>
                {
                    //sceneModel.transform.Translate(transform.forward * Time.fixedDeltaTime * 10 * (1 - progress));
                    sceneModel.transform.localPosition += (transform.forward * Time.fixedDeltaTime * 10 * (1 - progress));

                    roadRoot.transform.Translate(transform.forward * Time.fixedDeltaTime * 10 * (1 - progress));
                },
                stopAction: () =>
                {
                    EnterUi.SetActive(true);
                    CurrentMenuState = MenuState.WaitEnter;
                });
                CurrentMenuState = MenuState.None;
                break;
            case MenuState.WaitEnter:
                if (Input.GetMouseButtonDown(0))
                {
                    CurrentMenuState = MenuState.None;
                    Debug.Log("星穹列车！发车");

                    _ = CustomThread.TimerAsync(1f, progress =>
                    {
                        door_L.transform.eulerAngles = Vector3.up * 90 * progress;
                        door_R.transform.eulerAngles = Vector3.down * 90 * progress;
                    },
                    stopAction: () =>
                    {
                        _ = CustomThread.TimerAsync(0.8f, progress =>
                        {
                            doorPlane_L.GetComponent<Renderer>().material.color = Color.white * progress * 10;
                            doorPlane_R.GetComponent<Renderer>().material.color = Color.white * progress * 10;
                        },
                        stopAction: () =>
                        {
                            _ = CustomThread.TimerAsync(0.2f, progress =>
                            {
                                loadingPlane.transform.localScale = Vector3.one * progress;
                            });
                        });
                    });
                    _ = CustomThread.TimerAsync(3f, progress =>
                    {
                        trainModel.transform.Translate(-transform.forward * Time.fixedDeltaTime * 20 * progress);
                    },
                    stopAction: () =>
                    {
                        SceneManager.LoadScene(1);
                    });
                }
                break;
            case MenuState.None:
                break;
        }
    }
    public async void WaitLoad()
    {
        for (int i = 0; ; i++)
        {
            int index = i;
            if (!IsLoadFinish)
            {
                _ = CreatRoad(index);
                await Task.Delay(1000);
            }
            else
            {
                await CreatDoor(index);
            }
            if (IsQuit) break;
        }
    }
    [Button("加载完成")]
    public void LoadOver()
    {
        IsLoadFinish = true;
    }

    public async Task CreatRoad(int index)
    {
        //创造路面
        var newRoad = Instantiate(roadPrefab, roadRoot.transform);
        var z = (index + 3) * -20;
        //移动到上方
        await CustomThread.TimerAsync(0.3f, progress =>
        {
            newRoad.transform.localPosition = new(0, Mathf.Lerp(-5, 0.5f, progress), z);
        });
        //移动到正确位置
        await CustomThread.TimerAsync(0.1f, progress =>
        {
            newRoad.transform.localPosition = new(0, Mathf.Lerp(0.5f, 0, progress), z);
        });

    }
    public async Task CreatDoor(int index)
    {
        //创造路面
        var newRoad = doorPrefab;
        newRoad.SetActive(true);
        var z = (index + 3) * -20;
        //移动到上方
        await CustomThread.TimerAsync(0.4f, progress =>
        {
            newRoad.transform.localPosition = new(0, Mathf.Lerp(-5, 0.5f, progress), z);
        });
        //移动到正确位置
        await CustomThread.TimerAsync(0.1f, progress =>
        {
            newRoad.transform.localPosition = new(0, Mathf.Lerp(0.5f, 0, progress), z);
        });
    }
    private void OnApplicationQuit()
    {
        IsQuit = true;
    }
}
