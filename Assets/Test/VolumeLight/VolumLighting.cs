using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VolumLighting : VolumeComponent, IPostProcessComponent
{
    [Range(0, 3)]
    public FloatParameter lightIntensity = new FloatParameter(0);
    public ColorParameter volumColor = new ColorParameter(Color.white);
    public FloatParameter stepSize = new FloatParameter(0.1f);
    public FloatParameter maxDistance = new FloatParameter(1000);
    public IntParameter maxStep = new IntParameter(200);
    public ClampedFloatParameter blurIntensity = new ClampedFloatParameter(1, 0, 20);
    public ClampedIntParameter loop = new ClampedIntParameter(3, 1, 10);
    public ClampedFloatParameter bilaterFilterFactor = new ClampedFloatParameter(0.3f, 0, 1);

    public FloatParameter H = new FloatParameter(0.01f);
    public FloatParameter B = new FloatParameter(1.0f);
    public bool IsActive() => lightIntensity.value > 0;
    public bool IsTileCompatible() => false;
}