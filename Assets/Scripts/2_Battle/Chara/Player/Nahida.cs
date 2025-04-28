using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
class Nahida : Character
{
    #region 人物属性配置
    //初始赋值人物数据
    private void Awake()
    {
        CharacterInit();
        RegisterCharaData(new CharaData()
        {
            CharaName = "纳西妲",
            BaseAttack = 1000,
            BaseDefense = 0.5f,
            BaseCriticalDamage = 50,
            MaxElementalEnergy = 100,
            MaxActionPoint = 40,
            EnergyRecharge = 100,
            PlayerElement = ElementType.Herb
        });
        //注册作为玩家角色技能
        RegisterAttackAction(AttackAction);
        RegisterSkillAction(SkillAction);
        RegisterBurstAction(BrustAction);
        //注册作为敌人角色技能
        //添加角色自带buff
        RegisterBuff(new Buff(0)
            .RegisterEvent<BattleEventData>(BuffTriggerType.On, BuffEventType.TurnStart, async (data) =>
            {
                //回合开始,释放战技
            })
            .RegisterEvent<SkillData>(BuffTriggerType.After, BuffEventType.ReceiveSkillData, async (data) =>
            {
                //反击
            })
        );
    }
    #endregion
    #region 技能数据配置
    //配置技能的基础数据，如消耗/回复技能点数，类型，生效对象，镜头控制数据等
    public override SkillData BasicSkillData => new SkillData()
    {
        SkillNmae = "行相",
        SkillIcon = basicSkillIcon,
        SkillPointChange = 1,
        SkillTags = { SkillTag.SingleTarget, SkillTag.BasicAttack },
        DefaultTargets = BattleManager.CurrentBattle.charaList.Where(chara => chara.IsEnemy).Take(1).ToList(),
        TargetIsEnemy = true,
        Sender = this,
    };
    public override SkillData SpecialSkillData => new SkillData()
    {
        SkillNmae = "所闻遍计",
        SkillIcon = specialSkillIcon,
        SkillPointChange = -1,
        SkillTags = { SkillTag.AreaOfEffect, SkillTag.SpecialSkill },
        DefaultTargets = BattleManager.CurrentBattle.charaList.Where(chara => chara.IsEnemy).ToList(),
        TargetIsEnemy = true,
        Sender = this,
    };
    public override SkillData BrustSkillData => new SkillData()
    {
        SkillNmae = "心景幻成",
        SkillIcon = brustSkillIcon,
        BrustCharaIcon = largeCharaIcon,
        SkillPointChange = 0,
        SkillTags = { SkillTag.AreaOfEffect, SkillTag.Brust },
        DefaultTargets = BattleManager.CurrentBattle.charaList.Where(chara => chara.IsEnemy).Skip(2).Take(1).ToList(),
        TargetIsEnemy = true,
        Sender = this,
    };
    #endregion
    #region 招式演出
    [Header("人物普攻效果参数")]
    public GameObject attackPrefab;

    public float value;
    public float cubeScale;
    public GameObject skillPrefab;

    public override async Task AttackAction()
    {
        SkillPointManager.ChangePoint(BasicSkillData.SkillPointChange);
        CameraTrackManager.SetAttackPose(this);
        PlayAnimation(AnimationType.Attack);

        //根据玩家当前数值和技能数据生成一个数值快照
        BasicSkillData.CurrentCharaData = await GameEventManager.GetCurrentCharaData(this);
        await GameEventManager.SendSkillData(BasicSkillData);
        //_ = SendSkillData(BasicSkillData);
        //
        _ = CalculateHitPointsAsync(200, ElementType.Herb, 2, SelectManager.CurrentSelectTargets, 0.8f);
        //播放角色行为演出
        await AttackEffect(SelectManager.CurrentSelectTarget);
        await Task.Delay(1000);
        ActionBarManager.BasicActionCompleted();
    }

    public override async Task SkillAction()
    {
        SkillPointManager.ChangePoint(SpecialSkillData.SkillPointChange);
        CameraTrackManager.SetSkillPose(this);
        PlayAnimation(AnimationType.Skill);
        _ = CalculateHitPointsAsync(200, ElementType.Herb, 2, SelectManager.CurrentSelectTargets, 1);
        await SkillEffect(SelectManager.CurrentSelectTarget);
        await Task.Delay(1000);
        ActionBarManager.BasicActionCompleted();

    }

    public override async Task BrustAction()
    {
        SkillPointManager.ChangePoint(BrustSkillData.SkillPointChange);
        PlayAnimation(AnimationType.Skill_Pose);
        await Task.Delay(1000);
        ActionBarManager.ActiveActionCompleted();
    }
    //public override async Task EnemySkillAction()
    //{
    //    Debug.Log(name + "作为敌人进行攻击");
    //    await Task.Delay(1000);
    //    ActionBarManager.BasicActionCompleted();
    //}
    //攻击特效
    async Task AttackEffect(Character currentSelectTarget)
    {
        //初始化配置
        SkillNameManager.Show(AttackSkillName, IsEnemy);
        attackPrefab.SetActive(true);
        var basePrefab = attackPrefab.transform.GetChild(0);
        var showPrefab = attackPrefab.transform.GetChild(1);
        var ballPrefab = attackPrefab.transform.GetChild(2);
        var par = attackPrefab.transform.GetChild(3);
        Mesh bashMesh = basePrefab.GetComponent<MeshFilter>().mesh;
        Mesh showMesh = showPrefab.GetComponent<MeshFilter>().mesh;
        Vector3 biasPosition = currentSelectTarget.model.transform.position;
        biasPosition.y = cubeScale / 2f + 0.6f;
        biasPosition.y = cubeScale;
        Vector3[] originalVertices = bashMesh.vertices;
        showMesh.SetVertices(originalVertices);
        showPrefab.GetComponent<Renderer>().material.color = Color.white;
        ballPrefab.GetComponent<Renderer>().material.SetFloat("_Alpha", 0);

        attackPrefab.transform.position = biasPosition;
        ballPrefab.transform.position = biasPosition;
        await CustomThread.TimerAsync(0.4f, (progress) =>
        {
            showPrefab.transform.localScale = Vector3.one * cubeScale * progress;
            showPrefab.transform.position = biasPosition + Vector3.Lerp(-Vector3.one * cubeScale, Vector3.zero, progress);
        });
        //await Task.Delay(100);
        Vector3[] targetVertices = new Vector3[originalVertices.Length];
        await CustomThread.TimerAsync(0.2f, (progress) =>
        {
            for (int i = 0; i < originalVertices.Length; i++)
            {
                var scale = 1 + Mathf.Cos(Mathf.PI / 2 * bashMesh.vertices[i].z) * value * progress;
                targetVertices[i] = new Vector3(originalVertices[i].x * scale, originalVertices[i].y * scale, originalVertices[i].z);

            }
            showMesh.SetVertices(targetVertices);
        });
        await CustomThread.TimerAsync(0.1f, (progress) =>
        {
            for (int i = 0; i < originalVertices.Length; i++)
            {
                var scale = 1 + Mathf.Cos(Mathf.PI / 2 * bashMesh.vertices[i].z) * value * (1 - progress);
                targetVertices[i] = new Vector3(originalVertices[i].x * scale, originalVertices[i].y * scale, originalVertices[i].z);

            }
            showMesh.SetVertices(targetVertices);
        });
        await CustomThread.TimerAsync(0.1f, (progress) =>
        {
            showPrefab.transform.localScale = Vector3.one * cubeScale * Mathf.Lerp(1, 0, progress);
        });
        par.GetComponent<ParticleSystem>().Play();
        await CustomThread.TimerAsync(0.2f, (progress) =>
        {
            ballPrefab.transform.localScale = Vector3.one * cubeScale * Mathf.Lerp(2, 3, progress);
            ballPrefab.GetComponent<Renderer>().material.SetFloat("_Alpha", (progress * 0.5f));
        });
        await Task.Delay(100);
        await CustomThread.TimerAsync(0.2f, (progress) =>
        {
            ballPrefab.transform.localScale = Vector3.one * cubeScale * Mathf.Lerp(3, 5, progress);
            ballPrefab.GetComponent<Renderer>().material.SetFloat("_Alpha", (1 - progress) * 0.5f);
        });
        attackPrefab.SetActive(false);
    }
    async Task SkillEffect(Character currentSelectTarget)
    {
        //初始化配置
        SkillNameManager.Show(ElementalSkillName, IsEnemy);

    }
    #endregion
    #region 事件响应
    public override void OnAddState()
    {
        base.OnAddState();
    }
    public override void OnSelectAttackPose()
    {
        skillPrefab.SetActive(false);
    }
    public override void OnSelectSkillPose()
    {
        skillPrefab.SetActive(true);
        _ = CustomThread.TimerAsync(0.1f, progress =>
       {
           skillPrefab.transform.localScale = Vector3.one * progress;
       });
    }
    #endregion
}
