using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance;
    public GameObject BasicAttack;
    public GameObject SpecialSkill;
    public GameObject BrustSkill;
    public GameObject BrustEffect;
    public GameObject largeCharaPrefebs;

    //当前角色的行为数据
    public static ActionData BasicAttackData { get; set; }
    public static ActionData SpecialSkillData { get; set; }
    public static ActionData BrustSkillData { get; set; }

    //当前选择的行动类型
    public ActionType currentActionType = ActionType.None;
    //技能图标选中前后的尺寸
    Vector2 smallSize = new Vector2(110, 110);
    Vector2 largeSize = new Vector2(160, 160);
    private void Awake()
    {
        Instance = this;
        largeCharaPrefebs.SetActive(false);
    }

    private static string GetSkillTypeText(ActionData actionData) => actionData.CurrentSkillType switch
    {
        SkillType.SingleTarget => "单攻",
        SkillType.Diffusion => "扩散",
        SkillType.AreaOfEffect => "群攻",
        SkillType.Support => "辅助",
        SkillType.Hindrance => "妨碍",
        SkillType.Healing => "回复",
        _ => ""
    };
    public static void ShowBasicAndSpecialSkill(ActionData basicAttackData, ActionData specialSkillData)
    {
        BasicAttackData = basicAttackData;
        SpecialSkillData = specialSkillData;
        //Instance.currentActionType = ActionType.BasicAttack;
        //根据行为数据设置技能UIIcon图标和文字
        Instance.BasicAttack.SetActive(true);
        Instance.BasicAttack.GetComponent<Image>().sprite = basicAttackData.SkillIcon;
        Instance.BasicAttack.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = GetSkillTypeText(basicAttackData);
        Instance.SpecialSkill.SetActive(true);
        Instance.SpecialSkill.GetComponent<Image>().sprite = specialSkillData.SkillIcon;
        Instance.SpecialSkill.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = GetSkillTypeText(specialSkillData);
        Instance.BrustSkill.SetActive(false);
        //相机移动
        CameraTrackManager.SetAttackPose(BasicAttackData.Sender);
        //进入选择普攻模式
        Instance.SelectBasicAttack();
    }

    public static async void ShowBrustSkill(ActionData brustSkillData)
    {
        BrustSkillData = brustSkillData;
        Instance.currentActionType = ActionType.Brust;
        Instance.BasicAttack.SetActive(false);
        Instance.SpecialSkill.SetActive(false);
        Instance.BrustSkill.SetActive(true);
        Instance.BrustSkill.GetComponent<Image>().sprite = brustSkillData.SkillIcon;
        //人物动画切换
        //相机移动
        CameraTrackManager.SetBrustPose(BrustSkillData.Sender);
        //激活爆发特效
        Instance.BrustEffect.SetActive(true);
        Instance.BrustEffect.transform.position = BrustSkillData.Sender.transform.position;
        Instance.BrustEffect.transform.GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_Color", BrustSkillData.Sender.PlayerColor * 15);
        Instance.BrustEffect.transform.GetChild(1).GetComponent<MeshRenderer>().material.SetColor("_Color", BrustSkillData.Sender.PlayerColor * 15);
        //激活立绘界面，颜色从黑到高亮在变回正常
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
        //开启选择框
        SelectManager.Show(BrustSkillData);
    }
    //点击锁定的目标，确认执行操作
    public void ConfirmAbilityAction()
    {
        switch (currentActionType)
        {
            case ActionType.BasicAttack: SelectBasicAttack(); break;
            case ActionType.SpecialSkill: SelectSpecialSkill(); break;
            case ActionType.Brust: SelectBrustSkill(); break;
            default: Debug.LogError("异常行动指令，请检查"); break;
        }
    }
    public async void SelectBasicAttack()
    {
        //如果技能不可发动，择不做处理
        if (true)
        {
            //如果当前已选择BasicAttack，则直接触发攻击
            if (currentActionType == ActionType.BasicAttack)
            {
                //关闭技能UI
                Close();
                //关闭锁定框
                SelectManager.Close();
                Instance.currentActionType = ActionType.None;
                //设置回合状态为开始行动
                ActionBarManager.BasicActionStart();
                await BasicAttackData.Sender.AttackAction();
            }
            //如果当前未选择BasicAttack，则切换到BasicAttack
            else
            {
                currentActionType = ActionType.BasicAttack;

                //开启选择框
                SelectManager.Show(BasicAttackData);
                //初始化技能图标
                Instance.BasicAttack.transform.GetChild(0).gameObject.SetActive(true);
                Instance.SpecialSkill.transform.GetChild(0).gameObject.SetActive(false);
                //初始化技能点显示
                AbilityPointManager.PredictionChangePoint(BasicAttackData.SkillPointChange);
                await CustomThread.TimerAsync(0.1f, (progress) =>
                {
                    Instance.BasicAttack.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(smallSize, largeSize, progress);
                    Instance.SpecialSkill.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(largeSize, smallSize, progress);
                });
                //播放动作
                BasicAttackData.Sender.PlayAnimation(AnimationType.Attack_Pose);
                //行动条预测点数变动
                AbilityPointManager.PredictionChangePoint(BasicAttackData.SkillPointChange);
            }
        }
    }
    public async void SelectSpecialSkill()
    {
        //如果技能不可发动，择不做处理
        if (true)
        {
            //如果当前已选择SpecialSkill，则触发SpecialSkill
            if (currentActionType == ActionType.SpecialSkill)
            {
                //关闭技能选择
                Close();
                //关闭锁定框选择
                SelectManager.Close();
                //如果当前已选择SpecialSkill，则直接触发攻击
                Instance.currentActionType = ActionType.None;
                //设置回合状态为开始行动
                ActionBarManager.BasicActionStart();
                await SpecialSkillData.Sender.SkillAction();
            }
            //如果当前未选择SpecialSkill，则切换到SpecialSkill
            else
            {
                currentActionType = ActionType.SpecialSkill;
                //开启选择框
                SelectManager.Show(SpecialSkillData);
                Instance.BasicAttack.transform.GetChild(0).gameObject.SetActive(false);
                Instance.SpecialSkill.transform.GetChild(0).gameObject.SetActive(true);
                //初始化技能点显示
                AbilityPointManager.PredictionChangePoint(SpecialSkillData.SkillPointChange);
                //相机移动
                //技能图标变化
                await CustomThread.TimerAsync(0.1f, (progress) =>
                {
                    Instance.BasicAttack.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(largeSize, smallSize, progress);
                    Instance.SpecialSkill.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(smallSize, largeSize, progress);
                });
                //播放动作
                SpecialSkillData.Sender.PlayAnimation(AnimationType.Skill_Pose);
                AbilityPointManager.PredictionChangePoint(SpecialSkillData.SkillPointChange);
            }
        }
    }
    public async void SelectBrustSkill()
    {
        //关闭技能选择
        Close();
        //关闭锁定框选择
        SelectManager.Close();
        //关闭爆发特效
        Instance.BrustEffect.SetActive(false);
        //如果当前已选择SpecialSkill，则直接触发攻击
        Instance.currentActionType = ActionType.Brust;
        //直接触发元素爆发
        await SpecialSkillData.Sender.BrustAction();
        Instance.currentActionType = ActionType.None;
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
