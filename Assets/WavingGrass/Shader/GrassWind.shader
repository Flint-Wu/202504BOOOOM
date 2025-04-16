// Base found on https://lindenreid.wordpress.com/2018/01/07/waving-grass-shader-in-unity/
// Had to change many details to make it work. This asset should help people to save their time
// for debugging and experimenting. I also added some improvements like using an emission color.
// This needs a ramp and a gradient texture to work.
Shader "Custom/GrassWind" {
	Properties {
		_RampTex("Ramp", 2D) = "white" {} // 水平渐变纹理，用于光照
		_Color("Color", Color) = (1, 1, 1, 1)//草的颜色
        _WaveSpeed("Wave Speed", float) = 7.0//周期速度
        _WaveAmp("Wave Amp", float) = 1.2//影响草随风飘舞的幅度，越大越明显
        _HeightFactor("Height Factor", float) = 1.5//控制草在不同高度的动画强度
		_HeightCutoff("Height Cutoff", float) = 0.3//草的高度阈值，低于这个高度的草丛不受风的影响，模拟扎根在地面上的效果
        _WindTex("Wind Texture", 2D) = "white" {} // 表示风向和强度的灰度纹理
        _WorldSize("World Size", vector) = (40, 40, 0, 0)   // Use only x and y (y is z in 3D space)
        _WindSpeed("Wind Speed", vector) = (1.5, 1.5, 0, 0) // 风速,material的shader会覆盖shader下面的全局变量，所以这里的变量名不能和shader下面的变量名一样，不然就需要通过material来传递变量的值
        _YOffset("Y offset", float) = 0.0 // y offset, below this is no animation
        _MaxWidth("Max Displacement Width", Range(0, 2)) = 0.1 // width of the line around the dissolve
        _Radius("Radius", Range(0,5)) = 1 // width of the line around the dissolve
        _Brightness("Brightness", Range(0,20)) = 1.8 // Brightness factor
        _Emission("Emission", Color) = (0, 0, 0, 0) // Emission color
		_NoiseScale("Noise Scale", float) = 10.0
		_NoiseStrength("Noise Strength", Range(0, 1)) = 0.2
		_NoiseSpeed("Noise Speed", float) = 0.5
		_NoiseOctaves("Noise Octaves", Range(1, 6)) = 3
		_NoisePersistence("Noise Persistence", Range(0.1, 0.9)) = 0.5
		// _DebugBuffer("Debug Buffer", RWStructuredBuffer<float4>) = null
	}

	SubShader {
		//subshader是Unity渲染的核心部分，包含了材质的渲染状态、渲染队列、渲染器等信息
		// Tags用于设置渲染状态和渲染队列等信息，Pass用于定义渲染器的渲染过程
		Pass {
            Tags {
                "DisableBatching" = "True"//设置禁用批处理，这样可以避免在渲染时将多个物体合并成一个批次，使每个草丛都能单独渲染
            }

			CGPROGRAM
			//pragma 定义了编译器的指令，告诉编译器如何处理着色器代码
			//[CPU] → 顶点数据 → [顶点着色器] → 图元组装 → 光栅化 → [片元着色器] → 输出合并 → [屏幕]
			//vertex vert代表顶点着色器的入口函数，frag代表片元着色器的入口函数
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase // Shadows
			// #pragma target 4.5 // Needed for debugging, can be removed
            #include "UnityCG.cginc"
			
			// RWStructuredBuffer<float4> debugBuffer : register(u1); // Needed for debugging, can be removed

			// Properties
			sampler2D _RampTex;
            sampler2D _WindTex;
            float4 _WindTex_ST;
			float4 _Color;
			float4 _LightColor0; // Provided by Unity
            float4 _WorldSize;
            float _WaveSpeed;
            float _WaveAmp;
            float _HeightFactor;
			float _HeightCutoff;
            float2 _WindSpeed;

			float _MaxWidth;
			float _Radius;
			float _YOffset;

			float _Brightness;
			float4 _Emission;
			float _NoiseScale;
			float _NoiseStrength;
			float _NoiseSpeed;
			int _NoiseOctaves;
			float _NoisePersistence;
			uniform float3 _Positions[100];//最多100个物体可以和草丛交互
			uniform float _PositionArray;

			struct vertexInput {
				//：Postion代表从顶点着色器传入的顶点位置，Normal代表从顶点着色器传入的法线
				//float4 vertex : POSITION; // 4D position (x, y, z, w)w一般默认为1，w=1表示点的坐标是齐次坐标
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};
			float2 hash2(float2 p) {
				return frac(sin(float2(dot(p, float2(127.1, 311.7)), dot(p, float2(269.5, 183.3)))) * 43758.5453);
			}
			
			float perlinNoise(float2 p) {
				float2 pi = floor(p);
				float2 pf = frac(p);
				float2 u = pf * pf * (3.0 - 2.0 * pf);
				
				float n00 = 2.0 * dot(hash2(pi + float2(0.0, 0.0)) - 0.5, pf - float2(0.0, 0.0));
				float n10 = 2.0 * dot(hash2(pi + float2(1.0, 0.0)) - 0.5, pf - float2(1.0, 0.0));
				float n01 = 2.0 * dot(hash2(pi + float2(0.0, 1.0)) - 0.5, pf - float2(0.0, 1.0));
				float n11 = 2.0 * dot(hash2(pi + float2(1.0, 1.0)) - 0.5, pf - float2(1.0, 1.0));
				
				float nx0 = lerp(n00, n10, u.x);
				float nx1 = lerp(n01, n11, u.x);
				float nxy = lerp(nx0, nx1, u.y);
				
				return nxy * 0.5 + 0.5;
			}
			
			float fbm(float2 p, int octaves, float persistence) {
				float total = 0.0;
				float amplitude = 1.0;
				float maxValue = 0.0;
				
				for(int i = 0; i < octaves; i++) {
					total += perlinNoise(p) * amplitude;
					maxValue += amplitude;
					amplitude *= persistence;
					p *= 2.0;
				}
				
				return total / maxValue;
			}
			struct vertexOutput {
				//SV_POSITION是一个语义，用于顶点着色器的输出/片元着色器的输入，表示经过变换的裁剪空间位置
				float4 pos : SV_POSITION;
				float3 normal : NORMAL;
                // float2 sp : TEXCOORD0; // Test sample position by making it visible as color
			};

			//下面是HLSL/CG的代码，返回类型是vertexOutput，函数名是vert，参数是vertexInput类型的input
			vertexOutput vert(vertexInput input) {
				vertexOutput output;
				//unity的空间有以下几种：https://zhuanlan.zhihu.com/p/6911917465
				//1.模型空间：物体的局部坐标系，物体的顶点坐标是相对于物体的原点的坐标
				//2.世界空间：物体在场景中的位置，物体的顶点坐标是相对于场景的原点的坐标
				//3.裁剪空间：物体在屏幕上的位置，物体的顶点坐标是相对于屏幕的原点的坐标
				//4.屏幕空间：物体在屏幕上的位置，物体的顶点坐标是相对于屏幕的左下角的坐标

				// 将顶点位置转换到裁剪空间，使用Unity内置的函数UnityObjectToClipPos
				//裁剪空间是一个三维空间，裁剪空间中的坐标范围是[-1, 1]，表示屏幕的左下角和右上角
				//转换为裁剪空间的意义是避免了手动计算投影矩阵和视图矩阵，简化了代码
				output.pos = UnityObjectToClipPos(input.vertex);
				// 处理法线向量，从模型空间转换到世界空间，并保持归一化
				float4 normal4 = float4(input.normal, 0.0);
				output.normal = normalize(mul(normal4, unity_WorldToObject).xyz);

                // Get vertex world position
				//mul:矩阵乘法,得到顶点在世界空间中的位置
                float4 worldPos = mul(unity_ObjectToWorld, input.vertex);

                // 这段代码将三维世界坐标转换为二维纹理坐标，_WorldSize.xy是一个向量，表示世界空间的大小
                //简化风场是一个二维平面，忽略了z轴的影响（没有风剪切）
				float2 samplePos = worldPos.xz/_WorldSize.xy;
                // Scroll sample position based on time
                samplePos += _Time.x * _WindSpeed.xy;
				//samplePos += (_Time.y  + sin(_Time.y * 0.3)) * _WindSpeed.xy;
				//fmod为浮点数取模运算，返回值在[0, 1)之间，这就实现了了纹理坐标的循环效果
				samplePos = float2(fmod(samplePos.x, 1), fmod(samplePos.y, 1)).xyxy;

				
                //在定义的纹理坐标范围内采样风纹理，_WindTex是一个二维纹理，表示风的强度和方向
                float windSample = tex2Dlod(_WindTex, float4(samplePos, 0, 0));
                float2 noisePos = worldPos.xz / _NoiseScale + _Time.y * _NoiseSpeed;
				float noiseValue = fbm(noisePos, _NoiseOctaves, _NoisePersistence);

				// 将噪声与风力纹理混合
				windSample = lerp(windSample, noiseValue, _NoiseStrength);
				// output.sp = samplePos; // Test sample position by making it visible as color

                // No animation below _HeightCutoff
				//设置一个高度阈值，低于这个高度的草丛不受风的影响，模拟扎根在地面上的效果
                float heightFactor = input.vertex.y > _HeightCutoff;
				// Make animation stronger with height
				heightFactor = heightFactor * pow(abs(input.vertex.y), _HeightFactor);

                // Apply wave animation
                // output.pos.z += (sin(_WaveSpeed*windSample)*_WaveAmp * heightFactor);
				float interactionFactor;
				if (UNITY_MATRIX_P[3][3] == 1) { // Orthographic 假如是正交投影
					//output.pos.x 增加了一个值，这个值是根据风的强度和幅度计算出来的，表示草丛在风的作用下的位移
					//正交投影下摆动的幅度会更小，草丛在风的作用下的位移会更小
					output.pos.x += cos(_WaveSpeed*windSample)*_WaveAmp * heightFactor / 10;
					interactionFactor = 0.5;
				} else { // With perspective 假如是透视投影
					output.pos.x += cos(_WaveSpeed*windSample)*_WaveAmp * heightFactor;
					interactionFactor = 4;
				}

				// Interaction radius movement for every position in array
				// 表示与草与互动物体的交互效果
			    for (int i = 0; i < _PositionArray; i++){
					float3 dis = distance(_Positions[i], worldPos); // Distance for radius
					float3 radius = 1 - saturate(dis / _Radius); // In world radius based on objects interaction radius
					float3 sphereDisp = worldPos - _Positions[i]; // Position comparison
					sphereDisp *= radius; // Position multiplied by radius for falloff

					// Vertex movement based on falloff and clamped
					output.pos.x += /*float2(interactionFactor, 1)*/ interactionFactor * clamp(sphereDisp.x/*z*/ /** step(_YOffset, output.pos.y)*/, -_MaxWidth, _MaxWidth);
				}

				return output;
			}

			float4 frag(vertexOutput input) : COLOR {
				//表示片元着色器的输出颜色，返回值是一个四维向量，表示RGBA颜色值
				// Normalize light dir
				float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);

				// Apply lighting
				float ramp = clamp(dot(input.normal, lightDir), 0.001, 1.0);
				float3 lighting = tex2D(_RampTex, float2(ramp, 0.5)).rgb;
				
                // return float4(frac(input.sp.x), 0, 0, 1); // Test sample position by making it visible as color
				
				float3 rgb = _LightColor0.rgb * _Brightness * lighting * _Color.rgb + _Emission.xyz;
				return float4(rgb, 1.0);
			}
			// 在顶点和片元着色器之前添加噪声函数
			// 2D 柏林噪声实现
			ENDCG
		}

	}
}
