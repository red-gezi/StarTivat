Shader "Hidden/Kronnect/SSR_Exclude"
{
    Properties {
        _MainTex("", 2D) = "" {}
        _Color("", Color) = (1,1,1)
    }
    SubShader
    {

        Pass
        {
            Name "SSR Exclude Reflections"
            ZWrite Off
            ZTest Always
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);


            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 projPos : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.projPos = ComputeScreenPos(o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
                float z = i.projPos.z * 0.99;
                if (z > sceneZ) discard;
                return 0;
            }
            ENDCG
        }
    }
}
