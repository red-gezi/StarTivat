Shader "PostEffect/VolumeLighting"
{
    Properties
    {

        _MainTex ("", 2D) = "black" {}
        _MaxStep ("",float) = 200
        _MaxDistance ("",float) = 1000
        _LightIntensity ("",float) = 0.01
        _StepSize ("" , float) = 0.1

    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        ZWrite Off
		ZTest Always
		Cull Off

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        CBUFFER_START(UnityPerMaterial)
        float _MaxDistance;
        float _MaxStep;
        float _StepSize;
        float _LightIntensity;
        float _blurInt;
        half4 _LightColor0;
        float4 _MainTex_TexelSize;
        float _BilaterFilterFactor;
        CBUFFER_END
        TEXTURE2D_X_FLOAT(_CameraDepthTexture); SAMPLER(sampler_CameraDepthTexture);
        TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
        TEXTURE2D(_FinalColorTexure); SAMPLER(sampler_FinalColorTexure);
       // TEXTURE2D(_TempTargetVolumLighting); SAMPLER(sampler_TempTargetVolumLighting);
        ENDHLSL
        //计算体积光
        Pass
        {
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM

            // 设置关键字
            #pragma shader_feature _AdditionalLights

            // 接收阴影所需关键字
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _SHADOWS_SOFT

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"


            struct Attributes
            {
                float4 positionOS: POSITION;
                float3 normalOS: NORMAL;
                float4 tangentOS: TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS: SV_POSITION;
                float3 positionWS: TEXCOORD0;
                float3 normalWS: TEXCOORD1;
                float3 viewDirWS: TEXCOORD2;
                float3 positionOS : TEXCOORD3;
                float2 uv : TEXCOORD4;
            };




            float4 GetTheWorldPos(float2 ScreenUV , float Depth)
			{
				//获取像素的屏幕空间位置
				float3 ScreenPos = float3(ScreenUV , Depth);
				float4 normalScreenPos = float4(ScreenPos * 2.0 - 1.0 , 1.0);
				//得到ndc空间下像素位置
				float4 ndcPos = mul( unity_CameraInvProjection , normalScreenPos );
				ndcPos = float4(ndcPos.xyz / ndcPos.w , 1.0);
				//获取世界空间下像素位置
				float4 sencePos = mul( unity_CameraToWorld , ndcPos * float4(1,1,-1,1));
				sencePos = float4(sencePos.xyz , 1.0);
				return sencePos;
			}

            float GetShadow(float3 posWorld)
            {
                float4 shadowCoord = TransformWorldToShadowCoord(posWorld);
                float shadow = MainLightRealtimeShadow(shadowCoord);
                return shadow;
            }


            Varyings vert(Attributes v)
            {
                Varyings o;
                // 获取不同空间下坐标信息
                VertexPositionInputs positionInputs = GetVertexPositionInputs(v.positionOS.xyz);
                o.positionCS = positionInputs.positionCS;
                o.uv = v.uv;
                return o;
            }


            half4 frag(Varyings i): SV_Target
            {
                float2 uv = i.uv;
                float depth = SAMPLE_TEXTURE2D_X(_CameraDepthTexture,sampler_CameraDepthTexture, uv).r;
                depth = 1-depth;
                float3 ro = _WorldSpaceCameraPos.xyz;
                float3 worldPos = GetTheWorldPos(uv , depth).xyz;
                float3 rd = normalize(worldPos - ro);
                float3 currentPos = ro;
                float m_length = min(length(worldPos - ro) , _MaxDistance);
                float delta = _StepSize;
                float totalInt = 0;
                float d = 0;
                for(int j = 0; j < _MaxStep; j++)
                {
                    d += delta;
                    if(d > m_length) break;
                    currentPos += delta * rd;
                    totalInt += _LightIntensity * GetShadow(currentPos);
                }
                half3 lightCol = totalInt * _LightColor0.rgb;
                //half3 oCol = SAMPLE_TEXTURE2D(_MainTex , sampler_MainTex , uv).rgb;
                //half3 dCol = lightCol + oCol;
                return real4(lightCol , 1);

            }

            ENDHLSL

        }

        //高斯滤波
        Pass
        {
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM

            // 设置关键字
            #pragma shader_feature _AdditionalLights

            // 接收阴影所需关键字
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _SHADOWS_SOFT

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"


            struct Attributes
            {
                float4 positionOS: POSITION;
                float3 normalOS: NORMAL;
                float4 tangentOS: TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS: SV_POSITION;
                float3 positionWS: TEXCOORD0;
                float3 normalWS: TEXCOORD1;
                float3 viewDirWS: TEXCOORD2;
                float3 positionOS : TEXCOORD3;
                float2 uv : TEXCOORD4;
            };

            Varyings vert(Attributes v)
            {
                Varyings o;
                // 获取不同空间下坐标信息
                VertexPositionInputs positionInputs = GetVertexPositionInputs(v.positionOS.xyz);
                o.positionCS = positionInputs.positionCS;
                o.uv = v.uv;
                return o;
            }


            half4 frag(Varyings i): SV_Target
            {
                float2 uv = i.uv;
                float3 dir = float3(1,-1,0);
                float2 offsetInt =  _blurInt * _MainTex_TexelSize.xy;
                half3 oCol = 0;
                oCol += SAMPLE_TEXTURE2D(_MainTex , sampler_MainTex , uv ).rgb * 0.147761;
                oCol += SAMPLE_TEXTURE2D(_MainTex , sampler_MainTex , uv + dir.xz * offsetInt).rgb * 0.118318;
                oCol += SAMPLE_TEXTURE2D(_MainTex , sampler_MainTex , uv + dir.yz * offsetInt).rgb * 0.118318;
                oCol += SAMPLE_TEXTURE2D(_MainTex , sampler_MainTex , uv + dir.zx * offsetInt).rgb * 0.118318;
                oCol += SAMPLE_TEXTURE2D(_MainTex , sampler_MainTex , uv + dir.zy * offsetInt).rgb * 0.118318;
                oCol += SAMPLE_TEXTURE2D(_MainTex , sampler_MainTex , uv + dir.xx * offsetInt).rgb * 0.0947416;
                oCol += SAMPLE_TEXTURE2D(_MainTex , sampler_MainTex , uv + dir.yy * offsetInt).rgb * 0.0947416;
                oCol += SAMPLE_TEXTURE2D(_MainTex , sampler_MainTex , uv + dir.xy * offsetInt).rgb * 0.0947416;
                oCol += SAMPLE_TEXTURE2D(_MainTex , sampler_MainTex , uv + dir.yx * offsetInt).rgb * 0.0947416;
                return real4(oCol , 1);

            }
            ENDHLSL}

        //双边滤波
        Pass {
			Name "Example"
			Tags { "LightMode"="UniversalForward" }

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			struct Attributes {
				float4 positionOS	: POSITION;
				float2 uv		: TEXCOORD0;
			};

			struct Varyings {
				float4 positionCS 	: SV_POSITION;
				float4 uv[4] : TEXCOORD;
			};


			real Luminance(real3 linearRgb)
			{
				return dot(linearRgb, real3(0.2126729, 0.7151522, 0.0721750));
			}

			half CompareColor(real4 col1, real4 col2)
			{
				float l1 = Luminance(col1.rgb);
				float l2 = Luminance(col2.rgb);
				return smoothstep(_BilaterFilterFactor, 1.0, 1.0 - abs(l1 - l2));
			}


			Varyings vert(Attributes IN) {
				Varyings OUT;
				OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
				float3 dir = float3(1,-1,0);
				float2 delta = _MainTex_TexelSize.xy * _blurInt;
				OUT.uv[0] = float4(IN.uv , IN.uv + dir.xx * delta);
				OUT.uv[1] = float4(IN.uv + dir.yy * delta , IN.uv + dir.xx * 2 * delta);
				OUT.uv[2] = float4(IN.uv + dir.yy * 2 * delta , IN.uv + dir.xx * 3 * delta);
				OUT.uv[3] = float4(IN.uv + dir.yy *3 * delta ,0,0);
				return OUT;
			}

			half4 frag(Varyings IN) : SV_Target {
				//half4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
				real4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv[0].xy);
				real4 col0a = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex,IN.uv[0].zw);
				real4 col0b = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv[1].xy);
				real4 col1a = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv[1].zw);
				real4 col1b = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv[2].xy);
				real4 col2a = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv[2].zw);
				real4 col2b = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv[3].xy);

				half w = 0.37004405286;
				half w0a = CompareColor(col, col0a) * 0.31718061674;
				half w0b = CompareColor(col, col0b) * 0.31718061674;
				half w1a = CompareColor(col, col1a) * 0.19823788546;
				half w1b = CompareColor(col, col1b) * 0.19823788546;
				half w2a = CompareColor(col, col2a) * 0.11453744493;
				half w2b = CompareColor(col, col2b) * 0.11453744493;

				half3 result;
				result = w * col.rgb;
				result += w0a * col0a.rgb;
				result += w0b * col0b.rgb;
				result += w1a * col1a.rgb;
				result += w1b * col1b.rgb;
				result += w2a * col2a.rgb;
				result += w2b * col2b.rgb;

				result /= w + w0a + w0b + w1a + w1b + w2a + w2b;
				return real4(result , 1.0);
			}
			ENDHLSL
		}




        //混合pass
         Pass
        {
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM

            // 设置关键字
            #pragma shader_feature _AdditionalLights

            // 接收阴影所需关键字
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _SHADOWS_SOFT

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"


            struct Attributes
            {
                float4 positionOS: POSITION;
                float3 normalOS: NORMAL;
                float4 tangentOS: TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS: SV_POSITION;
                float3 positionWS: TEXCOORD0;
                float3 normalWS: TEXCOORD1;
                float3 viewDirWS: TEXCOORD2;
                float3 positionOS : TEXCOORD3;
                float2 uv : TEXCOORD4;
            };


            Varyings vert(Attributes v)
            {
                Varyings o;
                // 获取不同空间下坐标信息
                VertexPositionInputs positionInputs = GetVertexPositionInputs(v.positionOS.xyz);
                o.positionCS = positionInputs.positionCS;
                o.uv = v.uv;
                return o;
            }


            half4 frag(Varyings i): SV_Target
            {
                float2 uv = i.uv;
                real3 oCol = SAMPLE_TEXTURE2D(_MainTex , sampler_MainTex , uv ).rgb;
                real3 lCol = SAMPLE_TEXTURE2D(_FinalColorTexure , sampler_FinalColorTexure , uv).rgb;
                real4 finalCol = real4(oCol+lCol ,1);
                return finalCol;
            }

            ENDHLSL

        }
        //下面计算阴影的Pass可以直接通过使用URP内置的Pass计算
        //UsePass "Universal Render Pipeline/Lit/ShadowCaster"
        // or
        // 计算阴影的Pass

    }
    FallBack "Packages/com.unity.render-pipelines.universal/FallbackError"
}