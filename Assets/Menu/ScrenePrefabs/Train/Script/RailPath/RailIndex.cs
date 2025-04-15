
using UnityEngine;

public class RailIndex
{
    public RailIndex(int x, int y)
    {
        X = x;
        Y = y;
    }
    public int X { get; set; }
    public int Y { get; set; }
    public static RailIndex operator +(RailIndex first, RailIndex second) => new RailIndex(first.X + second.X, first.Y + second.Y);
    public static RailIndex operator -(RailIndex first, RailIndex second) => new RailIndex(first.X - second.X, first.Y - second.Y);
    public static RailIndex operator *(RailIndex first, int value) => new RailIndex(first.X * value, first.Y * value);
    public  Vector3 ToVector3() => new Vector3(X, 0, Y);
    // 重写GetHashCode方法，当Equals被重写时推荐也重写此方法
    public override int GetHashCode() => base.GetHashCode();
    // 重写Equals方法
    public override bool Equals(object obj)
    {
        if (obj is RailIndex other)
        {
            return this.X == other.X && this.Y == other.Y;
        }
        return false;
    }
    public static bool operator ==(RailIndex left, RailIndex right)
    {
        return left.Equals(right);
    }
    // 重载!=运算符
    public static bool operator !=(RailIndex left, RailIndex right)
    {
        return !left.Equals(right);
    }
}