using System;
using UnityEngine;

internal class BattleUISystem : InstanceBehaviour<BattleUISystem>
{
    public GameObject UI;
    internal static void ShowUI() => Instance.UI.SetActive(true);
    internal static void CloeUI() => Instance.UI.SetActive(false);
}