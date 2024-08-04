using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class SelectManager : MonoBehaviour
{
    static bool isSelectModeOpen;
    static ActionData currentActionData;
    public static List<Character> CurrentSelectTargets { get; set; } = new();
    public static Character CurrentSelectTarget => CurrentSelectTargets.FirstOrDefault();

    public static void Show(ActionData actionData)
    {
        isSelectModeOpen = true;
        currentActionData = actionData;
        CurrentSelectTargets = actionData.DefaultTargets;
        //关掉所有模型身上的大小框，
        RefreshLock();
    }

    private static void RefreshLock()
    {
        BattleManager.charaList.ForEach(chara => chara.largeLock.SetActive(false));
        BattleManager.charaList.ForEach(chara => chara.smallLock.SetActive(false));
        //启动目标模型的大框
        CurrentSelectTargets.ForEach(chara => chara.largeLock.SetActive(true));
        _ = ChangeLargeLockSize();
        //若果是扩散，启动两侧模型的小框
        if (currentActionData.CurrentSkillType == SkillType.Diffusion)
        {
            CurrentSelectTarget.Left?.smallLock.SetActive(true);
            CurrentSelectTarget.Right?.smallLock.SetActive(true);
            _ = ChangeSmallLockSize();
        }

        static async Task ChangeLargeLockSize()
        {
            for (int i = 10; i > 0; i--)
            {
                CurrentSelectTargets.ForEach(chara => chara.largeLock.transform.parent.localScale = Vector3.one * (i * 0.1f + 1));
                await Task.Delay(10);
            }
        }

        static async Task ChangeSmallLockSize()
        {
            for (int i = 10; i > 0; i--)
            {
                if (CurrentSelectTarget.Left != null)
                {
                    CurrentSelectTarget.Left.smallLock.transform.parent.localScale = Vector3.one * (i * 0.1f + 1);
                }
                if (CurrentSelectTarget.Right != null)
                {
                    CurrentSelectTarget.Right.smallLock.transform.parent.localScale = Vector3.one * (i * 0.1f + 1);
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
    public static void CharaClick(Character character)
    {
        if (isSelectModeOpen)
        {
            //如果目标已被选择
            if (CurrentSelectTargets.Contains(character))
            {
                //已被选择，则确认选择操作
                SkillManager.Instance.ConfirmAbilityAction();
            }
            else
            {
                if ((!character.IsEnemy ^ currentActionData.TargetIsEnemy))
                {
                    CurrentSelectTargets = new List<Character> { character };
                    RefreshLock();
                }
            }
        }
    }
}