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
    //�������������ϵͳ
    RailPathsSystemController BelongRailPathSystem => transform.parent.GetComponent<RailPathsSystemController>();
    public bool isCurve;//�Ƿ�����������
    public int pointsCount;
    public List<Vector3> Points { get; set; }
    public GameObject pointPrefab; // ����ʵ�����ĵ��Ԥ����

    //����������β������㳯��
    public Vector3 Direction => Points.Last() - Points.First();
    [ShowInInspector]
    public int x => Index.X;
    [ShowInInspector]
    public int y => Index.Y;
    //���쵱ǰ����
    public RailIndex Index=new(0,0);
    public float heigh;
    public RailIndex FIndex => Index + GetPlacementDirection(true);
    public RailIndex BIndex => Index - GetPlacementDirection(false);
    public Vector3 FPosition => transform.position + GetPlacementDirection(true).ToVector3() * 20;
    public Vector3 BPosition => transform.position - GetPlacementDirection(false).ToVector3() * 20;
    //���������������ӵ�����
    public List<RailIndex> ConnectIndexex => new List<RailIndex>() { FIndex, BIndex };

    //�����������ӵ�����
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
        //�ж�˫���Ƿ�����
        bool isConnect = newRail.Index == FIndex || newRail.Index == BIndex;
        if (!isConnect)
        {
            Debug.LogError("���ڿ鲻������������" + Index);
            return;
        }
        Debug.LogWarning("���ڿ������������" + Index);
        //�����������������Ĺ�ϵ
        if (!isCurve)//M ������������������
        {
            //������λ��������ǰ��
            if (newRail.FIndex == Index)
            {
                newRail.connectRails[RailPutType.FM] = this;
            }
            //������λ���������
            if (newRail.BIndex == Index)
            {
                newRail.connectRails[RailPutType.BM] = this;
            }
        }
        //������F������������ӣ���
        else if (newRail.Index == FIndex)
        {
            //������λ��������ǰ��
            if (newRail.FIndex == Index)
            {
                newRail.connectRails[RailPutType.FR] = this;
            }
            //������λ��������󷽷�
            if (newRail.BIndex == Index)
            {
                newRail.connectRails[RailPutType.BL] = this;
            }
        }
        else if (newRail.Index == BIndex)
        {
            //������λ��������ǰ��
            if (newRail.FIndex == Index)
            {
                newRail.connectRails[RailPutType.FL] = this;
            }
            //������λ��������󷽷�
            if (newRail.BIndex == Index)
            {
                newRail.connectRails[RailPutType.BR] = this;
            }
        }
        //������ͱ������ϵ
        if (!newRail.isCurve)//M �������Ǳ���������
        {
            //������λ�ڱ�����ǰ��
            if (newRail.Index == FIndex)
            {
                connectRails[RailPutType.FM] = newRail;
            }
            //������λ�ڱ������
            if (newRail.Index == BIndex)
            {
                connectRails[RailPutType.BM] = newRail;
            }
        }
        //������F������������ӣ���
        else if (newRail.Index == FIndex)
        {
            //����λǰ���뱾����ǰ������
            if (newRail.FIndex == Index)
            {
                connectRails[RailPutType.FR] = newRail;
            }
            //����λ���뱾����ǰ������
            if (newRail.BIndex == Index)
            {
                connectRails[RailPutType.FL] = newRail;
            }
        }
        //������B������������ӣ���
        else if (newRail.Index == BIndex)
        {
            //����λǰ���뱾����ǰ������
            if (newRail.FIndex == Index)
            {
                connectRails[RailPutType.BL] = newRail;
            }
            //����λ���뱾����ǰ������
            if (newRail.BIndex == Index)
            {
                connectRails[RailPutType.BR] = newRail;
            }
        }
    }
    //�ӹ��������ֵ����Ƴ�Ŀ������
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
        //���Ƕ�ӳ�䵽0~360��֮��
        angel = (angel + 360) % 360;
        return angel switch
        {
            0 => new(0, 1),
            90 => new(1, 0),
            180 => new(0, -1),
            270 => new(-1, 0),
            _ => throw new Exception("�Ƕȴ���"),
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
    //��ʼ��·����λ��
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
                float �� = s / r;
                x = Mathf.Cos(��) * r - r;
                y = 0;
                z = Mathf.Sin(��) * r - r;
            }
            // ʹ�����߼����θ߶�
            Vector3 rayOrigin = transform.position + new Vector3(x, 20, z); // ��һ���ϸߵ�λ�÷������ߣ�ȷ���ܻ��е���
            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit))
            {
                // ���߻����˵��λ�������ײ��
                y = hit.point.y - transform.position.y + 1; // ����y����Ϊ���θ߶ȼ���ƫ����
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
    //��ʼ����֧������λ��
    private void RefreshBranchPoint()
    {
        foreach (Transform point in transform.GetChild(2))
        {
            Vector3 targetPos = point.localPosition;
            // ʹ�����߼����θ߶�
            float x = targetPos.x;
            float z = targetPos.z;
            float y = 1;
            Vector3 rayOrigin = transform.position + new Vector3(targetPos.x, 20, targetPos.z); // ��һ���ϸߵ�λ�÷������ߣ�ȷ���ܻ��е���
            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit))
            {
                y = hit.point.y - transform.position.y + 1; // ����y����Ϊ���θ߶ȼ���ƫ����
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
    [Button("������ǰ��������")]
    public void AddFM_RailPath() => BelongRailPathSystem.CreatRail(this, RailPutType.FM);
    [Button("������ǰ��������")]
    public void AddFL_RailPath() => BelongRailPathSystem.CreatRail(this, RailPutType.FL);
    [Button("������ǰ��������")]
    public void AddFR_RailPath() => BelongRailPathSystem.CreatRail(this, RailPutType.FR);
    [Button("��������������")]
    public void AddBM_RailPath() => BelongRailPathSystem.CreatRail(this, RailPutType.BM);
    [Button("�������������")]
    public void AddBL_RailPath() => BelongRailPathSystem.CreatRail(this, RailPutType.BL);
    [Button("�����Һ�������")]
    public void AddBR_RailPath() => BelongRailPathSystem.CreatRail(this, RailPutType.BR);

    [Button("�Ƴ�����")]
    public void RemoveRail() => BelongRailPathSystem.Remove(this);
}
