﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Sprites/Outline"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        [PerRendererData] _Color("Tint", Color) = (1,1,1,1)
        [PerRendererData] _FlashColor ("Flash Color", Color) = (1,1,1,1)
        [PerRendererData] _Outline("Outline", Float) = 0
        [PerRendererData] _OutlineColor("Outline Color", Color) = (1,1,1,1)
        [PerRendererData] _OutlineSize("Outline Size", int) = 1
        [PerRendererData] _Rect("Rect Display", Vector) = (0,0,1,1)

    }

        SubShader
    {
        Tags
    {
        "Queue" = "Transparent"
        "IgnoreProjector" = "True"
        "RenderType" = "Transparent"
        "PreviewType" = "Plane"
        "CanUseSpriteAtlas" = "True"
    }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
    {
        CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile _ PIXELSNAP_ON
#pragma shader_feature ETC1_EXTERNAL_ALPHA
#include "UnityCG.cginc"

    struct appdata_t
    {
        float4 vertex   : POSITION;
        float4 color    : COLOR;
        float2 texcoord : TEXCOORD0;
    };

    struct v2f
    {
        float4 vertex   : SV_POSITION;
        fixed4 color : COLOR;
        float2 texcoord  : TEXCOORD0;
    };

    fixed4 _Color;
    float _Outline;
    fixed4 _OutlineColor;
    int _OutlineSize;
    fixed4 _Rect;
    fixed4 _FlashColor;

    v2f vert(appdata_t IN)
    {
        v2f OUT;
        OUT.vertex = UnityObjectToClipPos(IN.vertex);
        OUT.texcoord = IN.texcoord;
        OUT.color = IN.color * _Color;
#ifdef PIXELSNAP_ON
        OUT.vertex = UnityPixelSnap(OUT.vertex);
#endif

        return OUT;
    }

    sampler2D _MainTex;
    sampler2D _AlphaTex;
    float4 _MainTex_TexelSize;

    fixed4 SampleSpriteTexture(float2 uv)
    {
        fixed4 color = tex2D(_MainTex, uv);

        return color;
    }

    fixed4 frag(v2f IN) : SV_Target
    {
        fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;

    // If outline is enabled and there is a pixel, try to draw an outline.
    if (_Outline > 0 && c.a != 0) {
        float totalAlpha = 1.0;

        if (IN.texcoord.x < _Rect.x + _MainTex_TexelSize.x || IN.texcoord.y < _Rect.y + _MainTex_TexelSize.y ||
            IN.texcoord.x > _Rect.z - _MainTex_TexelSize.x || IN.texcoord.y > _Rect.w - _MainTex_TexelSize.y)
        {
            totalAlpha = 0;
        }
        else
        {
            [unroll(16)]
            for (int i = 1; i < _OutlineSize + 1; i++) {
                fixed4 pixelUp = tex2D(_MainTex, IN.texcoord + fixed2(0, i * _MainTex_TexelSize.y));
                fixed4 pixelDown = tex2D(_MainTex, IN.texcoord - fixed2(0, i *  _MainTex_TexelSize.y));
                fixed4 pixelRight = tex2D(_MainTex, IN.texcoord + fixed2(i * _MainTex_TexelSize.x, 0));
                fixed4 pixelLeft = tex2D(_MainTex, IN.texcoord - fixed2(i * _MainTex_TexelSize.x, 0));

                totalAlpha = totalAlpha * pixelUp.a * pixelDown.a * pixelRight.a * pixelLeft.a;
            }
        }

        if (totalAlpha == 0) {
            c.rgba = fixed4(1, 1, 1, 1) * _OutlineColor;
        }
    }

    c.rgb = lerp(c.rgb,_FlashColor.rgb,_FlashColor.a);
    c.rgb *= c.a;

    return c;
    }
        ENDCG
    }
    }
}