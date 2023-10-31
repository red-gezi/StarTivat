using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance;
    public GameObject BasicAttack;
    public GameObject SpecialSkill;
    public GameObject BrustSkill;
    //当前角色的行为数据
    public static ActionData BasicAttackData { get; set; }
    public static ActionData SpecialSkillData { get; set; }

    //当前选择的行动类型
    public ActionType currentActionType = ActionType.None;

    Vector2 smallSize = new Vector2(110, 110);
    Vector2 largeSize = new Vector2(160, 160);
    private void Awake() => Instance = this;
    private static string GetSkillTypeText(ActionData actionData)
    {
        return actionData.CurrentSkillType switch
        {
            SkillType.SingleTarget => "单攻",
            SkillType.Diffusion => "扩散",
            SkillType.AreaOfEffect => "群攻",
            SkillType.Support => "辅助",
            SkillType.Hindrance => "妨碍",
            SkillType.Healing => "回复",
            _ => ""
        };
    }
    public static void ShowBasicAndSpecialSkill(ActionData basicAttackData, ActionData specialSkillData)
    {
        BasicAttackData = basicAttackData;
        SpecialSkillData = specialSkillData;
        //Instance.currentActionType = ActionType.BasicAttack;
        //根据行为数据设置Icon图标和文字
        Instance.BasicAttack.SetActive(true);
        Instance.BasicAttack.GetComponent<Image>().sprite = basicAttackData.Icon;
        Instance.BasicAttack.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = GetSkillTypeText(basicAttackData);
        Instance.SpecialSkill.SetActive(true);
        Instance.SpecialSkill.GetComponent<Image>().sprite = specialSkillData.Icon;
        Instance.SpecialSkill.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = GetSkillTypeText(specialSkillData);
        Instance.BrustSkill.SetActive(false);
        //进入选择普攻模式
        Instance.SelectBasicAttack();
    }

    public static void ShowBrustSkill()
    {
        Instance.currentActionType = ActionType.Brust;
        Instance.BasicAttack.SetActive(false);
        Instance.SpecialSkill.SetActive(false);
        Instance.BrustSkill.SetActive(true);
        Instance.SelectBrustSkill();
    }
    public async void SelectBasicAttack()
    {
        //如果技能不可发动，择不做处理
        if (true)
        {
            if (currentActionType == ActionType.BasicAttack)
            {
                //如果当前已选择BasicAttack，则直接触发攻击
                await BasicAttackData.Sender.BasicAttackAction();
            }
            else
            {
                //如果当前未选择BasicAttack，则切换到BasicAttack
                currentActionType = ActionType.BasicAttack;
                //开启选择框
                SelectManager.Show(BasicAttackData);
                //初始化技能图标
                Instance.BasicAttack.transform.GetChild(0).gameObject.SetActive(true);
                Instance.SpecialSkill.transform.GetChild(0).gameObject.SetActive(false);
                //初始化技能点显示
                AbilityPointManager.PredictionChangePoint(BasicAttackData.AbilityPointChange);
                //相机移动
                CameraTrackManager.SetDefalutCharaRank(BasicAttackData.Sender.Rank+1);

                for (int i = 0; i < 10; i++)
                {
                    Instance.BasicAttack.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(smallSize, largeSize, i * 0.1f);
                    Instance.SpecialSkill.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(largeSize, smallSize, i * 0.1f);
                    await Task.Delay(10);
                }
                //播放动作
                BasicAttackData.Sender.PlayAnimation(AnimationType.BasicAttack);
            }
        }
    }
    public async void SelectSpecialSkill()
    {
        //如果技能不可发动，择不做处理
        if (true)
        {
            if (currentActionType == ActionType.SpecialSkill)
            {
                //如果当前已选择SpecialSkill，则直接触发攻击
                await SpecialSkillData.Sender.SpecialSkillAction();
            }
            else
            {
                //如果当前已选择SpecialSkill，则切换到SpecialSkill
                currentActionType = ActionType.SpecialSkill;
                //开启选择框
                SelectManager.Show(SpecialSkillData);
                Instance.BasicAttack.transform.GetChild(0).gameObject.SetActive(false);
                Instance.SpecialSkill.transform.GetChild(0).gameObject.SetActive(true);
                //初始化技能点显示
                AbilityPointManager.PredictionChangePoint(BasicAttackData.AbilityPointChange);
                //相机移动
                for (int i = 0; i < 10; i++)
                {
                    Instance.BasicAttack.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(largeSize, smallSize, i * 0.1f);
                    Instance.SpecialSkill.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(smallSize, largeSize, i * 0.1f);
                    await Task.Delay(10);
                }
                //播放动作
                SpecialSkillData.Sender.PlayAnimation(AnimationType.SpecialAttack);
            }
        }
    }
    public async void SelectBrustSkill()
    {
        //如果技能不可发动，择不做处理
        if (true)
        {
            if (currentActionType == ActionType.BasicAttack)
            {
                //如果当前已选择SpecialSkill，则直接触发攻击
                await SpecialSkillData.Sender.BrustSkillAction();
            }
            else
            {
                //如果当前已选择SpecialSkill，则切换到SpecialSkill
                currentActionType = ActionType.Brust;
                //开启选择框
                SelectManager.Show(SpecialSkillData);
                Instance.BasicAttack.transform.GetChild(0).gameObject.SetActive(false);
                Instance.SpecialSkill.transform.GetChild(0).gameObject.SetActive(false);
                Instance.BrustSkill.transform.GetChild(0).gameObject.SetActive(true);
                for (int i = 0; i < 10; i++)
                {
                    Instance.BasicAttack.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(largeSize, smallSize, i * 0.1f);
                    Instance.SpecialSkill.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(smallSize, largeSize, i * 0.1f);
                    await Task.Delay(10);
                }
                //播放动作
                await BasicAttackData.Sender.SpecialSkillAction();
            }
        }
    }
    public static void Close()
    {
        Instance.BasicAttack.SetActive(false);
        Instance.SpecialSkill.SetActive(false);
        Instance.BrustSkill.SetActive(false);
    }
    //播放大招时向两侧隐藏
    public static void Hide()
    {

    }
}
