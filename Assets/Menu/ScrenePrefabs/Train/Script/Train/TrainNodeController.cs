using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public enum TrainCarType
{
    Head,
    Compartment,
    Tail,
}
public class TrainNodeController : MonoBehaviour
{
    TrainCarType trainCarType;
    TrainNodeController lastCompartment;
    TrainNodeController nextCompartment;
    //当前所属的列车系统管理器
    public TrainSystemController trainSystemController;
    private Quaternion targetRotation; // 目标旋转量
    public float rotationDamping = 50f; // 旋转阻尼，值越大旋转越平滑
    [Header("当前铁轨与目标点")]
    //当前所在铁轨
    public RailController currentRailPath;
    //当前索引点
    public int currentNodeIndex;
    [Header("下一个移动目标的铁轨与目标点")]
    //当前所在铁轨
    public RailController nextRailPath;
    //当前索引点
    public int nextNodeIndex;

    bool isForward;
    //行驶方向
    public Vector3 drivingDirection => isForward ? Vector3.forward : Vector3.back;
    //查找下一个点
    public (RailController, int) FindNextRailAndPoint(bool isMoveReverse, string direction)
    {

        RailController targetRail = currentRailPath;
       int targetNodeIndex = currentNodeIndex;
        //根据铁轨同向还是反向，以及当前的移动方向，判断是朝正索引方向移动还是负索引方向移动
        bool isSameDirection = Vector3.Dot(transform.forward, currentRailPath.Direction) > 0;
        if (isSameDirection ^ isMoveReverse)
        {
            targetNodeIndex++;
        }
        else
        {
            targetNodeIndex--;
        }
        Dictionary<string, RailController> forwardBranches = new()
            {
                { "M", currentRailPath.connectRails[RailPutType.FM] },
                { "L", currentRailPath.connectRails[RailPutType.FL] },
                { "R", currentRailPath.connectRails[RailPutType.FR] }
            };
        Dictionary<string, RailController> backwardBranches = new()
            {
                { "M", currentRailPath.connectRails[RailPutType.BM]},
                { "L", currentRailPath.connectRails[RailPutType.BL]},
                { "R", currentRailPath.connectRails[RailPutType.BR]}
            };
        //如果超出当前铁轨范围，需要切换铁轨，且为正方向时，向F集合寻找目标
        if (targetNodeIndex > currentRailPath.Points.Count)
        {
            //前进的话根据同向还是反向判定下一个铁轨，
            //后退的话
            //根据当前车辆节点位置的不同，判断处理逻辑
            switch (trainCarType)
            {
                case TrainCarType.Head:
                    //不在倒车状态
                    if (!isMoveReverse)
                    {
                        targetRail = GetTargetBranch(forwardBranches);
                    }
                    //在倒车状态
                    else
                    {
                        //根据从历史依附路径查找下一个铁轨
                        targetRail = forwardBranches.FirstOrDefault(branch => trainSystemController.AttachedRail.Contains(branch.Value)).Value;
                    }
                    break;
                case TrainCarType.Compartment:
                    targetRail = forwardBranches.FirstOrDefault(branch => trainSystemController.AttachedRail.Contains(branch.Value)).Value;
                    //if (!isMoveReverse)
                    //{
                    //    targetRail = forwardBranches.FirstOrDefault(branch => trainSystemController.AttachedRail.Contains(branch.Value)).Value;
                    //}
                    //else
                    //{
                    //    targetRail = backwardBranches.FirstOrDefault(branch => trainSystemController.AttachedRail.Contains(branch.Value)).Value;
                    //}
                    break;
                case TrainCarType.Tail:
                    //不在倒车状态
                    if (!isMoveReverse)
                    {
                        //根据从历史依附路径查找下一个铁轨
                        targetRail = forwardBranches.FirstOrDefault(branch => trainSystemController.AttachedRail.Contains(branch.Value)).Value;
                    }
                    //在倒车状态
                    else
                    {
                        targetRail = GetTargetBranch(forwardBranches);
                    }
                    break;
                default: break;
            }
            //根据新铁轨与列车的朝向判定初始索引从那头开始
            isSameDirection = Vector3.Dot(transform.forward, targetRail.Direction) > 0;
            if (isSameDirection)
            {
                targetNodeIndex = 0;
            }
            else
            {
                targetNodeIndex = targetRail.Points.Count - 1;
            }
        }
        if (targetNodeIndex < 0)
        {
            //往B侧铁轨找找
        }


        return (targetRail, targetNodeIndex);

        RailController GetTargetBranch(Dictionary<string, RailController> branches)
        {
            // 尝试直接获取指定方向的分支
            if (direction == "M")
            {
                if (branches.TryGetValue("M", out targetRail) && targetRail != null) return targetRail;
                if (branches.TryGetValue("L", out targetRail) && targetRail != null) return targetRail;
                if (branches.TryGetValue("R", out targetRail) && targetRail != null) return targetRail;
            }
            if (direction == "L")
            {
                if (branches.TryGetValue("L", out targetRail) && targetRail != null) return targetRail;
                if (branches.TryGetValue("M", out targetRail) && targetRail != null) return targetRail;
                if (branches.TryGetValue("R", out targetRail) && targetRail != null) return targetRail;
            }
            if (direction == "R")
            {
                if (branches.TryGetValue("R", out targetRail) && targetRail != null) return targetRail;
                if (branches.TryGetValue("M", out targetRail) && targetRail != null) return targetRail;
                if (branches.TryGetValue("L", out targetRail) && targetRail != null) return targetRail;
            }
            return null;
        }
    }
    //前进
    public void Forward()
    {
        (nextRailPath, nextNodeIndex) = FindNextRailAndPoint(false,"M");
        //速度缓慢增加至上限
        float currentSpeed = trainSystemController.speed;

        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = nextRailPath.Points[nextNodeIndex];
        Vector3 moveDirection = (targetPosition - currentPosition).normalized;

        // 在更新位置前记录当前位置的方向
        Vector3 currentPositionDirection = (targetPosition - currentPosition).normalized;
        // 更新位置
        //transform.position += moveDirection * currentSpeed * Time.deltaTime;
        // 更新后计算新的方向
        Vector3 newPositionDirection = (targetPosition - currentPosition).normalized;
        // 检查移动前后方向是否相反
        bool isDirectionOpposite = Vector3.Dot(currentPositionDirection, newPositionDirection) < 0;
        // 如果方向相反（避免倒车时误判）
        if (isDirectionOpposite)
        {
            //设置当前点为目标点
            // 查找下一个点
            ( currentRailPath,  currentNodeIndex) = FindNextRailAndPoint(false, "M");
            //currentRailIndex++;
            //if (currentRailIndex == 100)
            //{
            //    historyPath.Enqueue(historyPath.Last().GetNextRailPath(isMovingForward, direction));
            //    currentRailIndex = 100;
            //}
            //currentRailIndex = (currentRailIndex + 1) % railPath.childCount;
            //targetPosition = railPath.GetChild(0).GetChild(currentRailIndex).position;
            moveDirection = (targetPosition - currentPosition).normalized;
        }
        // 让火车保持朝向路径的方向，增加插值效果以平滑旋转
        targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationDamping);
    }
    //后退
    public void Back()
    {

    }
    //停止
    public void Stop()
    {
        //速度缓慢变为0
    }
    // Update is called once per frame
    void Update()
    {
        if (currentRailPath != null)
        {
            Debug.DrawLine(transform.position, currentRailPath.Points[currentNodeIndex]);
        }
        if (nextRailPath != null)
        {
            Debug.DrawLine(transform.position, nextRailPath.Points[nextNodeIndex], Color.red);
        }
    }
}
