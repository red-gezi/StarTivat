Shader "Unlit/VolumeLight"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _LightColor ("Light Color", Color) = (1, 1, 1, 1)
        _LightIntensity ("Light Intensity", Float) = 1.0
        _StepSize ("Step Size", Float) = 0.01
        _NumSteps ("Number of Steps", Int) = 100
    }
    SubShader
    {
        Tags { "RenderType" ="Transparent" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            UNITY_DECLARE_SHADOWMAP(_CascadeShadowMapTexture);
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _LightColor;
            float _LightIntensity;
            float _StepSize;
            int _NumSteps;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }
            inline float4 GetCascadeWeights_SplitSpheres(float3 wpos)
            {
                float3 fromCenter0 = wpos.xyz - unity_ShadowSplitSpheres[0].xyz;
                float3 fromCenter1 = wpos.xyz - unity_ShadowSplitSpheres[1].xyz;
                float3 fromCenter2 = wpos.xyz - unity_ShadowSplitSpheres[2].xyz;
                float3 fromCenter3 = wpos.xyz - unity_ShadowSplitSpheres[3].xyz;
                float4 distances2 = float4(dot(fromCenter0, fromCenter0), dot(fromCenter1, fromCenter1), dot(fromCenter2, fromCenter2), dot(fromCenter3, fromCenter3));

                float4 weights = float4(distances2 < unity_ShadowSplitSqRadii);
                weights.yzw = saturate(weights.yzw - weights.xyz);
                return weights;
            }

            inline float3 GetCascadeShadowCoord(float4 wpos, float4 cascadeWeights)
            {
                float3 sc0 = mul(unity_WorldToShadow[0], wpos).xyz;
                float3 sc1 = mul(unity_WorldToShadow[1], wpos).xyz;
                float3 sc2 = mul(unity_WorldToShadow[2], wpos).xyz;
                float3 sc3 = mul(unity_WorldToShadow[3], wpos).xyz;

                float3 shadowMapCoordinate = float3(sc0 * cascadeWeights[0] + sc1 * cascadeWeights[1] + sc2 * cascadeWeights[2] + sc3 * cascadeWeights[3]);
                #if defined(UNITY_REVERSED_Z)
                float noCascadeWeights = 1 - dot(cascadeWeights, 1);
                shadowMapCoordinate.z += noCascadeWeights;
                #endif
                return shadowMapCoordinate;
            }
            float GetLightIntensity(float3 wpos)
            {
                float atten = 1;
                // sample cascade shadow map
                float4 cascadeWeights = GetCascadeWeights_SplitSpheres(wpos);
                float3 samplePos = GetCascadeShadowCoord(float4(wpos, 1), cascadeWeights);
                atten = UNITY_SAMPLE_SHADOW(_CascadeShadowMapTexture, samplePos.xyz);
                return atten;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 rayStart = _WorldSpaceCameraPos;
                float3 rayDir = normalize(i.worldPos - _WorldSpaceCameraPos);
                float3 currentPos = rayStart;
                float accumulatedIntensity = 0.0;

                for (int step = 0; step < _NumSteps; step ++)
                {
                    float lightIntensity = GetLightIntensity(currentPos);
                    accumulatedIntensity += lightIntensity * _StepSize;
                    currentPos += rayDir * _StepSize;
                }

                float4 finalColor = _LightColor * _LightIntensity * accumulatedIntensity;
                return finalColor;
            }
            ENDCG
        }
    }
}