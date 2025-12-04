using System;
using UnityEngine;

internal class BattleUIManager : InstanceBehaviour<BattleUIManager>
{
    public GameObject UI;
    internal static void ShowUI() => Instance.UI.SetActive(true);
    internal static void CloeUI() => Instance.UI.SetActive(false);
}