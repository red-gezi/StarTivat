﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class SelectManager : MonoBehaviour
{
    static bool isSelectModeOpen;
    static ActionData currentActionData;
    public static List<Character> currentSelectTargets = new();
    public static Character currentSelectTarget => currentSelectTargets.FirstOrDefault();

    public static void Show(ActionData actionData)
    {
        isSelectModeOpen = true;
        currentActionData = actionData;
        currentSelectTargets = actionData.DefaultTargets;
        //关掉所有模型身上的大小框，
        RefreshLock();
    }

    private static async void RefreshLock()
    {
        BattleManager.charaList.ForEach(chara => chara.largeLock.SetActive(false));
        BattleManager.charaList.ForEach(chara => chara.smallLock.SetActive(false));
        //启动目标模型的大框
        currentSelectTargets.ForEach(chara => chara.largeLock.SetActive(true));
        _ = ChangeLargeLockSize();
        //若果是扩散，启动两侧模型的小框
        if (currentActionData.CurrentSkillType == SkillType.Diffusion)
        {
            currentSelectTarget.Left?.smallLock.SetActive(true);
            currentSelectTarget.Right?.smallLock.SetActive(true);
            _ = ChangeSmallLockSize();
        }

        static async Task ChangeLargeLockSize()
        {
            for (int i = 10; i > 0; i--)
            {
                currentSelectTargets.ForEach(chara => chara.largeLock.transform.parent.localScale = Vector3.one * (i * 0.1f + 1));
                await Task.Delay(10);
            }
        }

        static async Task ChangeSmallLockSize()
        {
            for (int i = 10; i > 0; i--)
            {
                if (currentSelectTarget.Left != null)
                {
                    currentSelectTarget.Left.smallLock.transform.parent.localScale = Vector3.one * (i * 0.1f + 1);
                }
                if (currentSelectTarget.Right != null)
                {
                    currentSelectTarget.Right.smallLock.transform.parent.localScale = Vector3.one * (i * 0.1f + 1);
                }
                await Task.Delay(10);
            }
        }
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
            if (currentSelectTargets.Contains(character))
            {
                //如果当前是战技模式，则直接触发战技
                switch (currentActionData.CurrentActionType)
                {
                    case ActionType.BasicAttack:
                        await currentActionData.Sender.BasicAttackAction();
                        break;
                    case ActionType.SpecialSkill:
                        await currentActionData.Sender.SpecialSkillAction();
                        break;
                    case ActionType.Brust:
                        await currentActionData.Sender.BrustSkillAction();
                        break;
                    default:
                        Debug.LogError("异常行动指令，请检查");
                        break;
                }
            }
            else
            {
                var s1 = currentActionData.IsTargetEnemy;
                var s3 = character.IsEnemy;
                var s2 = (!character.IsEnemy ^ currentActionData.IsTargetEnemy);
                if ((!character.IsEnemy ^ currentActionData.IsTargetEnemy))
                {
                    currentSelectTargets = new List<Character> { character };
                    RefreshLock();
                }
            }
        }
    }
}