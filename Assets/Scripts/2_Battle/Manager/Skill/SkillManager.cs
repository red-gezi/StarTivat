using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
//������ͼ��Ĳ���
public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance;
    public GameObject BasicAttack;
    public GameObject SpecialSkill;
    public GameObject BrustSkill;
    public GameObject BrustEffect;
    public GameObject largeCharaPrefebs;

    //��ǰ��ɫ����Ϊ����
    public static SkillData BasicAttackData { get; set; }
    public static SkillData SpecialSkillData { get; set; }
    public static SkillData BrustSkillData { get; set; }

    //��ǰѡ����ж�����
    public ActionType currentActionType = ActionType.None;
    //����ͼ��ѡ��ǰ��ĳߴ�
    Vector2 smallSize = new Vector2(110, 110);
    Vector2 largeSize = new Vector2(160, 160);
    private void Awake()
    {
        Instance = this;
        largeCharaPrefebs.SetActive(false);
    }

    private static string GetSkillTypeText(SkillData actionData)
    {
        return actionData.SkillTags.Select(tag =>
        {
            return tag switch
            {
                SkillTag.SingleTarget => "����",
                SkillTag.Diffusion => "��ɢ",
                SkillTag.AreaOfEffect => "Ⱥ��",
                SkillTag.Support => "����",
                SkillTag.Hindrance => "����",
                SkillTag.Healing => "�ظ�",
                _ => ""
            };
        }).FirstOrDefault(tag => tag != "");
    }

    public static void ShowBasicAndSpecialSkill(SkillData basicAttackData, SkillData specialSkillData)
    {
        BasicAttackData = basicAttackData;
        SpecialSkillData = specialSkillData;
        //Instance.currentActionType = ActionType.BasicAttack;
        //������Ϊ�������ü���UIIconͼ�������
        Instance.BasicAttack.SetActive(true);
        Instance.BasicAttack.GetComponent<Image>().sprite = basicAttackData.SkillIcon;
        Instance.BasicAttack.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = GetSkillTypeText(basicAttackData);
        Instance.SpecialSkill.SetActive(true);
        Instance.SpecialSkill.GetComponent<Image>().sprite = specialSkillData.SkillIcon;
        Instance.SpecialSkill.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = GetSkillTypeText(specialSkillData);
        Instance.BrustSkill.SetActive(false);
        //����ƶ�
        CameraTrackManager.SetAttackPose(BasicAttackData.Sender);
        //����ѡ���չ�ģʽ
        Instance.SelectBasicAttack();
    }

    public static async void ShowBrustSkill(SkillData brustSkillData)
    {
        BrustSkillData = brustSkillData;
        Instance.currentActionType = ActionType.Brust;
        Instance.BasicAttack.SetActive(false);
        Instance.SpecialSkill.SetActive(false);
        Instance.BrustSkill.SetActive(true);
        Instance.BrustSkill.GetComponent<Image>().sprite = brustSkillData.SkillIcon;
        //���ﶯ���л�
        //����ƶ�
        CameraTrackManager.SetBrustPose(BrustSkillData.Sender);
        //�������Ч
        Instance.BrustEffect.SetActive(true);
        Instance.BrustEffect.transform.position = BrustSkillData.Sender.transform.position;
        Instance.BrustEffect.transform.GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_Color", BrustSkillData.Sender.CurrentCharaData.PlayerColor * 15);
        Instance.BrustEffect.transform.GetChild(1).GetComponent<MeshRenderer>().material.SetColor("_Color", BrustSkillData.Sender.CurrentCharaData.PlayerColor * 15);
        //����������棬��ɫ�Ӻڵ������ڱ������
        SoundEffectManager.Play("brust");
        var largeChara = Instance.largeCharaPrefebs.transform.GetChild(0).GetComponent<Image>();
        largeChara.sprite = brustSkillData.BrustCharaIcon;
        Instance.largeCharaPrefebs.SetActive(true);
        largeChara.material.SetFloat("_Light", 0);
        await CustomThread.TimerAsync(0.2f, (progress) =>
        {
            largeChara.transform.localScale = Vector3.one * (1 + 0.1f * (1 - progress));
            largeChara.material.SetFloat("_Light", Mathf.Lerp(0, 1.3f, progress));
        });
        await CustomThread.TimerAsync(0.3f, (progress) =>
        {
            largeChara.material.SetFloat("_Light", Mathf.Lerp(1.3f, 1, progress));
        });
        await Task.Delay(200);
        Instance.largeCharaPrefebs.SetActive(false);
        await Task.Delay(1000);
        //����ѡ���
        SelectManager.Show(BrustSkillData);
    }
    //���������Ŀ�꣬ȷ��ִ�в���
    public void ConfirmAbilityAction()
    {
        switch (currentActionType)
        {
            case ActionType.BasicAttack: SelectBasicAttack(); break;
            case ActionType.SpecialSkill: SelectSpecialSkill(); break;
            case ActionType.Brust: SelectBrustSkill(); break;
            default: Debug.LogError("�쳣�ж�ָ�����"); break;
        }
    }
    public async void SelectBasicAttack()
    {
        //������ܲ��ɷ�������������
        if (true)
        {
            //�����ǰ��ѡ��BasicAttack����ֱ�Ӵ�������
            if (currentActionType == ActionType.BasicAttack)
            {
                //�رռ���UI
                Close();
                //�ر�������
                SelectManager.Close();
                Instance.currentActionType = ActionType.None;
                //���ûغ�״̬Ϊ��ʼ�ж�
                ActionBarManager.BasicActionStart();
                //await BasicAttackData.Sender.AttackAction();
                await BasicAttackData.Sender.PlayerAbilitys.AttackAction();
            }
            //�����ǰδѡ��BasicAttack�����л���BasicAttack
            else
            {
                currentActionType = ActionType.BasicAttack;

                //����ѡ���
                SelectManager.Show(BasicAttackData);
                //��ʼ������ͼ��
                Instance.BasicAttack.transform.GetChild(0).gameObject.SetActive(true);
                Instance.SpecialSkill.transform.GetChild(0).gameObject.SetActive(false);
                //��ʼ�����ܵ���ʾ
                SkillPointManager.PredictionChangePoint(BasicAttackData.SkillPointChange);
                await CustomThread.TimerAsync(0.1f, (progress) =>
                {
                    Instance.BasicAttack.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(smallSize, largeSize, progress);
                    Instance.SpecialSkill.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(largeSize, smallSize, progress);
                });
                SoundEffectManager.Play("button");
                //���Ŷ���
                CameraTrackManager.SetAttackPose(BasicAttackData.Sender);
                BasicAttackData.Sender.PlayAnimation(AnimationType.Attack_Pose);
                BasicAttackData.Sender.OnSelectAttackPose();
                //�ж���Ԥ������䶯
                SkillPointManager.PredictionChangePoint(BasicAttackData.SkillPointChange);
            }
        }
    }
    public async void SelectSpecialSkill()
    {
        //������ܲ��ɷ�������������
        if (true)
        {
            //�����ǰ��ѡ��SpecialSkill���򴥷�SpecialSkill
            if (currentActionType == ActionType.SpecialSkill)
            {
                //�رռ���ѡ��
                Close();
                //�ر�������ѡ��
                SelectManager.Close();
                //�����ǰ��ѡ��SpecialSkill����ֱ�Ӵ�������
                Instance.currentActionType = ActionType.None;
                //���ûغ�״̬Ϊ��ʼ�ж�
                ActionBarManager.BasicActionStart();
                await SpecialSkillData.Sender.SkillAction();
            }
            //�����ǰδѡ��SpecialSkill�����л���SpecialSkill
            else
            {
                currentActionType = ActionType.SpecialSkill;
                //����ѡ���
                SelectManager.Show(SpecialSkillData);
                Instance.BasicAttack.transform.GetChild(0).gameObject.SetActive(false);
                Instance.SpecialSkill.transform.GetChild(0).gameObject.SetActive(true);
                //��ʼ�����ܵ���ʾ
                SkillPointManager.PredictionChangePoint(SpecialSkillData.SkillPointChange);
                //����ƶ�
                //����ͼ��仯
                await CustomThread.TimerAsync(0.1f, (progress) =>
                {
                    Instance.BasicAttack.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(largeSize, smallSize, progress);
                    Instance.SpecialSkill.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(smallSize, largeSize, progress);
                });
                SoundEffectManager.Play("button");
                //���Ŷ���
                CameraTrackManager.SetSkillPose(SpecialSkillData.Sender);
                SpecialSkillData.Sender.PlayAnimation(AnimationType.Skill_Pose);
                SkillPointManager.PredictionChangePoint(SpecialSkillData.SkillPointChange);
                SpecialSkillData.Sender.OnSelectSkillPose();
            }
        }
    }
    public async void SelectBrustSkill()
    {
        //�رռ���ѡ��
        Close();
        //�ر�������ѡ��
        SelectManager.Close();
        //�رձ�����Ч
        Instance.BrustEffect.SetActive(false);
        Instance.currentActionType = ActionType.Brust;
        //ֱ�Ӵ���Ԫ�ر���
        await SpecialSkillData.Sender.BrustAction();
        Instance.currentActionType = ActionType.None;
    }
    public async void ResetPose()
    {
        currentActionType = ActionType.BasicAttack;

        //����ѡ���
        SelectManager.Show(BasicAttackData);
        //��ʼ������ͼ��
        Instance.BasicAttack.transform.GetChild(0).gameObject.SetActive(true);
        Instance.SpecialSkill.transform.GetChild(0).gameObject.SetActive(false);
        //��ʼ�����ܵ���ʾ
        SkillPointManager.PredictionChangePoint(BasicAttackData.SkillPointChange);
        await CustomThread.TimerAsync(0.1f, (progress) =>
        {
            Instance.BasicAttack.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(smallSize, largeSize, progress);
            Instance.SpecialSkill.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(largeSize, smallSize, progress);
        });
        SoundEffectManager.Play("button");
        //���Ŷ���
        CameraTrackManager.SetAttackPose(BasicAttackData.Sender);
        BasicAttackData.Sender.PlayAnimation(AnimationType.Attack_Pose);
        BasicAttackData.Sender.OnSelectAttackPose();
        //�ж���Ԥ������䶯
        SkillPointManager.PredictionChangePoint(BasicAttackData.SkillPointChange);
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
