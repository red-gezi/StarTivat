using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RailPathsSystemController : MonoBehaviour
{
    public int pointsCount = 5; // 曲线上点的数量
    public GameObject LineRailPathPrefab;
    public GameObject CurveRailPathPrefab;
    public List<RailController> railPathControllers = new List<RailController>();
    public static RailPathsSystemController Instance { get; set; }
    private void Awake() => Instance = this;
    //传入火车节点，找到最接近的铁路节点
    public (RailController, Vector3) FindClostRailAndIndex(TrainNodeController trainNode)
    {
        var closestRailPath = railPathControllers
            .OrderBy(railPath => Vector3.Distance(railPath.transform.position, trainNode.transform.position))
            .FirstOrDefault();
        if (closestRailPath != null)
        {
            trainNode.currentRailPath = closestRailPath;
            var closestPoint = closestRailPath.Points
                .OrderBy(point => Vector3.Distance(point, trainNode.transform.position))
                .FirstOrDefault();
            trainNode.currentNodeIndex = closestRailPath.Points.IndexOf(closestPoint);
            return (closestRailPath, closestPoint);
        }
        else
        {
            Debug.LogError("No rail path found within the collection.");
            return (null, Vector3.zero);
        }
    }
    public List<RailController> FindRailByRailIndex(RailIndex index) =>
        railPathControllers
        .Where(rail => rail.Index == index)
        .ToList();
    public void CreatRailByIndex(bool isCurve, RailIndex index,int angel=0)
    {

        GameObject newRailModel = Instantiate(isCurve ? Instance.CurveRailPathPrefab : Instance.LineRailPathPrefab, transform);
        RailController newRail = newRailModel.GetComponent<RailController>();
        newRail.Index = index;
        newRailModel.transform.position = transform.position + new Vector3(index.X * 20, 0, index.Y * 20);
        newRailModel.transform.eulerAngles = new Vector3(0, angel, 0);
        //添加进列表
        railPathControllers.Add(newRail);
        //检索新铁轨两侧是否存在铁轨，是的话互相建立链接
        FindRailByRailIndex(newRail.FIndex).ForEach(rail => rail.AddConnectRail(newRail));
        FindRailByRailIndex(newRail.BIndex).ForEach(rail => rail.AddConnectRail(newRail));
    }
    public void CreatRail(RailController currentRail, RailPutType railPutType)
    {
        //铁道系统判断该出能否放置铁轨（禁止重复或者交叉）
        bool canPlaceRail = true;
        if (currentRail.connectRails[railPutType] != null)
        {
            canPlaceRail = false;
        }
        if (!canPlaceRail)
        {
            return;
        }
        //实际生成铁轨
        GameObject newRailModel = railPutType switch
        {
            RailPutType.FM or RailPutType.BM => Instantiate(Instance.LineRailPathPrefab, currentRail.transform.parent),
            _ => Instantiate(Instance.CurveRailPathPrefab, currentRail.transform.parent),
        };
        //计算坐标
        newRailModel.transform.position = railPutType switch
        {
            RailPutType.FL or RailPutType.FM or RailPutType.FR => currentRail.FPosition,
            RailPutType.BL or RailPutType.BM or RailPutType.BR => currentRail.BPosition,
            _ => throw new Exception("RailPutType Error"),
        };
        //计算角度
        newRailModel.transform.eulerAngles = railPutType switch
        {
            RailPutType.FL => currentRail.transform.eulerAngles + (currentRail.isCurve ? new Vector3(0, -90, 0) : Vector3.zero),
            RailPutType.FM => currentRail.transform.eulerAngles + (currentRail.isCurve ? new Vector3(0, -90, 0) : Vector3.zero),
            RailPutType.FR => currentRail.transform.eulerAngles - new Vector3(0, 90, 0) + (currentRail.isCurve ? new Vector3(0, -90, 0) : Vector3.zero),
            RailPutType.BL => currentRail.transform.eulerAngles + new Vector3(0, 90, 0),
            RailPutType.BM => currentRail.transform.eulerAngles,
            RailPutType.BR => currentRail.transform.eulerAngles + new Vector3(0, 180, 0),
            _ => throw new Exception("RailPutType Error"),
        };
        RailController newRail = newRailModel.GetComponent<RailController>();
        //配置铁轨参数
        newRail.Index = railPutType switch
        {
            RailPutType.FL => currentRail.FIndex,
            RailPutType.FM => currentRail.FIndex,
            RailPutType.FR => currentRail.FIndex,
            RailPutType.BL => currentRail.BIndex,
            RailPutType.BM => currentRail.BIndex,
            RailPutType.BR => currentRail.BIndex,
            RailPutType.None => new(0, 0),
            _ => throw new Exception("RailPutType Error"),
        };
        //添加进列表
        railPathControllers.Add(newRail);
        //检索新铁轨两侧是否存在铁轨，是的话互相建立链接
        FindRailByRailIndex(newRail.FIndex).ForEach(rail => rail.AddConnectRail(newRail));
        FindRailByRailIndex(newRail.BIndex).ForEach(rail => rail.AddConnectRail(newRail));
    }
    //移除一个铁轨
    public void Remove(RailController currentRail)
    {
        //从铁路集合中移除
        railPathControllers.Remove(currentRail);
        //从相邻铁轨中移除（要改进）
        railPathControllers.ForEach(rail => rail.RemoveConnectRail(currentRail));
        //销毁物体
        Destroy(currentRail.gameObject);
    }
    //保存铁轨数据
    public void Save()
    {

    }
    //加载铁轨数据
    public void Load()
    {

    }
    private void Start()
    {
        CreatRailByIndex(false, new(0, 0));
        CreatRailByIndex(false, new(0, 1));
        CreatRailByIndex(false, new(0, 2));
        CreatRailByIndex(false, new(0, 3));
        CreatRailByIndex(false, new(0, 4));
        CreatRailByIndex(false, new(0, 5));
    }
    //绘制范围
    private void OnDrawGizmos()
    {
        int bias = 1;
        int maxX = railPathControllers.Max(rail => rail.Index.X) + 1 + bias;
        int minX = railPathControllers.Min(rail => rail.Index.X) - bias;
        int maxZ = railPathControllers.Max(rail => rail.Index.Y) + 1 + bias;
        int minZ = railPathControllers.Min(rail => rail.Index.Y) - bias;
        for (int x = minX; x <= maxX; x++)
        {
            Vector3 start = new Vector3(x - 0.5f, 0.02f, minZ - 0.5f) * 20 + transform.position;
            Vector3 end = new Vector3(x - 0.5f, 0.02f, maxZ - 0.5f) * 20 + transform.position;
            Debug.DrawLine(start, end, Color.red);
        }
        for (int z = minZ; z <= maxZ; z++)
        {
            Vector3 start = new Vector3(minX - 0.5f, 0.02f, z - 0.5f) * 20 + transform.position;
            Vector3 end = new Vector3(maxX - 0.5f, 0.02f, z - 0.5f) * 20 + transform.position;
            Debug.DrawLine(start, end, Color.red);
        }
    }
}
