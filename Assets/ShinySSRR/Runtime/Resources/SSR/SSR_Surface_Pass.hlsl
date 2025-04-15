#ifndef SSR_SURF_FX
#define SSR_SURF_FX

	// Copyright 2021 Kronnect - All Rights Reserved.
    TEXTURE2D(_NoiseTex);
    TEXTURE2D(_BumpMap);
    TEXTURE2D(_SmoothnessMap);
    float4 _BumpMap_ST;
    float4 _NoiseTex_TexelSize;
    half   _BumpScale;

    float4 _MaterialData;
    #define SMOOTHNESS _MaterialData.x
    #define FRESNEL _MaterialData.y
    #define FUZZYNESS _MaterialData.z
    #define DECAY _MaterialData.w

    float4 _SSRSettings;
    #define THICKNESS _SSRSettings.x
    #define SAMPLES _SSRSettings.y
    #define BINARY_SEARCH_ITERATIONS _SSRSettings.z
    #define MAX_RAY_LENGTH _SSRSettings.w

    float3 _SSRSettings5;
    #define REFLECTIONS_THRESHOLD _SSRSettings5.y
    #define SKYBOX_INTENSITY _SSRSettings5.z

#if SSR_THICKNESS_FINE
    #define THICKNESS_FINE _SSRSettings5.x
#else
    #define THICKNESS_FINE THICKNESS
#endif

    float4 _SSRSettings2;
    #define JITTER _SSRSettings2.x
    #define CONTACT_HARDENING _SSRSettings2.y
    #define REFLECTIVITY _SSRSettings2.w

    float4 _SSRSettings3;
    #define INPUT_SIZE _SSRSettings3.xy
    #define GOLDEN_RATIO_ACUM _SSRSettings3.z
    #define DEPTH_BIAS _SSRSettings3.w

    float3 _DistortionData;
    #define DISTORTION_SPEED _DistortionData.xy

    TEXTURE2D(_MetallicGradientTex);
    TEXTURE2D(_SmoothnessGradientTex);

    struct AttributesSurf {
        float4 positionOS   : POSITION;
        float2 texcoord     : TEXCOORD0;
        float3 normalOS     : NORMAL;
        float4 tangentOS    : TANGENT;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

 	struct VaryingsSSRSurf {
    	float4 positionCS : SV_POSITION;
    	float2 uv : TEXCOORD0;
        float4 scrPos : TEXCOORD1;
        float3 positionVS : TEXCOORD2;
        #if SSR_NORMALMAP
            float4 normal    : TEXCOORD3;    // xyz: normal, w: viewDir.x
            float4 tangent   : TEXCOORD4;    // xyz: tangent, w: viewDir.y
            float4 bitangent : TEXCOORD5;    // xyz: bitangent, w: viewDir.z
        #else
            float3 normal    : TEXCOORD3;
        #endif
        #if SSR_SKYBOX
            float3 viewDirWS : TEXCOORD6;
            float3 normalWS  : TEXCOORD7;
        #endif
        UNITY_VERTEX_INPUT_INSTANCE_ID
        UNITY_VERTEX_OUTPUT_STEREO
	};

	VaryingsSSRSurf VertSSRSurf(AttributesSurf input) {

	    VaryingsSSRSurf output = (VaryingsSSRSurf)0;

        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_TRANSFER_INSTANCE_ID(input, output);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

        VertexPositionInputs positions = GetVertexPositionInputs(input.positionOS.xyz);
        VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

        output.positionCS = positions.positionCS;
        output.positionVS = positions.positionVS * float3(1,1,-1);
        output.scrPos     = ComputeScreenPos(positions.positionCS);
        output.uv         = TRANSFORM_TEX(input.texcoord, _BumpMap);

        half3 viewDirWS = GetCameraPositionWS() - positions.positionWS;

        #if SSR_NORMALMAP
            output.normal = half4(normalInput.normalWS, viewDirWS.x);
            output.tangent = half4(normalInput.tangentWS, viewDirWS.y);
            output.bitangent = half4(normalInput.bitangentWS, viewDirWS.z);
        #else
            output.normal = TransformWorldToViewDir(normalInput.normalWS) * float3(1,1,-1);
        #endif
        
        #if SSR_SKYBOX
            output.viewDirWS = viewDirWS;
            output.normalWS = normalInput.normalWS;
        #endif

        #if UNITY_REVERSED_Z
            output.positionCS.z += 0.001;
        #else
            output.positionCS.z -= 0.001;
        #endif

    	return output;
	}

    float collision;

#if SSR_METALLIC_WORKFLOW
	float4 SSR_Pass(float2 uv, float3 normalVS, float3 rayStart, float roughness, float reflectivity) {
#else
	float4 SSR_Pass(float2 uv, float3 normalVS, float3 rayStart, float roughness) {
#endif

        float3 viewDirVS = normalize(rayStart);
        float3 rayDir = reflect( viewDirVS, normalVS );

        // if ray is toward the camera, early exit (optional)
        //if (rayDir.z < 0) return 0.0.xxxx;

        float  rayLength = MAX_RAY_LENGTH;

        float3 rayEnd = rayStart + rayDir * rayLength;
        if (rayEnd.z < _ProjectionParams.y) {
            rayLength = (_ProjectionParams.y - rayStart.z) / rayDir.z;
            rayEnd = rayStart + rayDir * rayLength;
        }

        float4 sposStart = mul(unity_CameraProjection, float4(rayStart, 1.0));
        float4 sposEnd = mul(unity_CameraProjection, float4(rayEnd, 1.0));
        float k0 = rcp(sposStart.w);
        float q0 = rayStart.z * k0;
        float k1 = rcp(sposEnd.w);
        float q1 = rayEnd.z * k1;
        float4 p = float4(uv, q0, k0);

        // depth clip check
        float sceneDepth = GetLinearDepth(p.xy);
        float pz = rayStart.z;
        if (sceneDepth < pz - DEPTH_BIAS) {
            #if SSR_SKYBOX
                collision = 0;
            #endif
            return 0;
        }

        // length in pixels
        float2 uv1 = (sposEnd.xy * rcp(rayEnd.z) + 1.0) * 0.5;
        float2 duv = uv1 - uv;
        float2 duvPixel = abs(duv * INPUT_SIZE);
        float pixelDistance = max(duvPixel.x, duvPixel.y);
        int sampleCount = (int)clamp(pixelDistance, 1, SAMPLES);
        float4 pincr = float4(duv, q1-q0, k1-k0) * rcp(sampleCount);

        #if SSR_JITTER
            float jitter = SAMPLE_TEXTURE2D(_NoiseTex, sampler_PointRepeat, uv * INPUT_SIZE * _NoiseTex_TexelSize.xy + GOLDEN_RATIO_ACUM).r;
            pincr *= 1.0 + jitter * JITTER; // modifying pincr and p gives best results
            p += pincr * (jitter * JITTER);
        #endif

        float dist = 0;
        float3 hitp = 0;

        UNITY_LOOP
        for (int k = 0; k < sampleCount; k++) {
            p += pincr;
            if (any(floor(p.xy)!=0)) return 0.0.xxxx; // exit if out of screen space
            pz = p.z / p.w;

            float sceneBackDepth, depthDiff;
            #if SSR_BACK_FACES
                GetLinearDepths(p.xy, sceneDepth, sceneBackDepth);
                if (pz >= sceneDepth && pz <= sceneBackDepth) {
            #else
                sceneDepth = GetLinearDepth(p.xy);
                depthDiff = pz - sceneDepth;
                if (depthDiff > 0 && depthDiff < THICKNESS) {
            #endif
                float4 origPincr = pincr;
                p -= pincr;
                float reduction = 1.0;
                UNITY_LOOP
                for (int j = 0; j < BINARY_SEARCH_ITERATIONS; j++) {
                    reduction *= 0.5;
                    p += pincr * reduction;
                    pz = p.z / p.w;
                    sceneDepth = GetLinearDepth(p.xy);
                    depthDiff = sceneDepth - pz;
                    pincr = sign(depthDiff) * origPincr;
                }

                float hitAccuracy = 1.0 - abs(depthDiff) / THICKNESS;
                float candidateCollision = hitAccuracy;

#if SSR_THICKNESS_FINE
                if (candidateCollision > collision) {
                    hitp = float3(p.xy, pz);
                    collision = candidateCollision;
                }
                if (abs(depthDiff) < THICKNESS_FINE)
                    break;

                pincr = origPincr;
                p += pincr;
#else
                hitp = float3(p.xy, pz);
                collision = candidateCollision;
                break;
#endif
            }
        }

        if (collision > 0) {
            // intersection found

            // reduce collision intensity with distance to reflection point
            float zdist = (hitp.z - rayStart.z) / (0.0001 + rayEnd.z - rayStart.z);
            float rayFade = 1.0 - saturate(zdist);
            collision *= rayFade;
	    	    
            #if SSR_METALLIC_WORKFLOW
                float reflectionIntensity = reflectivity;
            #else
                float reflectionIntensity = (1.0 - roughness);
            #endif
            reflectionIntensity *= pow(collision, DECAY);

            // compute fresnel
            float fresnel = 1.0 - FRESNEL * abs(dot(normalVS, viewDirVS));
            float reflectionAmount = reflectionIntensity * fresnel; 

            // compute blur amount
            float wdist = rayLength * zdist;
            float blurAmount = max(0, wdist - CONTACT_HARDENING) * FUZZYNESS * roughness;

            // return hit pixel
            return float4(hitp.xy, blurAmount + 0.001, reflectionAmount);
        }
        return float4(0,0,0,0);
	}


	float4 FragSSRSurf (VaryingsSSRSurf input) : SV_Target {
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        input.scrPos.xy /= input.scrPos.w;
        //input.scrPos = SSRStereoTransformScreenSpaceTex(input.scrPos);

        float3 normalWS;
        #if SSR_SKYBOX
            normalWS = input.normalWS;
        #endif

        #if SSR_SCREEN_SPACE_NORMALS
            #if UNITY_VERSION >= 202130
                normalWS = SampleSceneNormals(input.scrPos.xy);
                float3 normalVS = TransformWorldToViewDir(normalWS);
                normalVS.z *= -1;
            #else
                float3 normalVS = SampleSceneNormals(input.scrPos.xy);
            #endif
        #elif SSR_NORMALMAP
            input.uv += fmod(_Time.xx * DISTORTION_SPEED, 10000);
            float4 packedNormal = SAMPLE_TEXTURE2D(_BumpMap, sampler_PointRepeat, input.uv);
            float3 normalTS = UnpackNormalScale(packedNormal, _BumpScale);
            half3 viewDirWS = half3(input.normal.w, input.tangent.w, input.bitangent.w);
            normalWS = TransformTangentToWorld(normalTS, half3x3(input.tangent.xyz, input.bitangent.xyz, input.normal.xyz));
            float3 normalVS = TransformWorldToViewDir(normalWS);
            normalVS.z *= -1;
        #else
            float3 normalVS = input.normal;
        #endif


       collision = -1;

       #if SSR_METALLIC_WORKFLOW
            #if SSR_SMOOTHNESSMAP
                float reflectivity = REFLECTIVITY * SAMPLE_TEXTURE2D(_SmoothnessMap, sampler_PointRepeat, input.uv).a;
            #else
                float reflectivity = REFLECTIVITY;
            #endif
            reflectivity = SAMPLE_TEXTURE2D_LOD(_MetallicGradientTex, sampler_LinearClamp, float2(reflectivity, 0), 0).r;
            float roughness = SAMPLE_TEXTURE2D_LOD(_SmoothnessGradientTex, sampler_LinearClamp, float2(1.0 - SMOOTHNESS, 0), 0).r;
   	        float4 reflection = SSR_Pass(input.scrPos.xy, normalVS, input.positionVS, roughness, reflectivity);
        #else
            #if SSR_SMOOTHNESSMAP
                float smoothness = SMOOTHNESS * SAMPLE_TEXTURE2D(_SmoothnessMap, sampler_PointRepeat, input.uv).a;
            #else
                float smoothness = SMOOTHNESS;
            #endif
            float roughness = 1.0 - max(0, smoothness - REFLECTIONS_THRESHOLD);
   	        float4 reflection = SSR_Pass(input.scrPos.xy, normalVS, input.positionVS, roughness);
        #endif

        #if SSR_SKYBOX
        if (collision < 0) {
            float3 viewDirWS = normalize(input.viewDirWS);
            float3 reflDir = -reflect(viewDirWS, normalWS);
            reflection.xyz = reflDir;
            float sm = 1.0 - roughness;
            reflection.w = -sm;
        }
        #endif

        return reflection;
	}


#endif // SSR_SURF_FX