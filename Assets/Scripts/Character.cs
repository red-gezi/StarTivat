using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
class CharaAtcion
{
    ActionType actionType;

}
interface IChara
{
    void WaitForSelectSkill(List<CharaAtcion> charaAction)
    {
        //开启图标
        //设置摄像机位置和角度
        //设置选择目标类型
    }

}
internal class Character : IChara
{
    public int HealthPoints { get; set; } // 生命值
    public int Attack { get; set; } // 攻击力
    public int Defense { get; set; } // 防御力
    public int ElementalMastery { get; set; } // 元素精通
    public float ElementalDamageBonus { get; set; } // 元素伤害加成
    public float EnergyRecharge { get; set; } // 元素充能效率
    public float CriticalRate { get; set; } // 暴击率
    public float CriticalDamage { get; set; } // 暴击伤害
    public float HealingBonus { get; set; } // 治疗加成
    public float PhysicalDamageBonus { get; set; } // 属性伤害加成
    public float ElementalEnergy { get; set; } // 元素能量
    public string ElementalSkill { get; set; } // 元素战技
    public string ElementalBurst { get; set; } // 元素爆发

    ActionType BasicAttackType { get; set; }
    ActionType SpecialSkillType { get; set; }
    ActionType BrustSkillType { get; set; }


}
public enum SkillType
{
    BasicAttack,
    SpecialSkill,
    Brust,
    CounterAttack,
    AdditionalAttack,
    ExtraAttack
}
public enum ActionType
{
    SingleTarget,
    AreaOfEffect,
    Support,
    Hindrance,
    Healing
}
class Ability
{

}
public class CameraTrackController
{

}
class ActionBar
{
    List<CharaAction> charaActions = new();
    class CharaAction
    {
        //行动格主体
        Character character;
        bool isTemp = false;
        string name = "";
        int basicActionValue = 100;
        int currentActionValue = 150;
        int showActionValue = 100;
        Action BasicAction;
        List<Action> ExternAction;
        public void Run()
        {
            //如果有普通行动，执行普通行动
            //如果没有普通行动，执行额外回合行动

            //如果额外回合执行完普通
            //如果存在依附主体，行动者buff回合减一，并重置冷却
            //否则直接消除该回合

        }
        public void AddAction(SkillType skillType)
        {

        }
    }

    //新增一名角色
    public void AddChara(ActionType actionType)
    {

    }
    //追加行为
    //
    public void AddAction(ActionType actionType)
    {

    }
}