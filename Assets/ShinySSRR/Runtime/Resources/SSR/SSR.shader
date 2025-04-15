Shader "Hidden/Kronnect/SSR_URP" {
Properties {
    _BumpMap("Normal Map", 2D) = "bump" {}
    _BumpScale("Normal Strength", Float) = 1.0
    _SmoothnessMap("Smoothness Map", 2D) = "white" {}
    _Color("", Color) = (1,1,1)
    _NoiseTex("", any) = "" {}
    _SSRSettings("", Vector) = (1,1,1,1)
    _SSRSettings2("", Vector) = (1,1,1,1)
    _StencilValue("Stencil Value", Int) = 0
    _StencilCompareFunction("Stencil Compare Function", Int) = 8
}

Subshader {	

    Tags { "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" "DisableBatching"="True" "ForceNoShadowCasting"="True" }
    ZWrite Off ZTest Always Cull Off

    HLSLINCLUDE
    #pragma target 3.0
    #pragma prefer_hlslcc gles
    #pragma exclude_renderers d3d11_9x
    
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"
    #include "SSR_Common.hlsl"
    ENDHLSL

  Pass { // 0
      Name "Copy exact"
      HLSLPROGRAM
      #pragma vertex VertSSR
      #pragma fragment FragCopyExact
      #include "SSR_Blends.hlsl"
      ENDHLSL
  }

  Pass { // 1
      Name "Surface reflection"
      ZTest LEqual
      ZWrite On
      HLSLPROGRAM
      #pragma vertex VertSSRSurf
      #pragma fragment FragSSRSurf
      #pragma multi_compile_local _ SSR_SCREEN_SPACE_NORMALS SSR_NORMALMAP
      #pragma multi_compile_local _ SSR_SMOOTHNESSMAP
      #pragma multi_compile_local _ SSR_JITTER
      #pragma multi_compile_local _ SSR_THICKNESS_FINE
      #pragma multi_compile_local _ SSR_METALLIC_WORKFLOW
      #pragma multi_compile _ SSR_BACK_FACES
      #pragma multi_compile _ SSR_SKYBOX
      #include "SSR_Surface_Pass.hlsl"
      ENDHLSL
  }

  Pass { // 2
      Name "Resolve"
      HLSLPROGRAM
      #pragma vertex VertSSR
      #pragma fragment FragResolve
      #pragma multi_compile _ SSR_SKYBOX
      #pragma multi_compile_local _ SSR_METALLIC_WORKFLOW
      #include "SSR_Solve.hlsl"
      ENDHLSL
  }    

  Pass { // 3
      Name "Blur horizontally"
      HLSLPROGRAM
      #pragma vertex VertBlur
      #pragma fragment FragBlur
      #pragma multi_compile_local _ SSR_DENOISE
      #define SSR_BLUR_HORIZ
      #include "SSR_Blur.hlsl"
      ENDHLSL
  }    
      
  Pass { // 4
      Name "Blur vertically"
	  HLSLPROGRAM
      #pragma vertex VertBlur
      #pragma fragment FragBlur
      #pragma multi_compile_local _ SSR_DENOISE
      #include "SSR_Blur.hlsl"
      ENDHLSL
  }    

  Pass { // 5
      Name "Debug"
      Blend One Zero
	  HLSLPROGRAM
      #pragma vertex VertSSR
      #pragma fragment FragCopyExact
      #include "SSR_Blends.hlsl"
      ENDHLSL
  }    

  Pass { // 6
      Name "Combine"
      Stencil {
        Ref [_StencilValue]
        Comp [_StencilCompareFunction]
      }
      Blend One OneMinusSrcAlpha // precomputed source alpha in Resolve pass
	  HLSLPROGRAM
      #pragma vertex VertSSR
      #pragma fragment FragCombine
      #pragma multi_compile_local_fragment _ SSR_DARK_REFLECTIONS SSR_METALLIC_BOOST
      #pragma multi_compile_local _ SSR_CUSTOM_SMOOTHNESS_METALLIC_PASS
      #include "SSR_Blends.hlsl"
      ENDHLSL
  }

  Pass { // 7
      Name "Combine with compare"
      Stencil {
        Ref [_StencilValue]
        Comp [_StencilCompareFunction]
      }
      Blend One OneMinusSrcAlpha // One One // precomputed source alpha in Resolve pass
	  HLSLPROGRAM
      #pragma vertex VertSSR
      #pragma fragment FragCombineWithCompare
      #pragma multi_compile_local_fragment _ SSR_DARK_REFLECTIONS SSR_METALLIC_BOOST
      #pragma multi_compile_local _ SSR_CUSTOM_SMOOTHNESS_METALLIC_PASS
      #include "SSR_Blends.hlsl"
      ENDHLSL
  }

  Pass { // 8
      Name "Deferred pass"
	  HLSLPROGRAM
      #pragma vertex VertSSR
      #pragma fragment FragSSR
      #pragma multi_compile_local _ SSR_JITTER
      #pragma multi_compile_local _ SSR_THICKNESS_FINE
      #pragma multi_compile_local _ SSR_METALLIC_WORKFLOW
      #pragma multi_compile_local _ SSR_CUSTOM_SMOOTHNESS_METALLIC_PASS
      #pragma multi_compile_local _ SSR_LIMIT_BOUNDS
      #pragma multi_compile _ _GBUFFER_NORMALS_OCT
      #pragma multi_compile _ SSR_BACK_FACES
      #pragma multi_compile _ SSR_SKYBOX
      #include "SSR_GBuf_Pass.hlsl"
      ENDHLSL
  }

  Pass { // 9
      Name "Copy with bilinear filter"
      HLSLPROGRAM
      #pragma vertex VertSSR
      #pragma fragment FragCopy
      #include "SSR_Blends.hlsl"
      ENDHLSL
  }

  Pass { // 10
      Name "Temporal Accumulation"
      HLSLPROGRAM
      #pragma vertex VertSSR
      #pragma fragment FragAcum
      #include "SSR_TAcum.hlsl"
      ENDHLSL
  }

  Pass { // 11
      Name "Debug Depth"
      HLSLPROGRAM
      #pragma vertex VertSSR
      #pragma fragment FragDebugDepth
      #include "SSR_Blends.hlsl"
      ENDHLSL
  }

  Pass { // 12
      Name "Debug Normals"
      HLSLPROGRAM
      #pragma vertex VertSSR
      #pragma fragment FragDebugNormals
      #pragma multi_compile _ _GBUFFER_NORMALS_OCT
      #include "SSR_Blends.hlsl"
      ENDHLSL
  }
  
  Pass { // 13
      Name "Copy Depth"
      HLSLPROGRAM
      #pragma vertex VertSSR
      #pragma fragment FragCopyDepth
      #pragma multi_compile _ SSR_BACK_FACES
      #include "SSR_Blends.hlsl"
      ENDHLSL
  }
    
}
FallBack Off
}
