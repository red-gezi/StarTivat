using UnityEngine;
using UnityEngine.Rendering;

namespace ShinySSRR {

    [ExecuteAlways]
    public class SkyboxBaker : MonoBehaviour {

        public static bool needSkyboxUpdate = true;
        RenderTexture skyboxCubemap;
        float lastSkyboxSnapshotTime;
        ShinyScreenSpaceRaytracedReflections settings;
        Camera cam;

        void OnEnable() {
            needSkyboxUpdate = true;
            cam = GetComponent<Camera>();
        }

        private void OnDestroy() {
            ReleaseSkyboxCubemap();
        }


        void LateUpdate() {
            settings = VolumeManager.instance?.stack?.GetComponent<ShinyScreenSpaceRaytracedReflections>();

            if (settings == null || settings.skyboxIntensity.value <= 0 || cam == null) return;

            if (settings.skyboxUpdateMode.value == SkyboxUpdateMode.Interval && Time.time - lastSkyboxSnapshotTime >= settings.skyboxUpdateInterval.value) {
                lastSkyboxSnapshotTime = Time.time;
                needSkyboxUpdate = true;
            }

            if (needSkyboxUpdate && cam.cameraType == CameraType.Game) {
                needSkyboxUpdate = false;
                UpdateSkyboxCubemap();
            }
        }

        void ReleaseSkyboxCubemap() {
            if (skyboxCubemap == null) return;
            skyboxCubemap.Release();
            DestroyImmediate(skyboxCubemap);
        }

        public void UpdateSkyboxCubemap() {

            if (settings.skyboxUpdateMode.value == SkyboxUpdateMode.CustomCubemap) {
                Shader.SetGlobalTexture(ShaderParams.SkyboxCubemap, settings.skyboxCustomCubemap.value);
                return;
            }

            if (cam == null) return;

            int cullingMask = cam.cullingMask;
            float farClipPlane = cam.farClipPlane;

            cam.cullingMask = 0;
            cam.farClipPlane = cam.nearClipPlane + 0.1f;

            int res = (int)Mathf.Pow(2, (int)settings.skyboxResolution.value + 4);
            res = Mathf.Clamp(res, 16, 8192);
            if (skyboxCubemap == null || skyboxCubemap.width != res) {
                ReleaseSkyboxCubemap();
                skyboxCubemap = new RenderTexture(res, res, 0);
                skyboxCubemap.dimension = TextureDimension.Cube;
            }
            cam.RenderToCubemap(skyboxCubemap);
            Shader.SetGlobalTexture(ShaderParams.SkyboxCubemap, skyboxCubemap);

            cam.cullingMask = cullingMask;
            cam.farClipPlane = farClipPlane;
        }


    }

}
