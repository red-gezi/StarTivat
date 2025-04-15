Shader "Custom/RayMarchingShader"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            fixed4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float distanceFunction(float3 p)
            {
                // 定义距离函数
                return length(p) - 1.0;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 origin = float3(0, 0, 0);
                float3 direction = normalize(i.vertex.xyz);
                float tMin = 0.0;
                float tMax = 10.0;
                float delta = 2.0;
                float t = tMin;

                while (t < tMax)
                {
                    float3 p = origin + t * direction;
                    float d = distanceFunction(p);
                    if (d < delta)
                        break;
                    t += d;
                }

                float3 color = (t < tMax) ? _Color.rgb : float3(0, 0, 0);
                return float4(color, 1.0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}