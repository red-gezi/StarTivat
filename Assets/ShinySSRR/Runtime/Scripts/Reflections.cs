/// <summary>
/// Shiny SSR - Screen Space Reflections - (c) 2021-2024 Kronnect
/// </summary>

using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShinySSRR {

    public enum Scope {
        OnlyThisObject = 0,
        IncludeChildren = 10
    }


    [Serializable]
    public struct SubMeshSettingsData {
        [Range(0, 1)]
        public float metallic;
        [Range(0, 1)]
        public float smoothness;
    }

    public enum SmoothnessSource {
        Custom,
        Material
    }

    public enum NormalSource {
        ScreenSpaceNormals,
        Material,
        Custom
    }

    [ExecuteInEditMode]
    public partial class Reflections : MonoBehaviour {


        [Tooltip("Prevent this object from receiving reflections")]
        public bool ignore;

        [Tooltip("On which objects should reflections apply")]
        public Scope scope;

        [Tooltip("Include only objects matching this layer mask")]
        public LayerMask layerMask = -1;

        [Tooltip("Include only objects including this text")]
        public string nameFilter;

        [Tooltip("Optionally specify which submeshes can receive reflections. Set this to 0 to apply reflections to all submeshes.")]
        public int subMeshMask;

        [Header("Material Features")]
        [Tooltip("Source for the smoothness map")]
        public SmoothnessSource smoothnessSource = SmoothnessSource.Material;

        public Texture customSmoothnessMap;

        [Tooltip("Custom material property name for the smoothness map")]
        public string materialSmoothnessMapPropertyName = "_MetallicGlossMap";

        [Tooltip("Custom material property name for the metallic intensity")]
        public string materialReflectivityPropertyName = "_Metallic";

        [Tooltip("Custom material property name for the smoothness intensity")]
        public string materialSmoothnessIntensityPropertyName = "_Smoothness";

        [Tooltip("Smoothness value used if material has no smoothness property.")]
        [Range(0, 1)]
        public float smoothness = 0.75f;

        [Tooltip("Metallic value used if material has no metallic property.")]
        [Range(0, 1)]
        public float metallic = 1f;

        [Tooltip("Apply a different smoothness multiplier per submesh")]
        public bool perSubMeshSmoothness;

        [Tooltip("Smoothness values per each submesh")]
        public SubMeshSettingsData[] subMeshSettings;

        public NormalSource normalSource = NormalSource.Material;

        [Tooltip("The normal/bump map")]
        public Texture normalMap;

        [Tooltip("The normal map strength")]
        public float normalMapStrength = 1f;

        [Tooltip("Custom material property name for the normal map")]
        public string materialNormalMapPropertyName = "_BumpMap";

        [Tooltip("Custom material property name for the normal map strength")]
        public string materialNormalMapScalePropertyName = "_BumpScale";

        [Header("Raytracing Settings")]
        public bool overrideGlobalSettings;

        [Tooltip("Max number of samples used during the raymarch loop")]
        [Range(4, 128)] public int sampleCount = 16;

        [HideInInspector]
        public float stepSize; // no longer used; kept for backward compatibility during upgrade

        [Tooltip("Maximum reflection distance")]
        public float maxRayLength;

        [Tooltip("Assumed thickness of geometry in the depth buffer before binary search")]
        public float thickness = 0.3f;

        [Tooltip("Number of refinements steps when a reflection hit is found")]
        [Range(0, 16)] public int binarySearchIterations = 6;

        [Tooltip("Increase accuracy of reflection hit after binary search by discarding points farther than a reduced thickness.")]
        public bool refineThickness;

        [Tooltip("Assumed thickness of geometry in the depth buffer after binary search")]
        [Range(0.005f, 1f)]
        public float thicknessFine = 0.05f;

        [Tooltip("Reflection decay with distance to reflective point")]
        public float decay = 2f;

        [Range(0, 1)]
        [Tooltip("Reduces reflection based on view angle")]
        public float fresnel = 0.75f;

        [Min(0)]
        [Tooltip("Ray dispersion with distance")]
        public float fuzzyness;

        [Tooltip("Makes sharpen reflections near objects")]
        public float contactHardening;


        [Tooltip("Jitter helps smoothing edges")]
        [Range(0, 1f)] public float jitter = 0.3f;

        [Header("Special Effects")]
        public Vector2 uvDistortionSpeed;

        [NonSerialized]
        public readonly List<SSR_Renderer> ssrRenderers = new List<SSR_Renderer>();

        [NonSerialized]
        public readonly List<Renderer> renderers = new List<Renderer>();

        public readonly static List<Reflections> instances = new List<Reflections>();
        public static bool needUpdateMaterials;
        public static Reflections currentEditingReflections;


        void OnEnable() {
            if ((int)smoothnessSource == 2) // migration from old Custom value (2 -> 0)
            {
                smoothnessSource = SmoothnessSource.Custom;
            }
            if (maxRayLength == 0) {
                maxRayLength = Mathf.Max(0.1f, stepSize * sampleCount);
            }
            Refresh();
            instances.Add(this);
        }

        void OnDisable() {
            if (instances != null) {
                int k = instances.IndexOf(this);
                if (k >= 0) {
                    instances.RemoveAt(k);
                }
            }
        }

        private void OnValidate() {
            fuzzyness = Mathf.Max(0, fuzzyness);
            thickness = Mathf.Max(0.01f, thickness);
            thicknessFine = Mathf.Max(0.01f, thicknessFine);
            contactHardening = Mathf.Max(0, contactHardening);
            decay = Mathf.Max(1f, decay);
            if (maxRayLength == 0) {
                maxRayLength = stepSize * sampleCount;
            }
            maxRayLength = Mathf.Max(0.1f, maxRayLength);
            Refresh();
        }

        private void OnDestroy() {
            if (ssrRenderers == null) return;
            for (int k = 0; k < ssrRenderers.Count; k++) {
                Material[] mats = ssrRenderers[k].ssrMaterials;
                if (mats != null) {
                    for (int j = 0; j < mats.Length; j++) {
                        if (mats[j] != null) DestroyImmediate(mats[j]);
                    }
                }
            }
        }

        public void UpdateMaterialPropertes() {
            int ssrCount = ssrRenderers.Count;
            for (int k = 0; k < ssrCount; k++) {
                SSR_Renderer ssr = ssrRenderers[k];
                if (ssr != null) {
                    ssr.UpdateMaterialProperties();
                }
            }
        }

        /// <summary>
        /// Updates the list of included renderers to receive reflections
        /// </summary>
        public void Refresh() {

            switch (scope) {
                case Scope.OnlyThisObject: {
                        renderers.Clear();
                        Renderer r = GetComponent<Renderer>();
                        if (r != null) {
                            renderers.Add(r);
                        }
                    }
                    break;
                case Scope.IncludeChildren:
                    GetComponentsInChildren(true, renderers);
                    // Check if some children has an exclusive reflections component
                    int rCount = renderers.Count;
                    bool usesNameFilter = !string.IsNullOrEmpty(nameFilter);
                    for (int k = 0; k < rCount; k++) {
                        Renderer r = renderers[k];
                        if (((1 << r.gameObject.layer) & layerMask) == 0) {
                            renderers[k] = null;
                            continue;
                        }
                        if (usesNameFilter) {
                            if (!r.name.Contains(nameFilter)) {
                                renderers[k] = null;
                                continue;
                            }
                        }
                        Reflections refl = r.GetComponent<Reflections>();
                        if (refl != null && refl != this) {
                            renderers[k] = null;
                        }
                    }
                    break;
            }

            // Prepare required ssr materials slots
            ssrRenderers.Clear();
            if (ignore) return;

            int renderersCount = renderers.Count;
            for (int k = 0; k < renderersCount; k++) {
                Renderer r = renderers[k];
                if (r == null) continue;
                Reflections refl = r.GetComponent<Reflections>();
                if (refl != null && refl.ignore) {
                    continue;
                }
                SSR_Renderer ssr = new SSR_Renderer {
                    renderer = r
                };
                ssrRenderers.Add(ssr);
            }
        }



        public class SSR_Renderer {
            public Renderer renderer;
            public Material[] ssrMaterials;
            readonly public List<Material> originalMaterials = new List<Material>();
            readonly List<Material> tempMaterials = new List<Material>();
            public bool isInitialized;
            public bool exclude;
            public Collider collider;
            public bool hasStaticBounds;
            public Bounds staticBounds;
            
            public void Init(Material ssrMat) {
                isInitialized = true;
                renderer.GetSharedMaterials(originalMaterials);
                collider = renderer.GetComponent<Collider>();
                hasStaticBounds = false;
                if (renderer.isPartOfStaticBatch && collider == null) {
                    MeshFilter mf = renderer.GetComponent<MeshFilter>();
                    if (mf != null) {
                        Mesh mesh = mf.sharedMesh;
                        if (mesh != null) {
                            int subMeshStartIndex = ((MeshRenderer)renderer).subMeshStartIndex;
                            staticBounds = mesh.GetSubMesh(subMeshStartIndex).bounds;
                            int subMeshesCount = originalMaterials.Count;
                            for (int k = 1; k < subMeshesCount; k++) {
                                staticBounds.Encapsulate(mesh.GetSubMesh(subMeshStartIndex + k).bounds);
                            }
                            hasStaticBounds = true;
                        }
                    }
                }
                if (ssrMaterials == null || ssrMaterials.Length != originalMaterials.Count) {
                    ssrMaterials = new Material[originalMaterials.Count];
                }
                for (int k = 0; k < ssrMaterials.Length; k++) {
                    if (ssrMaterials[k] != null) {
                        DestroyImmediate(ssrMaterials[k]);
                    }
                    ssrMaterials[k] = Instantiate(ssrMat);
                }
            }

            public void CheckMaterialChanges(Material shinyMat) {
                if (!isInitialized || renderer == null) return;
                renderer.GetSharedMaterials(tempMaterials);
                int tempMaterialsCount = tempMaterials.Count;
                if (tempMaterialsCount != originalMaterials.Count) {
                    Init(shinyMat);
                    return;
                }
                for(int k=0;k<tempMaterialsCount;k++) {
                    if (tempMaterials[k] != originalMaterials[k]) {
                        Init(shinyMat);
                        return;
                    };
                }
            }

            /// <summary>
            /// Forces a material update in next render frame
            /// </summary>
            public void UpdateMaterialProperties() {
                isInitialized = false;
            }


            public void UpdateMaterialProperties(Reflections go, ShinyScreenSpaceRaytracedReflections globalSettings) {
                if (go.subMeshSettings == null) {
                    go.subMeshSettings = new SubMeshSettingsData[0];
                }
                float totalReflectivity = 0;
                int ssrMaterialsCount = ssrMaterials != null ? ssrMaterials.Length : 0;
                NormalSource normalSource = go.normalSource;
                if (ShinySSRR.isDeferredActive) normalSource = NormalSource.Material;
                for (int s = 0; s < ssrMaterialsCount; s++) {
                    Material ssrMat = ssrMaterials[s];
                    Material originalMaterial = originalMaterials[s];

                    Texture metallicGlossMap = go.customSmoothnessMap;
                    float smoothness = go.smoothness;
                    float reflectivity = go.metallic;
                    if (go.perSubMeshSmoothness) {
                        if (s < go.subMeshSettings.Length) {
                            reflectivity = go.subMeshSettings[s].metallic;
                            smoothness = go.subMeshSettings[s].smoothness;
                        }
                    }
                    Texture normalMap = go.normalMap;
                    float normalMapScale = go.normalMapStrength;

                    bool hasSmoothnessMap = false;
                    bool hasNormalMap = false;

                    if (originalMaterial != null) {
                        if (go.smoothnessSource == SmoothnessSource.Material) {
                            if (!string.IsNullOrEmpty(go.materialReflectivityPropertyName) && originalMaterial.HasProperty(go.materialReflectivityPropertyName)) {
                                reflectivity = originalMaterial.GetFloat(go.materialReflectivityPropertyName);
                            } else if (originalMaterial.HasProperty(ShaderParams.Reflectivity)) {
                                reflectivity = originalMaterial.GetFloat(ShaderParams.Reflectivity);
                            }
                            if (!string.IsNullOrEmpty(go.materialSmoothnessIntensityPropertyName) && originalMaterial.HasProperty(go.materialSmoothnessIntensityPropertyName)) {
                                smoothness = originalMaterial.GetFloat(go.materialSmoothnessIntensityPropertyName);
                            } else if (originalMaterial.HasProperty(ShaderParams.Smoothness)) {
                                smoothness = originalMaterial.GetFloat(ShaderParams.Smoothness);
                            }
                            if (!string.IsNullOrEmpty(go.materialSmoothnessMapPropertyName) && originalMaterial.HasProperty(go.materialSmoothnessMapPropertyName)) {
                                metallicGlossMap = originalMaterial.GetTexture(go.materialSmoothnessMapPropertyName);
                            } else if (originalMaterial.HasProperty(ShaderParams.MetallicGlossMap)) {
                                metallicGlossMap = originalMaterial.GetTexture(ShaderParams.MetallicGlossMap);
                            }
                        } else {
                            // color alpha can also influence smoothness
                            if (originalMaterial.HasProperty(ShaderParams.Color)) {
                                smoothness *= originalMaterial.GetColor(ShaderParams.Color).a;
                            } else if (originalMaterial.HasProperty(ShaderParams.BaseColor)) {
                                smoothness *= originalMaterial.GetColor(ShaderParams.BaseColor).a;
                            }
                        }
                        if (metallicGlossMap != null) {
                            ssrMat.SetTexture(ShaderParams.SmoothnessMap, metallicGlossMap);
                            ssrMat.EnableKeyword(ShaderParams.SKW_SMOOTHNESSMAP);
                            hasSmoothnessMap = true;
                        }

                        if (normalSource == NormalSource.Material) {
                            if (!string.IsNullOrEmpty(go.materialNormalMapPropertyName) && originalMaterial.HasProperty(go.materialNormalMapPropertyName)) {
                                normalMap = originalMaterial.GetTexture(go.materialNormalMapPropertyName);
                            } else if (originalMaterial.HasProperty(ShaderParams.BumpMap)) {
                                normalMap = originalMaterial.GetTexture(ShaderParams.BumpMap);
                            }
                            if (!string.IsNullOrEmpty(go.materialNormalMapScalePropertyName) && originalMaterial.HasProperty(go.materialNormalMapScalePropertyName)) {
                                normalMapScale = originalMaterial.GetFloat(ShaderParams.BumpMap_Scale);
                            }
                        }

                        if (normalSource != NormalSource.ScreenSpaceNormals && normalMap != null) {
                            ssrMat.SetTexture(ShaderParams.BumpMap, normalMap);
                            if (originalMaterial.HasProperty(ShaderParams.BaseMap_ST)) {
                                ssrMat.SetVector(ShaderParams.BumpMap_ST, originalMaterial.GetVector(ShaderParams.BaseMap_ST));
                            }
                            ssrMat.SetFloat(ShaderParams.BumpMap_Scale, normalMapScale);
                            ssrMat.EnableKeyword(ShaderParams.SKW_NORMALMAP);
                            hasNormalMap = true;
                        }

                    }

                    if (!hasSmoothnessMap) {
                        ssrMat.DisableKeyword(ShaderParams.SKW_SMOOTHNESSMAP);
                    }
                    if (!hasNormalMap) {
                        ssrMat.DisableKeyword(ShaderParams.SKW_NORMALMAP);
                    }
                    if (normalSource == NormalSource.ScreenSpaceNormals && ShinySSRR.isUsingScreenSpaceNormals) {
                        ssrMat.EnableKeyword(ShaderParams.SKW_SCREEN_SPACE_NORMALS);
                    }
                    if (globalSettings.reflectionsWorkflow == ReflectionsWorkflow.MetallicAndSmoothness) {
                        totalReflectivity += reflectivity;
                        ssrMat.EnableKeyword(ShaderParams.SKW_METALLIC_WORKFLOW);
                    } else {
                        totalReflectivity += smoothness;
                        ssrMat.DisableKeyword(ShaderParams.SKW_METALLIC_WORKFLOW);
                    }

                    if (go.overrideGlobalSettings) {
                        if (go.jitter > 0) {
                            ssrMat.EnableKeyword(ShaderParams.SKW_JITTER);
                        } else {
                            ssrMat.DisableKeyword(ShaderParams.SKW_JITTER);
                        }
                        if (go.refineThickness) {
                            ssrMat.EnableKeyword(ShaderParams.SKW_REFINE_THICKNESS);
                        } else {
                            ssrMat.DisableKeyword(ShaderParams.SKW_REFINE_THICKNESS);
                        }
                        ssrMat.SetVector(ShaderParams.SSRSettings5, new Vector4(go.thicknessFine * go.thickness, globalSettings.smoothnessThreshold.value, globalSettings.skyboxIntensity.value, 0));
                        ssrMat.SetVector(ShaderParams.SSRSettings2, new Vector4(go.jitter, go.contactHardening + 0.0001f, globalSettings.reflectionsMultiplier.value, reflectivity));
                        ssrMat.SetVector(ShaderParams.SSRSettings, new Vector4(go.thickness, go.sampleCount, go.binarySearchIterations, go.maxRayLength));
                        ssrMat.SetVector(ShaderParams.MaterialData, new Vector4(smoothness, go.fresnel, go.fuzzyness + 1f, go.decay));
                    } else {
                        if (globalSettings.jitter.value > 0) {
                            ssrMat.EnableKeyword(ShaderParams.SKW_JITTER);
                        } else {
                            ssrMat.DisableKeyword(ShaderParams.SKW_JITTER);
                        }
                        if (globalSettings.refineThickness.value) {
                            ssrMat.EnableKeyword(ShaderParams.SKW_REFINE_THICKNESS);
                        } else {
                            ssrMat.DisableKeyword(ShaderParams.SKW_REFINE_THICKNESS);
                        }
                        ssrMat.SetVector(ShaderParams.SSRSettings5, new Vector4(globalSettings.thicknessFine.value * globalSettings.thickness.value, globalSettings.smoothnessThreshold.value, globalSettings.skyboxIntensity.value, 0));
                        ssrMat.SetVector(ShaderParams.SSRSettings2, new Vector4(globalSettings.jitter.value, globalSettings.contactHardening.value + 0.0001f, globalSettings.reflectionsMultiplier.value, reflectivity));
                        ssrMat.SetVector(ShaderParams.SSRSettings, new Vector4(globalSettings.thickness.value, globalSettings.sampleCount.value, globalSettings.binarySearchIterations.value, globalSettings.maxRayLength.value));
                        ssrMat.SetVector(ShaderParams.MaterialData, new Vector4(smoothness, globalSettings.fresnel.value, globalSettings.fuzzyness.value + 1f, globalSettings.decay.value));
                    }
                    ssrMat.SetVector(ShaderParams.DistortionData, new Vector4(go.uvDistortionSpeed.x, go.uvDistortionSpeed.y, 0, 0));
                }
                exclude = (totalReflectivity == 0);
            }
        }

        private void OnDrawGizmos() {
            for (int k = 0; k < ssrRenderers.Count; k++) {
                Bounds bounds = ssrRenderers[k].staticBounds;
                Gizmos.DrawWireCube(bounds.center, bounds.size);
            }

        }


        public void ApplyRaytracingPreset(RaytracingPreset preset) {
            switch (preset) {
                case RaytracingPreset.Fast:
                    sampleCount = 16;
                    maxRayLength = 6;
                    binarySearchIterations = 4;
                    thickness = Mathf.Max(0.5f, thickness);
                    refineThickness = false;
                    jitter = 0.3f;
                    break;
                case RaytracingPreset.Medium:
                    sampleCount = 24;
                    maxRayLength = 12;
                    thickness = Mathf.Max(0.5f, thickness);
                    binarySearchIterations = 5;
                    refineThickness = false;
                    break;
                case RaytracingPreset.High:
                    sampleCount = 48;
                    maxRayLength = 24;
                    binarySearchIterations = 6;
                    thickness = Mathf.Max(0.5f, thickness);
                    refineThickness = false;
                    thicknessFine = 0.05f;
                    break;
                case RaytracingPreset.Superb:
                    sampleCount = 128;
                    maxRayLength = 48;
                    binarySearchIterations = 7;
                    thickness = Mathf.Max(0.5f, thickness);
                    refineThickness = true;
                    thicknessFine = 0.05f;
                    break;
                case RaytracingPreset.Ultra:
                    sampleCount = 200;
                    maxRayLength = 64;
                    binarySearchIterations = 8;
                    thickness = Mathf.Max(0.5f, thickness);
                    refineThickness = true;
                    thicknessFine = 0.05f;
                    break;
            }
            Refresh();
        }
    }
}


