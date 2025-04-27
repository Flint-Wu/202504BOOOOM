Shader "PIDI Shaders Collection/Planar Reflections 4/Unlit/Debug Depth"
{
    Properties
    {
        [HideInInspector][PerRendererData] _ReflectionDepth ("Reflection Depth", 2D) = "black" {}
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
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 screenPos : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _ReflectionDepth;
			float4 _ReflectionDepth_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenPos = ComputeGrabScreenPos(o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{

				i.screenPos.xy /= i.screenPos.w;
				i.screenPos.x = 1-i.screenPos.x;


				fixed4 col = tex2D(_ReflectionDepth, i.screenPos.xy).r;
				col = saturate( pow( col, 8 * i.screenPos.w) + 0.25 );
				return col;
			}
			ENDCG
        }
    }
}
