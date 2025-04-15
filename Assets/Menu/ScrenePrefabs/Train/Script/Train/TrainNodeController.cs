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
    //��ǰ�������г�ϵͳ������
    public TrainSystemController trainSystemController;
    private Quaternion targetRotation; // Ŀ����ת��
    public float rotationDamping = 50f; // ��ת���ᣬֵԽ����תԽƽ��
    [Header("��ǰ������Ŀ���")]
    //��ǰ��������
    public RailController currentRailPath;
    //��ǰ������
    public int currentNodeIndex;
    [Header("��һ���ƶ�Ŀ���������Ŀ���")]
    //��ǰ��������
    public RailController nextRailPath;
    //��ǰ������
    public int nextNodeIndex;

    bool isForward;
    //��ʻ����
    public Vector3 drivingDirection => isForward ? Vector3.forward : Vector3.back;
    //������һ����
    public (RailController, int) FindNextRailAndPoint(bool isMoveReverse, string direction)
    {

        RailController targetRail = currentRailPath;
       int targetNodeIndex = currentNodeIndex;
        //��������ͬ���Ƿ����Լ���ǰ���ƶ������ж��ǳ������������ƶ����Ǹ����������ƶ�
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
        //���������ǰ���췶Χ����Ҫ�л����죬��Ϊ������ʱ����F����Ѱ��Ŀ��
        if (targetNodeIndex > currentRailPath.Points.Count)
        {
            //ǰ���Ļ�����ͬ���Ƿ����ж���һ�����죬
            //���˵Ļ�
            //���ݵ�ǰ�����ڵ�λ�õĲ�ͬ���жϴ����߼�
            switch (trainCarType)
            {
                case TrainCarType.Head:
                    //���ڵ���״̬
                    if (!isMoveReverse)
                    {
                        targetRail = GetTargetBranch(forwardBranches);
                    }
                    //�ڵ���״̬
                    else
                    {
                        //���ݴ���ʷ����·��������һ������
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
                    //���ڵ���״̬
                    if (!isMoveReverse)
                    {
                        //���ݴ���ʷ����·��������һ������
                        targetRail = forwardBranches.FirstOrDefault(branch => trainSystemController.AttachedRail.Contains(branch.Value)).Value;
                    }
                    //�ڵ���״̬
                    else
                    {
                        targetRail = GetTargetBranch(forwardBranches);
                    }
                    break;
                default: break;
            }
            //�������������г��ĳ����ж���ʼ��������ͷ��ʼ
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
            //��B����������
        }


        return (targetRail, targetNodeIndex);

        RailController GetTargetBranch(Dictionary<string, RailController> branches)
        {
            // ����ֱ�ӻ�ȡָ������ķ�֧
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
    //ǰ��
    public void Forward()
    {
        (nextRailPath, nextNodeIndex) = FindNextRailAndPoint(false,"M");
        //�ٶȻ�������������
        float currentSpeed = trainSystemController.speed;

        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = nextRailPath.Points[nextNodeIndex];
        Vector3 moveDirection = (targetPosition - currentPosition).normalized;

        // �ڸ���λ��ǰ��¼��ǰλ�õķ���
        Vector3 currentPositionDirection = (targetPosition - currentPosition).normalized;
        // ����λ��
        //transform.position += moveDirection * currentSpeed * Time.deltaTime;
        // ���º�����µķ���
        Vector3 newPositionDirection = (targetPosition - currentPosition).normalized;
        // ����ƶ�ǰ�����Ƿ��෴
        bool isDirectionOpposite = Vector3.Dot(currentPositionDirection, newPositionDirection) < 0;
        // ��������෴�����⵹��ʱ���У�
        if (isDirectionOpposite)
        {
            //���õ�ǰ��ΪĿ���
            // ������һ����
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
        // �û𳵱��ֳ���·���ķ������Ӳ�ֵЧ����ƽ����ת
        targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationDamping);
    }
    //����
    public void Back()
    {

    }
    //ֹͣ
    public void Stop()
    {
        //�ٶȻ�����Ϊ0
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
