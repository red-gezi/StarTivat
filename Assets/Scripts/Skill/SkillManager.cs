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
    //��ǰ��ɫ����Ϊ����
    public static ActionData BasicAttackData { get; set; }
    public static ActionData SpecialSkillData { get; set; }

    //��ǰѡ����ж�����
    public ActionType currentActionType = ActionType.None;

    Vector2 smallSize = new Vector2(110, 110);
    Vector2 largeSize = new Vector2(160, 160);
    private void Awake() => Instance = this;
    private static string GetSkillTypeText(ActionData actionData)
    {
        return actionData.skillType switch
        {
            SkillType.SingleTarget => "����",
            SkillType.Diffusion => "��ɢ",
            SkillType.AreaOfEffect => "Ⱥ��",
            SkillType.Support => "����",
            SkillType.Hindrance => "����",
            SkillType.Healing => "�ظ�",
            _ => ""
        };
    }
    public static void ShowBasicAndSpecialSkill(ActionData basicAttackData, ActionData specialSkillData)
    {
        BasicAttackData = basicAttackData;
        SpecialSkillData = specialSkillData;
        //Instance.currentActionType = ActionType.BasicAttack;
        //������Ϊ��������Iconͼ�������
        Instance.BasicAttack.SetActive(true);
        Instance.BasicAttack.GetComponent<Image>().sprite = basicAttackData.icon;
        Instance.BasicAttack.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = GetSkillTypeText(basicAttackData);
        Instance.SpecialSkill.SetActive(true);
        Instance.SpecialSkill.GetComponent<Image>().sprite = specialSkillData.icon;
        Instance.SpecialSkill.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = GetSkillTypeText(specialSkillData);
        Instance.BrustSkill.SetActive(false);
        //����ѡ���չ�ģʽ
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
        //������ܲ��ɷ�������������
        if (true)
        {

            if (currentActionType == ActionType.BasicAttack)
            {
                //�����ǰ��ѡ��BasicAttack����ֱ�Ӵ�������
                await BasicAttackData.sender.BasicAttackAction();
            }
            else
            {
                //�����ǰδѡ��BasicAttack�����л���BasicAttack
                currentActionType = ActionType.BasicAttack;
                //����ѡ���
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
        //������ܲ��ɷ�������������
        if (true)
        {
            if (currentActionType == ActionType.SpecialSkill)
            {
                //�����ǰ��ѡ��SpecialSkill����ֱ�Ӵ�������
                await SpecialSkillData.sender.SpecialSkillAction();
            }
            else
            {
                //�����ǰ��ѡ��SpecialSkill�����л���SpecialSkill
                currentActionType = ActionType.SpecialSkill;
                //����ѡ���
                SelectManager.Show(SpecialSkillData);
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
        //������ܲ��ɷ�������������
        if (true)
        {
            if (currentActionType == ActionType.SpecialSkill)
            {
                //�����ǰ��ѡ��SpecialSkill����ֱ�Ӵ�������
                await SpecialSkillData.sender.BrustSkillAction();
            }
            else
            {
                //�����ǰ��ѡ��SpecialSkill�����л���SpecialSkill
                currentActionType = ActionType.Brust;
                //����ѡ���
                SelectManager.Show(SpecialSkillData);
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
    public static void Close()
    {
        Instance.BasicAttack.SetActive(false);
        Instance.SpecialSkill.SetActive(false);
        Instance.BrustSkill.SetActive(false);
    }
    //���Ŵ���ʱ����������
    public static void Hide()
    {

    }
}
