//using System.Collections.Generic;
//using System.Threading.Tasks;
//using UnityEngine;
//enum Achievement_BuffName
//{
//    悲观开阔者,
//    乐观开阔者,
//    用剑干掉彦卿,
//    用枪干掉彦卿,
//    我朋友很多
//}
//public partial class Achievement_BuffList : BaseBuffList
//{
//    //public static SimulatedUniverseBuffList BuffList { get; set; }
//    //public Buff GetBuff(int bufferId) => Buffs.FirstOrDefault(buff => buff.id == bufferId).Clone();
//    //buff总表
//    public new static List<Buff> Buffs = new();
//    public new static void Init()
//    {
//        Buffs = new()
//        {
//            new Buff()
//                .RegisterName(Achievement_BuffName.悲观开阔者)
//                .RegisterEvent<RoomData>( BuffTriggerType.After, BuffEventType.对话, async eventData=>
//                {
//                   if ("台词标签为悲观") 计数+1;
//                   if (计数>5) 激活成就;
//                }),
//            new Buff()
//                .RegisterName(Achievement_BuffName.乐观开阔者)
//                .RegisterEvent<RoomData>( BuffTriggerType.After, BuffEventType.对话, async eventData=>
//                {
//                   if ("台词标签为乐观") 计数+1;
//                   if (计数>5) 激活成就;
//                }),
//            new Buff()
//                .RegisterName(Achievement_BuffName.用剑干掉彦卿)
//                .RegisterEvent<RoomData>( BuffTriggerType.After, BuffEventType.击败boss, async eventData=>
//                {
//                   if ("击杀者用剑")激活成就;
//                }),
//             new Buff()
//                .RegisterName(Achievement_BuffName.用枪干掉彦卿)
//                .RegisterEvent<RoomData>( BuffTriggerType.After, BuffEventType.击败boss, async eventData=>
//                {
//                    if ("击杀者用枪")激活成就;
//                }),
//              new Buff()
//                .RegisterName(Achievement_BuffName.我朋友很多)
//                .RegisterEvent<RoomData>( BuffTriggerType.After, BuffEventType.加好友, async eventData=>
//                {
//                   计数+1;
//                   if (计数>5) 激活成就;
//                }),
//        };
//    }
//}