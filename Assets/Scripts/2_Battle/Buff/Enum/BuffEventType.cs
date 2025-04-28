public enum BuffEventType
{
    //局外事件
    ItemSelect,//道具获得事件
    ItemGain,//道具获得事件
    ItemGainEffect,//道具获得生效事件
    GoldGain,
    GoldGainEffect,
    //流程事件
    TurnStart,
    TurnEnd,

    //获得角色数值
    GetCurrentCharaData,
    //发送技能数据
    SendSkillData,
    //接收技能数据
    ReceiveSkillData,
    ElementalReaction,
}
