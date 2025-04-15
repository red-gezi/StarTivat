#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class FBXAnimationExporter : EditorWindow
{
    [MenuItem("Tools/Export FBX Animations")]
    static void ExportFBXAnimations()
    {
        string selectedAssetPath = Selection.activeObject != null ? AssetDatabase.GetAssetPath(Selection.activeObject) : "";
        if (!string.IsNullOrEmpty(selectedAssetPath))
        {
            string folderPath = new FileInfo(selectedAssetPath).Directory.FullName + "\\Anim";
            if (!string.IsNullOrEmpty(folderPath))
            {
                // ȷ��·������
                Directory.CreateDirectory(folderPath);
                // ���� FBX �ļ�
                Object[] assets = AssetDatabase.LoadAllAssetsAtPath(selectedAssetPath);
                // ���˳� AnimationClip
                AnimationClip[] clips = assets.Where(asset => asset is AnimationClip).Cast<AnimationClip>().ToArray();
                foreach (AnimationClip clip in clips)
                {
                    // ���� AnimationClip ���ļ���
                    string oldValue = Directory.GetCurrentDirectory();
                    string clipPath = Path.Combine(folderPath.Replace(oldValue + "\\Assets\\",""), Path.GetFileName(clip.name) + ".fbx");
                    AssetDatabase.ExtractAsset(clip, clipPath);
                }
                Debug.Log("Animations exported successfully to: " + folderPath);
            }
        }
        else
        {
            Debug.LogError("No FBX file selected.");
        }
    }
}
#endif
