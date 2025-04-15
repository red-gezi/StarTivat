using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ShinySSRR {

    [CustomPropertyDrawer(typeof(SubMeshSettingsData))]
    public class SubMeshSettingsDrawer : PropertyDrawer {

        public override float GetPropertyHeight(SerializedProperty prop, GUIContent label) {
            GUIStyle style = GUI.skin.GetStyle("label");
            float lineHeight = style.CalcHeight(label, EditorGUIUtility.currentViewWidth);
            return lineHeight;
        }


        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label) {

            int propIndex = GetArrayIndex(prop);
            Rect firstColumn = position;
            firstColumn.width = EditorGUIUtility.labelWidth - firstColumn.x;
            Rect secondColumn = position;
            float valuesWidth = (EditorGUIUtility.currentViewWidth - firstColumn.xMax - 35) * 0.5f;
            secondColumn.x = firstColumn.xMax + 5;
            secondColumn.width = valuesWidth;
            Rect thirdColumn = position;
            thirdColumn.x = secondColumn.xMax + 5;
            thirdColumn.width = valuesWidth;
            Reflections refl = (Reflections)prop.serializedObject.targetObject;

            EditorGUIUtility.labelWidth = 80;

            if (refl.ssrRenderers != null && refl.ssrRenderers.Count == 1 && refl.ssrRenderers[0].originalMaterials != null && refl.ssrRenderers[0].originalMaterials.Count > 0) {
                List<Material> materials = refl.ssrRenderers[0].originalMaterials;
                int matIndex = propIndex;
                if (matIndex >= materials.Count) {
                    matIndex = materials.Count - 1;
                }
                EditorGUI.LabelField(firstColumn, materials[matIndex].name);
            } else {
                EditorGUI.LabelField(firstColumn, "SubMesh " + propIndex);
            }
            EditorGUI.PropertyField(secondColumn, prop.FindPropertyRelative("metallic"), new GUIContent("Metallic: "));
            EditorGUI.PropertyField(thirdColumn, prop.FindPropertyRelative("smoothness"), new GUIContent("Smoothness: "));
        }

        /// <summary>
        /// Returns the index of this property in the array
        /// </summary>
        int GetArrayIndex(SerializedProperty property) {
            string s = property.propertyPath;
            int bracket = s.LastIndexOf("[");
            if (bracket >= 0) {
                string indexStr = s.Substring(bracket + 1, s.Length - bracket - 2);
                int index;
                if (int.TryParse(indexStr, out index)) {
                    return index;
                }
            }
            return 0;
        }

    }

}
