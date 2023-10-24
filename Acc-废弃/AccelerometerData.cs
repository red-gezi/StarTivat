using System;

public class AccelerometerData
{
    public int rank;
    public DateTime summaryTime { get; set; }
    //加速度仪测量值
    public double accX = 0.0;
    public double accY = 0.0;
    public double accZ = 0.0;

    public double gyroX = 0.0;
    public double gyroY = 0.0;
    public double gyroZ = 0.0;

    public double angleX = 0.0;
    public double angleY = 0.0;
    public double angleZ = 0.0;
}