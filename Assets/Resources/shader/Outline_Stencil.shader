Shader "Outline_StencilTest_Transparent"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineWidth ("Outline width", Range(0.0, 4)) = 0.001
        _OutlineColor ("Outline Color", color) = (1.0, 1.0, 1.0, 1.0)
        _Alpha ("Transparency", Range(0, 1)) = 0.5 // 新增透明度参数
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True" }
        LOD 100
        
        // 禁用深度写入，因为我们处理的是透明物体
        ZWrite Off
        // 添加alpha混合
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Stencil
            {
                Ref 1
                Comp Always
                Pass Replace
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Alpha;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 color = tex2D(_MainTex, i.uv);
                // 应用全局透明度
                return fixed4(color.rgb, color.a * _Alpha);
            }
            ENDCG
        }

        Pass
        {
            Stencil
            {
                Ref 0
                Comp Equal
            }

            // 为轮廓也使用混合模式
            Blend SrcAlpha OneMinusSrcAlpha
            // 禁用深度写入
            ZWrite Off
            // 确保轮廓绘制在主物体之后
            Offset 0, -1

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            float _OutlineWidth;
            fixed4 _OutlineColor;
            float _Alpha;

            v2f vert (appdata v)
            {
                v2f o;
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                float3 clipNormal = mul((float3x3) UNITY_MATRIX_VP, mul((float3x3) UNITY_MATRIX_M, v.normal));
                o.vertex.xy += normalize(clipNormal).xy * _OutlineWidth;
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 应用轮廓颜色时也考虑透明度
                return fixed4(_OutlineColor.rgb, _OutlineColor.a * _Alpha);
            }

            ENDCG
        }
    }
}