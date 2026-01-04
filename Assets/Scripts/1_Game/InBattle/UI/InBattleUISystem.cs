using System;
using UnityEngine;

internal class InBattleUISystem : InstanceBehaviour<InBattleUISystem>
{
    public GameObject UI;
    internal static void ShowUI() => Instance.UI.SetActive(true);
    internal static void CloeUI() => Instance.UI.SetActive(false);
}