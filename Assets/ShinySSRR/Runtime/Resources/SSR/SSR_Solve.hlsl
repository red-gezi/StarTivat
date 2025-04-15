#ifndef SSR_SOLVE
#define SSR_SOLVE

	// Copyright 2021 Kronnect - All Rights Reserved.
    TEXTURE2D_X(_MainTex);
	float4 _MainTex_TexelSize;
    TEXTURE2D_X(_RayCastRT);

    float4 _MaterialData;
    #define FRESNEL _MaterialData.y
    #define FUZZYNESS _MaterialData.z

    float4 _SSRSettings2;
    #define ENERGY_CONSERVATION _SSRSettings2.y
    #define REFLECTIONS_MULTIPLIER _SSRSettings2.z

    float4 _SSRSettings4;
    #define REFLECTIONS_MIN_INTENSITY _SSRSettings4.y
    #define REFLECTIONS_MAX_INTENSITY _SSRSettings4.z

    float3 _SSRSettings5;
    #define SKYBOX_INTENSITY _SSRSettings5.z

    samplerCUBE _SkyboxCubemap;
    float3 _CameraViewDir;
    #define CAMERA_VIEW_DIR _CameraViewDir

	struct AttributesFS {
		float4 positionHCS : POSITION;
		float2 uv          : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

 	struct VaryingsSSR {
    	float4 positionCS : SV_POSITION;
    	float2 uv  : TEXCOORD0;
        UNITY_VERTEX_INPUT_INSTANCE_ID
        UNITY_VERTEX_OUTPUT_STEREO
	};


	VaryingsSSR VertSSR(AttributesFS input) {
	    VaryingsSSR output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_TRANSFER_INSTANCE_ID(input, output);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
        output.positionCS = float4(input.positionHCS.xyz, 1.0);

		#if UNITY_UV_STARTS_AT_TOP
		output.positionCS.y *= -1;
		#endif

        output.uv = input.uv;
    	return output;
	}

	half4 FragResolve (VaryingsSSR i) : SV_Target { 

        UNITY_SETUP_INSTANCE_ID(i);
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
        i.uv = SSRStereoTransformScreenSpaceTex(i.uv);

        half4 reflData = SAMPLE_TEXTURE2D_X(_RayCastRT, sampler_PointClamp, i.uv);
        if (reflData.w == 0) return 0;

        #if SSR_SKYBOX
            half4 reflection;
            if (reflData.w < 0) {
                reflection = texCUBE(_SkyboxCubemap, reflData.xyz);
                half smoothness = -reflData.w;
                float reflectionIntensity = smoothness * SKYBOX_INTENSITY;
                float fresnel = 1.0 - FRESNEL * (1.0 - abs(dot(reflData.xyz, CAMERA_VIEW_DIR)));
                float reflectionAmount = reflectionIntensity * fresnel; 
                float blurAmount = FUZZYNESS * (1 - smoothness);
                reflData.z = blurAmount + 0.001;
                reflData.w = reflectionAmount;
                reflData.xy = 0.5;
            } else {
                reflection = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, reflData.xy);
            }
        #else
      		half4 reflection = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, reflData.xy);
        #endif

        reflection.rgb = min(reflection.rgb, 8.0); // stop NAN pixels

        #if SSR_METALLIC_WORKFLOW
            half reflectionIntensity = reflData.a * REFLECTIONS_MULTIPLIER;
        #else
            half reflectionIntensity = clamp(reflData.a * REFLECTIONS_MULTIPLIER, REFLECTIONS_MIN_INTENSITY, REFLECTIONS_MAX_INTENSITY);
        #endif

        reflection.rgb *= reflectionIntensity;

        reflection.rgb = min(reflection.rgb, 1.2); // clamp max brightness

        // conserve energy
        half4 pixel = SAMPLE_TEXTURE2D_X(_MainTex, sampler_LinearClamp, i.uv);
        reflection.rgb -= min(0.5, pixel.rgb * reflectionIntensity);

        // keep blur factor in alpha channel
        reflection.a = reflData.z;
        return reflection;
	}



#endif // SSR_SOLVE