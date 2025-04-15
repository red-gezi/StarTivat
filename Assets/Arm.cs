using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Arm : MonoBehaviour
{
    public GameObject target;
    public Transform joint1;
    public Transform joint2;
    float armLength1 = 10;
    float armLength2 = 10;
    // Update is called once per frame
    void Update()
    {
        CalculateArmAngle();
    }
    public void CalculateArmAngle()
    {
        Vector3 start = transform.position;
        Vector3 end = target.transform.position;
        if (Vector3.Distance(start, end) > 20)
        {
            Vector3 direction = end - start;
            Vector3 unitDirection = direction.normalized;
            Vector3 newEnd = start + 19.9f * unitDirection;
            end = newEnd;
        }

        var intersectionPoints = FindSphereIntersectionPoints(start, armLength1, end, armLength2);

        if (intersectionPoints != null)
        {
            Debug.Log("最高交点: " + intersectionPoints.Item1);
            Debug.Log("最低交点: " + intersectionPoints.Item2);
            DrawWireSphere(start,10);
            DrawWireSphere(end, 10);
            Debug.DrawLine(start, intersectionPoints.Item1);
            Debug.DrawLine(intersectionPoints.Item1, end);

        }
        else
        {
            Console.WriteLine("没有交点");
        }

    }
    void DrawWireSphere(Vector3 center, float radius)
    {
        int horizontalSegments = 30; // 水平分割数
        int verticalSegments = 15;   // 垂直分割数
        float horizontalAngleStep = (2 * Mathf.PI) / horizontalSegments;
        float verticalAngleStep = Mathf.PI / verticalSegments;

        for (int i = 0; i <= verticalSegments; i++)
        {
            float theta = i * verticalAngleStep;
            float sinTheta = Mathf.Sin(theta);
            float cosTheta = Mathf.Cos(theta);

            for (int j = 0; j <= horizontalSegments; j++)
            {
                float phi = j * horizontalAngleStep;
                Vector3 pointA = new Vector3(radius * sinTheta * Mathf.Cos(phi), radius * cosTheta, radius * sinTheta * Mathf.Sin(phi));

                if (j < horizontalSegments)
                {
                    Vector3 pointB = new Vector3(radius * sinTheta * Mathf.Cos(phi + horizontalAngleStep), radius * cosTheta, radius * sinTheta * Mathf.Sin(phi + horizontalAngleStep));
                    Debug.DrawLine(center + pointA, center + pointB, Color.red);
                }

                if (i < verticalSegments)
                {
                    Vector3 pointC = new Vector3(radius * Mathf.Sin(theta + verticalAngleStep) * Mathf.Cos(phi), radius * Mathf.Cos(theta + verticalAngleStep), radius * Mathf.Sin(theta + verticalAngleStep) * Mathf.Sin(phi));
                    Debug.DrawLine(center + pointA, center + pointC, Color.red);
                }
            }
        }
    }
    public static Tuple<Vector3, Vector3> FindSphereIntersectionPoints(Vector3 center1, float radius1, Vector3 center2, float radius2)
    {
        // 计算球心之间的距离
        float distance = Vector3.Distance(center1, center2);

        // 判断是否有交点
        if (Mathf.Abs(radius1 - radius2) >= distance || distance >= radius1 + radius2)
        {
            return null; // 无交点
        }

        // 计算交点所在的平面
        float a = (radius1 * radius1 - radius2 * radius2 + distance * distance) / (2 * distance);
        float h = Mathf.Sqrt(radius1 * radius1 - a * a);

        // 计算交点所在的点
        Vector3 pointOnLine = center1 + (center2 - center1).normalized * a;

        // 计算垂直于连线的向量
        Vector3 normal = Vector3.Cross(center2 - center1, Vector3.up);
        if (Vector3.Dot(normal, Vector3.up) == 0)
        {
            normal = Vector3.Cross(center2 - center1, Vector3.right);
        }
        normal.Normalize();

        // 计算垂直于连线且指向y轴方向的向量
        Vector3 yNormal = Vector3.Cross(normal, center2 - center1);
        yNormal.Normalize();

        // 计算两个交点
        Vector3 point1 = pointOnLine + yNormal * h;
        Vector3 point2 = pointOnLine - yNormal * h;

        // 比较y轴方向上的最高点和最低点
        if (point1.y > point2.y)
        {
            return Tuple.Create(point1, point2);
        }
        else
        {
            return Tuple.Create(point2, point1);
        }
    }
}
