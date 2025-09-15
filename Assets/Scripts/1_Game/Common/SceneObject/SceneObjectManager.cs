using Sirenix.OdinInspector;
using UnityEngine;
//管理整个场景的物体
public partial class SceneObjectManager : InstanceBehaviour<SceneObjectManager>
{
    public Transform screenModelRoot;
    public Transform sceneDoorRoot;
    public Transform sceneDoor1 => sceneDoorRoot.GetChild(0);
    public Transform sceneDoor2 => sceneDoorRoot.GetChild(1);
    public Transform sceneDoor3 => sceneDoorRoot.GetChild(2);
    [Button("切换场景")]
    public void SwitchSceneModel(SceneModelType screenModel)
    {
        Debug.Log("切换场景为" + screenModel.ToString());
        foreach (Transform model in screenModelRoot)
        {
            model.gameObject.SetActive(model.name == screenModel.ToString());
        }
    }
    //销毁当前场景所有物体
    public void ClearAllObject()
    {

    }
}
