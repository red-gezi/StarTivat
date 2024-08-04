using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public abstract class Character : MonoBehaviour
{
    [HideInInspector]
    //角色类对应的模型
    public GameObject model;
    //////////////////////////////////////////////////角色图标////////////////////////////////////////////////////////////////////////////
    [Header("元素附着组件")]
    public Transform ElementalCanves;
    //锁定图标
    [Header("锁定图标")]
    public GameObject largeLock;
    public GameObject smallLock;

    //技能图标
    [Header("技能图标")]
    public Sprite basicAttackIcon;
    public Sprite specialSkillIcon;
    public Sprite brustSkillIcon;
    //在行动条与角色面板上的立绘
    public Sprite miniCharaIcon;
    //元素爆发时展现的立绘
    public Sprite largeCharaIcon;

    protected string ElementalSkillName { get; set; } // 元素战技名
    protected string ElementalBurstName { get; set; } // 元素爆发名

    //人物的能力点数
    public int maxAbilityPoint = 0;
    public int currentAbilityPoint = 0;
    public int abilityColor = 0;
    //////////////////////////////////////////////////摄像机点位////////////////////////////////////////////////////////////////////////////
    Transform playerPointGroup => transform.Find("PlayerPointGroup");
    Transform enemyPointGroup => transform.Find("EnemyPointGroup");
    public Transform Idle_Pose => playerPointGroup.GetChild(0);
    public Transform Idle_Show => playerPointGroup.GetChild(1);
    public Transform Attack_Pose => playerPointGroup.GetChild(2);
    public Transform Attack_Start => playerPointGroup.GetChild(3);
    public Transform Attack_End => playerPointGroup.GetChild(4);
    public Transform Skill_Pose => playerPointGroup.GetChild(5);
    public Transform Skill_Start => playerPointGroup.GetChild(6);
    public Transform Skill_End => playerPointGroup.GetChild(7);
    public Transform Brust_Pose => playerPointGroup.GetChild(8);
    public Transform Brust_Start => playerPointGroup.GetChild(9);
    public Transform Brust_End => playerPointGroup.GetChild(10);
    //整个角色正前方
    public Vector3 Chara_Forward => playerPointGroup.GetChild(11).position;
    //整个团队正前方（动态计算）
    public Vector3 Team_Forward;
    //作为敌人被攻击时的视角点位
    public Vector3 Enemy_Attacked;


    //////////////////////////////////////////////////角色相关信息////////////////////////////////////////////////////////////////////////////
    public bool IsEnemy { get; set; }//是否是敌人
    Dictionary<ElementType, int> AttachmentElements { get; set; } = new();

    //角色身上状态
    List<PlayerStateType> charaStates = new List<PlayerStateType>();
    List<PlayerStateType> enemyStates = new List<PlayerStateType>();
    //角色在自身排位置
    public int Rank => BattleManager.charaList.Where(chara => chara.IsEnemy == IsEnemy).ToList().IndexOf(this);
    //角色左侧的角色，可能为null
    public Character Left => BattleManager.charaList.FirstOrDefault(chara => chara.IsEnemy == IsEnemy && chara.Rank == Rank - 1);
    //角色右侧的角色，可能为null
    public Character Right => BattleManager.charaList.FirstOrDefault(chara => chara.IsEnemy == IsEnemy && chara.Rank == Rank + 1);
    //动画控制器
    public Animator animator => GetComponent<Animator>();
    //声音控制器
    public AudioSource audioSource => GetComponent<AudioSource>();
    //点击模型触发选中效果
    private void OnMouseDown() => SelectManager.CharaClick(this);

    //////////////////////////////////////////////////角色基础属性////////////////////////////////////////////////////////////////////////////
    public int MaxHealthPoints { get; set; } // 生命值上限
    public int CurrentHealthPoints { get; set; } // 当前生命值
    public int BaseAttack { get; set; } // 基础攻击力
    public float BaseDefense { get; set; } // 基础减伤比例
    public int CurrentAttack { get; set; } // 当前攻击力
    public float CurrentDefense { get; set; } // 当前减伤比例
    public int MaxActionPoint { get; set; } //行动力基准值
    public float ElementalMastery { get; set; } // 元素反应加成
    public float ElementalDamageBonus { get; set; } // 元素伤害加成
    public float EnergyRecharge { get; set; } // 元素充能效率
    public float CriticalRate { get; set; } = 50f; // 暴击率
    public float CriticalDamage { get; set; } // 暴击伤害
    public float HealingBonus { get; set; } // 治疗加成
    public float MaxElementalEnergy { get; set; } // 元素能量上限
    public float CurrentElementalEnergy { get; set; } // 当前元素能量

    public ElementType PlayerElement { get; set; }
    public Color PlayerColor => PlayerElement switch
    {
        ElementType.Anemo => new Color(0, 1, 0.4f),
        ElementType.Pyro => new Color(0.9f, 0.25f, 0),
        ElementType.Hydro => new Color(0f, 0.35f, 0.9f),
        ElementType.Electro => new Color(0.3f, 0, 0.75f),
        ElementType.Cryo => throw new System.NotImplementedException(),
        ElementType.Geo => throw new System.NotImplementedException(),
        ElementType.Herb => new Color(0, 1f, 0),
        _ => new Color(1, 1, 1),
    };
    //角色初始化
    public void CharacterInit(string charaName, int maxActionPoint, ElementType elementType, string elementalSkillName, string elementalBurstName)
    {
        InitElements();
        name = charaName;
        MaxActionPoint = maxActionPoint;
        PlayerElement = elementType;
        ElementalSkillName = elementalSkillName;
        ElementalBurstName = elementalBurstName;
    }
    private void CharacterInit(string charaName, int maxActionPoint, ElementType elementType, string elementalSkillName, string elementalBurstName, int maxHealthPoints, int baseAttack)
    {
        // 初始化角色属性
        MaxHealthPoints = maxHealthPoints;
        CurrentHealthPoints = MaxHealthPoints;
        BaseAttack = baseAttack;
        BaseDefense = 0f;
        CurrentAttack = BaseAttack;
        CurrentDefense = BaseDefense;
        MaxActionPoint = maxActionPoint;
        ElementalMastery = 0f;
        ElementalDamageBonus = 0f;
        EnergyRecharge = 0f;
        CriticalRate = 50f;
        CriticalDamage = 0f;
        HealingBonus = 0f;
        MaxElementalEnergy = 0f;
        CurrentElementalEnergy = 0f;

        // 设置角色名称和元素技能名称
        name = charaName;
        PlayerElement = elementType;
        ElementalSkillName = elementalSkillName;
        ElementalBurstName = elementalBurstName;
        InitElements();
    }
    //////////////////////////////////////////////////角色技能的相关配置数据////////////////////////////////////////////////////////////////////////////
    public abstract ActionData BasicAttackSkillData { get; }
    public abstract ActionData SpecialSkillData { get; }
    public abstract ActionData BrustSkillData { get; }
    //////////////////////////////////////////////////角色技能的具体流程////////////////////////////////////////////////////////////////////////////
    public abstract Task AttackAction();
    public abstract Task SkillAction();
    public abstract Task BrustAction();
    public abstract Task EnemySkillAction();
    //////////////////////////////////////////////////等待技能选择////////////////////////////////////////////////////////////////////////////
    public virtual void WaitForSelectSkill()
    {
        if (IsEnemy)
        {
            //直接标记回合状态为执行中
            ActionBarManager.BasicActionStart();
            //直接敌人操作
            EnemySkillAction();
        }
        else
        {
            //回合状态依旧为为执行前，等待玩家具体选择后再切换
            SkillManager.ShowBasicAndSpecialSkill(BasicAttackSkillData, SpecialSkillData);
        }
    }
    public virtual void WaitForBrustSkill() => SkillManager.ShowBrustSkill(BrustSkillData);

    public virtual void PlayAnimation(AnimationType animationType)
    {
        animator.CrossFade(animationType.ToString(), 0.2f);
    }
    public virtual void PlayAudio(AnimationType animationType)
    {
        audioSource.clip = null;
        audioSource.Play();
    }
    //////////////////////////////////////////////////元素附着判断////////////////////////////////////////////////////////////////////////////
    public void InitElements()
    {
        if (ElementalCanves != null)
        {
            for (int i = 0; i < 5; i++)
            {
                ElementalCanves.GetChild(i).GetComponent<Image>().material = new Material(ElementalCanves.GetChild(i).GetComponent<Image>().material);
                ElementalCanves.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
    public async Task AddElementsAcync(ElementType elementType, int timer)
    {
        AttachmentElements[elementType] = HasElements(elementType) ? Mathf.Max(timer, AttachmentElements[elementType]) : timer;
        await RefreshElementsUIAsync();
    }
    public async Task RemoveElementsAcync(ElementType elementType)
    {
        AttachmentElements.Remove(elementType);
        await RefreshElementsUIAsync();
    }
    public bool HasElements(ElementType elementType) => AttachmentElements.ContainsKey(elementType);
    public async Task RefreshElementsUIAsync()
    {
        await SetElementalCanvasActive(0, HasElements(ElementType.Pyro) || HasElements(ElementType.Burn), HasElements(ElementType.Burn));
        await SetElementalCanvasActive(1, HasElements(ElementType.Hydro));
        await SetElementalCanvasActive(2, HasElements(ElementType.Electro));
        await SetElementalCanvasActive(3, HasElements(ElementType.Cryo) || HasElements(ElementType.Frozen), HasElements(ElementType.Frozen));
        await SetElementalCanvasActive(4, HasElements(ElementType.Herb) || HasElements(ElementType.Stimulus), HasElements(ElementType.Stimulus));

        async Task SetElementalCanvasActive(int index, bool isActive, bool isVariantElement = false)
        {
            GameObject targetIcon = ElementalCanves.GetChild(index).gameObject;
            var lastActive = targetIcon.gameObject.activeSelf;
            //元素被新增
            if (!lastActive && isActive)
            {
                ElementalCanves.GetChild(index).gameObject.SetActive(true);
                //设置是否是变体元素
                ElementalCanves.GetChild(index).GetComponent<Image>().material.SetFloat("_isVariantElement", isVariantElement ? 1 : 0);
                ElementalCanves.GetChild(index).GetComponent<Image>().material.SetFloat("_isDisappeared", 0);
                await CustomThread.TimerAsync(0.2f, (progress =>
                {
                    targetIcon.transform.localScale = Vector3.one * (1 + progress * 0.2f);
                }));
                await CustomThread.TimerAsync(0.2f, (progress =>
                {
                    targetIcon.transform.localScale = Vector3.one * (1 + (1 - progress) * 0.2f);
                }));
            }
            //元素被移除
            if (lastActive && !isActive)
            {
                ElementalCanves.GetChild(index).GetComponent<Image>().material.SetFloat("_isDisappeared", 1);
                await Task.Delay(500);
                ElementalCanves.GetChild(index).gameObject.SetActive(false);
            }
        }
    }
    //////////////////////////////////////////////////游戏事件相应////////////////////////////////////////////////////////////////////////////
    /// <summary>
    /// 当玩家受到攻击时,参数：是否暴击，伤害元素类型，元素附着回合数，伤害值，是否是反应伤害
    /// </summary>
    public virtual async Task OnCharaHit(bool isCritical, ElementType elementType, int timer, int point, ReactionType reactionType = ReactionType.None)
    {
        //如果是超载\超导或者感电反应衍生伤害，只结算伤害，不触发元素附着
        if (reactionType == ReactionType.Overload || reactionType == ReactionType.SuperConductor || reactionType == ReactionType.ElectricShock)
        {
            await CharaUiManager.CreatElementReaction(model, reactionType);
        }
        else
        {
            //判断是否起元素反应
            switch (elementType)
            {
                case ElementType.Anemo://风
                    if (HasElements(ElementType.Pyro))
                    {
                        _ = Left?.OnCharaHit(false, ElementType.Pyro, timer, (int)(point * 0.25f));
                        await Right?.OnCharaHit(false, ElementType.Pyro, timer, (int)(point * 0.25f));
                        await CharaUiManager.CreatElementReaction(model, ReactionType.Disperse);

                    }
                    else if (HasElements(ElementType.Hydro))
                    {
                        _ = Left?.OnCharaHit(false, ElementType.Hydro, timer, (int)(point * 0.25f));
                        await Right?.OnCharaHit(false, ElementType.Hydro, timer, (int)(point * 0.25f));
                        await CharaUiManager.CreatElementReaction(model, ReactionType.Disperse);
                    }
                    else if (HasElements(ElementType.Electro))
                    {
                        Left?.OnCharaHit(false, ElementType.Electro, timer, (int)(point * 0.25f));
                        Right?.OnCharaHit(false, ElementType.Electro, timer, (int)(point * 0.25f));
                        await CharaUiManager.CreatElementReaction(model, ReactionType.Disperse);
                    }
                    else if (HasElements(ElementType.Cryo) || HasElements(ElementType.Frozen))
                    {
                        Left?.OnCharaHit(false, ElementType.Cryo, timer, (int)(point * 0.25f));
                        Right?.OnCharaHit(false, ElementType.Cryo, timer, (int)(point * 0.25f));
                        await CharaUiManager.CreatElementReaction(model, ReactionType.Disperse);
                    }
                    break;
                case ElementType.Pyro:
                    //火-水 蒸发
                    if (HasElements(ElementType.Hydro))
                    {
                        point = (int)(point * 1.5f);
                        await AddElementsAcync(ElementType.Pyro, timer);

                        _ = RemoveElementsAcync(ElementType.Pyro);
                        await RemoveElementsAcync(ElementType.Hydro);
                        await CharaUiManager.CreatElementReaction(model, ReactionType.Evaporation);
                    }
                    //火-雷 超载
                    else if (HasElements(ElementType.Electro))
                    {
                        await AddElementsAcync(ElementType.Pyro, timer);
                        await RemoveElementsAcync(ElementType.Electro);
                        await CharaUiManager.CreatElementReaction(model, ReactionType.Overload);
                        this?.OnCharaHit(false, ElementType.Electro, timer, (int)(point * 0.25f), ReactionType.Overload);
                        Left?.OnCharaHit(false, ElementType.Electro, timer, (int)(point * 0.25f), ReactionType.Overload);
                        Right?.OnCharaHit(false, ElementType.Electro, timer, (int)(point * 0.25f), ReactionType.Overload);
                    }
                    //火-冰/冻 融化
                    else if (HasElements(ElementType.Cryo) || HasElements(ElementType.Frozen))
                    {
                        point = (int)(point * 2f);
                        await RemoveElementsAcync(ElementType.Cryo);
                        await RemoveElementsAcync(ElementType.Frozen);
                        await CharaUiManager.CreatElementReaction(model, ReactionType.Melting);
                    }
                    //火-草 燃烧
                    else if (HasElements(ElementType.Herb) || HasElements(ElementType.Stimulus))
                    {
                        await AddElementsAcync(ElementType.Pyro, 2);
                        _ = RemoveElementsAcync(ElementType.Pyro);
                        await RemoveElementsAcync(ElementType.Herb);
                        await RemoveElementsAcync(ElementType.Stimulus);
                        await AddElementsAcync(ElementType.Burn, 2);
                        await CharaUiManager.CreatElementReaction(model, ReactionType.Combustion);
                    }
                    else
                    {
                        await AddElementsAcync(ElementType.Pyro, timer);
                    }
                    break;
                case ElementType.Hydro:
                    break;
                case ElementType.Electro:
                    //雷-火/燃 超载
                    if (HasElements(ElementType.Pyro) || HasElements(ElementType.Burn))
                    {

                        await AddElementsAcync(ElementType.Electro, timer);
                        _ = RemoveElementsAcync(ElementType.Pyro);
                        _ = RemoveElementsAcync(ElementType.Burn);
                        await RemoveElementsAcync(ElementType.Electro);
                        await CharaUiManager.CreatElementReaction(model, ReactionType.Overload);
                        this?.OnCharaHit(false, ElementType.Electro, timer, (int)(point * 0.25f), ReactionType.Overload);
                        Left?.OnCharaHit(false, ElementType.Electro, timer, (int)(point * 0.25f), ReactionType.Overload);
                        Right?.OnCharaHit(false, ElementType.Electro, timer, (int)(point * 0.25f), ReactionType.Overload);
                    }
                    //雷-水 感电
                    else if (HasElements(ElementType.Hydro))
                    {
                        BattleManager.charaList
                            .Where(chara => chara.HasElements(ElementType.Hydro))
                            .ToList()
                            .ForEach(async chara => await chara.OnCharaHit(false, ElementType.Electro, timer, (int)(point * 0.25f), ReactionType.ElectricShock));
                        RemoveElementsAcync(ElementType.Hydro);
                        await CharaUiManager.CreatElementReaction(model, ReactionType.Overload);
                    }
                    //雷-冰/冻 超导
                    else if (HasElements(ElementType.Cryo) || HasElements(ElementType.Frozen))
                    {
                        this?.OnCharaHit(false, ElementType.Electro, timer, (int)(point * 0.25f), ReactionType.SuperConductor);
                        Left?.OnCharaHit(false, ElementType.Electro, timer, (int)(point * 0.25f), ReactionType.SuperConductor);
                        Right?.OnCharaHit(false, ElementType.Electro, timer, (int)(point * 0.25f), ReactionType.SuperConductor);
                        RemoveElementsAcync(ElementType.Cryo);
                        RemoveElementsAcync(ElementType.Frozen);
                        await CharaUiManager.CreatElementReaction(model, ReactionType.SuperConductor);
                    }
                    //雷-草 原激化
                    else if (HasElements(ElementType.Herb))
                    {
                        await AddElementsAcync(ElementType.Electro, timer);

                        _ = RemoveElementsAcync(ElementType.Electro);
                        await RemoveElementsAcync(ElementType.Herb);

                        await AddElementsAcync(ElementType.Stimulus, timer);
                        await CharaUiManager.CreatElementReaction(model, ReactionType.OriginalActivation);
                    }
                    //雷-激 超激化
                    else if (HasElements(ElementType.Stimulus))
                    {
                        point = (int)(point * 1.5f);
                        await AddElementsAcync(ElementType.Electro, timer);
                        await RemoveElementsAcync(ElementType.Electro);
                        await AddElementsAcync(ElementType.Stimulus, timer);
                        await CharaUiManager.CreatElementReaction(model, ReactionType.SuperActivation);
                    }
                    else
                    {
                        await AddElementsAcync(ElementType.Electro, timer);
                    }
                    break;
                case ElementType.Cryo:
                    if (HasElements(ElementType.Pyro))
                    {

                    }
                    else if (HasElements(ElementType.Hydro))
                    {

                    }
                    else if (HasElements(ElementType.Electro))
                    {

                    }
                    else if (HasElements(ElementType.Cryo))
                    {

                    }
                    else
                    {
                        await AddElementsAcync(ElementType.Cryo, timer);
                    }
                    break;
                case ElementType.Geo:
                    if (HasElements(ElementType.Pyro))
                    {
                        await CharaUiManager.CreatElementReaction(model, ReactionType.Crystallize);
                    }
                    else if (HasElements(ElementType.Hydro))
                    {
                        await CharaUiManager.CreatElementReaction(model, ReactionType.Crystallize);
                    }
                    else if (HasElements(ElementType.Electro))
                    {
                        await CharaUiManager.CreatElementReaction(model, ReactionType.Crystallize);
                    }
                    else if (HasElements(ElementType.Cryo) || HasElements(ElementType.Frozen))
                    {
                        await CharaUiManager.CreatElementReaction(model, ReactionType.Crystallize); ;
                    }
                    break;
                case ElementType.Herb:
                    //草-火/燃 燃烧
                    if (HasElements(ElementType.Pyro) || HasElements(ElementType.Burn))
                    {
                        await RemoveElementsAcync(ElementType.Pyro);
                        await AddElementsAcync(ElementType.Burn, 2);
                        await CharaUiManager.CreatElementReaction(model, ReactionType.Combustion);
                    }
                    //草-水 绽放
                    else if (HasElements(ElementType.Hydro))
                    {
                        await RemoveElementsAcync(ElementType.Hydro);
                        await CharaUiManager.CreatElementReaction(model, ReactionType.Bloom);
                        //添加一个种子状态
                    }
                    //草-雷 原激化
                    else if (HasElements(ElementType.Electro))
                    {
                        await RemoveElementsAcync(ElementType.Electro);
                        await AddElementsAcync(ElementType.Stimulus, timer);
                        await CharaUiManager.CreatElementReaction(model, ReactionType.OriginalActivation);
                    }
                    //草-激 蔓激化
                    else if (HasElements(ElementType.Stimulus))
                    {
                        point = (int)(point * 1.5f);
                        await AddElementsAcync(ElementType.Stimulus, timer);
                        await CharaUiManager.CreatElementReaction(model, ReactionType.RapidActivation);
                    }
                    else
                    {
                        await AddElementsAcync(ElementType.Herb, timer);
                    }
                    break;
                case ElementType.Physical:
                    if (HasElements(ElementType.Frozen))
                    {
                        point = (int)(point * 3f);
                        await RemoveElementsAcync(ElementType.Frozen);
                        await CharaUiManager.CreatElementReaction(model, ReactionType.ShatteredIce);
                    }
                    break;
                case ElementType.Cure:
                    break;
                case ElementType.Shield:
                    break;
                default:
                    break;
            }
        }
        switch (elementType)
        {
            case ElementType.Cure:
                break;
            case ElementType.Shield:
                break;
            default:
                //判定防御减伤
                point = (int)(point * ((100 - BaseDefense) * 0.01f));
                //判定护盾
                //跳盾量减少
                //判定血量
                break;
        }
        //跳数字
        await CharaUiManager.CreatNumber(isCritical, model, elementType, (int)point);
        BroadcastManager.BroadcastEvent(BoardcastHit, new CharaEvent());
        await Task.Delay(4000);
    }
    public virtual void BoardcastHit(CharaEvent e)
    {

    }
    /// <summary>
    /// 当敌人受到攻击时
    /// </summary>
    public virtual void OnEnemyHit() { }
    public virtual void OnCharaDead() { }
    public virtual void OnEnemyDead() { }
    public virtual void OnCharaRevived() { }
    public virtual void OnEnemyRevived() { }
    ///////////////////////////计算公式//////////////////////////////
    /// <summary>
    /// 输入伤害倍率和目标，根据当前玩家的攻击力、暴击、爆伤、buff、debuff等决定伤害
    /// </summary>
    /// <param name="DamageMultipler"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public async Task CalculateHitPointsAsync(int DamageMultipler, ElementType elementType, int timer, List<Character> targets)
    {
        await Task.WhenAll(targets.Select(target => CalculateHitPointsAsync(DamageMultipler, elementType, timer, target)));
        //targets.ForEach(async target =>await CalculateHitPointsAsync(DamageMultipler, elementType, timer, target));
        async Task CalculateHitPointsAsync(int DamageMultipler, ElementType elementType, int timer, Character target)
        {
            CurrentAttack = BaseAttack;
            float point = CurrentAttack;
            bool isCritical = Random.Range(0f, 100f) < CriticalRate;
            if (isCritical)
            {
                point *= (int)((100 + CriticalDamage) * 0.01f);
            }
            await target.OnCharaHit(isCritical, elementType, timer, (int)point);
        }
    }
    public void GetAttack() { }
    public void GetMaxHealthPoints() { }
    public void GetCurrentDefense() { }
}