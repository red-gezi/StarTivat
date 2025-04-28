using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharaInfoBoardManager : MonoBehaviour
{
    public GameObject charaInfoBoard;
    public GameObject openButton;
    public GameObject charaList;
    bool isShowEnemy;
    public Image BrustIcon;
    public Text BrustValue;

    public Text charaNname;
    public Text hp;
    public TextMeshProUGUI attack;
    public TextMeshProUGUI criticalRate;
    public TextMeshProUGUI speed;
    public TextMeshProUGUI defense;
    public TextMeshProUGUI criticalDamage;
    public Text buffCount;
    public Text debuffCount;
    private void Awake()
    {
        CloseCharaInfoBoard();
    }
    public void OpenCharaInfoBoard()
    {
        //打开面板
        charaInfoBoard.SetActive(true);
        openButton.SetActive(false);
        RefreshCharaList();
        SwitchCurrentChara(0);
    }
    public void CloseCharaInfoBoard()
    {
        charaInfoBoard.SetActive(false);
        openButton.SetActive(true);
        //还原摄像机位置
        SkillManager.Instance.ResetPose();
    }
    public void SwitchCurrentChara(int index)
    {
        //设置摄像机位置
        var character = isShowEnemy
            ? BattleManager.CurrentBattle.EnemyList[index]
            : BattleManager.CurrentBattle.PlayerList[index];
        CameraTrackManager.SetIdleShow(character);
        RefreshCharaInfoBoard(character);
    }
    public void SwitchCamp(int index)
    {
        isShowEnemy = !isShowEnemy;
        RefreshCharaList();
        SwitchCurrentChara(0);
    }
    public void RefreshCharaList()
    {
        int count = isShowEnemy ?
            BattleManager.CurrentBattle.EnemyList.Count :
            BattleManager.CurrentBattle.PlayerList.Count;
        for (int i = 0; i < 5; i++)
        {

            Transform charaItem = charaList.transform.GetChild(i);
            charaItem.gameObject.SetActive(i < count);
            if (i < count)
            {
                var character = isShowEnemy
                ? BattleManager.CurrentBattle.EnemyList[i]
                : BattleManager.CurrentBattle.PlayerList[i];
                charaItem.GetChild(0).GetChild(0).GetComponent<Image>().sprite = character.miniCharaIcon;
            }
        }
    }
    public void RefreshCharaInfoBoard(Character character)
    {
        var targetData = character.CurrentCharaData;
        Debug.Log(targetData.ToJson());
        //刷新数值
        BrustIcon.sprite= character.brustSkillIcon;
        BrustValue.text= $"{targetData.CurrentElementalEnergy}/{targetData.MaxElementalEnergy}";
        charaNname.text = targetData.CharaName;
        hp.text = $"{targetData.BaseHP}/{targetData.MaxHP}";
        attack.text = $"{targetData.BaseAttack}+<color={(targetData.AttackBonus>0? "#16D2EB":"red")}>{targetData.AttackBonus}</color>";
        criticalRate.text = $"{targetData.CriticalRate}%";
        speed.text = $"{targetData.MaxActionPoint}+<color={(targetData.AttackBonus > 0 ? "#16D2EB" : "red")}>{5}</color>";
        defense.text = $"{targetData.TotalDefenseBonus}%";
        criticalDamage.text = $"{targetData.BaseCriticalDamage}%";
        buffCount.text ="5";
        debuffCount.text = "14";

    }
}