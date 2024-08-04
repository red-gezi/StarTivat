using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class CharaUiManager : MonoBehaviour
{
    public GameObject smallPointPrefab;
    public GameObject largePointPrefab;
    public GameObject elementReactionPrefab;
    public GameObject buffTextPrefab;
    public GameObject target;
    public Color ElementReactionColor;
    static CharaUiManager Instance { get; set; }
    private void Awake() => Instance = this;
    public static async Task CreatNumber(bool isCritical, GameObject target, ElementType elementType, int point)
    {
        GameObject targetPrefab = isCritical ? Instance.largePointPrefab : Instance.smallPointPrefab;
        GameObject pointPrefab = Instantiate(targetPrefab, targetPrefab.transform.parent);
        pointPrefab.SetActive(true);
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(target.transform.position + Vector3.up + Random.onUnitSphere * 0.2f);
        screenPosition -= new Vector3(Screen.width / 2, Screen.height / 2);

        pointPrefab.transform.localPosition = screenPosition;
        pointPrefab.transform.forward = Camera.main.transform.forward;
        pointPrefab.GetComponent<TextMeshProUGUI>().text = (isCritical ? "����\r\n" : "") + point;
        Color color = elementType switch
        {
            ElementType.Anemo => new Color(0, 1, 0.4f),
            ElementType.Pyro => new Color(0.9f, 0.25f, 0),
            ElementType.Hydro => new Color(0f, 0.35f, 0.9f),
            ElementType.Electro => new Color(0.3f, 0, 0.75f),
            ElementType.Cryo => throw new System.NotImplementedException(),
            ElementType.Frozen => throw new System.NotImplementedException(),
            ElementType.Geo => throw new System.NotImplementedException(),
            ElementType.Herb => new Color(0, 1f, 0),
            ElementType.Stimulus => new Color(0, 1f, 0),
            ElementType.Physical => throw new System.NotImplementedException(),
            ElementType.Cure => new Color(0, 0.9f, 0),
            ElementType.Shield => throw new System.NotImplementedException(),
            ElementType.Burn => throw new System.NotImplementedException(),
            _ => throw new System.NotImplementedException(),
        };
        pointPrefab.GetComponent<TextMeshProUGUI>().color = color;
        pointPrefab.GetComponent<TextMeshProUGUI>().outlineColor = color * 0.8f;

        //await CustomThread.TimerAsync(2, progress =>
        //{
        //    pointPrefab.transform.localPosition = screenPosition + Vector3.up * 250 * progress;
        //});
        pointPrefab.transform.localPosition = screenPosition;
        await CustomThread.TimerAsync(1, progress =>
        {
            pointPrefab.transform.localScale = Vector3.one * (2 - progress);
        });
        DestroyImmediate(pointPrefab);
    }
    [Button("���췴Ӧ����")]
    public static async Task CreatElementReaction(GameObject target, ReactionType reaction)
    {
        GameObject reactionPrefab = Instantiate(Instance.elementReactionPrefab, Instance.elementReactionPrefab.transform.parent);
        reactionPrefab.SetActive(true);
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(target.transform.position + Vector3.up + Random.onUnitSphere * 0.2f);
        screenPosition -= new Vector3(Screen.width / 2, Screen.height / 2);

        reactionPrefab.transform.localPosition = screenPosition;
        reactionPrefab.transform.forward = Camera.main.transform.forward;
        reactionPrefab.GetComponent<TextMeshProUGUI>().text = reaction switch
        {
            ReactionType.None => "��",
            ReactionType.Crystallize => "�ᾧ",
            ReactionType.Disperse => "��ɢ",
            ReactionType.Melting => "�ڻ�",
            ReactionType.Evaporation => "����",
            ReactionType.Overload => "����",
            ReactionType.SuperConductor => "����",
            ReactionType.ElectricShock => "�е�",
            ReactionType.Freezing => "����",
            ReactionType.ShatteredIce => "���",
            ReactionType.Combustion => "ȼ��",
            ReactionType.Bloom => "����",
            ReactionType.SuperBloom => "������",
            ReactionType.IntenseBloom => "������",
            ReactionType.OriginalActivation => "ԭ����",
            ReactionType.SuperActivation => "������",
            ReactionType.RapidActivation => "������",
            _ => throw new System.NotImplementedException(),
        };
        Color color = reaction switch
        {
            ReactionType.None => throw new System.NotImplementedException(),
            //��ɫ
            ReactionType.Crystallize => throw new System.NotImplementedException(),
            //��ɫ
            ReactionType.Disperse => new Color(0, 1, 0.4f),
            //��ɫ
            ReactionType.Melting => new Color(0.9f, 0.25f, 0),
            ReactionType.Combustion => new Color(0.9f, 0.25f, 0),
            ReactionType.IntenseBloom => new Color(0.9f, 0.25f, 0),
            //��ɫ
            ReactionType.Overload => new Color(0.85f, 0.20f, 0.9f),
            //ˮɫ
            ReactionType.Evaporation => new Color(0f, 0.35f, 0.9f),
            //��ɫ
            ReactionType.SuperBloom => new Color(0.3f, 0, 0.75f),
            ReactionType.SuperActivation => new Color(0.3f, 0, 0.75f),
            ReactionType.SuperConductor => new Color(0.3f, 0, 0.75f),
            ReactionType.ElectricShock => new Color(0.3f, 0, 0.75f),
            //��ɫ
            ReactionType.Bloom => new Color(0, 1f, 0),
            ReactionType.OriginalActivation => new Color(0, 1f, 0),
            ReactionType.RapidActivation => new Color(0, 1f, 0),
            //��ɫ
            ReactionType.Freezing => throw new System.NotImplementedException(),
            //��ɫ
            ReactionType.ShatteredIce => throw new System.NotImplementedException(),
            _ => throw new System.NotImplementedException(),
        };
        reactionPrefab.GetComponent<TextMeshProUGUI>().color = color;
        reactionPrefab.GetComponent<TextMeshProUGUI>().outlineColor = color * 0.8f;
        //await CustomThread.TimerAsync(2, progress =>
        //{
        //    reactionPrefab.transform.localPosition = screenPosition + Vector3.up * 250 * progress;
        //});

        reactionPrefab.transform.localPosition = screenPosition;
        await CustomThread.TimerAsync(1, progress =>
        {
            reactionPrefab.transform.localScale = Vector3.one * (2 - progress);
        });
        DestroyImmediate(reactionPrefab);
    }
    [Button("������ʽ")]
    public async void CreatTest(EffectType effectType)
    {
        var targets = BattleManager.EnemyList.Select(enemy => enemy.gameObject).ToList();
        for (int i = 0; i < 10; i++)
        {
            int j = Random.Range(0, 5);
            //CreatSmallPoint(targets[j], Random.Range(555, 15555));
            //CreatLargePoint(targets[j], Random.Range(555, 15555));
            CreatElementReaction(targets[j], (ReactionType)Random.Range(0, 10));
            await Task.Delay(1000);
        }
    }
    [Button("����Buff")]
    public async void CreatBuffOrDeBuff(EffectType effectType)
    {
        await Task.Delay(50);
        string showText = effectType switch
        {
            EffectType.Buff => throw new System.NotImplementedException(),
            EffectType.Debuff => throw new System.NotImplementedException(),
            _ => throw new System.NotImplementedException(),
        };
        GameObject buff = Instantiate(buffTextPrefab, buffTextPrefab.transform.parent);
        buff.SetActive(true);
        buff.GetComponent<TextMeshProUGUI>().text = showText;
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(target.transform.position + Vector3.up);
        screenPosition -= new Vector3(Screen.width / 2, Screen.height / 2);

        buff.transform.localPosition = screenPosition;
        buff.transform.forward = Camera.main.transform.forward;
        await CustomThread.TimerAsync(2, progress =>
        {
            buff.transform.localPosition = screenPosition + Vector3.up * 50 * progress;
        });
        DestroyImmediate(buff);
    }
}