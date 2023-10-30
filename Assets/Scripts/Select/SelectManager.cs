using System.Collections.Generic;
using UnityEngine;

public class SelectManager : MonoBehaviour
{
    static bool isSelectModeOpen;
    static ActionData currentActionData;
    static List<Character> currentSelectTarget;

    public static void Show(ActionData actionData)
    {
        isSelectModeOpen = true;
        currentActionData = actionData;
        currentSelectTarget = actionData.DefaultTargets;
        //关掉所有模型身上的大小框，
        RefreshLock();
    }

    private static void RefreshLock()
    {
        BattleManager.charaList.ForEach(chara => chara.largeLock.SetActive(false));
        BattleManager.charaList.ForEach(chara => chara.smallLock.SetActive(false));
        //启动目标模型的大框
        currentSelectTarget.ForEach(chara => chara.largeLock.SetActive(true));
        //若果是扩散，启动两侧模型的小框
    }

    //结束选择模式
    public static void Close()
    {
        isSelectModeOpen = false;
        //关掉所有模型身上的大小框，
        //关掉所有模型身上的大小框，
        BattleManager.charaList.ForEach(chara => chara.largeLock.SetActive(false));
        BattleManager.charaList.ForEach(chara => chara.smallLock.SetActive(false));
    }
    public static async void CharaClick(Character character)
    {
        if (isSelectModeOpen)
        {
            //如果目标已被选择
            if (currentSelectTarget.Contains(character))
            {
                //如果当前是战技模式，则直接触发战技
                switch (currentActionData.actionType)
                {
                    case ActionType.BasicAttack:
                        await currentActionData.sender.BasicAttackAction();
                        break;
                    case ActionType.SpecialSkill:
                        await currentActionData.sender.SpecialSkillAction();
                        break;
                    case ActionType.Brust:
                        await currentActionData.sender.BrustSkillAction();
                        break;
                    default:
                        Debug.LogError("异常行动指令，请检查");
                        break;
                }
            }
            else
            {
                var s1 = currentActionData.isEnemyTarget;
                var s3 = character.IsEnemy;
                var s2 = (!character.IsEnemy ^ currentActionData.isEnemyTarget);
                if ((!character.IsEnemy ^ currentActionData.isEnemyTarget))
                {
                    currentSelectTarget = new List<Character> { character };
                    RefreshLock();
                }
            }
        }
    }
}