using System;
using System.Collections.Generic;

public class TeamCharaData
{
    public PlayerName CharaNameType { get; set; }
    public Dictionary<string, string> ShowCharaName { get; set; } = new() { };
    private float healthPercentage = 1f;
    //角色稀有度
    public bool IsGold { get; set; }
    public float HealthPercentage
    {
        get => healthPercentage;
        set => healthPercentage = Math.Clamp(value, 0.01f, 1f);
    }
    public bool IsDead { get; set; } = false;


    public TeamCharaData(PlayerName charaNameType, string showCharaName)

    {
        CharaNameType = charaNameType;
        ShowCharaName["ch"] = showCharaName;
        IsGold = ((int)charaNameType)>=5000;
    }
}