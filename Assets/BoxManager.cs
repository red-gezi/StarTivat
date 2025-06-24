using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class BoxManager : MonoBehaviour
{
    public enum BoxType
    {
        怪物,
        元素石碑,
        顺序元素石碑,
        密码元素石碑,
        骰子,
    }
    [EnumToggleButtons] // 将枚举显示为按钮组
    public BoxType 宝箱类型;
    [ShowIf("宝箱类型", BoxType.怪物)]
    public List<GameObject> monster;
    [ShowIf("宝箱类型", BoxType.元素石碑)]
    public List<GameObject> elementStones;
    [ShowIf("宝箱类型", BoxType.元素石碑)]
    public List<GameObject> elementCount;
    [ShowIf("宝箱类型", BoxType.顺序元素石碑)] 
    public List<GameObject> orderElementStone;
    [ShowInInspector]
    [ShowIf("宝箱类型", BoxType.骰子)]
    public List<(GameObject 骰子, int 目标值)> points;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
