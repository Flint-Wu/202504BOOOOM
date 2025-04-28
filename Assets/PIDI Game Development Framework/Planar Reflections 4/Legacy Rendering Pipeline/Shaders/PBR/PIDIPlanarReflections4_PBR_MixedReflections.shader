/*
 * PIDI Planar Reflections 4
 * Developed  by : Jorge Pinal Negrete.
 * Copyright© 2015-2021, Jorge Pinal Negrete.  All Rights Reserved. 
 *  
*/
Shader "PIDI Shaders Collection/Planar Reflections 4/PBR/Static + Real-time mix" {
	Properties {
		
		[Space(12)]
		[Header(Dynamic Reflection Properties)]
		_ReflectionTint("Reflection Tint", Color) = (1,1,1,1) //The color tint to be applied to the reflection
		[PerRendererData] _ReflectionTex ("Reflection Texture", 2D) = "black" {} //The render texture containing the real-time reflection
		[PerRendererData] _ReflectionDepth("Reflection Depth", 2D) = "white"{}

		[Space(12)]
		[Header(Static Reflection Properties)]
        _CubemapRef("Cubemap Reflection", CUBE ) = ""{} //Pre-baked cubemap to mix
		[Toggle]_BoxMode("Use Box Projection",Float) = 0
		_BoxPosition ("Env Box Start", Vector) = (0, 0, 0)
		_BoxSize ("Env Box Size", Vector) = (10, 10, 10)

	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#include "../CGInclude/PlanarReflections.cginc"
		#pragma surface surf Standard noshadow
		#pragma shader_feature_local _USE_DEPTH
		#pragma shader_feature_local _USE_FOG   
		#pragma target 3.0

		sampler2D _MainTex;
		samplerCUBE _CubemapRef;

		half _BoxMode;
		fixed4 _BoxPosition;
		fixed4 _BoxSize;

		struct Input {
			float4 screenPos;
			float3 viewDir;
			float3 worldRefl;
			fixed3 worldPos;
			INTERNAL_DATA
		};

		float4 _EyeOffset;
		fixed4 _Color;		
        fixed4 _ChromaKeyColor;		
        half _ChromaTolerance;
		
		void surf (Input IN, inout SurfaceOutputStandard o) {
		
			//We calculate the screen UV coordinates ( and ensure IN.screenPos.w is never 0 )
			float2 screenUV = IN.screenPos.xy / max( IN.screenPos.w, 0.0001 );
			
			screenUV.x = 1-screenUV.x;

			half4 c = half4(0,0,0,0);
			

			//BPCM
			fixed3 nReflDirection = max(normalize(WorldReflectionVector (IN, o.Normal)),0.0001);
   
			float3 boxStart = _BoxPosition - _BoxSize / 2.0;
			float3 firstPlaneIntersect = (boxStart + _BoxSize - IN.worldPos) / nReflDirection;
			float3 secondPlaneIntersect = (boxStart - IN.worldPos) / nReflDirection;
			float3 furthestPlane = (nReflDirection > 0.0) ? firstPlaneIntersect : secondPlaneIntersect;
			float3 intersectDistance = min(min(furthestPlane.x, furthestPlane.y), furthestPlane.z);
			float3 intersectPosition = IN.worldPos + nReflDirection * intersectDistance;
			//END BPCM
#if _USE_FOG
#if !_USE_DEPTH
			half4 reflectionColor = pow(PBRBasedBlurFog(c, o.Smoothness, IN.screenPos, 32, IN.viewDir, o.Normal), 1 + 0.5 * o.Metallic);
#else
			half4 reflectionColor = pow(PBRBasedBlurDepthFog(c, o.Smoothness, IN.screenPos, 32, IN.viewDir, o.Normal), 1 + 0.5 * o.Metallic);
#endif
#else
#if !_USE_DEPTH
			half4 reflectionColor = pow(PBRBasedBlur(c, o.Smoothness, IN.screenPos, 32, IN.viewDir, o.Normal), 1 + 0.5 * o.Metallic);
#else
			half4 reflectionColor = pow(PBRBasedBlurDepth(c, o.Smoothness, IN.screenPos, 32, IN.viewDir, o.Normal), 1 + 0.5 * o.Metallic);
#endif
#endif
			
			o.Emission = lerp( texCUBE (_CubemapRef, lerp(WorldReflectionVector (IN, o.Normal),intersectPosition - _BoxPosition,_BoxMode)).rgb, reflectionColor.rgb, 1-floor(tex2D(_ReflectionDepth,screenUV).r))*_ReflectionTint*_ReflectionTint.a;
			
			o.Alpha = 1;
		}
		
		
		ENDCG
		
		
	

		
	} 
	FallBack "Diffuse"
}
