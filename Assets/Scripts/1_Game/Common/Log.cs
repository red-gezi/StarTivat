
using UnityEngine;

public class Log
{
    public static void Show(string text,int level=0)
    {
        switch (level)
        {
            case 0:Debug.Log(text); break;
            case 1:Debug.LogWarning(text); break;
            case 2:Debug.LogError(text); break;
            default:
                break;
        }
    }
}