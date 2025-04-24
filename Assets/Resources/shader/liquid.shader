// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
 
Shader "Unlit/FX/Liquid"
{
    Properties
    {
        [Header(Main)]
        [HDR]_Tint ("Tint", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        [HDR]_TopColor ("Top Color", Color) = (1,1,1,1)
        [Header(Foam)]
        [HDR]_FoamColor ("Foam Line Color", Color) = (1,1,1,1)
        _Line ("Foam Line Width", Range(0,0.1)) = 0.0    
        _LineSmooth ("Foam Line Smoothness", Range(0,0.1)) = 0.0    
        [Header(Rim)]
        [HDR]_RimColor ("Rim Color", Color) = (1,1,1,1)
        _RimPower ("Rim Power", Range(0,10)) = 0.0
        [Header(Sine)]
        _Freq ("Frequency", Range(0,15)) = 8
        _Amplitude ("Amplitude", Range(0,0.5)) = 0.15
        _ModelHeight ("Model Height", Range(0,10)) = 0.017
    }
    
    SubShader
    {
        Tags {"Queue"="Geometry"  "DisableBatching" = "True" }
        
        Pass
        {
            Zwrite On
            Cull Off // we want the front and back faces
            AlphaToMask On // transparency
 
            CGPROGRAM
            #pragma enable_d3d11_debug_symbols
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL; 
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 viewDir : COLOR;
                float3 normal : COLOR2;     
                float3 fillPosition : TEXCOORD2;
                float3 worldNormal : TEXCOORD3;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float3 _FillAmount;
            float _WobbleX, _WobbleZ;
            float4 _TopColor, _RimColor, _FoamColor, _Tint;
            float _Line, _RimPower, _LineSmooth;
            float _Freq, _Amplitude;
 
            float _ModelHeight;
            float _TopY;
            // https://docs.unity3d.com/Packages/com.unity.shadergraph@6.9/manual/Rotate-About-Axis-Node.html
            /// <summary>
            /// 将输入向量 In 围绕轴 Axis 旋转 （旋转） 的值。旋转角度的单位可通过参数 Unit 进行选择。
            /// </summary>
            float3 Unity_RotateAboutAxis_Degrees(float3 In, float3 Axis, float Rotation)
            {
                Rotation = radians(Rotation);
                float s = sin(Rotation);
                float c = cos(Rotation);
                float one_minus_c = 1.0 - c;
 
                Axis = normalize(Axis);
                float3x3 rot_mat = 
                {   one_minus_c * Axis.x * Axis.x + c, one_minus_c * Axis.x * Axis.y - Axis.z * s, one_minus_c * Axis.z * Axis.x + Axis.y * s,
                    one_minus_c * Axis.x * Axis.y + Axis.z * s, one_minus_c * Axis.y * Axis.y + c, one_minus_c * Axis.y * Axis.z - Axis.x * s,
                    one_minus_c * Axis.z * Axis.x - Axis.y * s, one_minus_c * Axis.y * Axis.z + Axis.x * s, one_minus_c * Axis.z * Axis.z + c
                };
                float3 Out = mul(rot_mat,  In);
                return Out;
            }
 
 
            v2f vert (appdata v)
            {
                v2f o;
                //将顶点坐标转换为裁剪空间坐标，v.vertex是顶点坐标（模型的局部坐标），o.vertex是裁剪空间坐标
                //裁剪空间在本代码中的作用是将顶点坐标转换为屏幕坐标，以便进行后续的光栅化和片元着色器处理
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                // 得到世界坐标：代表了顶点与液面的相对位置关系

                float3 worldPos = mul (unity_ObjectToWorld, v.vertex.xyz); 
                //对坐标进行归一化处理，_ModelHeight是一个浮点数，表示模型的高度
                float3 normWorldPos = worldPos/ _ModelHeight;
                //计算相对填充位置
                float3 worldPosOffset = float3(normWorldPos.x, normWorldPos.y , normWorldPos.z) - (_FillAmount);
                //float3 worldPosOffset = float3(worldPos.x, worldPos.y, worldPos.z) - (_FillAmount);
                //计算位置偏移向量绕Z轴和X轴旋转90度
                float3 worldPosX= Unity_RotateAboutAxis_Degrees(worldPosOffset, float3(0,0,1),90);
                float3 worldPosZ = Unity_RotateAboutAxis_Degrees(worldPosOffset, float3(1,0,0),90);
                // combine rotations with worldPos, based on sine wave from script
                float3 worldPosAdjusted = normWorldPos + (worldPosX  * _WobbleX)+ (worldPosZ* _WobbleZ); 
                // 顶点的填充位置,fillPosition.y相当于顶点相对于液面的高度
                o.fillPosition =  worldPosAdjusted - (_FillAmount);
                o.viewDir = normalize(WorldSpaceViewDir(v.vertex));
                o.normal = v.normal;
                o.worldNormal  = mul ((float4x4)unity_ObjectToWorld, v.normal );
                return o;
            }
            
            fixed4 frag (v2f i, fixed facing : VFACE) : SV_Target
            {          
                float3 worldNormal = mul( unity_ObjectToWorld, float4( i.normal, 0.0 ) ).xyz;
                // rim light
                //边缘光计算（菲涅尔效应）
                //dot函数计算两个向量的点积，点积越大，两个向量越接近，值越小，两个向量越远              
                float fresnel = pow(1 - saturate(dot(worldNormal, i.viewDir)), _RimPower);          
                float4 RimResult = fresnel * _RimColor;
                RimResult *= _RimColor;
                
                // add movement based deform, using a sine wave
                //创建液面的波动效果，使用正弦函数来模拟液体的波动，波纹振幅和频率可以通过参数来控制
                //o.fillPosition是一个三维向量，表示顶点在液体中的位置，o.fillPosition.y表示顶点相对于液面的高度
                float wobbleIntensity =  abs(_WobbleX) + abs(_WobbleZ);            
                float wobble = sin((i.fillPosition.x * _Freq) + (i.fillPosition.z * _Freq ) + ( _Time.y)) * (_Amplitude *wobbleIntensity);               
                float movingfillPosition = i.fillPosition.y + wobble;
 
                // sample the texture based on the fill line
                //根据填充线的位置采样纹理，_MainTex是一个二维纹理，表示液体的颜色和纹理
                fixed4 col = tex2D(_MainTex, movingfillPosition) * _Tint;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
 
                // foam edge
                //创建液面的泡沫边缘效果，使用step函数和smoothstep函数来创建泡沫的边缘效果
                //movingfillPosition<=0.5表示顶点在液体中的位置，movingfillPosition>0.5表示顶点在液体上方
                
                //采用normTop，归一化顶部高度来取代0.5，这样模型不论是否居中都能正常工作
                float normTop = _TopY/ _ModelHeight;

                float cutoffTop = step(movingfillPosition, normTop);
                float foam = cutoffTop * smoothstep(normTop - _Line- _LineSmooth, normTop - _Line ,movingfillPosition);
                float4 foamColored = foam * _FoamColor;
 
                // rest of the liquid minus the foam
                //液面主体渲染，减去泡沫部分
                float result = cutoffTop - foam;
                float4 resultColored = result * col;
 
                // both together, with the texture
                //6. 组合液体效果，添加边缘光特效到Rgb通道
                float4 finalResult = resultColored + foamColored;               
                finalResult.rgb += RimResult;
 
                // little edge on the top of the backfaces
                //液面顶部处理
                float backfaceFoam = (cutoffTop * smoothstep(normTop - (0.2 * _Line)- _LineSmooth,normTop - (0.2 * _Line),movingfillPosition ));
                float4 backfaceFoamColor = _FoamColor * backfaceFoam;
                // color of backfaces/ top
                float4 topColor = (_TopColor * (1-backfaceFoam) + backfaceFoamColor) * (foam + result);
 
                // clip above the cutoff
                //8. 裁剪与面朝向处理
                clip(result + foam - 0.01);
 
                //VFACE returns positive for front facing, negative for backfacing
                return facing > 0 ? finalResult: topColor;
                
                
            }
            ENDCG
        }
        
    }
}