Shader "AnythingCafe/OutlineGlow"  
{  
    Properties  
    {  
        _Color ("Color", Color) = (1, 1, 1, 1)  
        _BackgroundColor ("BackgroundColor", Color) = (0, 0, 0, 1)  
        _MainTex ("Base (RGB)", 2D) = "white" {}  
    }  
    SubShader  
    {  
        Pass  
        {  
            CGPROGRAM  
            #pragma vertex vert  
            #pragma fragment frag  

            #include "UnityCG.cginc"  

            float sdfCircle(float2 coord, float2 center, float radius)  
            {  
                float2 offset = coord - center;  
                return sqrt((offset.x * offset.x) + (offset.y * offset.y)) - radius;  
            }  

            float4 render(float d, float3 color, float stroke)   
            {  
                float anti = fwidth(d) * 1.0;  
                float4 colorLayer = float4(color, 1.0 - smoothstep(-anti, anti, d));  
                bool flag = step(0.000001, stroke);  
                float4 strokeLayer = float4(float3(0.05, 0.05, 0.05), 1.0 - smoothstep(-anti, anti, d - stroke));  
                return float4(lerp(strokeLayer.rgb, colorLayer.rgb, colorLayer.a), strokeLayer.a) * flag + colorLayer * (1 - flag);  
            }  

            struct appdata  
            {  
                float4 vertex : POSITION;  
                float2 uv : TEXCOORD0;  
            };  

            struct v2f  
            {  
                float4 pos : SV_POSITION;  
                float2 uv : TEXCOORD0;  
            };  

            fixed4 _Color;  
            fixed4 _BackgroundColor;  
            sampler2D _MainTex;  

            v2f vert (appdata v)  
            {  
                v2f o;  
                o.pos = UnityObjectToClipPos(v.vertex);  
                o.uv = v.uv;  
                return o;  
            }  

            fixed4 frag(v2f i) : SV_Target  
            {  
                float2 pixelPos = i.uv * _ScreenParams.xy;  
                float a = sdfCircle(pixelPos, float2(0.5, 0.5) * _ScreenParams.xy, 100);  
                float4 layer1 = render(a, _Color, fwidth(a) * 2.0);  
                return lerp(_BackgroundColor, layer1, layer1.a);  
            }  
            ENDCG  
        }  
    }  
}
