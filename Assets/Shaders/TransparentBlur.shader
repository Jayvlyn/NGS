Shader "Custom/TransparentBlur"
{
    Properties
    {
        _BlurSize ("Blur Size", Range(0, 20)) = 5
        _Intensity ("Blur Intensity", Range(0, 1)) = 1
    }

    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "IgnoreProjector"="True"
        }

        GrabPass 
        { 
            "_BlurBackgroundTexture"
        }

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
                float4 pos : SV_POSITION;
                float4 grabPos : TEXCOORD0;
            };

            sampler2D _BlurBackgroundTexture;
            float4 _BlurBackgroundTexture_TexelSize;
            float _BlurSize;
            float _Intensity;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.grabPos = ComputeGrabScreenPos(o.pos);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                float2 grabUV = i.grabPos.xy / i.grabPos.w;
                
                half4 sum = half4(0, 0, 0, 0);
                float blur = _BlurSize * 0.01;

                // Sample a 3x3 grid
                for(int x = -1; x <= 1; x++)
                {
                    for(int y = -1; y <= 1; y++)
                    {
                        float2 offset = float2(x, y) * _BlurBackgroundTexture_TexelSize.xy * blur;
                        sum += tex2D(_BlurBackgroundTexture, grabUV + offset);
                    }
                }
                sum /= 9.0;

                half4 originalColor = tex2D(_BlurBackgroundTexture, grabUV);
                return lerp(originalColor, sum, _Intensity);
            }
            ENDCG
        }
    }
    Fallback Off
} 