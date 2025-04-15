using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

namespace ShinySSRR {

    [CustomEditor(typeof(Reflections))]
    public class ReflectionsEditor : Editor {

        SerializedProperty ignore, scope, layerMask, nameFilter, subMeshMask;
        SerializedProperty smoothness, metallic, perSubMeshSmoothness, subMeshSettings;
        SerializedProperty materialSmoothnessMapPropertyName, materialSmoothnessIntensityPropertyName, materialReflectivityPropertyName;
        SerializedProperty smoothnessSource, customSmoothnessMap;
        SerializedProperty normalSource, normalMap, normalMapStrength, materialNormalMapPropertyName, materialNormalMapScalePropertyName;
        SerializedProperty fresnel, fuzzyness, contactHardening;
        SerializedProperty overrideGlobalSettings;
        SerializedProperty sampleCount, maxRayLength, thickness, binarySearchIterations, refineThickness, thicknessFine, decay, jitter;
        SerializedProperty uvDistortionSpeed;
        Volume shinyVolume;
        Reflections ssr;

        private void OnEnable() {
            ignore = serializedObject.FindProperty("ignore");
            scope = serializedObject.FindProperty("scope");
            layerMask = serializedObject.FindProperty("layerMask");
            nameFilter = serializedObject.FindProperty("nameFilter");
            subMeshMask = serializedObject.FindProperty("subMeshMask");
            smoothness = serializedObject.FindProperty("smoothness");
            metallic = serializedObject.FindProperty("metallic");
            smoothnessSource = serializedObject.FindProperty("smoothnessSource");
            customSmoothnessMap = serializedObject.FindProperty("customSmoothnessMap");
            materialSmoothnessMapPropertyName = serializedObject.FindProperty("materialSmoothnessMapPropertyName");
            materialSmoothnessIntensityPropertyName = serializedObject.FindProperty("materialSmoothnessIntensityPropertyName");
            materialReflectivityPropertyName = serializedObject.FindProperty("materialReflectivityPropertyName");
            perSubMeshSmoothness = serializedObject.FindProperty("perSubMeshSmoothness");
            subMeshSettings = serializedObject.FindProperty("subMeshSettings");
            normalSource = serializedObject.FindProperty("normalSource");
            normalMap = serializedObject.FindProperty("normalMap");
            normalMapStrength = serializedObject.FindProperty("normalMapStrength");
            materialNormalMapPropertyName = serializedObject.FindProperty("materialNormalMapPropertyName");
            materialNormalMapScalePropertyName = serializedObject.FindProperty("materialNormalMapScalePropertyName");
            fresnel = serializedObject.FindProperty("fresnel");
            fuzzyness = serializedObject.FindProperty("fuzzyness");
            contactHardening = serializedObject.FindProperty("contactHardening");
            overrideGlobalSettings = serializedObject.FindProperty("overrideGlobalSettings");
            sampleCount = serializedObject.FindProperty("sampleCount");
            maxRayLength = serializedObject.FindProperty("maxRayLength");
            binarySearchIterations = serializedObject.FindProperty("binarySearchIterations");
            thickness = serializedObject.FindProperty("thickness");
            refineThickness = serializedObject.FindProperty("refineThickness");
            thicknessFine = serializedObject.FindProperty("thicknessFine");
            decay = serializedObject.FindProperty("decay");
            jitter = serializedObject.FindProperty("jitter");
            uvDistortionSpeed = serializedObject.FindProperty("uvDistortionSpeed");

            ssr = (Reflections)target;
            Reflections.currentEditingReflections = ssr;
            FindShinySSRRVolume();
        }

        private void OnDisable() {
            Reflections.currentEditingReflections = null;
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

            UniversalRenderPipelineAsset pipe = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
            if (pipe == null) {
                EditorGUILayout.HelpBox("Universal Rendering Pipeline asset is not set in 'Project Settings / Graphics or Quality' !", MessageType.Error);
                EditorGUILayout.Separator();
                GUI.enabled = false;
            } else if (!ShinySSRR.installed) {
                EditorGUILayout.HelpBox("Shiny SSR Render Feature must be added to the rendering pipeline renderer.", MessageType.Error);
                if (GUILayout.Button("Go to Universal Rendering Pipeline Asset")) {
                    Selection.activeObject = pipe;
                }
                EditorGUILayout.Separator();
                GUI.enabled = false;
            } else {
                if (!pipe.supportsCameraDepthTexture) {
                    EditorGUILayout.HelpBox("Depth Texture option is required. Check Universal Rendering Pipeline asset!", MessageType.Warning);
                    if (GUILayout.Button("Go to Universal Rendering Pipeline Asset")) {
                        Selection.activeObject = pipe;
                    }
                    EditorGUILayout.Separator();
                    GUI.enabled = false;
                }
                EditorGUILayout.BeginVertical(GUI.skin.box);

                if (shinyVolume != null) {
                    if (GUILayout.Button("Show Volume Settings")) {
                        Selection.SetActiveObjectWithContext(shinyVolume, null);
                        GUIUtility.ExitGUI();
                    }
                } else if (pipe != null) {
                    if (GUILayout.Button("Show URP Renderer Settings")) {
                        var so = new SerializedObject(pipe);
                        var prop = so.FindProperty("m_RendererDataList");
                        if (prop != null && prop.arraySize > 0) {
                            var o = prop.GetArrayElementAtIndex(0);
                            if (o != null) {
                                Selection.SetActiveObjectWithContext(o.objectReferenceValue, null);
                                GUIUtility.ExitGUI();
                            }
                        }
                    }
                }
                EditorGUILayout.EndVertical();
            }

            // ensure submesh array size matches materials count
            Reflections refl = (Reflections)target;
            if (refl.ssrRenderers != null && refl.ssrRenderers.Count == 1 && refl.ssrRenderers[0].originalMaterials != null) {
                List<Material> materials = refl.ssrRenderers[0].originalMaterials;
                if (refl.subMeshSettings == null) {
                    refl.subMeshSettings = new SubMeshSettingsData[materials.Count];
                } else if (refl.subMeshSettings.Length < materials.Count) {
                    System.Array.Resize(ref refl.subMeshSettings, materials.Count);
                }
            }
            Reflections.currentEditingReflections = refl;

            serializedObject.Update();

            EditorGUILayout.PropertyField(ignore);
            if (!ignore.boolValue) {

                if (refl.renderers?.Count == 0) {
                    if (scope.intValue == (int)Scope.OnlyThisObject) {
                        EditorGUILayout.HelpBox("No renderers found on this gameobject. Switch to 'Include Children' or add this script to another object which contains a renderer.", MessageType.Warning);
                    } else {
                        EditorGUILayout.HelpBox("No renderers found under this gameobject.", MessageType.Warning);
                    }
                }

                EditorGUILayout.PropertyField(scope);
                if (scope.intValue == (int)Scope.IncludeChildren) {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(layerMask);
                    EditorGUILayout.PropertyField(nameFilter);
                    EditorGUILayout.PropertyField(subMeshMask);
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.PropertyField(smoothnessSource);
                bool usesMaterialSmoothness = smoothnessSource.intValue == (int)SmoothnessSource.Material;
                EditorGUI.indentLevel++;
                switch (smoothnessSource.intValue) {
                    case (int)SmoothnessSource.Material:
                        EditorGUILayout.LabelField("Default values (used only if not present in the material):", EditorStyles.miniLabel);
                        EditorGUILayout.PropertyField(customSmoothnessMap, new GUIContent("Smoothness Map (A)"));
                        EditorGUILayout.PropertyField(metallic);
                        EditorGUILayout.PropertyField(smoothness);
                        break;
                    case (int)SmoothnessSource.Custom:
                        EditorGUILayout.PropertyField(customSmoothnessMap, new GUIContent("Smoothness Map (A)"));
                        if (!perSubMeshSmoothness.boolValue) {
                            EditorGUILayout.PropertyField(metallic);
                            EditorGUILayout.PropertyField(smoothness);
                        }
                        EditorGUILayout.PropertyField(perSubMeshSmoothness, new GUIContent("Per SubMesh Values"));
                        if (perSubMeshSmoothness.boolValue) {
                            EditorGUILayout.PropertyField(subMeshSettings, new GUIContent("Intensities"), true);
                        }
                        break;
                }
                EditorGUI.indentLevel--;

                EditorGUILayout.PropertyField(normalSource);
                bool usesMaterialNormal = normalSource.intValue == (int)NormalSource.Material;
                if (ShinySSRR.isDeferredActive && !usesMaterialNormal) {
                    EditorGUILayout.HelpBox("In deferred rendering path, Reflections scripts will use material normal maps always.", MessageType.Info);
                    usesMaterialNormal = true;
                }

                EditorGUI.indentLevel++;
                switch (normalSource.intValue) {
                    case (int)NormalSource.ScreenSpaceNormals:
                        if (ShinySSRR.isDeferredActive) {
                            EditorGUILayout.HelpBox("This option should not be used in deferred rendering.", MessageType.Warning);
                        } else if (!ShinySSRR.isUsingScreenSpaceNormals) {
                            EditorGUILayout.HelpBox("Screen Space Normals option must be enabled in the Shiny SSR Render Feature.", MessageType.Warning);
                            if (GUILayout.Button("Go to Universal Rendering Pipeline Asset")) {
                                Selection.activeObject = pipe;
                            }
                            EditorGUILayout.Separator();
                        }
                        break;
                    case (int)NormalSource.Material:
                        EditorGUILayout.LabelField("Default values (used only if not present in the material):", EditorStyles.miniLabel);
                        EditorGUILayout.PropertyField(normalMap);
                        EditorGUILayout.PropertyField(normalMapStrength);
                        break;
                    case (int)NormalSource.Custom:
                        EditorGUILayout.PropertyField(normalMap);
                        EditorGUILayout.PropertyField(normalMapStrength);
                        break;
                }
                EditorGUI.indentLevel--;

                if (usesMaterialSmoothness || perSubMeshSmoothness.boolValue || usesMaterialNormal) {
                    EditorGUILayout.Separator();
                    EditorGUILayout.LabelField("Material Property Names", EditorStyles.miniBoldLabel);
                    if (usesMaterialSmoothness || perSubMeshSmoothness.boolValue) {
                        EditorGUILayout.PropertyField(materialSmoothnessMapPropertyName, new GUIContent("Smoothness Map", "The material property name for the smoothness map"));
                        EditorGUILayout.PropertyField(materialReflectivityPropertyName, new GUIContent("Metallic", "The material property name for the metallic intensity"));
                        EditorGUILayout.PropertyField(materialSmoothnessIntensityPropertyName, new GUIContent("Smoothness", "The material property name for the smoothness intensity"));
                    }
                    if (usesMaterialNormal) {
                        EditorGUILayout.PropertyField(materialNormalMapPropertyName, new GUIContent("NormalMap", "The material property name for the normal map"));
                        EditorGUILayout.PropertyField(materialNormalMapScalePropertyName, new GUIContent("NormalMap Strength", "The material property name for the normal map strength or scale"));
                    }
                }

                EditorGUILayout.PropertyField(overrideGlobalSettings, new GUIContent("Override Volume Settings"));
                if (overrideGlobalSettings.boolValue) {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Apply Quality Preset:", GUILayout.Width(EditorGUIUtility.labelWidth));
                    if (GUILayout.Button("Fast")) {
                        ApplyRaytracingPreset(RaytracingPreset.Fast);
                    }
                    if (GUILayout.Button("Medium")) {
                        ApplyRaytracingPreset(RaytracingPreset.Medium);
                    }
                    if (GUILayout.Button("High")) {
                        ApplyRaytracingPreset(RaytracingPreset.High);
                    }
                    if (GUILayout.Button("Superb")) {
                        ApplyRaytracingPreset(RaytracingPreset.Superb);
                    }
                    if (GUILayout.Button("Ultra")) {
                        ApplyRaytracingPreset(RaytracingPreset.Ultra);
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.PropertyField(sampleCount);
                    EditorGUILayout.PropertyField(maxRayLength);
                    EditorGUILayout.PropertyField(thickness);
                    EditorGUILayout.PropertyField(binarySearchIterations);
                    EditorGUILayout.PropertyField(refineThickness);
                    if (refineThickness.boolValue) {
                        EditorGUILayout.PropertyField(thicknessFine);
                    }
                    EditorGUILayout.PropertyField(jitter);
                    EditorGUILayout.PropertyField(fresnel);
                    EditorGUILayout.PropertyField(decay);
                    EditorGUILayout.PropertyField(fuzzyness, new GUIContent("Fuzziness"));
                    EditorGUILayout.PropertyField(contactHardening);
                    EditorGUI.indentLevel--;
                }
            }

            EditorGUILayout.PropertyField(uvDistortionSpeed);

            serializedObject.ApplyModifiedProperties();
        }


        void ApplyRaytracingPreset(RaytracingPreset preset) {
            ssr.ApplyRaytracingPreset(preset);
            EditorUtility.SetDirty(ssr);
        }

    }
}