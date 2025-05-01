Shader "Unlit/PaletteSwap"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SourcePalette ("Source Palette", 2D) = "white" {}
        _TargetPalette ("Target Palette", 2D) = "white" {}
        _Threshold ("Color Match Threshold", Range(0, 1)) = 0.01
    }
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
        LOD 100

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            sampler2D _SourcePalette;
            sampler2D _TargetPalette;
            float4 _MainTex_ST;
            float4 _SourcePalette_ST;
            float4 _TargetPalette_ST;
            float _Threshold;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float ColorDistance(float3 col1, float3 col2)
            {
                float3 delta = abs(col1 - col2);
                return max(max(delta.r, delta.g), delta.b);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;
                
                // If pixel is fully transparent, return it as is
                if (col.a < 0.01)
                    return fixed4(0, 0, 0, 0);
                
                // Find the closest color in the source palette
                float minDist = 1.0;
                float bestIndex = -1;
                
                // Sample each pixel in the source palette
                [unroll(16)]
                for (int x = 0; x < 16; x++)
                {
                    float2 paletteUV = float2((x + 0.5) / 16.0, 0.5);
                    float4 paletteColor = tex2D(_SourcePalette, paletteUV);
                    
                    // Skip fully transparent palette colors
                    if (paletteColor.a < 0.01) continue;
                    
                    float dist = ColorDistance(col.rgb, paletteColor.rgb);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        bestIndex = x;
                    }
                }
                
                // If we found a close enough match, use the corresponding target color
                if (bestIndex >= 0 && minDist < _Threshold)
                {
                    float2 targetUV = float2((bestIndex + 0.5) / 16.0, 0.5);
                    fixed4 targetColor = tex2D(_TargetPalette, targetUV);
                    return fixed4(targetColor.rgb, col.a);
                }
                
                // If no match found, return original color
                return col;
            }
            ENDCG
        }
    }
}
