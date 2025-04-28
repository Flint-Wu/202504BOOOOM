Shader "Custom/VinesGrow" {
 
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Tint ("Color", Color) = (1,1,1,1)
        _grow ("Growth Factor", Range(0,20)) = 0
        _growIntensity ("Growth Intensity", Range(0,5)) = 1.0
        _thershold ("Threshold", Range(0,1)) = 0.5 // 尖端的大小
        _sliceOffset ("Slice Offset", Range(-1,1)) = 0 // 控制切片平面的偏移
        //生长轴为网格X,Y,Z轴
        _growAxis ("Growth Axis", Float) = 1 // 0: X轴, 1: Y轴, 2: Z轴
        _expandScale("Scale", Float) = 1.0 // 缩放因子
    }
 
    SubShader {
        Tags {"Queue"="Geometry" "RenderType"="Opaque" "DisableBatching"="True"}

        CGPROGRAM
        #pragma target 3.0
        #pragma surface surf Lambert vertex:vert addshadow

        // 声明变量
        sampler2D _MainTex;
        fixed4 _Tint;
        float _grow;
        float _growIntensity;
        float _sliceOffset;
        float _thershold;
        float _growAxis; // 0: X轴, 1: Y轴, 2: Z轴
        float _expandScale; // 缩放因子


        struct Input {
            float2 uv_MainTex;
            float sliceAmount; // 切片值
        };

        // 顶点操作函数 - 仅传递切片信息
        void vert(inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            
            //转换v.vertex为世界坐标
            // 计算生长值，决定切片位置
            float vertexHeight = 0.0f;
            float sliceHeight = 0.0f;
            if(_growAxis == 1) // Y轴生长
            {

                vertexHeight = v.vertex.y;
                //float vertexWidth = abs(v.vertex.x) + abs(v.vertex.z);
                
                // 生长因子影响切片位置
                sliceHeight = _grow * _growIntensity - _sliceOffset;
                if(vertexHeight<sliceHeight)
                {
                    //节点向中心移动
                    float scale = saturate((sliceHeight-vertexHeight)/_expandScale);
                    v.vertex.x = v.vertex.x * scale;
                    v.vertex.z = v.vertex.z * scale;
                }
            }
            else if(_growAxis == 0) // X轴生长
            {
                vertexHeight = v.vertex.x;
                //float vertexWidth = abs(v.vertex.y) + abs(v.vertex.z);
                
                // 生长因子影响切片位置
                sliceHeight = _grow * _growIntensity - _sliceOffset;
                if(vertexHeight<sliceHeight)
                {
                    //节点向中心移动
                    float scale = saturate((sliceHeight-vertexHeight)/_expandScale);
                    v.vertex.y = v.vertex.y * scale;
                    v.vertex.z = v.vertex.z * scale;
                }
            }
            else if(_growAxis == 2) // Z轴生长
            {
                vertexHeight = v.vertex.z;
                //float vertexWidth = abs(v.vertex.x) + abs(v.vertex.y);
                
                // 生长因子影响切片位置
                sliceHeight = _grow * _growIntensity - _sliceOffset;
                if(vertexHeight<sliceHeight)
                {
                    //节点向中心移动
                    float scale = saturate((sliceHeight-vertexHeight)/_expandScale);
                    v.vertex.x = v.vertex.x * scale;
                    v.vertex.y = v.vertex.y * scale;
                }
            }
            // else
            // {
            //     // 节点向外移动
            //     float scale = saturate((vertexHeight-sliceHeight)/3);
            //     v.vertex.x = v.vertex.x * scale;
            //     v.vertex.z = v.vertex.z * scale;
            // }
            // 传递顶点与切片面的相对位置
            o.sliceAmount = vertexHeight - sliceHeight;
        }
        
        // 表面着色器
        void surf(Input IN, inout SurfaceOutput o) {
            // 低于切片面的像素被丢弃
            clip(-IN.sliceAmount-0.5f);
            
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Tint;
            
            // 切片边缘高亮效果
            float edgeEffect = 1.0 - saturate(IN.sliceAmount * 10); // 控制边缘高亮宽度
            c.rgb = lerp(c.rgb, c.rgb * 1.5, edgeEffect * 0.5); // 边缘高亮
            
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }

        ENDCG
    }
     
    Fallback "Diffuse"
}