using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
public class MoNiYuZhouBuffList : IBaseBuffList
{
    public static MoNiYuZhouBuffList BuffList { get; set; }
    public static void Init() => BuffList = new();
    public Buff GetBuff(int bufferId) => Buffs.FirstOrDefault(buff => buff.id == bufferId).Clone();
    public enum BufferName
    {
        基础流程,
        获得金币翻倍,
        每次获得奇物后获得30金币,
        获得1个金币翻倍奇物,
        获得奇物数量加一,
        空奇物,
        获得金钱时若金钱大于50则减50并获得欠条,
        欠条,
        //祝福
        祝福_琥珀,
        祝福_苹果酿,
        祝福_妙论派,
        //奇物
        奇物_相遇之缘,
        奇物_纠缠之缘,
        奇物_原石,

    }
    //buff总表
    public List<Buff> Buffs = new()
    {
        new Buff((int)BufferName.基础流程)
            .RegisterEvent<OutBattleEventData>( BuffTriggerType.On, BuffEventType.EnterRoom, async eventData=>
            {

            })
            .RegisterEvent<OutBattleEventData>( BuffTriggerType.On, BuffEventType.DestoryObject, async eventData=>
            {

            })
            .RegisterEvent<OutBattleEventData>( BuffTriggerType.On, BuffEventType.GoldGain, async eventData=>
            {
               //算出真实事件
                //打开ui
                //await Task.Delay(1000);
                eventData.AddLog($"已获得金币{eventData.TargetValue}");
                OutBattleManager.ChangeGold( eventData.TargetValue);
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
                    Buff targetBuff =GameManager.CurrentBuffList.GetBuff(index);
                    OutBattleManager.AddBuff(targetBuff);
                    eventData.TargetBuffs = new List<Buff> { targetBuff };
                    eventData.AddLog($"已获得道具{(BufferName)targetBuff.id},尝试触发道具的获得效果");
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
                    OutBattleManager.AddBuff(targetBuff);
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
            .RegisterEvent<SkillData>( BuffTriggerType.On, BuffEventType.ReceiveSkillData, async skillData=>
            {
                //具体的接收规则,根据tag类型,计算伤害
               //计算数值
                
                if (skillData.SkillTags.Contains( SkillTag.Healing))
                {
                    await GameEventManager.BroadcastCharaEvent( BuffEventType.Healing,skillData);
                }
                else if (skillData.SkillTags.Contains(SkillTag.Shield))
                {
                    await GameEventManager.BroadcastCharaEvent( BuffEventType.Shield,skillData);
                }
                else
                {
                    int point=0;
                    bool isCritical=false;
                    //判断是否起元素反应
                    switch (skillData.SkillElement)
                    {
                        case ElementType.Anemo://风
                            if (skillData.Receiver.HasElements(ElementType.Pyro))
                            {
                                _ = GameEventManager.ElementalReaction(new ElementalReactionData( (int)(point * 0.25f),false, ElementType.Pyro, skillData.TurnsRemaining,skillData.Receiver.Left, ReactionType.Disperse));
                                _ = GameEventManager.ElementalReaction(new ElementalReactionData( (int)(point * 0.25f),false, ElementType.Pyro, skillData.TurnsRemaining,skillData.Receiver.Right, ReactionType.Disperse));
                                _ = GameEventManager.ElementalReaction(new ElementalReactionData( (int)(point * 0.25f),false, ElementType.Pyro, skillData.TurnsRemaining,skillData.Receiver, ReactionType.Disperse));
                                await GameEventManager.BroadcastCharaEvent( BuffEventType.Hit,skillData);
                            }
                            else if (skillData.Receiver.HasElements(ElementType.Hydro))
                            {
                                _ = GameEventManager.ElementalReaction(new ElementalReactionData( (int)(point * 0.25f),false, ElementType.Hydro, skillData.TurnsRemaining,skillData.Receiver.Left, ReactionType.Disperse));
                                _ = GameEventManager.ElementalReaction(new ElementalReactionData( (int)(point * 0.25f),false, ElementType.Hydro, skillData.TurnsRemaining,skillData.Receiver.Right, ReactionType.Disperse));
                                _ = GameEventManager.ElementalReaction(new ElementalReactionData( (int)(point * 0.25f),false, ElementType.Hydro, skillData.TurnsRemaining,skillData.Receiver, ReactionType.Disperse));
                                await GameEventManager.BroadcastCharaEvent( BuffEventType.Hit,skillData);

                            }
                            else if (skillData.Receiver.HasElements(ElementType.Electro))
                            {
                                _ = GameEventManager.ElementalReaction(new ElementalReactionData( (int)(point * 0.25f),false, ElementType.Electro, skillData.TurnsRemaining,skillData.Receiver.Left, ReactionType.Disperse));
                                _ = GameEventManager.ElementalReaction(new ElementalReactionData( (int)(point * 0.25f),false, ElementType.Electro, skillData.TurnsRemaining,skillData.Receiver.Right, ReactionType.Disperse));
                                _ = GameEventManager.ElementalReaction(new ElementalReactionData( (int)(point * 0.25f),false, ElementType.Electro, skillData.TurnsRemaining,skillData.Receiver, ReactionType.Disperse));
                                await GameEventManager.BroadcastCharaEvent( BuffEventType.Hit,skillData);

                            }
                            else if (skillData.Receiver.HasElements(ElementType.Cryo) || skillData.Receiver.HasElements(ElementType.Frozen))
                            {
                                _ = GameEventManager.ElementalReaction(new ElementalReactionData( (int)(point * 0.25f),false, ElementType.Cryo, skillData.TurnsRemaining,skillData.Receiver.Left, ReactionType.Disperse));
                                _ = GameEventManager.ElementalReaction(new ElementalReactionData( (int)(point * 0.25f),false, ElementType.Cryo, skillData.TurnsRemaining,skillData.Receiver.Right, ReactionType.Disperse));
                                _ = GameEventManager.ElementalReaction(new ElementalReactionData( (int)(point * 0.25f),false, ElementType.Cryo, skillData.TurnsRemaining,skillData.Receiver, ReactionType.Disperse));
                                await GameEventManager.BroadcastCharaEvent( BuffEventType.Hit,skillData);

                            }
                            break;
                        case ElementType.Pyro:
                            //火-水 蒸发
                            if (skillData.Receiver.HasElements(ElementType.Hydro))
                            {
                                await skillData.Receiver.AddElementsAcync(ElementType.Pyro, skillData.TurnsRemaining);
                                _ = skillData.Receiver.RemoveElementsAcync(ElementType.Pyro);
                                await skillData.Receiver.RemoveElementsAcync(ElementType.Hydro);
                                await GameEventManager.ElementalReaction(new ElementalReactionData( (int)(point *1.5f),false, ElementType.Pyro, skillData.TurnsRemaining,skillData.Receiver, ReactionType.Evaporation));
                                await CharaUiManager.CreatReactionText(skillData.Receiver, ReactionType.Evaporation);
                            }
                            //火-雷 超载
                            else if (skillData.Receiver.HasElements(ElementType.Electro))
                            {
                                await skillData.Receiver.AddElementsAcync(ElementType.Pyro, skillData.TurnsRemaining);
                                await skillData.Receiver.RemoveElementsAcync(ElementType.Electro);
                                await CharaUiManager.CreatReactionText(skillData.Receiver, ReactionType.Overload);
                                _ = GameEventManager.ElementalReaction(new ElementalReactionData( (int)(point * 0.25f),false, ElementType.Electro, skillData.TurnsRemaining,skillData.Receiver.Left, ReactionType.Overload));
                                _ = GameEventManager.ElementalReaction(new ElementalReactionData( (int)(point * 0.25f),false, ElementType.Electro, skillData.TurnsRemaining,skillData.Receiver.Right, ReactionType.Overload));
                                _ = GameEventManager.ElementalReaction(new ElementalReactionData( (int)(point * 0.25f),false, ElementType.Electro, skillData.TurnsRemaining,skillData.Receiver, ReactionType.Overload));
                                await GameEventManager.BroadcastCharaEvent( BuffEventType.Hit,skillData);
                            }
                            //火-冰/冻 融化
                            else if (skillData.Receiver.HasElements(ElementType.Cryo) || skillData.Receiver.HasElements(ElementType.Frozen))
                            {
                                point = (int)(point * 2f);
                                await skillData.Receiver.RemoveElementsAcync(ElementType.Cryo);
                                await skillData.Receiver.RemoveElementsAcync(ElementType.Frozen);
                                await CharaUiManager.CreatReactionText(skillData.Receiver, ReactionType.Melting);
                            }
                            //火-草 燃烧
                            else if (skillData.Receiver.HasElements(ElementType.Herb) || skillData.Receiver.HasElements(ElementType.Stimulus))
                            {
                                await skillData.Receiver.AddElementsAcync(ElementType.Pyro, 2);
                                _ = skillData.Receiver.RemoveElementsAcync(ElementType.Pyro);
                                await skillData.Receiver.RemoveElementsAcync(ElementType.Herb);
                                await skillData.Receiver.RemoveElementsAcync(ElementType.Stimulus);
                                await skillData.Receiver.AddElementsAcync(ElementType.Burn, 2);
                                await CharaUiManager.CreatReactionText(skillData.Receiver, ReactionType.Combustion);
                            }
                            else
                            {
                                await skillData.Receiver.AddElementsAcync(ElementType.Pyro, skillData.TurnsRemaining);
                            }
                            break;
                        case ElementType.Hydro:
                            break;
                        //case ElementType.Electro:
                        //    //雷-火/燃 超载
                        //    if (skillData.Target.HasElements(ElementType.Pyro) || skillData.Target.HasElements(ElementType.Burn))
                        //    {

                        //        await skillData.Target.AddElementsAcync(ElementType.Electro, skillData.TurnsRemaining);
                        //        _ = skillData.Target.RemoveElementsAcync(ElementType.Pyro);
                        //        _ = skillData.Target.RemoveElementsAcync(ElementType.Burn);
                        //        await skillData.Target.RemoveElementsAcync(ElementType.Electro);
                        //        await CharaUiManager.CreatReactionText(skillData.Target, ReactionType.Overload);
                        //        this?.OnCharaHit(false, ElementType.Electro, skillData.TurnsRemaining, (int)(point * 0.25f), ReactionType.Overload);
                        //        Left?.OnCharaHit(false, ElementType.Electro, skillData.TurnsRemaining, (int)(point * 0.25f), ReactionType.Overload);
                        //        Right?.OnCharaHit(false, ElementType.Electro, skillData.TurnsRemaining, (int)(point * 0.25f), ReactionType.Overload);
                        //    }
                        //    //雷-水 感电
                        //    else if (skillData.Target.HasElements(ElementType.Hydro))
                        //    {
                        //        BattleManager.CurrentBattle.charaList
                        //             .Where(chara => chara.HasElements(ElementType.Hydro))
                        //             .ToList()
                        //             .ForEach(async chara => await chara.OnCharaHit(false, ElementType.Electro, skillData.TurnsRemaining, (int)(point * 0.25f), ReactionType.ElectricShock));
                        //        skillData.Target.RemoveElementsAcync(ElementType.Hydro);
                        //        await CharaUiManager.CreatReactionText(skillData.Target, ReactionType.Overload);
                        //    }
                        //    //雷-冰/冻 超导
                        //    else if (skillData.Target.HasElements(ElementType.Cryo) || skillData.Target.HasElements(ElementType.Frozen))
                        //    {
                        //        this?.OnCharaHit(false, ElementType.Electro, skillData.TurnsRemaining, (int)(point * 0.25f), ReactionType.SuperConductor);
                        //        Left?.OnCharaHit(false, ElementType.Electro, skillData.TurnsRemaining, (int)(point * 0.25f), ReactionType.SuperConductor);
                        //        Right?.OnCharaHit(false, ElementType.Electro, skillData.TurnsRemaining, (int)(point * 0.25f), ReactionType.SuperConductor);
                        //        skillData.Target.RemoveElementsAcync(ElementType.Cryo);
                        //        skillData.Target.RemoveElementsAcync(ElementType.Frozen);
                        //        await CharaUiManager.CreatReactionText(skillData.Target, ReactionType.SuperConductor);
                        //    }
                        //    //雷-草 原激化
                        //    else if (skillData.Target.HasElements(ElementType.Herb))
                        //    {
                        //        await skillData.Target.AddElementsAcync(ElementType.Electro, skillData.TurnsRemaining);

                        //        _ = skillData.Target.RemoveElementsAcync(ElementType.Electro);
                        //        await skillData.Target.RemoveElementsAcync(ElementType.Herb);

                        //        await skillData.Target.AddElementsAcync(ElementType.Stimulus, skillData.TurnsRemaining);
                        //        await CharaUiManager.CreatReactionText(skillData.Target, ReactionType.OriginalActivation);
                        //    }
                        //    //雷-激 超激化
                        //    else if (skillData.Target.HasElements(ElementType.Stimulus))
                        //    {
                        //        point = (int)(point * 1.5f);
                        //        await skillData.Target.AddElementsAcync(ElementType.Electro, skillData.TurnsRemaining);
                        //        await skillData.Target.RemoveElementsAcync(ElementType.Electro);
                        //        await skillData.Target.AddElementsAcync(ElementType.Stimulus, skillData.TurnsRemaining);
                        //        await CharaUiManager.CreatReactionText(skillData.Target, ReactionType.SuperActivation);
                        //    }
                        //    else
                        //    {
                        //        await skillData.Target.AddElementsAcync(ElementType.Electro, skillData.TurnsRemaining);
                        //    }
                        //    break;
                        //case ElementType.Cryo:
                        //    if (skillData.Target.HasElements(ElementType.Pyro))
                        //    {

                        //    }
                        //    else if (skillData.Target.HasElements(ElementType.Hydro))
                        //    {

                        //    }
                        //    else if (skillData.Target.HasElements(ElementType.Electro))
                        //    {

                        //    }
                        //    else if (skillData.Target.HasElements(ElementType.Cryo))
                        //    {

                        //    }
                        //    else
                        //    {
                        //        await skillData.Target.AddElementsAcync(ElementType.Cryo, skillData.TurnsRemaining);
                        //    }
                        //    break;
                        //case ElementType.Geo:
                        //    if (skillData.Target.HasElements(ElementType.Pyro))
                        //    {
                        //        await CharaUiManager.CreatReactionText(skillData.Target, ReactionType.Crystallize);
                        //    }
                        //    else if (skillData.Target.HasElements(ElementType.Hydro))
                        //    {
                        //        await CharaUiManager.CreatReactionText(skillData.Target, ReactionType.Crystallize);
                        //    }
                        //    else if (skillData.Target.HasElements(ElementType.Electro))
                        //    {
                        //        await CharaUiManager.CreatReactionText(skillData.Target, ReactionType.Crystallize);
                        //    }
                        //    else if (skillData.Target.HasElements(ElementType.Cryo) || skillData.Target.HasElements(ElementType.Frozen))
                        //    {
                        //        await CharaUiManager.CreatReactionText(skillData.Target, ReactionType.Crystallize); ;
                        //    }
                        //    break;
                        _ = GameEventManager.ElementalReaction(new ElementalReactionData( (int)(point * 0.25f),false, ElementType.Electro, skillData.TurnsRemaining,skillData.Receiver, ReactionType.Overload));
                                await GameEventManager.BroadcastCharaEvent( BuffEventType.Hit,skillData);
                        case ElementType.Herb:
                            //草-火/燃 燃烧
                            if (skillData.Receiver.HasElements(ElementType.Pyro) || skillData.Receiver.HasElements(ElementType.Burn))
                            {
                                await skillData.Receiver.RemoveElementsAcync(ElementType.Pyro);
                                await skillData.Receiver.AddElementsAcync(ElementType.Burn, 2);
                                _ = GameEventManager.ElementalReaction(new ElementalReactionData( (int)(point * 0.25f),false, ElementType.Electro, skillData.TurnsRemaining,skillData.Receiver, ReactionType.Combustion));
                            }
                            //草-水 绽放
                            else if (skillData.Receiver.HasElements(ElementType.Hydro))
                            {
                                await skillData.Receiver.RemoveElementsAcync(ElementType.Hydro);
                                _ = GameEventManager.ElementalReaction(new ElementalReactionData( (int)(point * 0.25f),false, ElementType.Electro, skillData.TurnsRemaining,skillData.Receiver, ReactionType.Bloom));
                                //添加一个种子状态
                            }
                            //草-雷 原激化
                            else if (skillData.Receiver.HasElements(ElementType.Electro))
                            {
                                await skillData.Receiver.RemoveElementsAcync(ElementType.Electro);
                                await skillData.Receiver.AddElementsAcync(ElementType.Stimulus, skillData.TurnsRemaining);
                                _ = GameEventManager.ElementalReaction(new ElementalReactionData( (int)(point * 1.4f),false, ElementType.Electro, skillData.TurnsRemaining,skillData.Receiver, ReactionType.OriginalActivation));
                            }
                            //草-激 蔓激化
                            else if (skillData.Receiver.HasElements(ElementType.Stimulus))
                            {
                                await skillData.Receiver.AddElementsAcync(ElementType.Stimulus, skillData.TurnsRemaining);
                                _ = GameEventManager.ElementalReaction(new ElementalReactionData( (int)(point * 1.4f),false, ElementType.Electro, skillData.TurnsRemaining,skillData.Receiver, ReactionType.RapidActivation));
                            }
                            else
                            {
                                await skillData.Receiver.AddElementsAcync(ElementType.Herb, skillData.TurnsRemaining);
                                await GameEventManager.BroadcastCharaEvent( BuffEventType.Hit,skillData);
                            }
                            break;
                        case ElementType.Physical:
                            if (skillData.Receiver.HasElements(ElementType.Frozen))
                            {
                                await skillData.Receiver.RemoveElementsAcync(ElementType.Frozen);
                                _ = GameEventManager.ElementalReaction(new ElementalReactionData( (int)(point * 2.5f),false, ElementType.Physical, skillData.TurnsRemaining,skillData.Receiver, ReactionType.ShatteredIce));
                                await GameEventManager.BroadcastCharaEvent( BuffEventType.Hit,skillData);
                            }
                            else
                            {
                                await GameEventManager.BroadcastCharaEvent( BuffEventType.Hit,skillData);
                            }
                            break;
                        default:
                            Debug.LogError("未定义元素");
                            break;
                    }
                }
            })
            .RegisterEvent<ElementalReactionData>( BuffTriggerType.On, BuffEventType.ElementalReaction, async eventData=>
            {
                //生成文字
                await CharaUiManager.CreatReactionText(eventData.Receiver, eventData.CurrentReactionType);
                //生成数字
                //await CharaUiManager.CreatNumber(eventData.isCritical, eventData.Target, eventData.SkillElement, point);
                //造成伤害
                ////如果是超载\超导或者感电反应衍生伤害，只结算伤害，不触发元素附着
                //if (eventData.CurrentReactionType== ReactionType.Overload || eventData.CurrentReactionType == ReactionType.SuperConductor || eventData.CurrentReactionType == ReactionType.ElectricShock)
                //{
                //    await CharaUiManager.CreatReactionText(eventData.Target, eventData.CurrentReactionType);
                //}
            })
            .RegisterEvent<SkillData>( BuffTriggerType.On, BuffEventType.Hit, async eventData=>
            {
                //await CharaUiManager.CreatNumber(eventData.isCritical, eventData.Target, eventData.SkillElement, point);

            })
            .RegisterEvent<SkillData>( BuffTriggerType.On, BuffEventType.Shield, async eventData=>
            {
                int  point=0;
                //判定防御减伤
                 point = (int)(point * ((100 - eventData.CurrentCharaData.TotalDefenseBonus) * 0.01f));
                //判定护盾
                //跳盾量减少
                //判定血量
            })
            .RegisterEvent<SkillData>( BuffTriggerType.On, BuffEventType.Buff, async eventData=>
            {

            })
            .RegisterEvent<SkillData>( BuffTriggerType.On, BuffEventType.DeBuff, async eventData=>
            {
                int  point= (int)(eventData.SkillHPMultiplier * eventData.CurrentCharaData.MaxHP);
                eventData.Receiver.CurrentCharaData.CurrentHealthPoints+=point;
            })
            .RegisterEvent<SkillData>( BuffTriggerType.On, BuffEventType.Healing, async eventData=>
            {
                int  point= (int)(eventData.SkillHPMultiplier * eventData.CurrentCharaData.MaxHP);
                eventData.Receiver.CurrentCharaData.CurrentHealthPoints+=point;
                //弹个ui
            }),
        new Buff((int)BufferName.获得金币翻倍){rank=1}
            .RegisterEvent<OutBattleEventData>( BuffTriggerType.Before, BuffEventType.GoldGain, async (data)=>
        {
            data.TargetValue*=2;
            data.AddLog("获得金币修改为"+data.TargetValue);
            //await Task.Delay(1000);
        }),

        new Buff((int)BufferName.每次获得奇物后获得30金币)
            .RegisterEvent <OutBattleEventData>( BuffTriggerType.After, BuffEventType.ItemGain,async (data)=>
        {
            await GameEventManager.GetGoldAsync(30);
            data.AddLog("获得了碎片" + 30);
        }),

        new Buff((int)BufferName.获得1个金币翻倍奇物)
            .RegisterEvent<OutBattleEventData>(BuffTriggerType.On,  BuffEventType.ItemGainEffect,async (data)=>
        {
            //await Task.Delay(2000);
            var targets =new List<int>
            {
                (int)MoNiYuZhouBuffList.BufferName.获得金币翻倍,
            };
            await GameEventManager.GetItem(MoNiYuZhouBuffList.BuffList,targets);
            data.AddLog("获得奇物——获得金币翻倍");
            //await Task.Delay(2000);
        }),

        new Buff((int)BufferName.获得奇物数量加一)
            .RegisterEvent<OutBattleEventData>(BuffTriggerType.Before,  BuffEventType.ItemGain,async (data)=>
        {
            data.AddLog("获得奇物数量加一");
        }),

        new Buff((int)BufferName.空奇物)
            .RegisterEvent<OutBattleEventData>(BuffTriggerType.Before,  BuffEventType.ItemGain,async (data)=>
        {
            data.AddLog("获得奇物——白板奇物");
        }),

        new Buff((int)BufferName.欠条)
            .RegisterEvent<OutBattleEventData>(BuffTriggerType.Before,  BuffEventType.ItemGain,async (data)=>
        {
            data.AddLog("获得奇物——欠条");
        }),

        new Buff((int)BufferName.获得金钱时若金钱大于50则减50并获得欠条)
            .RegisterEvent<OutBattleEventData>(BuffTriggerType.Before,  BuffEventType.GoldGain,async (data)=>
        {
            if (data.TargetValue>50)
            {
                data.TargetValue-=50;
                var targets =new List<int>
                {
                    (int)MoNiYuZhouBuffList.BufferName.欠条,
                };
                await GameEventManager.GetItem(MoNiYuZhouBuffList.BuffList,targets);
                data.AddLog("获得的金钱-50并得到欠条");
            }
        }),

        new Buff((int)BufferName.祝福_妙论派,ElementType.Herb,3, "妙论派","元素反应加成提高<color=yellow>15%</color>")
            .RegisterEvent<OutBattleEventData>(BuffTriggerType.On,  BuffEventType.ItemGain,async (data)=>
        {
        }),
        new Buff((int)BufferName.祝福_琥珀,ElementType.Geo,2, "琥珀","护盾自然破碎时产生产生共振，对对方全体造成<color=yellow>200%</color>盾量伤害")
            .RegisterEvent<OutBattleEventData>(BuffTriggerType.Before,  BuffEventType.ItemGain,async (data)=>
        {
        }),
        new Buff((int)BufferName.祝福_苹果酿,ElementType.Anemo,1, "苹果酿","速度提高<color=yellow>20%</color>")
            .RegisterEvent<OutBattleEventData>(BuffTriggerType.Before,  BuffEventType.ItemGain,async (data)=>
        {
        }),

        new Buff((int)BufferName.奇物_相遇之缘, CurioType.相遇之缘,2, "相遇之缘","能带来奇迹的等价交换之物")
            .RegisterEvent<OutBattleEventData>(BuffTriggerType.Before,  BuffEventType.ItemGain,async (data)=>
        {
        }),
        new Buff((int)BufferName.奇物_纠缠之缘, CurioType.纠缠之缘,3, "纠缠之缘","能带来没有任何作用的奇迹的等价交换之物")
            .RegisterEvent<OutBattleEventData>(BuffTriggerType.Before,  BuffEventType.ItemGain,async (data)=>
        {
        }),
        new Buff((int)BufferName.奇物_原石, CurioType.原石,1, "原石","财富的象征")
            .RegisterTag(BuffTag.微妙)
            .RegisterEvent<OutBattleEventData>(BuffTriggerType.Before,  BuffEventType.ItemGain,async (data)=>
        {
        }),
    };
}