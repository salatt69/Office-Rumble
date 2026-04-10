Shader "Custom/SkyClouds 2D"
{
    Properties
    {
        _SkyColorTop ("Sky Color Top", Color) = (0.2, 0.5, 0.9, 1)
        _SkyColorBottom ("Sky Color Bottom", Color) = (0.6, 0.8, 1.0, 1)
        _CloudColor ("Cloud Color", Color) = (1, 1, 1, 1)
        _CloudDensity ("Cloud Density", Range(0, 2)) = 0.8
        _CloudSpeed ("Cloud Speed", Range(0, 1)) = 0.1
        _CloudScale ("Cloud Scale", Range(0.5, 10)) = 3.0
        _Brightness ("Brightness", Range(0, 2)) = 1.0
        _PixelSize ("Pixel Size", Int) = 0
    }

    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Opaque" }
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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _SkyColorTop;
            float4 _SkyColorBottom;
            float4 _CloudColor;
            float _CloudDensity;
            float _CloudSpeed;
            float _CloudScale;
            float _Brightness;
            int _PixelSize;

            float hash(float2 p)
            {
                float3 p3 = frac(float3(p.xyx) * 0.1031);
                p3 += dot(p3, p3.yzx + 33.33);
                return frac((p3.x + p3.y) * p3.z);
            }

            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                f = f * f * (3.0 - 2.0 * f);

                float a = hash(i);
                float b = hash(i + float2(1.0, 0.0));
                float c = hash(i + float2(0.0, 1.0));
                float d = hash(i + float2(1.0, 1.0));

                return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
            }

            float fbm(float2 p)
            {
                float f = 0.0;
                float amp = 0.5;
                for(int i = 0; i < 5; i++)
                {
                    f += amp * noise(p);
                    p *= 2.0;
                    amp *= 0.5;
                }
                return f;
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float4 screenPos = ComputeScreenPos(o.vertex);
                o.uv = screenPos.xy / screenPos.w;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                
                if(_PixelSize > 0)
                {
                    float2 pixelCount = float2(_PixelSize, _PixelSize);
                    uv = floor(uv * pixelCount) / pixelCount;
                }
                
                uv.x += _Time.y * _CloudSpeed;

                float n1 = fbm(uv * _CloudScale);
                float n2 = fbm(uv * _CloudScale * 2.0 + float2(_Time.y * _CloudSpeed * 0.5, 0.0));
                
                float cloud = n1 * 0.6 + n2 * 0.4;
                cloud = smoothstep(0.4, 0.7, cloud * _CloudDensity);

                float3 sky = lerp(_SkyColorBottom.rgb, _SkyColorTop.rgb, uv.y);
                float3 finalColor = lerp(sky, _CloudColor.rgb, cloud) * _Brightness;

                return float4(finalColor, 1.0);
            }
            ENDCG
        }
    }
}