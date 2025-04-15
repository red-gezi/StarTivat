using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


public class TrainSystemController : MonoBehaviour
{
    public Transform railPath; // 铁轨路径的Transform组件
    public float speed = 5f; // 火车的最大移动速度
    public int currentRailIndex = 0; // 当前路径点索引
    /// <summary>
    /// 0停止，1前进，2后退
    /// </summary>
    public int moveState = 0;
    private string selectBranch = "M";// 默认中间分支

    public RailPathsSystemController currentRailPathsSystemController;

    public TrainNodeController trainHead;
    public TrainNodeController trainTail;
    //车的车厢
    public List<TrainNodeController> trainCars;
    //实际上列车个每节节点
    List<TrainNodeController> trainNodes = new();
    //每个节点与上个节点的距离
    public List<int> trainNodesBias;
    //车的历史路径22666
    public List<RailController> AttachedRail => trainNodes.Select(node => node.currentRailPath).Distinct().ToList();
    public void Start()
    {
        //初始化车厢
        trainNodes.Add(trainHead);
        trainNodes.AddRange(trainCars);
        trainNodes.Add(trainTail);
        trainNodes.ForEach(node => node.trainSystemController = this);
        //将列车起始点移动到最接近的铁轨上
        var (startRail, startPoint) = currentRailPathsSystemController.FindClostRailAndIndex(trainHead);
        transform.eulerAngles = startRail.transform.eulerAngles;
        transform.position -= (trainHead.transform.position - startPoint);

        //定位每个车厢对应的铁道和锚点(加载存档或者自动检索),从铁路系统中找到每个车辆节点最接近的点
        trainNodes.ForEach(node => currentRailPathsSystemController.FindClostRailAndIndex(node));

    }
    public void InitTrain()
    {

    }
    void Update()
    {
        OnKeyDown();
        switch (moveState)
        {
            case 0: trainNodes.ForEach(node => node.Stop()); break;
            case 1: trainNodes.ForEach(node => node.Forward()); break;
            case 2: trainNodes.ForEach(node => node.Back()); break;
        }
    }
    public void OnKeyDown()
    {
        if (Input.GetKey( KeyCode.LeftShift))
        {
            //短路
            return;
        }
        // 控制火车的移动状态
        if (Input.GetKeyDown(KeyCode.Space))
        {
            moveState = 0;
            Debug.Log("0");
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            moveState = 1;
            Debug.Log("1");
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            moveState = 2;
            Debug.Log("2");
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            selectBranch = "L";
            Debug.Log("L");
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            selectBranch = "R";
            Debug.Log("R");
        }
        else
        {
            selectBranch = "M";
            Debug.Log("M");
        }
    }
}
