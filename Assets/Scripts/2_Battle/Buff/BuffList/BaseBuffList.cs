using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class BaseBuffList : IBaseBuffList
{
    public static BaseBuffList BuffList { get; set; }
    public static void Init() => BuffList = new();
    public Buff GetBuff(int bufferId) => Buffs.FirstOrDefault(buff => buff.id == bufferId).Clone();
    public enum BufferName
    {
        BaseBuff,
    }
    //buff总表
    public List<Buff> Buffs = new()
    {
        new Buff((int)BufferName.BaseBuff)
            .RegisterEvent<OutBattleEventData>( BuffTriggerType.On, BuffEventType.GoldGain, async eventData=>
            {
               //算出真实事件
                //打开ui
                //await Task.Delay(1000);
                OutBattleEventData outBattleEventData = ((OutBattleEventData)eventData);

                outBattleEventData.AddLog($"已获得金币{outBattleEventData.TargetValue}");
                OutBattleManager.CurrentOutBattleInfo.Gold += outBattleEventData.TargetValue;
            })
            .RegisterEvent<OutBattleEventData>( BuffTriggerType.On, BuffEventType.ItemGain, async eventData=>
            {
                //根据算出符合要求的真实的的buffid
                //将真正的目标buff
                //打开ui，显示获得的道具
                // 产生一个物品获得事件
                //角色获得道具
                await Task.Delay(1000);
                foreach (int index in eventData.TargetBuffIndex)
                {
                    Buff targetBuff = eventData.BelongBuffList.GetBuff(index);
                    OutBattleManager.CurrentOutBattleInfo.AddBuff(targetBuff);
                    eventData.TargetBuffs = new List<Buff> { targetBuff };
                    eventData.AddLog($"已获得道具{(MoNiYuZhouBuffList.BufferName)targetBuff.id},尝试触发道具的获得效果");
                    // 等待异步任务完成
                    await GameEventManager.TriggerTargetEventAsync(BuffEventType.ItemGainEffect, eventData);
                }
            })
            .RegisterEvent<OutBattleEventData>( BuffTriggerType.On, BuffEventType.ItemSelect, async eventData=>
            {
                    //根据算出符合要求的真实的的buffid
                //将真正的目标buff
                //打开ui，显示获得的道具
                // 产生一个物品获得事件
                //角色获得道具
                await Task.Delay(1000);
                foreach (int index in eventData.TargetBuffIndex)
                {
                    Buff targetBuff = eventData.BelongBuffList.GetBuff(index);
                    OutBattleManager.CurrentOutBattleInfo.AddBuff(targetBuff);
                    eventData.TargetBuffs = new List<Buff> { targetBuff };
                    eventData.AddLog($"已获得道具{(MoNiYuZhouBuffList.BufferName)targetBuff.id},尝试触发道具的获得效果");
                    // 等待异步任务完成
                    await GameEventManager.TriggerTargetEventAsync(BuffEventType.ItemGainEffect, eventData);
                }
            })
            .RegisterEvent<SkillData>( BuffTriggerType.On, BuffEventType.SendSkillData, async eventData=>
            {
                if (eventData.SkillTags.Contains(SkillTag.AreaOfEffect))
                {
                    //敌方全体接收
                    foreach (var chara in eventData.Receiver.SameCamp)
                    {
                        _ = GameEventManager.ReceiveSkillData(eventData.Clone().RedirectTarget(chara));
                    }
                }
                else if (eventData.SkillTags.Contains(SkillTag.AreaOfEffect))
                {

                    _ = GameEventManager.ReceiveSkillData(eventData.Clone());
                    _ = GameEventManager.ReceiveSkillData(eventData.Clone().RedirectTarget(eventData.Receiver.Left));
                    _ = GameEventManager.ReceiveSkillData(eventData.Clone().RedirectTarget(eventData.Receiver.Right));
                }
                else
                {
                    _ = GameEventManager.ReceiveSkillData(eventData.Clone());
                }
            })
            .RegisterEvent<SkillData>( BuffTriggerType.On, BuffEventType.ReceiveSkillData, async eventData=>
            {
               // //具体的接收规则,根据tag类型,计算伤害
               ////计算数值
               // int point=0;
               
               //     //判断是否起元素反应
               //     switch (eventData.SkillElement)
               //     {
               //         case ElementType.Anemo://风
               //             if (eventData.Receiver.HasElements(ElementType.Pyro))
               //             {
               //                _ = GameEventManager.ElementalReaction(new ElementalReactionData( (int)(point * 0.25f),false, ElementType.Pyro, eventData.TurnsRemaining,eventData.Target.Left, ReactionType.Disperse));
               //                await GameEventManager.ElementalReaction(new ElementalReactionData( (int)(point * 0.25f),false, ElementType.Pyro, eventData.TurnsRemaining,eventData.Target.Right, ReactionType.Disperse));
               //                 await CharaUiManager.CreatReactionText(model, ReactionType.Disperse);
               //             }
               //             else if (eventData.Receiver.HasElements(ElementType.Hydro))
               //             {
               //                 _ = Left?.OnCharaHit(false, ElementType.Hydro, timer, (int)(point * 0.25f));
               //                 await Right?.OnCharaHit(false, ElementType.Hydro, timer, (int)(point * 0.25f));
               //                 await CharaUiManager.CreatReactionText(model, ReactionType.Disperse);
               //             }
               //             else if (eventData.Receiver.HasElements(ElementType.Electro))
               //             {
               //                 Left?.OnCharaHit(false, ElementType.Electro, timer, (int)(point * 0.25f));
               //                 Right?.OnCharaHit(false, ElementType.Electro, timer, (int)(point * 0.25f));
               //                 await CharaUiManager.CreatReactionText(model, ReactionType.Disperse);
               //             }
               //             else if (eventData.Receiver.HasElements(ElementType.Cryo) || HasElements(ElementType.Frozen))
               //             {
               //                 Left?.OnCharaHit(false, ElementType.Cryo, timer, (int)(point * 0.25f));
               //                 Right?.OnCharaHit(false, ElementType.Cryo, timer, (int)(point * 0.25f));
               //                 await CharaUiManager.CreatReactionText(model, ReactionType.Disperse);
               //             }
               //             break;
               //         case ElementType.Pyro:
               //             //火-水 蒸发
               //             if (eventData.Receiver.HasElements(ElementType.Hydro))
               //             {
               //                 point = (int)(point * 1.5f);
               //                 await eventData.Receiver.AddElementsAcync(ElementType.Pyro, timer);

               //                 _ = eventData.Receiver.RemoveElementsAcync(ElementType.Pyro);
               //                 await eventData.Receiver.RemoveElementsAcync(ElementType.Hydro);
               //                 await CharaUiManager.CreatReactionText(model, ReactionType.Evaporation);
               //             }
               //             //火-雷 超载
               //             else if (HasElements(ElementType.Electro))
               //             {
               //                 await AddElementsAcync(ElementType.Pyro, timer);
               //                 await RemoveElementsAcync(ElementType.Electro);
               //                 await CharaUiManager.CreatReactionText(model, ReactionType.Overload);
               //                 this?.OnCharaHit(false, ElementType.Electro, timer, (int)(point * 0.25f), ReactionType.Overload);
               //                 Left?.OnCharaHit(false, ElementType.Electro, timer, (int)(point * 0.25f), ReactionType.Overload);
               //                 Right?.OnCharaHit(false, ElementType.Electro, timer, (int)(point * 0.25f), ReactionType.Overload);
               //             }
               //             //火-冰/冻 融化
               //             else if (HasElements(ElementType.Cryo) || HasElements(ElementType.Frozen))
               //             {
               //                 point = (int)(point * 2f);
               //                 await RemoveElementsAcync(ElementType.Cryo);
               //                 await RemoveElementsAcync(ElementType.Frozen);
               //                 await CharaUiManager.CreatReactionText(model, ReactionType.Melting);
               //             }
               //             //火-草 燃烧
               //             else if (HasElements(ElementType.Herb) || HasElements(ElementType.Stimulus))
               //             {
               //                 await AddElementsAcync(ElementType.Pyro, 2);
               //                 _ = RemoveElementsAcync(ElementType.Pyro);
               //                 await RemoveElementsAcync(ElementType.Herb);
               //                 await RemoveElementsAcync(ElementType.Stimulus);
               //                 await AddElementsAcync(ElementType.Burn, 2);
               //                 await CharaUiManager.CreatReactionText(model, ReactionType.Combustion);
               //             }
               //             else
               //             {
               //                 await AddElementsAcync(ElementType.Pyro, timer);
               //             }
               //             break;
               //         case ElementType.Hydro:
               //             break;
               //         case ElementType.Electro:
               //             //雷-火/燃 超载
               //             if (HasElements(ElementType.Pyro) || HasElements(ElementType.Burn))
               //             {

               //                 await AddElementsAcync(ElementType.Electro, timer);
               //                 _ = RemoveElementsAcync(ElementType.Pyro);
               //                 _ = RemoveElementsAcync(ElementType.Burn);
               //                 await RemoveElementsAcync(ElementType.Electro);
               //                 await CharaUiManager.CreatReactionText(model, ReactionType.Overload);
               //                 this?.OnCharaHit(false, ElementType.Electro, timer, (int)(point * 0.25f), ReactionType.Overload);
               //                 Left?.OnCharaHit(false, ElementType.Electro, timer, (int)(point * 0.25f), ReactionType.Overload);
               //                 Right?.OnCharaHit(false, ElementType.Electro, timer, (int)(point * 0.25f), ReactionType.Overload);
               //             }
               //             //雷-水 感电
               //             else if (HasElements(ElementType.Hydro))
               //             {
               //                 BattleManager.CurrentBattle.charaList
               //                      .Where(chara => chara.HasElements(ElementType.Hydro))
               //                      .ToList()
               //                      .ForEach(async chara => await chara.OnCharaHit(false, ElementType.Electro, timer, (int)(point * 0.25f), ReactionType.ElectricShock));
               //                 RemoveElementsAcync(ElementType.Hydro);
               //                 await CharaUiManager.CreatReactionText(model, ReactionType.Overload);
               //             }
               //             //雷-冰/冻 超导
               //             else if (HasElements(ElementType.Cryo) || HasElements(ElementType.Frozen))
               //             {
               //                 this?.OnCharaHit(false, ElementType.Electro, timer, (int)(point * 0.25f), ReactionType.SuperConductor);
               //                 Left?.OnCharaHit(false, ElementType.Electro, timer, (int)(point * 0.25f), ReactionType.SuperConductor);
               //                 Right?.OnCharaHit(false, ElementType.Electro, timer, (int)(point * 0.25f), ReactionType.SuperConductor);
               //                 RemoveElementsAcync(ElementType.Cryo);
               //                 RemoveElementsAcync(ElementType.Frozen);
               //                 await CharaUiManager.CreatReactionText(model, ReactionType.SuperConductor);
               //             }
               //             //雷-草 原激化
               //             else if (HasElements(ElementType.Herb))
               //             {
               //                 await AddElementsAcync(ElementType.Electro, timer);

               //                 _ = RemoveElementsAcync(ElementType.Electro);
               //                 await RemoveElementsAcync(ElementType.Herb);

               //                 await AddElementsAcync(ElementType.Stimulus, timer);
               //                 await CharaUiManager.CreatReactionText(model, ReactionType.OriginalActivation);
               //             }
               //             //雷-激 超激化
               //             else if (HasElements(ElementType.Stimulus))
               //             {
               //                 point = (int)(point * 1.5f);
               //                 await AddElementsAcync(ElementType.Electro, timer);
               //                 await RemoveElementsAcync(ElementType.Electro);
               //                 await AddElementsAcync(ElementType.Stimulus, timer);
               //                 await CharaUiManager.CreatReactionText(model, ReactionType.SuperActivation);
               //             }
               //             else
               //             {
               //                 await AddElementsAcync(ElementType.Electro, timer);
               //             }
               //             break;
               //         case ElementType.Cryo:
               //             if (HasElements(ElementType.Pyro))
               //             {

               //             }
               //             else if (HasElements(ElementType.Hydro))
               //             {

               //             }
               //             else if (HasElements(ElementType.Electro))
               //             {

               //             }
               //             else if (HasElements(ElementType.Cryo))
               //             {

               //             }
               //             else
               //             {
               //                 await AddElementsAcync(ElementType.Cryo, timer);
               //             }
               //             break;
               //         case ElementType.Geo:
               //             if (HasElements(ElementType.Pyro))
               //             {
               //                 await CharaUiManager.CreatReactionText(model, ReactionType.Crystallize);
               //             }
               //             else if (HasElements(ElementType.Hydro))
               //             {
               //                 await CharaUiManager.CreatReactionText(model, ReactionType.Crystallize);
               //             }
               //             else if (HasElements(ElementType.Electro))
               //             {
               //                 await CharaUiManager.CreatReactionText(model, ReactionType.Crystallize);
               //             }
               //             else if (HasElements(ElementType.Cryo) || HasElements(ElementType.Frozen))
               //             {
               //                 await CharaUiManager.CreatReactionText(model, ReactionType.Crystallize); ;
               //             }
               //             break;
               //         case ElementType.Herb:
               //             //草-火/燃 燃烧
               //             if (HasElements(ElementType.Pyro) || HasElements(ElementType.Burn))
               //             {
               //                 await RemoveElementsAcync(ElementType.Pyro);
               //                 await AddElementsAcync(ElementType.Burn, 2);
               //                 await CharaUiManager.CreatReactionText(model, ReactionType.Combustion);
               //             }
               //             //草-水 绽放
               //             else if (HasElements(ElementType.Hydro))
               //             {
               //                 await RemoveElementsAcync(ElementType.Hydro);
               //                 await CharaUiManager.CreatReactionText(model, ReactionType.Bloom);
               //                 //添加一个种子状态
               //             }
               //             //草-雷 原激化
               //             else if (HasElements(ElementType.Electro))
               //             {
               //                 await RemoveElementsAcync(ElementType.Electro);
               //                 await AddElementsAcync(ElementType.Stimulus, timer);
               //                 await CharaUiManager.CreatReactionText(model, ReactionType.OriginalActivation);
               //             }
               //             //草-激 蔓激化
               //             else if (HasElements(ElementType.Stimulus))
               //             {
               //                 point = (int)(point * 1.5f);
               //                 await AddElementsAcync(ElementType.Stimulus, timer);
               //                 await CharaUiManager.CreatReactionText(model, ReactionType.RapidActivation);
               //             }
               //             else
               //             {
               //                 await AddElementsAcync(ElementType.Herb, timer);
               //             }
               //             break;
               //         case ElementType.Physical:
               //             if (HasElements(ElementType.Frozen))
               //             {
               //                 point = (int)(point * 3f);
               //                 await RemoveElementsAcync(ElementType.Frozen);
               //                 await CharaUiManager.CreatReactionText(model, ReactionType.ShatteredIce);
               //             }
               //             break;
               //         case ElementType.Cure:
               //             break;
               //         case ElementType.Shield:
               //             break;
               //         default:
               //             break;
               //     }
               // switch (elementType)
               // {
               //     case ElementType.Cure:
               //         break;
               //     case ElementType.Shield:
               //         break;
               //     default:
               //         //判定防御减伤
               //         point = (int)(point * ((100 - CurrentCharaData.TotalDefenseBonus) * 0.01f));
               //         //判定护盾
               //         //跳盾量减少
               //         //判定血量
               //         break;
               // }
               // //跳数字
               // await CharaUiManager.CreatNumber(isCritical, model, elementType, point);
               // BroadcastManager.BroadcastEvent(BoardcastHitEvent, new CharaEvent());
               // //await Task.Delay(4000);


            })
            .RegisterEvent<ElementalReactionData>( BuffTriggerType.On, BuffEventType.ElementalReaction, async eventData=>
            {
                //如果是超载\超导或者感电反应衍生伤害，只结算伤害，不触发元素附着
                if (eventData.CurrentReactionType== ReactionType.Overload || eventData.CurrentReactionType == ReactionType.SuperConductor || eventData.CurrentReactionType == ReactionType.ElectricShock)
                {
                    await CharaUiManager.CreatReactionText(eventData.Target, eventData.CurrentReactionType);
                }
            })
    };
}