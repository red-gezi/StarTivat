using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;

public class RailController : MonoBehaviour
{
    //所属的铁轨控制系统
    RailPathsSystemController BelongRailPathSystem => transform.parent.GetComponent<RailPathsSystemController>();
    public bool isCurve;//是否是曲线铁轨
    public int pointsCount;
    public List<Vector3> Points { get; set; }
    public GameObject pointPrefab; // 用于实例化的点的预制体

    //根据铁轨首尾坐标计算朝向
    public Vector3 Direction => Points.Last() - Points.First();
    [ShowInInspector]
    public int x => Index.X;
    [ShowInInspector]
    public int y => Index.Y;
    //铁轨当前索引
    public RailIndex Index=new(0,0);
    public float heigh;
    public RailIndex FIndex => Index + GetPlacementDirection(true);
    public RailIndex BIndex => Index - GetPlacementDirection(false);
    public Vector3 FPosition => transform.position + GetPlacementDirection(true).ToVector3() * 20;
    public Vector3 BPosition => transform.position - GetPlacementDirection(false).ToVector3() * 20;
    //铁轨两个方向连接点索引
    public List<RailIndex> ConnectIndexex => new List<RailIndex>() { FIndex, BIndex };

    //六个方向连接的铁轨
    [ShowInInspector]
    public Dictionary<RailPutType, RailController> connectRails = new()
    {
        { RailPutType.FL,null },
        { RailPutType.FM,null },
        { RailPutType.FR,null },
        { RailPutType.BL,null },
        { RailPutType.BM,null },
        { RailPutType.BR,null },
    };
    public void AddConnectRail(RailController newRail)
    {
        //判断双方是否连接
        bool isConnect = newRail.Index == FIndex || newRail.Index == BIndex;
        if (!isConnect)
        {
            Debug.LogError("相邻块不存在连接铁轨" + Index);
            return;
        }
        Debug.LogWarning("相邻块存在连接铁轨" + Index);
        //本铁轨与对于新铁轨的关系
        if (!isCurve)//M 本铁轨是新铁轨正中
        {
            //本铁轨位于新铁轨前方
            if (newRail.FIndex == Index)
            {
                newRail.connectRails[RailPutType.FM] = this;
            }
            //本铁轨位于新铁轨后方
            if (newRail.BIndex == Index)
            {
                newRail.connectRails[RailPutType.BM] = this;
            }
        }
        //本铁轨F面与新铁轨相接，则
        else if (newRail.Index == FIndex)
        {
            //本铁轨位于新铁轨前方
            if (newRail.FIndex == Index)
            {
                newRail.connectRails[RailPutType.FR] = this;
            }
            //本铁轨位于新铁轨后方方
            if (newRail.BIndex == Index)
            {
                newRail.connectRails[RailPutType.BL] = this;
            }
        }
        else if (newRail.Index == BIndex)
        {
            //本铁轨位于新铁轨前方
            if (newRail.FIndex == Index)
            {
                newRail.connectRails[RailPutType.FL] = this;
            }
            //本铁轨位于新铁轨后方方
            if (newRail.BIndex == Index)
            {
                newRail.connectRails[RailPutType.BR] = this;
            }
        }
        //新铁轨和本铁轨关系
        if (!newRail.isCurve)//M 新铁轨是本铁轨正中
        {
            //新铁轨位于本铁轨前方
            if (newRail.Index == FIndex)
            {
                connectRails[RailPutType.FM] = newRail;
            }
            //新铁轨位于本铁轨后方
            if (newRail.Index == BIndex)
            {
                connectRails[RailPutType.BM] = newRail;
            }
        }
        //本铁轨F面与新铁轨相接，则
        else if (newRail.Index == FIndex)
        {
            //新铁位前方与本铁轨前方相连
            if (newRail.FIndex == Index)
            {
                connectRails[RailPutType.FR] = newRail;
            }
            //新铁位后方与本铁轨前方相连
            if (newRail.BIndex == Index)
            {
                connectRails[RailPutType.FL] = newRail;
            }
        }
        //本铁轨B面与新铁轨相接，则
        else if (newRail.Index == BIndex)
        {
            //新铁位前方与本铁轨前方相连
            if (newRail.FIndex == Index)
            {
                connectRails[RailPutType.BL] = newRail;
            }
            //新铁位后方与本铁轨前方相连
            if (newRail.BIndex == Index)
            {
                connectRails[RailPutType.BR] = newRail;
            }
        }
    }
    //从关联铁轨字典中移除目标铁轨
    public void RemoveConnectRail(RailController targetRail)
    {
        connectRails
            .Where(kvp => kvp.Value == targetRail)
            .ToList()
            .ForEach(kvp => connectRails[kvp.Key] = null);
    }
    public RailIndex GetPlacementDirection(bool isForward = true)
    {
        int angel = (int)Mathf.Round(transform.eulerAngles.y);
        if (isCurve && isForward)
        {
            angel -= 90;
        }
        //将角度映射到0~360度之间
        angel = (angel + 360) % 360;
        return angel switch
        {
            0 => new(0, 1),
            90 => new(1, 0),
            180 => new(0, -1),
            270 => new(-1, 0),
            _ => throw new Exception("角度错误"),
        };
    }
    private void Awake()
    {
        CreatPathPoint();
        RefreshPathPoint();
        RefreshBranchPoint();
    }
    public void InitRailPath()
    {

    }
    private void CreatPathPoint()
    {
        for (int i = 0; i < pointsCount; i++)
        {
            GameObject point = Instantiate(pointPrefab);
            point.transform.parent = transform.GetChild(0);
        }
    }
    //初始化路径点位置
    private void RefreshPathPoint()
    {
        for (int i = 0; i < pointsCount; i++)
        {
            float x, y, z, t;
            if (!isCurve)
            {
                t = (float)i / (pointsCount - 1);
                x = 0;
                y = 0;
                z = Mathf.Lerp(-10, 10, t);
            }
            else
            {
                int r = 10;
                float totalArcLength = Mathf.PI * r / 2;
                float arcLengthPerPoint = totalArcLength / (pointsCount - 1);

                float s = i * arcLengthPerPoint;
                float θ = s / r;
                x = Mathf.Cos(θ) * r - r;
                y = 0;
                z = Mathf.Sin(θ) * r - r;
            }
            // 使用射线检测地形高度
            Vector3 rayOrigin = transform.position + new Vector3(x, 20, z); // 从一个较高的位置发射射线，确保能击中地面
            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit))
            {
                // 射线击中了地形或其他碰撞器
                y = hit.point.y - transform.position.y + 1; // 设置y坐标为地形高度加上偏移量
                //Debug.Log(hit.point.y);
            }
            else
            {
                Debug.LogWarning("Raycast did not hit anything.");
            }
            transform.GetChild(0).GetChild(i).transform.localPosition = new Vector3(x, y, z);
        }
        Points = transform.GetChild(0)
                          .Cast<Transform>()
                          .Select(child => child.position)
                          .ToList();
    }
    //初始化分支创建点位置
    private void RefreshBranchPoint()
    {
        foreach (Transform point in transform.GetChild(2))
        {
            Vector3 targetPos = point.localPosition;
            // 使用射线检测地形高度
            float x = targetPos.x;
            float z = targetPos.z;
            float y = 1;
            Vector3 rayOrigin = transform.position + new Vector3(targetPos.x, 20, targetPos.z); // 从一个较高的位置发射射线，确保能击中地面
            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit))
            {
                y = hit.point.y - transform.position.y + 1; // 设置y坐标为地形高度加上偏移量
                //Debug.Log(hit.point.y);
                Debug.DrawRay(rayOrigin, Vector3.down * 20);
            }
            else
            {
                Debug.LogWarning("Raycast did not hit anything.");
            }
            point.transform.localPosition = new Vector3(x, y, z);

        }
    }

    private void Update()
    {
        RefreshPathPoint();
        RefreshBranchPoint();
        for (int i = 0; i < pointsCount - 1; i++)
        {
            Vector3 start = transform.GetChild(0).GetChild(i).position;
            Vector3 end = transform.GetChild(0).GetChild(i + 1).position;
            Debug.DrawLine(start, end, Color.red, 0.1f);
        }
    }
    [Button("铺设正前方向铁轨")]
    public void AddFM_RailPath() => BelongRailPathSystem.CreatRail(this, RailPutType.FM);
    [Button("铺设左前方向铁轨")]
    public void AddFL_RailPath() => BelongRailPathSystem.CreatRail(this, RailPutType.FL);
    [Button("铺设右前方向铁轨")]
    public void AddFR_RailPath() => BelongRailPathSystem.CreatRail(this, RailPutType.FR);
    [Button("铺设正后方向铁轨")]
    public void AddBM_RailPath() => BelongRailPathSystem.CreatRail(this, RailPutType.BM);
    [Button("铺设左后方向铁轨")]
    public void AddBL_RailPath() => BelongRailPathSystem.CreatRail(this, RailPutType.BL);
    [Button("铺设右后方向铁轨")]
    public void AddBR_RailPath() => BelongRailPathSystem.CreatRail(this, RailPutType.BR);

    [Button("移除铁轨")]
    public void RemoveRail() => BelongRailPathSystem.Remove(this);
}
