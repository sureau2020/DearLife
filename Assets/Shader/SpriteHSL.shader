Shader "Unlit/SpriteHSL"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Hue("Hue", Range(-1,1)) = 0
        _Saturation("Saturation", Range(0,2)) = 1
        _Lightness("Lightness", Range(0,2)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha   // 正确透明混合
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

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
            float _Hue;
            float _Saturation;
            float _Lightness;

            v2f vert(appdata v) 
            { 
                v2f o; 
                o.vertex = UnityObjectToClipPos(v.vertex); 
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o; 
            }

            // --- HSL 转换函数 ---
            float3 rgb2hsl(float3 c)
            {
                float maxc = max(c.r, max(c.g, c.b));
                float minc = min(c.r, min(c.g, c.b));
                float l = (maxc + minc) * 0.5;
                float s = 0;
                float h = 0;

                if (maxc != minc)
                {
                    float d = maxc - minc;
                    s = l > 0.5 ? d / (2.0 - maxc - minc) : d / (maxc + minc);

                    if (maxc == c.r) h = (c.g - c.b) / d + (c.g < c.b ? 6 : 0);
                    else if (maxc == c.g) h = (c.b - c.r) / d + 2;
                    else h = (c.r - c.g) / d + 4;

                    h /= 6;
                }

                return float3(h, s, l);
            }

            float3 hsl2rgb(float3 hsl)
            {
                float3 rgb;
                if (hsl.y == 0)
                {
                    rgb = float3(hsl.z, hsl.z, hsl.z);
                }
                else
                {
                    float q = hsl.z < 0.5 ? hsl.z * (1 + hsl.y) : hsl.z + hsl.y - hsl.z * hsl.y;
                    float p = 2 * hsl.z - q;
                    float3 t = float3(hsl.x + 1.0/3.0, hsl.x, hsl.x - 1.0/3.0);

                    for (int i = 0; i < 3; i++)
                    {
                        if (t[i] < 0) t[i] += 1;
                        if (t[i] > 1) t[i] -= 1;

                        if (t[i] < 1.0/6.0) rgb[i] = p + (q - p) * 6 * t[i];
                        else if (t[i] < 0.5) rgb[i] = q;
                        else if (t[i] < 2.0/3.0) rgb[i] = p + (q - p) * (2.0/3.0 - t[i]) * 6;
                        else rgb[i] = p;
                    }
                }
                return rgb;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                // 保留透明像素
                if (col.a == 0) discard;

                float3 hsl = rgb2hsl(col.rgb);
                hsl.x = frac(hsl.x + _Hue);       
                hsl.y *= _Saturation;
                hsl.z *= _Lightness;
                col.rgb = hsl2rgb(hsl);

                return col;
            }
            ENDCG
        }
    }
}
