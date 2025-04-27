/*
 * PIDI Planar Reflections 4
 * Developed  by : Jorge Pinal Negrete.
 * Copyright© 2015-2021, Jorge Pinal Negrete.  All Rights Reserved. 
 *  
*/

Shader "PIDI Shaders Collection/Planar Reflections 4/PBR/Metallic (Alpha)"
{
    Properties
    {

        [Space(10)]
        [Header(Main Color)]
        [Space(10)]
        _Color ("Color", Color) = (1,1,1,1)
        [NoScaleOffset]_MainTex ("Albedo (RGB)", 2D) = "white" {}

        [Header(Metallic and Gloss)]
		[Space(10)]
        [Toggle]_IsRoughness("Gloss as Roughness", Float) = 0
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        [NoScaleOffset]_MetallicGlossMap("Metallic (R) Gloss(A)", 2D) = "white" {}

        [Header(Normals and Parallax Mapping)]
		[Space(10)]
		_BumpScale("Scale", Float) = 1.0
		[NoScaleOffset]_BumpMap("Normal Map", 2D) = "bump" {}

        [Space(10)]
        _Parallax("Height Scale", Range(0.005, 0.08)) = 0.02
		[NoScaleOffset]_ParallaxMap("Height Map", 2D) = "gray" {}

		[Header(Occlusion)]
		[Space(10)]
		_OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
		[NoScaleOffset]_OcclusionMap("Occlusion", 2D) = "white" {}

		[Space(12)]
		[Header(Material Emission)]
        [Space(10)]
		[Enum(Additive,0,Masked,1)]_EmissionMode("Emission/Reflection Blend Mode", Float) = 0 //Blend mode for the emission and reflection channels
		_EmissionColor("Emission Color (RGB) Intensity (16*Alpha)", Color) = (1,1,1,0.5)
		[NoScaleOffset]_EmissionMap("Emission Map (RGB) Mask (A)", 2D) = "black"{}//Emissive map

        [Space(10)]
        _TilingOffset("Tiling & Offset", Vector) = (1, 1, 0, 0)

        [Space(10)]
		[Header(Secondary Maps)]
		[Space(10)]

        [Toggle(_DETAIL_RENDERING)]_Detail_Rendering("Use Secondary Detail Maps", Float) = 1
		
        _DetailMask("Detail Mask", 2D) = "white" {}

        [Space(10)]
		[NoScaleOffset]_DetailAlbedoMap("Detail Albedo x2", 2D) = "grey" {}
		[NoScaleOffset]_DetailBumpScale("Scale", Float) = 1.0
		[NoScaleOffset]_DetailBumpMap("Normal Map", 2D) = "bump" {}

        [Space(10)]
        [Enum(UV0,0,UV1,1)]_DetailUVChannel("UV Set", Float) = 0
        _DetailTilingOffset("Detail Tiling & Offset", Vector) = ( 2, 2, 0, 0)

        [Space(16)]
        [Header(Reflection Settings)]
        [Space(8)]
        _AlbedoReflectionTint("Albedo Based Tint", Range(0,1)) = 0.5
        _ReflectionTint("Reflection Tint", Color) = (1,1,1,1)

        [Space(8)]
        _RefDistortion("BumpMap distortion", Range(0,0.1)) = 0.003

        [Space(8)]
        [Toggle(_USE_DEPTH)]_UseReflectionDepth("Use Reflection Depth", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200

        ZWrite On

        Cull Back

        CGPROGRAM
        #include "../CGInclude/PlanarReflections.cginc"
        #pragma surface surf Standard fullforwardshadows alpha:blend 
        #pragma shader_feature_local _DETAIL_RENDERING
        #pragma shader_feature_local _USE_DEPTH
        #pragma shader_feature_local _USE_FOG   
        #pragma target 3.0



        struct Input
        {
            float2 uv_MainTex;
            float2 uv_DetailMask;
            float2 uv2_BumpMap;
            float4 screenPos;
            float3 viewDir;
        };

        
        half _BumpScale;
        half _Glossiness;
        half _Metallic;
        half _IsRoughness;
        half _OcclusionStrength;

        half _DetailBumpScale;

        half _EmissionMode;
        half _DetailUVChannel;

        half4 _TilingOffset;
        half4 _DetailTilingOffset;
        
        sampler2D _MainTex;
        sampler2D _MetallicGlossMap;
        sampler2D _BumpMap;
        sampler2D _OcclusionMap;
        sampler2D _EmissionMap;

        
        sampler2D _DetailMask;
        sampler2D _DetailBumpMap;
        sampler2D _DetailAlbedoMap;

        fixed4 _Color;
        fixed4 _EmissionColor;


        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {

            half2 mainUV = _TilingOffset.zw + IN.uv_MainTex * _TilingOffset.xy;

            half4 dColor = half4(0,0,0,0);
            half3 dNormal = half3(0,0,0);
            half dMask = 0;

            #if _DETAIL_RENDERING

            half2 dUV = _DetailTilingOffset.zw + lerp( IN.uv_MainTex, IN.uv2_BumpMap, _DetailUVChannel ) * _DetailTilingOffset.xy;

            dMask = tex2D(_DetailMask, IN.uv_DetailMask).r;
            dNormal = UnpackScaleNormal(tex2D(_DetailBumpMap, dUV), _DetailBumpScale * dMask);
            dColor = tex2D( _DetailAlbedoMap, dUV) * unity_ColorSpaceDouble.r;

            #endif

            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, mainUV ) * _Color;
            fixed4 m = tex2D(_MetallicGlossMap, mainUV);

            m.a = lerp( m.a, 1-m.a, _IsRoughness );

            half3 mainNormal = UnpackScaleNormal( tex2D(_BumpMap, mainUV), _BumpScale );

            o.Albedo = c.rgb * lerp( dColor, 1, 1-dMask );
            o.Normal = normalize( half3(mainNormal.x + dNormal.x, mainNormal.y + dNormal.y, mainNormal.z) );
            o.Metallic = _Metallic * m.r;
            o.Smoothness = _Glossiness * m.a;

            half4 e = tex2D( _EmissionMap, mainUV );
            e.rgb *= _EmissionColor.rgb * 16 * _EmissionColor.a;  

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
            o.Emission = e.rgb + lerp( reflectionColor, reflectionColor * (1-e.a), _EmissionMode );
            o.Occlusion = tex2D(_OcclusionMap, mainUV) * _OcclusionStrength;

            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
