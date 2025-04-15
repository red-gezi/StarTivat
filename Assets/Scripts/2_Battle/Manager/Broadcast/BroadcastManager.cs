//一个全局通报的广播事件
using System;
using UnityEngine;

class BroadcastManager
{
    //public static void BroadcastEvent(CharaEvent e, CharaEvent charaEvent)
    //{
    //   BattleManager.Instance.charaList.ForEach(chara => chara.BoardcastHitEvent(e));
    //}
    public static void BroadcastEvent(Action<CharaEvent> func, CharaEvent e)
    {
       BattleManager.CurrentBattle.charaList.ForEach(chara => chara.BoardcastHitEvent(e));
        if (func != null)
        {
            func.Invoke(e);
        }
    }
}
public class CharaEvent
{

}