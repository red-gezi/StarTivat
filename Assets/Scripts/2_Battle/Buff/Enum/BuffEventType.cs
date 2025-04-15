public enum BuffEventType
{
    //角色显示事件
    GetCurrentCharaData,
    //局外事件
    ItemSelect,//道具获得事件
    ItemGain,//道具获得事件
    ItemGainEffect,//道具获得生效事件
    GoldGain,
    GoldGainEffect,
    //局内事件
    SkillCast,
    TakeDamage,
    TurnStart,
    TurnEnd
}
