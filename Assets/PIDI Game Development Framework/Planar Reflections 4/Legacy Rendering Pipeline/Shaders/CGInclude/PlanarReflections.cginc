
sampler2D _ReflectionTex;
sampler2D _ReflectionTexBlur;
sampler2D _ReflectionDepth;
sampler2D _ReflectionFog;

half4 _ReflectionTint;

half4 _ReflectionTex_TexelSize;

float _ReflectionToAlbedoMix;
float _AlbedoReflectionTint;

half _RefDistortion;


half2 screen2ReflectionUVs( float4 screenPosition ){

	half2 screenPos = screenPosition.xy / max(screenPosition.w,0.0001);
	screenPos.x = 1-screenPos.x;
	return screenPos;

}



half4 PBRBasedBlur( half3 albedo, half smoothness, half4 screenPosition, half maxBlur, half3 viewDir, half3 normal ){

	half2 reflectionUVs = screen2ReflectionUVs(screenPosition);

	half4 reflectionColor = half4(0,0,0,0);
	
	half texLOD = lerp(4,0, smoothness);

	uint samples = max(texLOD*32,1);
	uint sLOD = 1 << (uint)texLOD;
	float sigma = float(samples)*0.25;

	uint s = samples /sLOD;

	reflectionUVs +=  + normal * _RefDistortion;

	for (uint i = 0; i < s*s; i++){
		float2 d = float2(i%s,i/s)*float(sLOD)-float(samples)/2.0;
		float2 uvs = reflectionUVs+_ReflectionTex_TexelSize.xy*d;
#if USE_FOG
		half4 fog = tex2Dlod(_ReflectionFog, half4(uvs, 0, texLOD));
		reflectionColor += exp(-0.5 * dot(d /= sigma, d)) / (6.28 * sigma * sigma) * lerp(tex2Dlod(_ReflectionTex, half4(uvs, 0, texLOD)), half4(fog.rgb, 1), 1);
#else
		reflectionColor += exp(-0.5 * dot(d /= sigma, d)) / (6.28 * sigma * sigma) * tex2Dlod(_ReflectionTex, half4(uvs, 0, texLOD));
#endif
	}

	reflectionColor.a = max(reflectionColor.a,0.000001);

	reflectionColor /= reflectionColor.a;

	reflectionColor.rgb = lerp( reflectionColor.rgb * _ReflectionTint.rgb, reflectionColor.rgb * _ReflectionTint.rgb * albedo.rgb, _AlbedoReflectionTint * length( albedo.rgb ) );

	reflectionColor.rgb *= max( smoothness, 0.02 );

	half fresnelValue = saturate( dot(normal, viewDir) ); 

	reflectionColor *= lerp( 0.5, 1, 1 - fresnelValue );

	return reflectionColor;

}


half4 PBRBasedBlurDepth( half3 albedo, half smoothness, half4 screenPosition, half maxBlur, half3 viewDir, half3 normal ){

	half2 reflectionUVs = screen2ReflectionUVs(screenPosition);

	half4 reflectionColor = half4(0,0,0,0);
	
	half rDepth = pow( tex2D(_ReflectionDepth, reflectionUVs).r , 4 * screenPosition.w );

	half texLOD = lerp(4,0, saturate( smoothness + rDepth * 0.5 ) );

	maxBlur = lerp( 0, maxBlur, 1 - rDepth * 0.25 );

	uint samples = max(texLOD*32,1);
	uint sLOD = 1 << (uint)texLOD;
	float sigma = float(samples)*0.25;

	uint s = samples /sLOD;

	reflectionUVs +=  + normal * _RefDistortion;

	for (uint i = 0; i < s*s; i++){
		float2 d = float2(i%s,i/s)*float(sLOD)-float(samples)/2.0;
		float2 uvs = reflectionUVs+_ReflectionTex_TexelSize.xy*d;
		
#if USE_FOG
		half4 fog = tex2Dlod(_ReflectionFog, half4(uvs, 0, texLOD));
		reflectionColor += exp(-0.5 * dot(d /= sigma, d)) / (6.28 * sigma * sigma) * lerp( tex2Dlod(_ReflectionTex, half4(uvs, 0, texLOD)), half4(fog.rgb,1), fog.a );
#else
		reflectionColor += exp(-0.5 * dot(d /= sigma, d)) / (6.28 * sigma * sigma) * tex2Dlod(_ReflectionTex, half4(uvs, 0, texLOD));
#endif
	}

	reflectionColor.a = max(reflectionColor.a,0.000001);

	reflectionColor /= reflectionColor.a;

	reflectionColor.rgb = lerp( reflectionColor.rgb * _ReflectionTint.rgb, reflectionColor.rgb * _ReflectionTint.rgb * albedo.rgb, _AlbedoReflectionTint * length( albedo.rgb ) );

	reflectionColor.rgb *= max( smoothness, 0.02 );

	half fresnelValue = saturate( dot(normal, viewDir) ); 

	reflectionColor *= lerp( 0.5, 1, 1 - fresnelValue );

	return reflectionColor;

}



half4 PBRBasedBlurFog( half3 albedo, half smoothness, half4 screenPosition, half maxBlur, half3 viewDir, half3 normal ){

	half2 reflectionUVs = screen2ReflectionUVs(screenPosition);

	half4 reflectionColor = half4(0,0,0,0);
	
	half texLOD = lerp(4,0, smoothness);

	uint samples = max(texLOD*32,1);
	uint sLOD = 1 << (uint)texLOD;
	float sigma = float(samples)*0.25;

	uint s = samples /sLOD;

	reflectionUVs +=  + normal * _RefDistortion;

	for (uint i = 0; i < s*s; i++){
		float2 d = float2(i%s,i/s)*float(sLOD)-float(samples)/2.0;
		float2 uvs = reflectionUVs+_ReflectionTex_TexelSize.xy*d;
		half4 fog = tex2Dlod(_ReflectionFog, half4(uvs, 0, texLOD));
		reflectionColor += exp(-0.5 * dot(d /= sigma, d)) / (6.28 * sigma * sigma) * lerp(tex2Dlod(_ReflectionTex, half4(uvs, 0, texLOD)), half4(fog.rgb, 1), fog.a);

	}

	reflectionColor.a = max(reflectionColor.a,0.000001);

	reflectionColor /= reflectionColor.a;

	reflectionColor.rgb = lerp( reflectionColor.rgb * _ReflectionTint.rgb, reflectionColor.rgb * _ReflectionTint.rgb * albedo.rgb, _AlbedoReflectionTint * length( albedo.rgb ) );

	reflectionColor.rgb *= max( smoothness, 0.02 );

	half fresnelValue = saturate( dot(normal, viewDir) ); 

	reflectionColor *= lerp( 0.5, 1, 1 - fresnelValue );

	return reflectionColor;

}


half4 PBRBasedBlurDepthFog( half3 albedo, half smoothness, half4 screenPosition, half maxBlur, half3 viewDir, half3 normal ){

	half2 reflectionUVs = screen2ReflectionUVs(screenPosition);

	half4 reflectionColor = half4(0,0,0,0);
	
	half rDepth = pow( tex2D(_ReflectionDepth, reflectionUVs).r , 4 * screenPosition.w );

	half texLOD = lerp(4,0, saturate( smoothness + rDepth * 0.5 ) );

	maxBlur = lerp( 0, maxBlur, 1 - rDepth * 0.25 );

	uint samples = max(texLOD*32,1);
	uint sLOD = 1 << (uint)texLOD;
	float sigma = float(samples)*0.25;

	uint s = samples /sLOD;

	reflectionUVs +=  + normal * _RefDistortion;

	for (uint i = 0; i < s*s; i++){
		float2 d = float2(i%s,i/s)*float(sLOD)-float(samples)/2.0;
		float2 uvs = reflectionUVs+_ReflectionTex_TexelSize.xy*d;

		half4 fog = tex2Dlod(_ReflectionFog, half4(uvs, 0, texLOD));
		reflectionColor += exp(-0.5 * dot(d /= sigma, d)) / (6.28 * sigma * sigma) * lerp( tex2Dlod(_ReflectionTex, half4(uvs, 0, texLOD)), half4(fog.rgb,1), fog.a );

	}

	reflectionColor.a = max(reflectionColor.a,0.000001);

	reflectionColor /= reflectionColor.a;

	reflectionColor.rgb = lerp( reflectionColor.rgb * _ReflectionTint.rgb, reflectionColor.rgb * _ReflectionTint.rgb * albedo.rgb, _AlbedoReflectionTint * length( albedo.rgb ) );

	reflectionColor.rgb *= max( smoothness, 0.02 );

	half fresnelValue = saturate( dot(normal, viewDir) ); 

	reflectionColor *= lerp( 0.5, 1, 1 - fresnelValue );

	return reflectionColor;

}