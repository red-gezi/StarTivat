#ifndef SSR_COMMON
#define SSR_COMMON

	// XR is not supported but we leave this macro for future uses
	#define SSRStereoTransformScreenSpaceTex(x) UnityStereoTransformScreenSpaceTex(x)
	//#define SSRStereoTransformScreenSpaceTex(x) x

    #define dot2(x) dot(x, x)

	inline half getLuma(float3 rgb) { 
		const half3 lum = float3(0.299, 0.587, 0.114);
		return dot(rgb, lum);
	}




TEXTURE2D_X_HALF(_DownscaledShinyDepthRT);
float4 _DownscaledShinyDepthRT_TexelSize;

TEXTURE2D_X_HALF(_DownscaledShinyBackDepthRT);
float4 _DownscaledShinyBackDepthRT_TexelSize;

inline float GetLinearDepth(float2 uv) {
    float depth = SAMPLE_TEXTURE2D_X_LOD(_DownscaledShinyDepthRT, sampler_PointClamp, uv, 0).r;
    return depth;
}

inline void GetLinearDepths(float2 uv, out float sceneDepth, out float sceneBackDepth) {
    float2 depths = SAMPLE_TEXTURE2D_X_LOD(_DownscaledShinyDepthRT, sampler_PointClamp, uv, 0).xy;
    sceneDepth = depths.x;
    sceneBackDepth = depths.y;
}

#define dot2(x) dot(x, x)

#endif // SSR_BLUR