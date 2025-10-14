using MagicaCloth2;
using Sirenix.OdinInspector;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MagicaCloth2Fixer : MonoBehaviour
{
    private async void Start()
    {
        // 确保在骨骼系统初始化后重建布料
        //StartCoroutine(FixClothAfterSpawn());
    }

    private IEnumerator FixClothAfterSpawn()
    {
        // 等待骨骼系统完全初始化
        yield return new WaitForEndOfFrame();

        // 修复所有MagicaCloth组件
        FixAllClothComponents();
    }
    [Button("修复权重")]
    public void FixAllClothComponents()
    {
        MagicaCloth[] cloths = GetComponentsInChildren<MagicaCloth>();
        foreach (MagicaCloth cloth in cloths)
        {
            // 重建布料系统 - MagicaCloth2官方方法
            cloth.BuildAndRun();
            Debug.Log($"[AutoFix] 已修复 {cloth.name} 的权重问题");
        }
    }
}