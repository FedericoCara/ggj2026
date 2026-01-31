Shader "Custom/SpriteOutlineAlwaysOnTop"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineThickness ("Outline Thickness (px)", Range(0, 10)) = 1
    }
    SubShader
    {
        Tags
        {
            "Queue"="Overlay"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            fixed4 _Color;
            fixed4 _OutlineColor;
            float _OutlineThickness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 texel = _MainTex_TexelSize.xy * _OutlineThickness;
                float center = tex2D(_MainTex, i.uv).a;

                float a1 = tex2D(_MainTex, i.uv + float2(texel.x, 0)).a;
                float a2 = tex2D(_MainTex, i.uv + float2(-texel.x, 0)).a;
                float a3 = tex2D(_MainTex, i.uv + float2(0, texel.y)).a;
                float a4 = tex2D(_MainTex, i.uv + float2(0, -texel.y)).a;
                float a5 = tex2D(_MainTex, i.uv + float2(texel.x, texel.y)).a;
                float a6 = tex2D(_MainTex, i.uv + float2(-texel.x, texel.y)).a;
                float a7 = tex2D(_MainTex, i.uv + float2(texel.x, -texel.y)).a;
                float a8 = tex2D(_MainTex, i.uv + float2(-texel.x, -texel.y)).a;

                float maxNeighbor = max(a1, max(a2, max(a3, max(a4, max(a5, max(a6, max(a7, a8)))))));
                float outline = saturate(maxNeighbor - center);

                fixed4 outCol = _OutlineColor;
                outCol.rgb *= i.color.rgb;
                outCol.a *= i.color.a * outline;
                return outCol;
            }
            ENDCG
        }
    }
}
