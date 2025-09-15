public enum AnimationType
{
    Idle,          //待机
    Attack_Pose,   //切换到普攻
    Skill_Pose,    //切换到战技
    Burst_Pose,    //触发大招
    Attack,        //普通攻击
    Skill,         //元素战技
    Burst,         //元素爆发
    LightHit,      //受到轻击
    HeavyHit,      //受到重击
    Controlled,    //被控制
    Defeated,      //倒下
    Revive,        //复活

    World_Attack,  //大世界攻击
    World_Skill,   //大世界技能
    Walk,
}

public enum VoiceType
{
    entrance,      //轮到人物操作
    Attack,   //切换到普功
    Skill,    //切换到战技
    Burst,         //元素爆发
    LightHit,      //受到轻击
    HeavyHit,      //受到重击
    Controlled,    //被控制
    Defeated,      //倒下
    Revive,        //复活

    World_Attack,  //大世界攻击
    World_Skill,   //大世界技能
    Walk,
}