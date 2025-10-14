using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class SS_OutlineVolume : VolumeComponent, IPostProcessComponent
{
    // 描边宽度参数 (0.0f - 20.0f)
    public ClampedFloatParameter OutlineWidth = new ClampedFloatParameter(0.0f, 0f, 20f);

    // 描边颜色参数 (默认黑色)
    public ColorParameter OutlineColor = new ColorParameter(Color.black, true, true, true);

    // 是否激活描边效果
    public bool IsActive() => this.OutlineWidth.value > 0.0f;

    // 是否支持Tile渲染（通常为false）
    public bool IsTileCompatible() => false;
}