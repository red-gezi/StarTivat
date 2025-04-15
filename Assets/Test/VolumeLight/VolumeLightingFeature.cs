using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VolumeLightingFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class LightingEventSetting
    {
        public RenderPassEvent Event = RenderPassEvent.BeforeRenderingPostProcessing;
    }
    public LightingEventSetting settings = new LightingEventSetting();
    class VolumeLightingPass : ScriptableRenderPass
    {
        private RTHandle destination;
        VolumLighting volumeLighting;
        Material volumeLightingMaterail;
        static readonly int FinalId = Shader.PropertyToID("_FinalColorTexure");
        static readonly int TempTargetId = Shader.PropertyToID("_TempTargetVolumLighting");
        static readonly int BlurRTId = Shader.PropertyToID("_BlurRenderTexture");
        static readonly int MaxStepId = Shader.PropertyToID("_MaxStep");
        static readonly int MaxDistanceId = Shader.PropertyToID("_MaxDistance");
        static readonly int StepSizeId = Shader.PropertyToID("_StepSize");
        static readonly int LightIntensityId = Shader.PropertyToID("_LightIntensity");
        static readonly int BlurIntID = Shader.PropertyToID("_blurInt");
        static readonly int BilaterFilterFactorID = Shader.PropertyToID("_BilaterFilterFactor");
        static readonly int BID = Shader.PropertyToID("_B");
        static readonly int HID = Shader.PropertyToID("_H");
        static readonly int VolumColorID = Shader.PropertyToID("_VolumColor");
        static readonly string k_RenderTag = "Render Volume Lighting Effects";

        public VolumeLightingPass(RenderPassEvent evt)
        {
            renderPassEvent = evt;
            var shader = Shader.Find("PostEffect/VolumeLighting");
            if (shader == null)
            {
                Debug.LogError("PostEffect/VolumeLighting路径下无法找到着色器");
                return;
            }
            volumeLightingMaterail = CoreUtils.CreateEngineMaterial(shader);
        }
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
        }
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (!renderingData.cameraData.postProcessEnabled) return;
            var stac = VolumeManager.instance.stack;
            volumeLighting = stac.GetComponent<VolumLighting>();
            if (volumeLighting == null)
            {
                Debug.LogError("VolumLighting为空");
                return;
            }
            if (!volumeLighting.IsActive())

            {

                return;
            }

            var cmd = CommandBufferPool.Get(k_RenderTag);
            var cameraData = renderingData.cameraData;
            var source = cameraData.renderer.cameraColorTargetHandle;
            //var source = currentTarget;
            //int destination = TempTargetId;
            volumeLightingMaterail.SetInt(MaxStepId, volumeLighting.maxStep.value);
            volumeLightingMaterail.SetFloat(MaxDistanceId, volumeLighting.maxDistance.value);
            volumeLightingMaterail.SetFloat(StepSizeId, volumeLighting.stepSize.value);
            volumeLightingMaterail.SetFloat(LightIntensityId, volumeLighting.lightIntensity.value);
            volumeLightingMaterail.SetFloat(BlurIntID, volumeLighting.blurIntensity.value);
            volumeLightingMaterail.SetFloat(BilaterFilterFactorID, volumeLighting.bilaterFilterFactor.value);
            volumeLightingMaterail.SetFloat(BID, volumeLighting.B.value);
            volumeLightingMaterail.SetFloat(HID, volumeLighting.H.value);
            volumeLightingMaterail.SetColor(VolumColorID, volumeLighting.volumColor.value);
            int shaderPass = 0;
            int BlurPass = 2;
            int BlendPass = 3;
            RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
            //cmd.GetTemporaryRT(destination, desc);
            cmd.GetTemporaryRT(BlurRTId, desc);
            cmd.GetTemporaryRT(FinalId, desc);
            cmd.Blit(source, destination, volumeLightingMaterail, shaderPass);
            for (int i = 0; i < volumeLighting.loop.value; i++)
            {
                cmd.Blit(destination, BlurRTId, volumeLightingMaterail, BlurPass);
                cmd.Blit(BlurRTId, destination);
            }
            cmd.Blit(destination, FinalId);
            cmd.Blit(source, destination);
            cmd.Blit(destination, source, volumeLightingMaterail, BlendPass);
            //Blitter.BlitCameraTexture(cmd, source, destination);


            using (new ProfilingScope(cmd, profilingSampler))
            {
                //CoreUtils.SetRenderTarget(cmd, cameraData.renderer.GetCameraColorBackBuffer(cmd));
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
            // cmd.ReleaseTemporaryRT(destination);
        }
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }
        public void Setup(RenderingData renderingData)
        {
            var colorCopyDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            colorCopyDescriptor.depthBufferBits = (int)DepthBits.None;
            RenderingUtils.ReAllocateIfNeeded(ref destination, colorCopyDescriptor, name: "VolumeLightDestination");
        }


    }
    VolumeLightingPass volumeLightingPass;
    public override void Create()
    {
        volumeLightingPass = new VolumeLightingPass(settings.Event);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        volumeLightingPass.Setup(renderingData);
        renderer.EnqueuePass(volumeLightingPass);
    }


}