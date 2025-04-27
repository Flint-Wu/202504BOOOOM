// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "PIDI Shaders Collection/Planar Reflections 4/ASE/Metallic"
{
	Properties
	{
		[Space(10)][Header(Albedo Color)][Space(10)]_Color("Color", Color) = (1,1,1,1)
		[NoScaleOffset][SingleLineTexture]_MainTex("Albedo (RGB)", 2D) = "white" {}
		[Header(Metallic and Gloss)][Space(10)][Toggle]_IsRoughness("Gloss as Roughness", Float) = 0
		_Glossiness("Smoothness", Range( 0 , 1)) = 0.25
		_Metallic("Metallic", Range( 0 , 1)) = 0
		[SingleLineTexture][Space(5)]_MetalGlossMap("Metallic (R) Gloss (A) Map", 2D) = "white" {}
		[Space(10)][Header(Bump Mapping)][Space(10)]_BumpScale("Scale", Float) = 1
		[SingleLineTexture][Space(5)]_BumpMap("Normal Map", 2D) = "bump" {}
		[Space(10)]_TilingOffset("Tiling and Offset", Vector) = (1,1,0,0)
		[Space(10)][Header(Emission Settings)][Space(10)][Toggle(_EMISSIONSUPPORT)] _EmissionSupport("Use Emission", Float) = 0
		[Enum(Additive,0,Masked,1)]_EmissionMode("Emission Mode", Float) = 0
		_EmissionColor("Emission Color", Color) = (0,0,0,0.1254902)
		[SingleLineTexture]_EmissionMap("Emission Map", 2D) = "white" {}
		[Space(10)][Header(Secondary Maps)][Space(10)][Toggle(_DETAILRENDERING_ON)] _DetailRendering("Use Secondary Detail Maps", Float) = 0
		_DetailMask("Detail Mask", 2D) = "white" {}
		[Space(10)][Enum(UV0,0,UV1,1)]_DetailUVChannel("UV Channel", Float) = 0
		[NoScaleOffset][SingleLineTexture]_DetailAlbedoMap("Detail Albedo x2", 2D) = "white" {}
		[SingleLineTexture]_DetailBumpMap("Normal Map", 2D) = "white" {}
		_DetailBumpScale("Scale", Float) = 1
		_DetailTilingOffset("Detail Tiling & Offset", Vector) = (1,1,0,0)
		[HideInInspector][PerRendererData]_ReflectionFog("_ReflectionFog", 2D) = "white" {}
		[HideInInspector][PerRendererData]_ReflectionTex("_ReflectionTex", 2D) = "white" {}
		[HideInInspector][PerRendererData]_ReflectionDepth("ReflectionDepth", 2D) = "white" {}
		[Space(10)][Header(Reflection Settings)][Space(10)]_ReflectionTint("Reflection Tint", Color) = (1,1,1,1)
		_RefDistortion("Reflection Distortion", Range( 0 , 1)) = 0.02
		[Toggle(_USEREFLECTIONDEPTH_ON)] _UseReflectionDepth("Use Reflection Depth", Float) = 0
		_AlbedoReflectionTint("Albedo Based Tint", Range( 0 , 1)) = 0
		[HideInInspector][Toggle]_USE_FOG("_USE_FOG", Float) = 0
		[HideInInspector] _texcoord2( "", 2D ) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _DETAILRENDERING_ON
		#pragma shader_feature_local _EMISSIONSUPPORT
		#pragma shader_feature_local _USEREFLECTIONDEPTH_ON
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
			float2 uv2_texcoord2;
			float4 screenPos;
			float3 worldPos;
		};

		uniform float4 _ReflectionTint;
		uniform sampler2D _BumpMap;
		uniform float4 _TilingOffset;
		uniform float _BumpScale;
		uniform sampler2D _DetailBumpMap;
		uniform float4 _DetailTilingOffset;
		uniform float _DetailUVChannel;
		uniform sampler2D _DetailMask;
		uniform float4 _DetailMask_ST;
		uniform float _DetailBumpScale;
		uniform float4 _Color;
		uniform sampler2D _MainTex;
		uniform sampler2D _DetailAlbedoMap;
		uniform float _USE_FOG;
		uniform sampler2D _ReflectionTex;
		float4 _ReflectionTex_TexelSize;
		uniform float _AlbedoReflectionTint;
		uniform float _RefDistortion;
		uniform float _IsRoughness;
		uniform sampler2D _MetalGlossMap;
		uniform float _Glossiness;
		uniform sampler2D _ReflectionDepth;
		uniform sampler2D _ReflectionFog;
		uniform sampler2D _EmissionMap;
		uniform float _EmissionMode;
		uniform float4 _EmissionColor;
		uniform float _Metallic;


		float3 FastPBRBlurRef12_g38( float4 albedo, sampler2D _ReflectionTex, float4 _ReflectionTint, float2 reflectionUVs, float2 reflectionTexels, float _AlbedoReflectionTint, float reflectionDistortion, float smoothness, float3 normal, float3 viewDir, float maxBlur, float depth )
		{
			half4 reflectionColor = half4(0,0,0,0);
				half texLOD = lerp(4,0, saturate( smoothness + depth * 0.5 ) );
			maxBlur = lerp(0, maxBlur, 1 - depth * 0.25 );
				uint samples = max(texLOD*32,1);
				uint sLOD = 1 << (uint)texLOD;
				float sigma = float(samples)*0.25;
				uint s = samples /sLOD;
				reflectionUVs +=  + normal * reflectionDistortion;
				for (uint i = 0; i < s*s; i++){
					float2 d = float2(i%s,i/s)*float(sLOD)-float(samples)/2.0;
					float2 uvs = reflectionUVs+reflectionTexels.xy*d;
					reflectionColor += exp(-0.5*dot(d/=sigma,d))/(6.28*sigma*sigma) * tex2Dlod( _ReflectionTex, half4(uvs, 0, texLOD) );
				}
				reflectionColor.a = max(reflectionColor.a,0.000001);
				reflectionColor /= reflectionColor.a;
				reflectionColor.rgb = lerp( reflectionColor.rgb * _ReflectionTint.rgb, reflectionColor.rgb * _ReflectionTint.rgb * albedo.rgb, _AlbedoReflectionTint * length( albedo.rgb ) );
				reflectionColor.rgb *= max( smoothness, 0.02 );
				half fresnelValue = saturate( dot(normal, viewDir) ); 
				reflectionColor *= lerp( 0.5, 1, 1 - fresnelValue );
				return reflectionColor.rgb;
		}


		float3 FastPBRBlurRefFog42_g38( float4 albedo, sampler2D _ReflectionTex, float4 _ReflectionTint, float2 reflectionUVs, float2 reflectionTexels, float _AlbedoReflectionTint, float reflectionDistortion, float smoothness, float3 normal, float3 viewDir, float maxBlur, float depth, sampler2D fog )
		{
			half4 reflectionColor = half4(0,0,0,0);
				half texLOD = lerp(4,0, saturate( smoothness + depth * 0.5 ) );
			maxBlur = lerp(0, maxBlur, 1 - depth * 0.25 );
				uint samples = max(texLOD*32,1);
				uint sLOD = 1 << (uint)texLOD;
				float sigma = float(samples)*0.25;
				uint s = samples /sLOD;
				reflectionUVs +=  + normal * reflectionDistortion;
				for (uint i = 0; i < s*s; i++){
					float2 d = float2(i%s,i/s)*float(sLOD)-float(samples)/2.0;
					float2 uvs = reflectionUVs+reflectionTexels.xy*d;
			half4 reflectionFog = tex2Dlod(fog, half4(uvs, 0, texLOD));
					reflectionColor += exp(-0.5*dot(d/=sigma,d))/(6.28*sigma*sigma) * lerp(tex2Dlod(_ReflectionTex, half4(uvs, 0, texLOD)), half4(reflectionFog.rgb, 1), reflectionFog.a);
				}
				reflectionColor.a = max(reflectionColor.a,0.000001);
				reflectionColor /= reflectionColor.a;
				reflectionColor.rgb = lerp( reflectionColor.rgb * _ReflectionTint.rgb, reflectionColor.rgb * _ReflectionTint.rgb * albedo.rgb, _AlbedoReflectionTint * length( albedo.rgb ) );
				reflectionColor.rgb *= max( smoothness, 0.02 );
				half fresnelValue = saturate( dot(normal, viewDir) ); 
				reflectionColor *= lerp( 0.5, 1, 1 - fresnelValue );
				return reflectionColor.rgb;
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 appendResult53 = (float2(_TilingOffset.x , _TilingOffset.y));
			float2 appendResult55 = (float2(_TilingOffset.z , _TilingOffset.w));
			float2 uv_TexCoord7 = i.uv_texcoord * appendResult53 + appendResult55;
			float2 appendResult74 = (float2(_DetailTilingOffset.x , _DetailTilingOffset.y));
			float2 appendResult75 = (float2(_DetailTilingOffset.z , _DetailTilingOffset.w));
			float2 uv_TexCoord77 = i.uv_texcoord * appendResult74 + appendResult75;
			float2 uv2_TexCoord76 = i.uv2_texcoord2 * appendResult74 + appendResult75;
			float2 lerpResult82 = lerp( uv_TexCoord77 , uv2_TexCoord76 , _DetailUVChannel);
			float2 uv_DetailMask = i.uv_texcoord * _DetailMask_ST.xy + _DetailMask_ST.zw;
			float temp_output_100_0 = length( tex2D( _DetailMask, uv_DetailMask ) );
			#ifdef _DETAILRENDERING_ON
				float3 staticSwitch91 = BlendNormals( UnpackScaleNormal( float4( UnpackNormal( tex2D( _BumpMap, uv_TexCoord7 ) ) , 0.0 ), _BumpScale ) , UnpackScaleNormal( tex2D( _DetailBumpMap, lerpResult82 ), saturate( ( temp_output_100_0 * _DetailBumpScale ) ) ) );
			#else
				float3 staticSwitch91 = UnpackScaleNormal( float4( UnpackNormal( tex2D( _BumpMap, uv_TexCoord7 ) ) , 0.0 ), _BumpScale );
			#endif
			o.Normal = staticSwitch91;
			float4 tex2DNode3 = tex2D( _MainTex, uv_TexCoord7 );
			float4 lerpResult103 = lerp( tex2D( _DetailAlbedoMap, lerpResult82 ) , float4( 1,1,1,1 ) , temp_output_100_0);
			#ifdef _DETAILRENDERING_ON
				float4 staticSwitch81 = ( lerpResult103 * unity_ColorSpaceDouble.r * tex2DNode3 );
			#else
				float4 staticSwitch81 = tex2DNode3;
			#endif
			float4 temp_output_4_0 = ( _Color * staticSwitch81 );
			o.Albedo = temp_output_4_0.rgb;
			float4 temp_output_24_0_g38 = temp_output_4_0;
			float4 albedo12_g38 = temp_output_24_0_g38;
			sampler2D _ReflectionTex12_g38 = _ReflectionTex;
			float4 _ReflectionTint12_g38 = _ReflectionTint;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float2 appendResult3_g38 = (float2(( 1.0 - ( ase_screenPos.x / ase_screenPos.w ) ) , ( ase_screenPos.y / ase_screenPos.w )));
			float2 reflectionUVs12_g38 = appendResult3_g38;
			float2 appendResult20_g38 = (float2(_ReflectionTex_TexelSize.x , _ReflectionTex_TexelSize.y));
			float2 reflectionTexels12_g38 = appendResult20_g38;
			float _AlbedoReflectionTint12_g38 = _AlbedoReflectionTint;
			float reflectionDistortion12_g38 = _RefDistortion;
			float4 tex2DNode57 = tex2D( _MetalGlossMap, uv_TexCoord7 );
			float temp_output_23_0_g38 = (( _IsRoughness )?( ( ( 1.0 - tex2DNode57.a ) * ( 1.0 - _Glossiness ) ) ):( ( tex2DNode57.a * _Glossiness ) ));
			float smoothness12_g38 = temp_output_23_0_g38;
			float3 temp_output_26_0_g38 = staticSwitch91;
			float3 normal12_g38 = temp_output_26_0_g38;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 viewDir12_g38 = ase_worldViewDir;
			float lerpResult31_g38 = lerp( 32.0 , 1.0 , 0.0);
			float maxBlur12_g38 = lerpResult31_g38;
			float temp_output_33_0_g38 = pow( tex2D( _ReflectionDepth, appendResult3_g38 ).r , ( ase_screenPos.w * 4.0 ) );
			#ifdef _USEREFLECTIONDEPTH_ON
				float staticSwitch34_g38 = temp_output_33_0_g38;
			#else
				float staticSwitch34_g38 = 0.0;
			#endif
			float depth12_g38 = staticSwitch34_g38;
			float3 localFastPBRBlurRef12_g38 = FastPBRBlurRef12_g38( albedo12_g38 , _ReflectionTex12_g38 , _ReflectionTint12_g38 , reflectionUVs12_g38 , reflectionTexels12_g38 , _AlbedoReflectionTint12_g38 , reflectionDistortion12_g38 , smoothness12_g38 , normal12_g38 , viewDir12_g38 , maxBlur12_g38 , depth12_g38 );
			float4 albedo42_g38 = temp_output_24_0_g38;
			sampler2D _ReflectionTex42_g38 = _ReflectionTex;
			float4 _ReflectionTint42_g38 = _ReflectionTint;
			float2 reflectionUVs42_g38 = appendResult3_g38;
			float2 reflectionTexels42_g38 = appendResult20_g38;
			float _AlbedoReflectionTint42_g38 = _AlbedoReflectionTint;
			float reflectionDistortion42_g38 = _RefDistortion;
			float smoothness42_g38 = temp_output_23_0_g38;
			float3 normal42_g38 = temp_output_26_0_g38;
			float3 viewDir42_g38 = ase_worldViewDir;
			float maxBlur42_g38 = lerpResult31_g38;
			float depth42_g38 = staticSwitch34_g38;
			sampler2D fog42_g38 = _ReflectionFog;
			float3 localFastPBRBlurRefFog42_g38 = FastPBRBlurRefFog42_g38( albedo42_g38 , _ReflectionTex42_g38 , _ReflectionTint42_g38 , reflectionUVs42_g38 , reflectionTexels42_g38 , _AlbedoReflectionTint42_g38 , reflectionDistortion42_g38 , smoothness42_g38 , normal42_g38 , viewDir42_g38 , maxBlur42_g38 , depth42_g38 , fog42_g38 );
			float3 temp_output_142_10 = (( _USE_FOG )?( localFastPBRBlurRefFog42_g38 ):( localFastPBRBlurRef12_g38 ));
			float4 tex2DNode109 = tex2D( _EmissionMap, uv_TexCoord7 );
			float3 lerpResult113 = lerp( temp_output_142_10 , ( ( 1.0 - tex2DNode109.a ) * temp_output_142_10 ) , _EmissionMode);
			float3 appendResult123 = (float3(_EmissionColor.r , _EmissionColor.g , _EmissionColor.b));
			float3 appendResult120 = (float3(tex2DNode109.r , tex2DNode109.g , tex2DNode109.b));
			#ifdef _EMISSIONSUPPORT
				float3 staticSwitch111 = ( lerpResult113 + ( appendResult123 * float3( 16,16,16 ) * appendResult120 * _EmissionColor.a * tex2DNode109.a ) );
			#else
				float3 staticSwitch111 = temp_output_142_10;
			#endif
			o.Emission = staticSwitch111;
			o.Metallic = ( tex2DNode57.r * _Metallic );
			float temp_output_60_0 = (( _IsRoughness )?( ( ( 1.0 - tex2DNode57.a ) * ( 1.0 - _Glossiness ) ) ):( ( tex2DNode57.a * _Glossiness ) ));
			o.Smoothness = temp_output_60_0;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18900
368;182;1718;925;2323.807;715.3636;1.835805;True;False
Node;AmplifyShaderEditor.CommentaryNode;71;-4221.099,-1451.679;Inherit;False;1024.762;548.3641;Main texture set UV coordinates;7;79;82;76;77;73;75;74;Detail UVs;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;56;-4217.625,-2083.51;Inherit;False;768.5251;561.3982;Main texture set UV coordinates;4;52;55;53;7;Main UVs;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector4Node;73;-4166.409,-1317.968;Inherit;False;Property;_DetailTilingOffset;Detail Tiling & Offset;19;0;Create;False;0;0;0;False;0;False;1,1,0,0;4,4,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector4Node;52;-4170.287,-1957.459;Inherit;False;Property;_TilingOffset;Tiling and Offset;8;0;Create;False;0;0;0;True;1;Space(10);False;1,1,0,0;2,2,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;69;-3105.776,178.5776;Inherit;False;1284.308;1297.123;Comment;14;42;43;31;97;49;47;98;46;45;91;50;99;100;105;Normalmap;1,1,1,1;0;0
Node;AmplifyShaderEditor.DynamicAppendNode;53;-3909.427,-2006.853;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;55;-3909.221,-1801.198;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;99;-3064.586,1143.761;Inherit;True;Property;_DetailMask;Detail Mask;14;0;Create;False;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.DynamicAppendNode;75;-3905.343,-1161.707;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;74;-3905.549,-1367.362;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;77;-3684.5,-1376.912;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;76;-3673.025,-1159.68;Inherit;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;79;-3603.877,-1025.112;Inherit;False;Property;_DetailUVChannel;UV Channel;15;0;Create;False;0;0;0;False;2;Space(10);Enum(UV0,0,UV1,1);False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;98;-2848.189,1159.047;Inherit;True;Property;_DetailMask;Detail Mask;23;0;Fetch;True;0;0;0;False;0;False;-1;None;None;True;0;False;black;Auto;False;Instance;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;7;-3680.497,-1923.161;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LengthOpNode;100;-2552.278,1195.521;Inherit;False;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;82;-3358.877,-1281.112;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;67;-2557.136,-2047.087;Inherit;False;1123.752;845.9572;;8;4;2;81;3;72;101;102;103;Albedo;1,1,1,1;0;0
Node;AmplifyShaderEditor.WireNode;86;-2928.878,-1820.42;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;50;-3046.06,1043.792;Inherit;False;Property;_DetailBumpScale;Scale;18;0;Create;False;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;97;-2374.557,1050.286;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;68;-1413.93,-269.4559;Inherit;False;953.355;841.0697;Comment;9;32;57;62;65;61;66;60;59;58;Metallic & Gloss;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;45;-3053.617,228.5776;Inherit;True;Property;_BumpMap;Normal Map;7;1;[SingleLineTexture];Create;False;0;0;0;False;1;Space(5);False;-1;None;6f5e0593c1b25774da9ea9dce850a026;True;0;True;bump;LockedToTexture2D;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;72;-2519.912,-1677.623;Inherit;True;Property;_DetailAlbedoMap;Detail Albedo x2;16;2;[NoScaleOffset];[SingleLineTexture];Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;87;-2556.453,-346.2296;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-1363.93,371.8257;Inherit;False;Property;_Glossiness;Smoothness;3;0;Create;False;0;0;0;False;0;False;0.25;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorSpaceDouble;101;-2516.907,-1488.403;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;46;-3050.795,743.6623;Inherit;True;Property;_DetailBumpMap;Normal Map;17;1;[SingleLineTexture];Create;False;0;0;0;False;0;False;-1;None;None;True;0;True;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;49;-2952.849,422.2945;Inherit;False;Property;_BumpScale;Scale;6;0;Create;False;0;0;0;False;3;Space(10);Header(Bump Mapping);Space(10);False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;3;-2372.219,-1921.197;Inherit;True;Property;_MainTex;Albedo (RGB);1;2;[NoScaleOffset];[SingleLineTexture];Create;False;0;0;0;False;0;False;-1;None;97204b14e5f25984e9a65f306002767f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;105;-2731.164,971.7071;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;57;-1075.1,-219.4561;Inherit;True;Property;_MetalGlossMap;Metallic (R) Gloss (A) Map;5;1;[SingleLineTexture];Create;False;0;0;0;False;1;Space(5);False;-1;None;6bdc80a30f4463948bb42d9323a571f3;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;103;-2235.704,-1411.845;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;1,1,1,1;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;47;-2655.521,305.3789;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.UnpackScaleNormalNode;43;-2587.712,608.21;Inherit;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.UnpackScaleNormalNode;31;-2585.279,406.8788;Inherit;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;102;-2058.683,-1531.021;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;62;-1067.178,192.0739;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;65;-1075.1,460.6139;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;61;-768.3195,141.7239;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;66;-891.7248,431.1425;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;124;-1216.014,960.2109;Inherit;False;1723.157;722.514;;11;109;114;122;119;123;120;115;121;113;112;111;Emission;1,1,1,1;0;0
Node;AmplifyShaderEditor.ColorNode;2;-1881.418,-1997.087;Inherit;False;Property;_Color;Color;0;0;Create;True;0;0;0;False;3;Space(10);Header(Albedo Color);Space(10);False;1,1,1,1;0.6226414,0.5259708,0.3553753,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;81;-1864.936,-1643.565;Inherit;False;Property;_DetailRendering;Use Secondary Detail Maps;13;0;Create;False;0;0;0;False;3;Space(10);Header(Secondary Maps);Space(10);False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.BlendNormalsNode;42;-2313.775,508.2928;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ToggleSwitchNode;60;-697.575,402.4594;Inherit;False;Property;_IsRoughness;Gloss as Roughness;2;0;Create;False;0;0;0;False;2;Header(Metallic and Gloss);Space(10);False;0;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;109;-1166.014,1274.88;Inherit;True;Property;_EmissionMap;Emission Map;12;1;[SingleLineTexture];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;91;-2096.397,398.822;Inherit;False;Property;_DetailRendering;Detail Rendering;13;0;Create;True;0;0;0;False;3;Space(10);Header(Secondary Maps);Space(10);False;0;0;0;True;;Toggle;2;Key0;Key1;Reference;81;True;True;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-1604.995,-1781.174;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;114;-606.9431,1385.697;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;122;-1003.629,1010.211;Inherit;False;Property;_EmissionColor;Emission Color;11;0;Create;True;0;0;0;False;0;False;0,0,0,0.1254902;0,0,0,0.1254902;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;142;-425.2595,665.9066;Inherit;False;PlanarReflections4_PBR;20;;38;5ef7ed07aeb8d0b4bbb6b9175ed08b72;0;3;26;FLOAT3;0,0,1;False;23;FLOAT;0;False;24;COLOR;0,0,0,0;False;2;FLOAT3;10;FLOAT;11
Node;AmplifyShaderEditor.DynamicAppendNode;123;-774.6287,1040.211;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;119;-397.8862,1566.726;Inherit;False;Property;_EmissionMode;Emission Mode;10;1;[Enum];Create;True;0;2;Additive;0;Masked;1;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;120;-834.256,1232.271;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;115;-359.7587,1446.92;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;113;-191.1189,1372.284;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;121;-645.6287,1204.211;Inherit;False;5;5;0;FLOAT3;0,0,0;False;1;FLOAT3;16,16,16;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;112;39.4341,1189.96;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;59;-1172.145,52.42611;Inherit;False;Property;_Metallic;Metallic;4;0;Create;True;0;0;0;False;0;False;0;0.457;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;111;252.1428,1181.797;Inherit;False;Property;_EmissionSupport;Use Emission;9;0;Create;False;0;0;0;False;3;Space(10);Header(Emission Settings);Space(10);False;0;0;0;True;_EMISSIONSUPPORT;Toggle;2;Key0;Key1;Create;True;False;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;58;-743.8514,38.25405;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;84;-270.1646,-1463.511;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;29;597.4816,36.25024;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;PIDI Shaders Collection/Planar Reflections 4/ASE/Metallic;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;53;0;52;1
WireConnection;53;1;52;2
WireConnection;55;0;52;3
WireConnection;55;1;52;4
WireConnection;75;0;73;3
WireConnection;75;1;73;4
WireConnection;74;0;73;1
WireConnection;74;1;73;2
WireConnection;77;0;74;0
WireConnection;77;1;75;0
WireConnection;76;0;74;0
WireConnection;76;1;75;0
WireConnection;98;0;99;0
WireConnection;7;0;53;0
WireConnection;7;1;55;0
WireConnection;100;0;98;0
WireConnection;82;0;77;0
WireConnection;82;1;76;0
WireConnection;82;2;79;0
WireConnection;86;0;7;0
WireConnection;97;0;100;0
WireConnection;97;1;50;0
WireConnection;45;1;7;0
WireConnection;72;1;82;0
WireConnection;87;0;86;0
WireConnection;46;1;82;0
WireConnection;3;1;7;0
WireConnection;105;0;97;0
WireConnection;57;1;87;0
WireConnection;103;0;72;0
WireConnection;103;2;100;0
WireConnection;47;0;45;0
WireConnection;43;0;46;0
WireConnection;43;1;105;0
WireConnection;31;0;47;0
WireConnection;31;1;49;0
WireConnection;102;0;103;0
WireConnection;102;1;101;1
WireConnection;102;2;3;0
WireConnection;62;0;57;4
WireConnection;65;0;32;0
WireConnection;61;0;57;4
WireConnection;61;1;32;0
WireConnection;66;0;62;0
WireConnection;66;1;65;0
WireConnection;81;1;3;0
WireConnection;81;0;102;0
WireConnection;42;0;31;0
WireConnection;42;1;43;0
WireConnection;60;0;61;0
WireConnection;60;1;66;0
WireConnection;109;1;7;0
WireConnection;91;1;31;0
WireConnection;91;0;42;0
WireConnection;4;0;2;0
WireConnection;4;1;81;0
WireConnection;114;0;109;4
WireConnection;142;26;91;0
WireConnection;142;23;60;0
WireConnection;142;24;4;0
WireConnection;123;0;122;1
WireConnection;123;1;122;2
WireConnection;123;2;122;3
WireConnection;120;0;109;1
WireConnection;120;1;109;2
WireConnection;120;2;109;3
WireConnection;115;0;114;0
WireConnection;115;1;142;10
WireConnection;113;0;142;10
WireConnection;113;1;115;0
WireConnection;113;2;119;0
WireConnection;121;0;123;0
WireConnection;121;2;120;0
WireConnection;121;3;122;4
WireConnection;121;4;109;4
WireConnection;112;0;113;0
WireConnection;112;1;121;0
WireConnection;111;1;142;10
WireConnection;111;0;112;0
WireConnection;58;0;57;1
WireConnection;58;1;59;0
WireConnection;84;0;4;0
WireConnection;29;0;84;0
WireConnection;29;1;91;0
WireConnection;29;2;111;0
WireConnection;29;3;58;0
WireConnection;29;4;60;0
ASEEND*/
//CHKSM=72DF849E14D60ED9311FB69930C79AD46C7BAD71