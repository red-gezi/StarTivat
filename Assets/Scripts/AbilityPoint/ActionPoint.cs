public class AbilityPoint
{
    static int maxPoint = 5;
    static int currentPoint = 0;
    public static void Init()
    {
        currentPoint = 3;
        RefreshUI();
    }
    public static void ChangePoint(int point)
    {
        currentPoint += point;
        RefreshUI();
    }
    public static void RefreshUI()
    {

    }
}