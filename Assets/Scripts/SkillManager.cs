using System.Collections;
using System.Collections.Generic;
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
        return actionData.skillType switch
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
        Instance.BasicAttack.GetComponent<Image>().sprite = basicAttackData.icon;
        Instance.BasicAttack.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = GetSkillTypeText(basicAttackData);
        Instance.SpecialSkill.SetActive(true);
        Instance.SpecialSkill.GetComponent<Image>().sprite = specialSkillData.icon;
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
                await BasicAttackData.sender.BasicAttackAction();
            }
            else
            {
                //如果当前未选择BasicAttack，则切换到BasicAttack
                currentActionType = ActionType.BasicAttack;
                //开启选择框
                SelectManager.Show(BasicAttackData);
                Instance.BasicAttack.transform.GetChild(0).gameObject.SetActive(true);
                Instance.SpecialSkill.transform.GetChild(0).gameObject.SetActive(false);
                var small = new Vector2(110, 110);
                var large = new Vector2(160, 160);
                for (int i = 0; i < 10; i++)
                {
                    Instance.BasicAttack.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(small, large, i * 0.1f);
                    Instance.SpecialSkill.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(large, small, i * 0.1f);
                    await Task.Delay(10);
                }
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
                await SpecialSkillData.sender.SpecialSkillAction();
            }
            else
            {
                //如果当前已选择SpecialSkill，则切换到SpecialSkill
                currentActionType = ActionType.SpecialSkill;
                Instance.BasicAttack.transform.GetChild(0).gameObject.SetActive(false);
                Instance.SpecialSkill.transform.GetChild(0).gameObject.SetActive(true);
                for (int i = 0; i < 10; i++)
                {
                    Instance.BasicAttack.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(largeSize, smallSize, i * 0.1f);
                    Instance.SpecialSkill.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(smallSize, largeSize, i * 0.1f);
                    await Task.Delay(10);
                }
            }
        }
    }
    public async void SelectBrustSkill()
    {
        //如果技能不可发动，择不做处理
        if (true)
        {
            if (currentActionType == ActionType.SpecialSkill)
            {
                //如果当前已选择SpecialSkill，则直接触发攻击
                await SpecialSkillData.sender.SpecialSkillAction();
            }
            else
            {
                //如果当前已选择SpecialSkill，则切换到SpecialSkill
                currentActionType = ActionType.SpecialSkill;
                Instance.BasicAttack.transform.GetChild(0).gameObject.SetActive(false);
                Instance.SpecialSkill.transform.GetChild(0).gameObject.SetActive(true);
                for (int i = 0; i < 10; i++)
                {
                    Instance.BasicAttack.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(largeSize, smallSize, i * 0.1f);
                    Instance.SpecialSkill.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(smallSize, largeSize, i * 0.1f);
                    await Task.Delay(10);
                }
            }
        }
    }
    public void Close()
    {

    }
    //播放大招时向两侧隐藏
    public void Hide()
    {

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
public class SelectManager:MonoBehaviour
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
        //BattleManager.Instance.charaList;
        //启动目标模型的大框
        //若果是扩散，启动两侧模型的小框
    }
    //结束选择模式
    public void Close()
    {
        isSelectModeOpen = false;
        //关掉所有模型身上的大小框，
    }
    public async void CharaClick(Character character)
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
                        await currentActionData.sender.SpecialSkillAction();
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
                if (character.enabled == currentActionData.isTargetBelongPlayer)
                {

                }
            }
        }
    }
}