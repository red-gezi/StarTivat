
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Wit.SDK.Modular.Sensor.Modular.DataProcessor.Constant;
using Wit.SDK.Modular.WitSensorApi.Modular.JY901;

public class Accelerometer
{
    static bool isSummary = false;
    static int summaryRank = 0;
    //推算的当前坐标
    double positionX = 0.0;
    double positionY = 0.0;
    double positionZ = 0.0;
    public static Queue<AccelerometerData> summaryDatas = new();
    public static AccelerometerData currentData = new();
    public void UpdatePosition(double accX, double accY, double accZ, double gyroX, double gyroY, double gyroZ, double deltaTime)
    {

    }
    private static JY901 accele { get; set; } = new JY901();
    public static void OpenPort()
    {
        string portName = "Com5";
        int baudrate = 921600;
        try
        {
            accele.OnRecord += JY901_OnRecord;
            if (accele.IsOpen())
            {
                return;
            }
            accele.SetPortName(portName);
            accele.SetBaudrate(baudrate);
            accele.Open();

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return;
        }
    }
    public static void ClosePort()
    {
        try
        {
            // 如果已经打开了设备就关闭设备   Turn off the device if it is already on
            if (accele.IsOpen())
            {
                accele.OnRecord -= JY901_OnRecord;
                //accele.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return;
        }
    }
    static Random rand=new Random();
    public static object[] GetSummaryData(string key)
    {
        object[] data = new object[100];
        try
        {
            var summaryDatasArray = summaryDatas.ToArray();
            for (int i = 0; i < data.Length; i++)
            {
                if (i < summaryDatas.Count)
                {
                    var summaryData = summaryDatasArray[i];
                    //data[i] = new { rank = i, accX = summaryData?.angleX };
                    data[i] = new { rank = i, accX = rand.Next() };
                }
                else
                {
                    data[i] = new { rank = i, accX = 0 };
                }
            }
        }
        catch { }

        return data;
    }
    public static void StartSummary()
    {
        File.WriteAllText("data.txt", "");
        summaryRank = 0;
        summaryDatas.Clear();
        isSummary = true;
    }
    public static void EndSummary() => isSummary = false;
    public static void OpenSummary()
    {
        ProcessStartInfo info = new ProcessStartInfo();
        info.FileName = "data.txt";
        info.UseShellExecute = true;
        Process.Start(info);
    }
    private static void JY901_OnRecord(JY901 acc)
    {
        try
        {
            currentData.accX = double.Parse(acc.GetDeviceData(WitSensorKey.AccX));
            currentData.accY = double.Parse(acc.GetDeviceData(WitSensorKey.AccY));
            currentData.accZ = double.Parse(acc.GetDeviceData(WitSensorKey.AccZ));

            currentData.gyroX = double.Parse(acc.GetDeviceData(WitSensorKey.AsX));
            currentData.gyroY = double.Parse(acc.GetDeviceData(WitSensorKey.AsY));
            currentData.gyroZ = double.Parse(acc.GetDeviceData(WitSensorKey.AsZ));

            currentData.angleX = double.Parse(acc.GetDeviceData(WitSensorKey.AngleX));
            currentData.angleY = double.Parse(acc.GetDeviceData(WitSensorKey.AngleY));
            currentData.angleZ = double.Parse(acc.GetDeviceData(WitSensorKey.AngleZ));

            currentData.summaryTime = DateTime.Now;
            //Console.WriteLine($"acc {accX} {accY} {accZ}");
            //Console.WriteLine($"gyro {gyroX} {gyroY} {gyroZ}");
            //Console.WriteLine($"angle {angleX} {angleY} {angleZ}");
            currentData.rank = summaryRank++;
            summaryDatas.Enqueue(currentData);
            if (summaryDatas.Count > 100)
            {
                summaryDatas.Dequeue();
            }
            if (isSummary)
            {
                //using (StreamWriter writer = new StreamWriter("data.txt", true))
                //{
                //    writer.WriteLine(currentData.ToJson());
                //}
                File.WriteAllText("data.txt", currentData.ToJson());
            }
        }
        catch (Exception)
        {


        }

    }
}