using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class SceneModelManager : MonoBehaviour
{
    public static SceneModelManager Instance;
    public Transform ScreenModelRoot;
    private void Awake() => Instance = this;
    [Button("ÇÐ»»³¡¾°")]
    public void SwitchScreenModel(SceneModelType screenModel)
    {
        foreach (Transform model in ScreenModelRoot)
        {
            model.gameObject.SetActive(model.name == screenModel.ToString());
        }
    }
}
