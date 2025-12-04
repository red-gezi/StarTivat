using Sirenix.OdinInspector;
using UnityEngine;

public class CreatNewChara : MonoBehaviour
{
    [Button("创建新人物")]
    public void CreatChara(GameObject model, PlayerName charaName)
    {
        Log.Show("开始以首个子物体为模板创建新角色");
        //创建人物条例
        var newChara = Instantiate(transform.GetChild(0).gameObject, transform);
        newChara.name=charaName.ToString();
        //附加新模型，配置脚本
        var newmodel = Instantiate(model, newChara.transform);
    }
}