/// <summary>
/// Shiny SSR - Screen Space Reflections for URP - (c) Kronnect
/// </summary>

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
#if UNITY_2023_3_OR_NEWER
using UnityEngine.Rendering.RenderGraphModule;
#endif

namespace ShinySSRR {

    public class ShinySSRR : ScriptableRendererFeature {

        public class SmoothnessMetallicPass : ScriptableRenderPass {

            const string m_ProfilerTag = "Shiny SSR Smoothness Metallic Pass";

            static class ShaderParams {
                public const string SmoothnessMetallicRTName = "_SmoothnessMetallicRT";
                public static int SmoothnessMetallicRT = Shader.PropertyToID(SmoothnessMetallicRTName);
            }

            FilteringSettings filterSettings;
            readonly List<ShaderTagId> shaderTagIdList = new List<ShaderTagId>();
            RTHandle smootnessMetallicRT;

            public SmoothnessMetallicPass() {
                RenderTargetIdentifier rti = new RenderTargetIdentifier(ShaderParams.SmoothnessMetallicRT, 0, CubemapFace.Unknown, -1);
                smootnessMetallicRT = RTHandles.Alloc(rti, name: ShaderParams.SmoothnessMetallicRTName);
                shaderTagIdList.Add(new ShaderTagId("SmoothnessMetallic"));
                filterSettings = new FilteringSettings(RenderQueueRange.opaque);
            }

#if UNITY_2023_3_OR_NEWER
            [Obsolete]
#endif
            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor) {

                RenderTextureDescriptor desc = cameraTextureDescriptor;
                desc.colorFormat = RenderTextureFormat.RGHalf; // r = smoothness, g = metallic
                desc.depthBufferBits = 24;
                desc.msaaSamples = 1;

                cmd.GetTemporaryRT(ShaderParams.SmoothnessMetallicRT, desc, FilterMode.Point);
                cmd.SetGlobalTexture(ShaderParams.SmoothnessMetallicRT, smootnessMetallicRT);
                ConfigureTarget(smootnessMetallicRT);
                ConfigureClear(ClearFlag.All, Color.black);
            }

#if UNITY_2023_3_OR_NEWER
            [Obsolete]
#endif
            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {

                CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);

                SortingCriteria sortingCriteria = SortingCriteria.CommonOpaque;
                var drawSettings = CreateDrawingSettings(shaderTagIdList, ref renderingData, sortingCriteria);
                context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref filterSettings);

                context.ExecuteCommandBuffer(cmd);

                CommandBufferPool.Release(cmd);
            }

#if UNITY_2023_3_OR_NEWER
            class PassData {
                public RendererListHandle rendererListHandle;
                public UniversalCameraData cameraData;
            }

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData) {

                using (var builder = renderGraph.AddUnsafePass<PassData>(m_ProfilerTag, out var passData)) {

                    builder.AllowPassCulling(false);

                    UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
                    passData.cameraData = cameraData;
                    UniversalLightData lightData = frameData.Get<UniversalLightData>();
                    UniversalRenderingData renderingData = frameData.Get<UniversalRenderingData>();

                    SortingCriteria sortingCriteria = SortingCriteria.CommonOpaque;
                    var drawingSettings = CreateDrawingSettings(shaderTagIdList, renderingData, cameraData, lightData, sortingCriteria);

                    RendererListParams listParams = new RendererListParams(renderingData.cullResults, drawingSettings, filterSettings);
                    passData.rendererListHandle = renderGraph.CreateRendererList(listParams);
                    builder.UseRendererList(passData.rendererListHandle);

                    builder.SetRenderFunc((PassData passData, UnsafeGraphContext context) => {

                        RenderTextureDescriptor desc = passData.cameraData.cameraTargetDescriptor;
                        desc.colorFormat = RenderTextureFormat.RGHalf; // r = smoothness, g = metallic
                        desc.depthBufferBits = 24;
                        desc.msaaSamples = 1;

                        CommandBuffer cmd = CommandBufferHelpers.GetNativeCommandBuffer(context.cmd);

                        cmd.GetTemporaryRT(ShaderParams.SmoothnessMetallicRT, desc, FilterMode.Point);
                        cmd.SetGlobalTexture(ShaderParams.SmoothnessMetallicRT, smootnessMetallicRT);

                        RenderTargetIdentifier rti = new RenderTargetIdentifier(ShaderParams.SmoothnessMetallicRT, 0, CubemapFace.Unknown, -1);
                        cmd.SetRenderTarget(rti);
                        cmd.ClearRenderTarget(true, true, Color.black);

                        cmd.DrawRendererList(passData.rendererListHandle);

                    });
                }
            }
#endif


            public override void FrameCleanup(CommandBuffer cmd) {
                if (cmd == null) return;
                cmd.ReleaseTemporaryRT(ShaderParams.SmoothnessMetallicRT);
            }

            public void CleanUp() {
                RTHandles.Release(smootnessMetallicRT);
            }
        }

        class SSRPass : ScriptableRenderPass {

            enum Pass {
                CopyExact = 0,
                SSRSurf = 1,
                Resolve = 2,
                BlurHoriz = 3,
                BlurVert = 4,
                Debug = 5,
                Combine = 6,
                CombineWithCompare = 7,
                GBuffPass = 8,
                Copy = 9,
                TemporalAccum = 10,
                DebugDepth = 11,
                DebugNormals = 12,
                CopyDepth = 13
            }

            const string SHINY_CBUFNAME = "Shiny SSR";
            const float GOLDEN_RATIO = 0.618033989f;
            ScriptableRenderer renderer;
            static Material mat, matExclude;
            Texture noiseTex;

            [NonSerialized]
            static public ShinyScreenSpaceRaytracedReflections settings;

            static ShinySSRR ssrFeature;
            static readonly Plane[] frustumPlanes = new Plane[6];
            const int MIP_COUNT = 5;
            static int[] rtPyramid;
            static readonly Dictionary<Camera, RenderTexture> prevs = new Dictionary<Camera, RenderTexture>();
            Texture2D metallicGradientTex, smoothnessGradientTex;

            class PassData {
                public CommandBuffer cmd;
                public Camera cam;
                public RenderTextureDescriptor sourceDesc;
#if UNITY_2022_2_OR_NEWER
                public RTHandle source;
#else
                public RenderTargetIdentifier source;
#endif
#if UNITY_2023_3_OR_NEWER
                public TextureHandle colorTexture, depthTexture, cameraNormalsTexture, motionVectorTexture;
#endif
            }

            readonly PassData passData = new PassData();

            public bool Setup(ScriptableRenderer renderer, ShinySSRR ssrFeature) {

                settings = VolumeManager.instance.stack.GetComponent<ShinyScreenSpaceRaytracedReflections>();
                if (settings == null || !settings.IsActive()) return false;
                SSRPass.ssrFeature = ssrFeature;
                this.renderer = renderer;
                this.renderPassEvent = ssrFeature.renderPassEvent;
                if (mat == null) {
                    Shader shader = Shader.Find("Hidden/Kronnect/SSR_URP");
                    mat = CoreUtils.CreateEngineMaterial(shader);
                }
                if (matExclude == null) {
                    Shader shader = Shader.Find("Hidden/Kronnect/SSR_Exclude");
                    if (shader != null) matExclude = CoreUtils.CreateEngineMaterial(shader);
                }
                if (noiseTex == null) {
                    noiseTex = Resources.Load<Texture>("SSR/blueNoiseSSR64");
                }
                mat.SetTexture(ShaderParams.NoiseTex, noiseTex);

                // set global settings
                mat.SetVector(ShaderParams.SSRSettings2, new Vector4(settings.jitter.value, settings.contactHardening.value + 0.0001f, settings.reflectionsMultiplier.value, 0));
                mat.SetVector(ShaderParams.SSRSettings4, new Vector4(settings.separationPos.value, settings.reflectionsMinIntensity.value, settings.reflectionsMaxIntensity.value, settings.specularSoftenPower.value));
                mat.SetVector(ShaderParams.SSRBlurStrength, new Vector4(settings.blurStrength.value.x, settings.blurStrength.value.y, settings.vignetteSize.value, settings.vignettePower.value));
                mat.SetVector(ShaderParams.SSRSettings5, new Vector4(settings.thicknessFine.value * settings.thickness.value, settings.smoothnessThreshold.value, settings.skyboxIntensity.value, 0));
                float metallicBoost = settings.metallicBoost.value;
                mat.SetVector(ShaderParams.SSRSettings6, new Vector4(settings.nearCameraAttenuationStart.value, settings.nearCameraAttenuationRange.value, metallicBoost, 0));
                if (settings.specularControl.value) {
                    mat.EnableKeyword(ShaderParams.SKW_DENOISE);
                } else {
                    mat.DisableKeyword(ShaderParams.SKW_DENOISE);
                }
                mat.SetFloat(ShaderParams.MinimumBlur, settings.minimumBlur.value);
                mat.SetInt(ShaderParams.StencilValue, settings.stencilValue.value);
                mat.SetInt(ShaderParams.StencilCompareFunction, settings.stencilCheck.value ? (int)settings.stencilCompareFunction.value : (int)CompareFunction.Always);

                mat.DisableKeyword(ShaderParams.SKW_METALLIC_BOOST);
                if (settings.reflectionStyle.value == ReflectionStyle.DarkReflections) {
                    mat.EnableKeyword(ShaderParams.SKW_DARK_REFLECTIONS);
                } else {
                    mat.DisableKeyword(ShaderParams.SKW_DARK_REFLECTIONS);
                    if (metallicBoost > 0 && ssrFeature.useDeferred) {
                        mat.EnableKeyword(ShaderParams.SKW_METALLIC_BOOST);
                    }
                }

                if (settings.computeBackFaces.value) {
                    Shader.EnableKeyword(ShaderParams.SKW_BACK_FACES);
                    Shader.SetGlobalFloat(ShaderParams.MinimumThickness, settings.thicknessMinimum.value);
                } else {
                    Shader.DisableKeyword(ShaderParams.SKW_BACK_FACES);
                }

                if (settings.skyboxIntensity.value > 0) {
                    Shader.EnableKeyword(ShaderParams.SKW_SKYBOX);
                } else {
                    Shader.DisableKeyword(ShaderParams.SKW_SKYBOX);
                }

                if (ssrFeature.useDeferred || ssrFeature.customSmoothnessMetallicPass) {
                    if (settings.jitter.value > 0) {
                        mat.EnableKeyword(ShaderParams.SKW_JITTER);
                    } else {
                        mat.DisableKeyword(ShaderParams.SKW_JITTER);
                    }
                    if (settings.refineThickness.value) {
                        mat.EnableKeyword(ShaderParams.SKW_REFINE_THICKNESS);
                    } else {
                        mat.DisableKeyword(ShaderParams.SKW_REFINE_THICKNESS);
                    }
                    if (ssrFeature.customSmoothnessMetallicPass) {
                        mat.EnableKeyword(ShaderParams.SKW_CUSTOM_SMOOTHNESS_METALLIC_PASS);
                    } else {
                        mat.DisableKeyword(ShaderParams.SKW_CUSTOM_SMOOTHNESS_METALLIC_PASS);
                    }
                    mat.SetVector(ShaderParams.SSRSettings, new Vector4(settings.thickness.value, settings.sampleCount.value, settings.binarySearchIterations.value, settings.maxRayLength.value));
                    mat.SetVector(ShaderParams.MaterialData, new Vector4(0, settings.fresnel.value, settings.fuzzyness.value + 1f, settings.decay.value));
                }

                if (settings.reflectionsWorkflow.value == ReflectionsWorkflow.MetallicAndSmoothness) {
                    mat.EnableKeyword(ShaderParams.SKW_METALLIC_WORKFLOW);
                    UpdateGradientTextures();
                } else {
                    mat.DisableKeyword(ShaderParams.SKW_METALLIC_WORKFLOW);
                }

                if (rtPyramid == null || rtPyramid.Length != MIP_COUNT) {
                    rtPyramid = new int[MIP_COUNT];
                    for (int k = 0; k < rtPyramid.Length; k++) {
                        rtPyramid[k] = Shader.PropertyToID("_BlurRTMip" + k);
                    }
                }

                return true;
            }


            void UpdateGradientTextures() {
                UpdateGradientTexture(ref metallicGradientTex, settings.reflectionsIntensityCurve.value, ref ShinyScreenSpaceRaytracedReflections.metallicGradientCachedId);
                UpdateGradientTexture(ref smoothnessGradientTex, settings.reflectionsSmoothnessCurve.value, ref ShinyScreenSpaceRaytracedReflections.smoothnessGradientCachedId);
                Shader.SetGlobalTexture(ShaderParams.MetallicGradientTex, metallicGradientTex);
                Shader.SetGlobalTexture(ShaderParams.SmoothnessGradientTex, smoothnessGradientTex);
            }

            Color[] colors;
            void UpdateGradientTexture(ref Texture2D tex, AnimationCurve curve, ref float cachedId) {
                if (colors == null || colors.Length != 256) {
                    colors = new Color[256];
                    cachedId = -1;
                }
                if (tex == null) {
                    tex = new Texture2D(256, 1, TextureFormat.RHalf, false, true);
                    cachedId = -1;
                }
                // quick test, evaluate 3 curve points
                float sum = curve.Evaluate(0) + curve.Evaluate(0.5f) + curve.Evaluate(1f) + 1;
                if (sum == cachedId) return;
                cachedId = sum;

                for (int k = 0; k < 256; k++) {
                    float t = (float)k / 255;
                    float v = curve.Evaluate(t);
                    colors[k].r = v;
                }

                tex.SetPixels(colors);
                tex.Apply();
            }


#if UNITY_2023_3_OR_NEWER
            [Obsolete]
#endif
            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor) {
                ConfigureInput(GetRequiredInputs());
            }


            ScriptableRenderPassInput GetRequiredInputs() {
                ScriptableRenderPassInput inputs = ScriptableRenderPassInput.Depth;
                if (ssrFeature.enableScreenSpaceNormalsPass && !ssrFeature.useDeferred) {
                    inputs |= ScriptableRenderPassInput.Normal;
                }
#if UNITY_2021_3_OR_NEWER
                if (settings.temporalFilter.value && Application.isPlaying) {
                    inputs |= ScriptableRenderPassInput.Motion;
                }
#endif
                return inputs;
            }


#if UNITY_2023_3_OR_NEWER
            [Obsolete]
#endif
            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {

                Camera cam = renderingData.cameraData.camera;

                // ignore SceneView depending on setting
                if (cam.cameraType == CameraType.SceneView) {
                    if (!settings.showInSceneView.value) return;
                } else {
                    // ignore any camera other than GameView
                    if (cam.cameraType != CameraType.Game) return;
                }

                // execute reflections passes
                CommandBuffer cmd = CommandBufferPool.Get(SHINY_CBUFNAME);
                passData.cmd = cmd;
                passData.cam = renderingData.cameraData.camera;
                passData.sourceDesc = renderingData.cameraData.cameraTargetDescriptor;
#if UNITY_2022_2_OR_NEWER
                passData.source = renderer.cameraColorTargetHandle;
#else
                passData.source = renderer.cameraColorTarget;
#endif
                ExecuteInternal(passData);
                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }

#if UNITY_2023_3_OR_NEWER

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData) {

                using (var builder = renderGraph.AddUnsafePass<PassData>("Radiant GI RG Pass", out var passData)) {

                    builder.AllowPassCulling(false);

                    ScriptableRenderPassInput inputs = GetRequiredInputs();
                    ConfigureInput(inputs);

                    UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
                    passData.cam = cameraData.camera;
                    passData.sourceDesc = cameraData.cameraTargetDescriptor;

                    UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

                    passData.colorTexture = resourceData.activeColorTexture;
                    builder.UseTexture(resourceData.activeColorTexture, AccessFlags.ReadWrite);
                    passData.depthTexture = resourceData.cameraDepthTexture;
                    builder.UseTexture(resourceData.cameraDepthTexture, AccessFlags.Read);

                    if ((inputs & ScriptableRenderPassInput.Normal) != 0) {
                        builder.UseTexture(resourceData.cameraNormalsTexture, AccessFlags.Read);
                        passData.cameraNormalsTexture = resourceData.cameraNormalsTexture;
                    }
                    if ((inputs & ScriptableRenderPassInput.Motion) != 0) {
                        builder.UseTexture(resourceData.motionVectorColor, AccessFlags.Read);
                        passData.motionVectorTexture = resourceData.motionVectorColor;
                    }

                    builder.SetRenderFunc((PassData passData, UnsafeGraphContext context) => {

                        if (passData.depthTexture.IsValid()) {
                            mat.SetTexture(ShaderParams.CameraDepthTexture, passData.depthTexture);
                        }
                        if (passData.cameraNormalsTexture.IsValid()) {
                            mat.SetTexture(ShaderParams.CameraNormalsTexture, passData.cameraNormalsTexture);
                        }
                        if (passData.motionVectorTexture.IsValid()) {
                            mat.SetTexture(ShaderParams.MotionVectorTexture, passData.motionVectorTexture);
                        }

                        passData.source = passData.colorTexture;
                        passData.cmd = CommandBufferHelpers.GetNativeCommandBuffer(context.cmd);
                        ExecuteInternal(passData);
                    });
                }
            }

#endif


            static void ExecuteInternal(PassData passData) {

                RenderTextureDescriptor sourceDesc = passData.sourceDesc;
                sourceDesc.colorFormat = settings.lowPrecision.value ? RenderTextureFormat.ARGB32 : RenderTextureFormat.ARGBHalf;
                sourceDesc.width /= settings.downsampling.value;
                sourceDesc.height /= settings.downsampling.value;
                sourceDesc.msaaSamples = 1;

                float goldenFactor = GOLDEN_RATIO;
                if (settings.animatedJitter.value) {
                    goldenFactor *= (Time.frameCount % 480);
                }
                Shader.SetGlobalVector(ShaderParams.SSRSettings3, new Vector4(sourceDesc.width, sourceDesc.height, goldenFactor, settings.depthBias.value));

#if UNITY_2022_2_OR_NEWER
                RTHandle source = passData.source;
#else
                RenderTargetIdentifier source = passData.source;
#endif
                CommandBuffer cmd = passData.cmd;
                Camera cam = passData.cam;

                bool useReflectionsScripts = true;
                int count = Reflections.instances.Count;
                bool usingDeferredPass = ssrFeature.useDeferred && !settings.skipDeferredPass.value;
                if (usingDeferredPass || ssrFeature.customSmoothnessMetallicPass) {
                    useReflectionsScripts = settings.useReflectionsScripts.value && !ssrFeature.customSmoothnessMetallicPass;

                    ComputeDepth(cmd, sourceDesc);

                    // pass camera matrices
                    mat.SetMatrix(ShaderParams.WorldToViewDir, cam.worldToCameraMatrix);
                    mat.SetMatrix(ShaderParams.ViewToWorldDir, cam.cameraToWorldMatrix);

                    if (settings.skyboxIntensity.value > 0 && settings.skyboxContributionPass.value != SkyboxContributionPass.Forward) {
                        cmd.EnableShaderKeyword(ShaderParams.SKW_SKYBOX);
                    } else {
                        cmd.DisableShaderKeyword(ShaderParams.SKW_SKYBOX);
                    }

                    if (settings.useCustomBounds.value) {
                        mat.EnableKeyword(ShaderParams.SKW_LIMIT_BOUNDS);
                        mat.SetVector(ShaderParams.BoundsMin, settings.boundsMin.value);
                        mat.SetVector(ShaderParams.BoundsSize, settings.boundsMax.value - settings.boundsMin.value);
                    } else {
                        mat.DisableKeyword(ShaderParams.SKW_LIMIT_BOUNDS);
                    }

                    // prepare ssr target
                    cmd.GetTemporaryRT(ShaderParams.RayCast, sourceDesc, FilterMode.Point);
                    if (count > 0 && useReflectionsScripts) {
                        cmd.SetRenderTarget(ShaderParams.RayCast);
                        cmd.ClearRenderTarget(true, false, new Color(0, 0, 0, 0));
                    }

                    // raytrace using gbuffers
                    FullScreenBlit(cmd, source, ShaderParams.RayCast, Pass.GBuffPass);

                }

                // early exit if no reflection objects
                if (count == 0 && !usingDeferredPass) return;

                if (count > 0 && useReflectionsScripts) {
                    bool firstSSR = !usingDeferredPass;

                    if (settings.skyboxIntensity.value > 0 && settings.skyboxContributionPass.value != SkyboxContributionPass.Deferred) {
                        cmd.EnableShaderKeyword(ShaderParams.SKW_SKYBOX);
                    } else {
                        cmd.DisableShaderKeyword(ShaderParams.SKW_SKYBOX);
                    }

                    GeometryUtility.CalculateFrustumPlanes(cam, frustumPlanes);
                    int reflectionsScriptsLayerMask = settings.reflectionsScriptsLayerMask.value;

                    for (int k = 0; k < count; k++) {
                        Reflections go = Reflections.instances[k];
                        if (go == null || (reflectionsScriptsLayerMask & (1 << go.gameObject.layer)) == 0) continue;
                        int rendererCount = go.ssrRenderers.Count;
                        for (int j = 0; j < rendererCount; j++) {
                            Reflections.SSR_Renderer ssrRenderer = go.ssrRenderers[j];

                            if (Reflections.needUpdateMaterials) {
                                ssrRenderer.CheckMaterialChanges(mat);
                            }

                            Renderer goRenderer = ssrRenderer.renderer;
                            if (goRenderer == null || !goRenderer.isVisible) continue;

                            // if object is part of static batch, check collider bounds (if existing)
                            if (goRenderer.isPartOfStaticBatch) {
                                if (ssrRenderer.hasStaticBounds) {
                                    // check artifically computed bounds
                                    if (!GeometryUtility.TestPlanesAABB(frustumPlanes, ssrRenderer.staticBounds)) continue;
                                } else if (ssrRenderer.collider != null) {
                                    // check if object is visible by current camera using collider bounds
                                    if (!GeometryUtility.TestPlanesAABB(frustumPlanes, ssrRenderer.collider.bounds)) continue;
                                }
                            } else {
                                // check if object is visible by current camera using renderer bounds
                                if (!GeometryUtility.TestPlanesAABB(frustumPlanes, goRenderer.bounds)) continue;
                            }

                            if (!ssrRenderer.isInitialized) {
                                ssrRenderer.Init(mat);
                                ssrRenderer.UpdateMaterialProperties(go, settings);
                            }
#if UNITY_EDITOR
                        else if (!Application.isPlaying) {
                                ssrRenderer.CheckMaterialChanges(mat);
                                ssrRenderer.UpdateMaterialProperties(go, settings);
                            } else if (Reflections.currentEditingReflections == go) {
                                ssrRenderer.UpdateMaterialProperties(go, settings);
                            }
#endif
                            else if (Reflections.needUpdateMaterials) {
                                ssrRenderer.UpdateMaterialProperties(go, settings);
                            }

                            if (ssrRenderer.exclude) continue;

                            if (firstSSR) {
                                firstSSR = false;

                                ComputeDepth(cmd, sourceDesc);

                                // prepare ssr target
                                cmd.GetTemporaryRT(ShaderParams.RayCast, sourceDesc, FilterMode.Point);
                                cmd.SetRenderTarget(ShaderParams.RayCast, 0, CubemapFace.Unknown, -1);
                                cmd.ClearRenderTarget(true, true, new Color(0, 0, 0, 0));
                            }
                            for (int s = 0; s < ssrRenderer.ssrMaterials.Length; s++) {
                                if (go.subMeshMask <= 0 || ((1 << s) & go.subMeshMask) != 0) {
                                    Material ssrMat = ssrRenderer.ssrMaterials[s];
                                    cmd.DrawRenderer(goRenderer, ssrMat, s, (int)Pass.SSRSurf);
                                }
                            }
                        }
                    }

                    Reflections.needUpdateMaterials = false;

                    if (firstSSR) return;
                }


                // Erase excluded objects
                int countExcluded = excludedReflections.Count;
                for (int k = 0; k < countExcluded; k++) {
                    ExcludeReflections er = excludedReflections[k];
                    if (er == null || !er.enabled || er.renderers == null) continue;
                    int rCount = er.renderers.Length;
                    for (int j = 0; j < rCount; j++) {
                        Renderer r = er.renderers[j];
                        if (!r.isVisible) continue;
                        cmd.DrawRenderer(r, matExclude);
                    }
                }

                RenderTargetIdentifier reflectionsTex;
                if (settings.reflectionStyle.value == ReflectionStyle.ShinyReflections) {
                    // Resolve reflections
                    RenderTextureDescriptor copyDesc = sourceDesc;
                    copyDesc.depthBufferBits = 0;

                    if (settings.skyboxIntensity.value > 0) {
                        mat.SetVector(ShaderParams.CameraViewDir, cam.transform.forward);
                        cmd.EnableShaderKeyword(ShaderParams.SKW_SKYBOX);
                    } else {
                        cmd.DisableShaderKeyword(ShaderParams.SKW_SKYBOX);
                    }

                    cmd.GetTemporaryRT(ShaderParams.ReflectionsTex, copyDesc);
                    FullScreenBlit(cmd, source, ShaderParams.ReflectionsTex, Pass.Resolve);
                    RenderTargetIdentifier input = ShaderParams.ReflectionsTex;

#if UNITY_2021_3_OR_NEWER
                    prevs.TryGetValue(cam, out RenderTexture prev);

                    if (settings.temporalFilter.value && cam.cameraType == CameraType.Game && Application.isPlaying) {

                        if (prev != null && (prev.width != copyDesc.width || prev.height != copyDesc.height)) {
                            prev.Release();
                            prev = null;
                        }

                        RenderTextureDescriptor acumDesc = copyDesc;
                        Pass acumPass = Pass.TemporalAccum;

                        if (prev == null) {
                            prev = new RenderTexture(acumDesc);
                            prev.Create();
                            prevs[cam] = prev;
                            acumPass = Pass.Copy;
                        }

                        mat.SetFloat(ShaderParams.TemporalResponseSpeed, settings.temporalFilterResponseSpeed.value);
                        Shader.SetGlobalTexture(ShaderParams.PrevResolveNameId, prev);
                        cmd.GetTemporaryRT(ShaderParams.TempAcum, acumDesc, FilterMode.Bilinear);
                        FullScreenBlit(cmd, ShaderParams.ReflectionsTex, ShaderParams.TempAcum, acumPass);
                        FullScreenBlit(cmd, ShaderParams.TempAcum, prev, Pass.Copy); // do not use CopyExact as its fragment clamps color values - also, cmd.CopyTexture does not work correctly here
                        input = ShaderParams.TempAcum;
                    } else if (prev != null) {
                        prev.Release();
                        DestroyImmediate(prev);
                    }
#endif
                    reflectionsTex = input;

                    // Pyramid blur
                    int blurDownsampling = settings.blurDownsampling.value;
                    copyDesc.width /= blurDownsampling;
                    copyDesc.height /= blurDownsampling;
                    for (int k = 0; k < MIP_COUNT; k++) {
                        copyDesc.width = Mathf.Max(2, copyDesc.width / 2);
                        copyDesc.height = Mathf.Max(2, copyDesc.height / 2);
                        cmd.GetTemporaryRT(rtPyramid[k], copyDesc, FilterMode.Bilinear);
                        cmd.GetTemporaryRT(ShaderParams.BlurRT, copyDesc, FilterMode.Bilinear);
                        FullScreenBlit(cmd, input, ShaderParams.BlurRT, Pass.BlurHoriz);
                        FullScreenBlit(cmd, ShaderParams.BlurRT, rtPyramid[k], Pass.BlurVert);
                        cmd.ReleaseTemporaryRT(ShaderParams.BlurRT);
                        input = rtPyramid[k];
                    }
                } else {
                    reflectionsTex = ShaderParams.RayCast;
                }

                // Output
                int finalPass;
                switch (settings.outputMode.value) {
                    case OutputMode.Final: finalPass = (int)Pass.Combine; break;
                    case OutputMode.SideBySideComparison: finalPass = (int)Pass.CombineWithCompare; break;
                    case OutputMode.DebugDepth: finalPass = (int)Pass.DebugDepth; break;
                    case OutputMode.DebugDeferredNormals: finalPass = (int)Pass.DebugNormals; break;
                    default:
                        finalPass = (int)Pass.Debug; break;
                }
                FullScreenBlit(cmd, reflectionsTex, source, (Pass)finalPass);

                if (settings.stopNaN.value) {
                    RenderTextureDescriptor nanDesc = passData.sourceDesc;
                    nanDesc.depthBufferBits = 0;
                    nanDesc.msaaSamples = 1;
                    cmd.GetTemporaryRT(ShaderParams.NaNBuffer, nanDesc);
                    FullScreenBlit(cmd, source, ShaderParams.NaNBuffer, Pass.CopyExact);
                    FullScreenBlit(cmd, ShaderParams.NaNBuffer, source, Pass.CopyExact);
                }

                // Clean up
                for (int k = 0; k < rtPyramid.Length; k++) {
                    cmd.ReleaseTemporaryRT(rtPyramid[k]);
                }
                cmd.ReleaseTemporaryRT(ShaderParams.ReflectionsTex);
                cmd.ReleaseTemporaryRT(ShaderParams.RayCast);
                cmd.ReleaseTemporaryRT(ShaderParams.DownscaledDepthRT);
            }

            static void ComputeDepth(CommandBuffer cmd, RenderTextureDescriptor desc) {
                desc.colorFormat = settings.computeBackFaces.value ? RenderTextureFormat.RGHalf : RenderTextureFormat.RHalf;
                desc.sRGB = false;
                desc.depthBufferBits = 0;
                cmd.GetTemporaryRT(ShaderParams.DownscaledDepthRT, desc, FilterMode.Point);
                FullScreenBlit(cmd, ShaderParams.DownscaledDepthRT, Pass.CopyDepth);
            }


            static Mesh _fullScreenMesh;

            static Mesh fullscreenMesh {
                get {
                    if (_fullScreenMesh != null) {
                        return _fullScreenMesh;
                    }
                    float num = 1f;
                    float num2 = 0f;
                    Mesh val = new Mesh();
                    _fullScreenMesh = val;
                    _fullScreenMesh.SetVertices(new List<Vector3> {
            new Vector3 (-1f, -1f, 0f),
            new Vector3 (-1f, 1f, 0f),
            new Vector3 (1f, -1f, 0f),
            new Vector3 (1f, 1f, 0f)
        });
                    _fullScreenMesh.SetUVs(0, new List<Vector2> {
            new Vector2 (0f, num2),
            new Vector2 (0f, num),
            new Vector2 (1f, num2),
            new Vector2 (1f, num)
        });
                    _fullScreenMesh.SetIndices(new int[6] { 0, 1, 2, 2, 1, 3 }, (MeshTopology)0, 0, false);
                    _fullScreenMesh.UploadMeshData(true);
                    return _fullScreenMesh;
                }
            }

            static void FullScreenBlit(CommandBuffer cmd, RenderTargetIdentifier destination, Pass pass) {
                cmd.SetRenderTarget(destination, 0, CubemapFace.Unknown, -1);
                cmd.DrawMesh(fullscreenMesh, Matrix4x4.identity, mat, 0, (int)pass);
            }

            static void FullScreenBlit(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, Pass pass) {
                cmd.SetRenderTarget(destination, 0, CubemapFace.Unknown, -1);
                cmd.SetGlobalTexture(ShaderParams.MainTex, source);
                cmd.DrawMesh(fullscreenMesh, Matrix4x4.identity, mat, 0, (int)pass);
            }

            /// Cleanup any allocated resources that were created during the execution of this render pass.
            public override void FrameCleanup(CommandBuffer cmd) {
            }


            public void CleanUp() {
                CoreUtils.Destroy(mat);
                CoreUtils.Destroy(matExclude);
                if (prevs != null) {
                    foreach (RenderTexture rt in prevs.Values) {
                        if (rt != null) {
                            rt.Release();
                            DestroyImmediate(rt);
                        }
                    }
                    prevs.Clear();
                }
                CoreUtils.Destroy(metallicGradientTex);
                CoreUtils.Destroy(smoothnessGradientTex);
            }
        }

        class SSRBackfacesPass : ScriptableRenderPass {

            const string m_ProfilerTag = "Shiny SSR Backfaces Pass";
            const string m_DepthOnlyShader = "Universal Render Pipeline/Unlit";

            static FilteringSettings filterSettings;
            static readonly List<ShaderTagId> shaderTagIdList = new List<ShaderTagId>();
            Material depthOnlyMaterial;
            RTHandle m_Depth;
            ShinyScreenSpaceRaytracedReflections settings;

            public SSRBackfacesPass() {
                RenderTargetIdentifier rti = new RenderTargetIdentifier(ShaderParams.DownscaledBackDepthRT, 0, CubemapFace.Unknown, -1);
                m_Depth = RTHandles.Alloc(rti, name: ShaderParams.DownscaledBackDepthTextureName);
                shaderTagIdList.Clear();
                shaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));
                shaderTagIdList.Add(new ShaderTagId("UniversalForward"));
                shaderTagIdList.Add(new ShaderTagId("LightweightForward"));
                filterSettings = new FilteringSettings(RenderQueueRange.opaque, -1);
            }

            public void Setup(ShinyScreenSpaceRaytracedReflections settings) {
                this.renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
                this.settings = settings;
                filterSettings.layerMask = settings.computeBackFacesLayerMask.value;
            }

#if UNITY_2023_3_OR_NEWER
            [Obsolete]
#endif
            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor) {
                RenderTextureDescriptor depthDesc = cameraTextureDescriptor;
                int downsampling = settings.downsampling.value;
                depthDesc.width = Mathf.CeilToInt(depthDesc.width / downsampling);
                depthDesc.height = Mathf.CeilToInt(depthDesc.height / downsampling);
                depthDesc.colorFormat = RenderTextureFormat.Depth;
                depthDesc.depthBufferBits = 24;
                depthDesc.msaaSamples = 1;

                cmd.GetTemporaryRT(ShaderParams.DownscaledBackDepthRT, depthDesc, FilterMode.Point);
                cmd.SetGlobalTexture(ShaderParams.DownscaledBackDepthRT, m_Depth);
                ConfigureTarget(m_Depth);
                ConfigureClear(ClearFlag.All, Color.black);
            }

#if UNITY_2023_3_OR_NEWER
            [Obsolete]
#endif
            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
                CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                SortingCriteria sortingCriteria = SortingCriteria.CommonOpaque;
                var drawSettings = CreateDrawingSettings(shaderTagIdList, ref renderingData, sortingCriteria);
                drawSettings.perObjectData = PerObjectData.None;
                if (depthOnlyMaterial == null) {
                    Shader depthOnly = Shader.Find(m_DepthOnlyShader);
                    depthOnlyMaterial = new Material(depthOnly);
                }
                drawSettings.overrideMaterial = depthOnlyMaterial;
                context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref filterSettings);

                context.ExecuteCommandBuffer(cmd);

                CommandBufferPool.Release(cmd);
            }

#if UNITY_2023_3_OR_NEWER
            class PassData {
                public RendererListHandle rendererListHandle;
                public RenderTextureDescriptor sourceDesc;
            }

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData) {

                using (var builder = renderGraph.AddUnsafePass<PassData>(m_ProfilerTag, out var passData)) {

                    builder.AllowPassCulling(false);

                    UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
                    UniversalLightData lightData = frameData.Get<UniversalLightData>();
                    UniversalRenderingData renderingData = frameData.Get<UniversalRenderingData>();
                    passData.sourceDesc = cameraData.cameraTargetDescriptor;

                    SortingCriteria sortingCriteria = SortingCriteria.CommonOpaque;
                    var drawingSettings = CreateDrawingSettings(shaderTagIdList, renderingData, cameraData, lightData, sortingCriteria);
                    drawingSettings.perObjectData = PerObjectData.None;
                    if (depthOnlyMaterial == null) {
                        Shader depthOnly = Shader.Find(m_DepthOnlyShader);
                        depthOnlyMaterial = new Material(depthOnly);
                    }
                    drawingSettings.overrideMaterial = depthOnlyMaterial;

                    RendererListParams listParams = new RendererListParams(renderingData.cullResults, drawingSettings, filterSettings);
                    passData.rendererListHandle = renderGraph.CreateRendererList(listParams);
                    builder.UseRendererList(passData.rendererListHandle);

                    builder.SetRenderFunc((PassData passData, UnsafeGraphContext context) => {

                        CommandBuffer cmd = CommandBufferHelpers.GetNativeCommandBuffer(context.cmd);

                        RenderTextureDescriptor depthDesc = passData.sourceDesc;
                        int downsampling = settings.downsampling.value;
                        depthDesc.width = Mathf.CeilToInt(depthDesc.width / downsampling);
                        depthDesc.height = Mathf.CeilToInt(depthDesc.height / downsampling);
                        depthDesc.colorFormat = RenderTextureFormat.Depth;
                        depthDesc.depthBufferBits = 24;
                        depthDesc.msaaSamples = 1;

                        cmd.GetTemporaryRT(ShaderParams.DownscaledBackDepthRT, depthDesc, FilterMode.Point);
                        cmd.SetGlobalTexture(ShaderParams.DownscaledBackDepthRT, m_Depth);
                        RenderTargetIdentifier rti = new RenderTargetIdentifier(ShaderParams.DownscaledBackDepthRT, 0, CubemapFace.Unknown, -1);
                        cmd.SetRenderTarget(rti);
                        cmd.ClearRenderTarget(true, true, Color.black);

                        cmd.DrawRendererList(passData.rendererListHandle);
                    });
                }
            }

#endif

            public override void FrameCleanup(CommandBuffer cmd) {
                if (cmd == null) return;
                cmd.ReleaseTemporaryRT(ShaderParams.DownscaledBackDepthRT);
            }


            public void CleanUp() {
                RTHandles.Release(m_Depth);
            }

        }

        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        [Tooltip("Use deferred g-buffers (requires deferred rendering in URP 12 or later)")]
        public bool useDeferred;

        [Tooltip("Requests screen space normals to be used with Reflections scripts that need them")]
        public bool enableScreenSpaceNormalsPass;

        [Tooltip("Executes a SmoothnessMetallic pass on shaders that support it in forward rendering path")]
        public bool customSmoothnessMetallicPass;

        [Tooltip("Allows Shiny to be executed even if camera has Post Processing option disabled.")]
        public bool ignorePostProcessingOption = true;

        [Tooltip("On which cameras should Shiny effects be applied")]
        public LayerMask cameraLayerMask = -1;

        public static bool installed;
        public static bool isDeferredActive;
        public static bool isSmoothnessMetallicPassActive;
        public static bool isUsingScreenSpaceNormals;
        public static bool isEnabled = true;

        SSRPass renderPass;
        SSRBackfacesPass backfacesPass;
        SmoothnessMetallicPass smoothnessMetallicPass;
        SkyboxBaker skyboxBaker;

        void OnDestroy() {
            installed = false;
            if (renderPass != null) {
                renderPass.CleanUp();
            }
            if (backfacesPass != null) {
                backfacesPass.CleanUp();
            }
            if (smoothnessMetallicPass != null) {
                smoothnessMetallicPass.CleanUp();
            }
            if (skyboxBaker != null) {
                DestroyImmediate(skyboxBaker);
            }
        }

        public override void Create() {
            if (renderPass == null) {
                renderPass = new SSRPass();
            }
            if (backfacesPass == null) {
                backfacesPass = new SSRBackfacesPass();
            }
            if (smoothnessMetallicPass == null) {
                smoothnessMetallicPass = new SmoothnessMetallicPass();
                smoothnessMetallicPass.renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
            }
        }


        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
            installed = true;
            if (!isEnabled) return;

            isDeferredActive = useDeferred;
            isSmoothnessMetallicPassActive = customSmoothnessMetallicPass && !useDeferred;
            isUsingScreenSpaceNormals = enableScreenSpaceNormalsPass && !useDeferred;

            if (!renderingData.postProcessingEnabled && !ignorePostProcessingOption) return;

            if (renderingData.cameraData.cameraTargetDescriptor.colorFormat == RenderTextureFormat.Depth) return;

            Camera cam = renderingData.cameraData.camera;
            if ((cameraLayerMask.value & (1 << cam.gameObject.layer)) == 0) return;

            if (renderPass.Setup(renderer, this)) {
                if (SSRPass.settings.skyboxIntensity.value > 0) {
                    CheckSkyboxBaker();
                }

                if (isSmoothnessMetallicPassActive) {
                    renderer.EnqueuePass(smoothnessMetallicPass);
                }
                if (SSRPass.settings.computeBackFaces.value) {
                    backfacesPass.Setup(SSRPass.settings);
                    renderer.EnqueuePass(backfacesPass);
                }
                renderer.EnqueuePass(renderPass);
            }
        }

        void CheckSkyboxBaker() {
            if (skyboxBaker != null) return;

            skyboxBaker = Misc.FindObjectOfType<SkyboxBaker>(true);
            if (skyboxBaker != null) return;

            Camera cam = Camera.main;
            if (cam == null) return;

            skyboxBaker = cam.gameObject.AddComponent<SkyboxBaker>();
        }

        /// <summary>
        /// Performs a refresh of the Reflections script and the materials used
        /// </summary>
        public void Refresh() {
            Reflections.needUpdateMaterials = true;
        }


        static readonly List<ExcludeReflections> excludedReflections = new List<ExcludeReflections>();

        public static void RegisterExcludeReflections(ExcludeReflections o) {
            if (!excludedReflections.Contains(o)) {
                excludedReflections.Add(o);
            }
        }

        public static void UnregisterExcludeReflections(ExcludeReflections o) {
            if (excludedReflections.Contains(o)) {
                excludedReflections.Remove(o);
            }
        }
    }

}
