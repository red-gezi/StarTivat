using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

namespace ShinySSRR {

    [CustomEditor(typeof(ShinySSRR))]
    public class RenderFeatureEditor : Editor {

        SerializedProperty useDeferred, enableScreenSpaceNormalsPass, customSmoothnessMetallicPass, renderPassEvent, ignorePostProcessingOption, cameraLayerMask;
        Volume shinyVolume;

        private void OnEnable() {
            renderPassEvent = serializedObject.FindProperty("renderPassEvent");
            useDeferred = serializedObject.FindProperty("useDeferred");
            ignorePostProcessingOption = serializedObject.FindProperty("ignorePostProcessingOption");
            cameraLayerMask = serializedObject.FindProperty("cameraLayerMask");
            customSmoothnessMetallicPass = serializedObject.FindProperty("customSmoothnessMetallicPass");
            enableScreenSpaceNormalsPass = serializedObject.FindProperty("enableScreenSpaceNormalsPass");

            FindShinySSRRVolume();
        }


        void FindShinySSRRVolume() {
            Volume[] vols = Misc.FindObjectsOfType<Volume>(true);
            foreach (Volume volume in vols) {
                if (volume.sharedProfile != null && volume.sharedProfile.Has<ShinyScreenSpaceRaytracedReflections>()) {
                    shinyVolume = volume;
                    return;
                }
            }
        }


        public override void OnInspectorGUI() {
            EditorGUILayout.PropertyField(renderPassEvent);
            EditorGUILayout.PropertyField(cameraLayerMask);
            EditorGUILayout.PropertyField(ignorePostProcessingOption);
            EditorGUILayout.PropertyField(useDeferred);
            EditorGUILayout.Separator();
            if (shinyVolume != null) {
                EditorGUILayout.HelpBox("Select the Post Processing Volume to customize Shiny SSR settings.", MessageType.Info);
                if (GUILayout.Button("Show Volume Settings")) {
                    Selection.SetActiveObjectWithContext(shinyVolume, null);
                    GUIUtility.ExitGUI();
                }
            } else {
                EditorGUILayout.HelpBox("Create a Post Processing volume in the scene to customize Shiny SSR settings.", MessageType.Info);
            }

            EditorGUILayout.Separator();
            if (!useDeferred.boolValue) {
                GUILayout.Label("Advanced", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(customSmoothnessMetallicPass);
                if (!customSmoothnessMetallicPass.boolValue) {
                    EditorGUILayout.HelpBox("In forward rendering, reflections can be added to the scene in two ways:\nA) adding a Reflections script to the objects you want to receive reflections, OR\nB) enabling the 'Custom Smoothness Metallic Pass' option (requires that the shaders support a specific pass named 'SmoothnessMetallic').", MessageType.Info);
                }
                EditorGUILayout.PropertyField(enableScreenSpaceNormalsPass, new GUIContent("Enable Screen Space Normals"));
                if (!enableScreenSpaceNormalsPass.boolValue) {
                    EditorGUILayout.HelpBox("In forward rendering, surface normals are obtained from the bump map texture attached to the object material unless this 'Screen Space Normals' option is enabled. In this case, a full screen normals pass is used. This option is recommended if you use shaders that alter the surface normals.", MessageType.Info);
                }

            }
        }

    }
}