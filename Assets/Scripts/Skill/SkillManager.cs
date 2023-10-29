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

    //当前选择的行动类型
    ActionType currentActionType = ActionType.None;

    Vector2 smallSize = new Vector2(110, 110);
    Vector2 largeSize = new Vector2(160, 160);
    private void Awake() => Instance = this;
    private static string GetSkillTypeText(ActionData actionData)
    {
        return actionData.skill switch
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
    public static void ShowBasicAndSpecialSkill(ActionData BasicAttackData, ActionData SpecialSkillData)
    {
        Instance.currentActionType = ActionType.BasicAttack;
        Instance.BasicAttack.SetActive(true);
        Instance.BasicAttack.GetComponent<Image>().sprite = BasicAttackData.icon;
        Instance.BasicAttack.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = GetSkillTypeText(BasicAttackData);
        Instance.SpecialSkill.SetActive(true);
        Instance.SpecialSkill.GetComponent<Image>().sprite = SpecialSkillData.icon;
        Instance.SpecialSkill.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = GetSkillTypeText(SpecialSkillData);
        Instance.BrustSkill.SetActive(false);
        Instance.SelectBasicAttack();
    }

    public static void ShowBrustSkill()
    {
        Instance.currentActionType = ActionType.Brust;
        Instance.BasicAttack.SetActive(false);
        Instance.SpecialSkill.SetActive(false);
        Instance.BrustSkill.SetActive(true);
    }
    public async void SelectBasicAttack()
    {
        //如果技能不可发动，择不做处理
        if (true)
        {

            if (currentActionType == ActionType.BasicAttack)
            {
                //如果当前已选择BasicAttack，则直接触发攻击

            }
            else
            {
                //如果当前未选择BasicAttack，则切换到BasicAttack
                currentActionType = ActionType.BasicAttack;
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
