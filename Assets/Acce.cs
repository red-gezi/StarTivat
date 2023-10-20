using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Acce : MonoBehaviour
{
    public GameObject cube;
    // Start is called before the first frame update
    List<Vector3> pos = new List<Vector3>();
    void Start()
    {
        //var gyroDataList = File.ReadAllLines("Assets/data.txt").Select(text => text.ToObject<GyroData>()).ToList();
        //var gyroDataProcessor = new GyroDataProcessor();
        //pos = gyroDataProcessor.ProcessGyroData(gyroDataList);

        //foreach (var position in positions)
        //{
        //    Debug.Log($"X: {position.x}, Y: {position.y}, Z: {position.z}");
        //}
    }
    string text;
    private void Update()
    {
        //for (int i = 0; i < pos.Count - 1; i++)
        //{
        //    Debug.DrawLine(pos[i], pos[i + 1]);
        //}

        string path = @"D:\VSProject\ROSCar\RosCarWeb\data.txt";
        using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            // 读取文本内容
            using (StreamReader reader = new StreamReader(stream))
            {
                text = reader.ReadToEnd();
                // 打印文本内容
                Console.WriteLine(text);
            }
        }
        if (text != "")
        {
            GyroData data = text.ToObject<GyroData>();
            Vector3 acc = new Vector3((float)data.AccX, (float)data.AccY, (float)data.AccZ);
            Vector3 angle = new Vector3((float)data.AngleX, (float)data.AngleY, (float)data.AngleZ);
            //cube.transform.position += acc;
            //cube.transform.eulerAngles = new Vector3(angle.x, angle.y, angle.z);
            cube.transform.eulerAngles = new Vector3(0, 0, 0);
            cube.transform.Rotate(Vector3.left, angle.x);
            cube.transform.Rotate(Vector3.back, angle.y);
            cube.transform.Rotate(Vector3.up, -angle.z);
            Debug.Log(new Vector3(angle.x, angle.y, angle.z));
        }



    }
    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 100, 50), "开始测量"))
        {
            Accelerometer.OpenPort();
        }
        if (GUI.Button(new Rect(0, 100, 100, 50), "重置位置"))
        {
            cube.transform.position = Vector3.zero;

        }
    }
}


public class GyroData
{
    public int Rank { get; set; }
    public double AccX { get; set; }
    public double AccY { get; set; }
    public double AccZ { get; set; }
    public double GyroX { get; set; }
    public double GyroY { get; set; }
    public double GyroZ { get; set; }
    public double AngleX { get; set; }
    public double AngleY { get; set; }
    public double AngleZ { get; set; }
    public DateTime SummaryTime { get; set; }
}

//public class GyroDataProcessor
//{
//    private double positionX = 0.0f;
//    private double positionY = 0.0f;
//    private double positionZ = 0.0f;

//    public List<Vector3> ProcessGyroData(List<GyroData> gyroDataList)
//    {
//        var positions = new List<Vector3>();

//        foreach (var gyroData in gyroDataList)
//        {
//            var deltaTime = 0.01f;

//            // 计算加速度在各轴上的变化
//            var accelerationX = gyroData.AccX;
//            var accelerationY = gyroData.AccY;
//            var accelerationZ = gyroData.AccZ;

//            // 计算速度变化
//            positionX += (float)(accelerationX * deltaTime * deltaTime);
//            positionY += (float)(accelerationY * deltaTime * deltaTime);
//            positionZ += (float)(accelerationZ * deltaTime * deltaTime);
//            // 更新上一个时间点
//            // 添加当前时刻的坐标
//            positions.Add(new Vector3((float)positionX, (float)positionY, (float)positionZ));
//        }
//        return positions;
//    }
//}