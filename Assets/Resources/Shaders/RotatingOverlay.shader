Shader "Unlit/RotatingOverlay"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
        _Mask1 ("Mask1", Color) = (1,1,1,1)
        _Mask2 ("Mask2", Color) = (1,1,1,1)
        _Overlay ("Overlay Texture", 2D) = "white" {}
        _Speed ("Move Speed", Float) = 1
        _Offset ("Spin Center Offset", Vector) = (0, 0, 0, 0)
        _Scale ("Overlay Scale", Float) = 1
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
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
			#include "UnityCG.cginc"
            #include "Assets/Resources/Shaders/utils.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
			};
			
			fixed4 _Color;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;

				return OUT;
			}

			sampler2D _MainTex;
            sampler2D _Overlay;
            float4 _MainTex_TexelSize;
            float4 _Overlay_TexelSize;
            fixed4 _Mask1;
            fixed4 _Mask2;
            float _Speed;
            float4 _Offset;
            float _Scale;

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);

				return color;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
                // get the normal color
				fixed4 c = SampleSpriteTexture (IN.texcoord) * IN.color;

                // get the mask color
                // it's gonna be a different size texture, so normalize to pixels for uv lookup
                fixed2 maskUV = (IN.texcoord / _MainTex_TexelSize) * _Overlay_TexelSize;
                //then rotate it
                maskUV = rotateUV((maskUV + _Offset)*_Scale, _Time.w * _Speed);
                fixed4 overlay = tex2D (_Overlay, maskUV);

                // if there's a match with either mask color
                if (any(compareColor(c, _Mask1, 0.001) || compareColor(c, _Mask2, 0.001))) {
                    c.rgb = lerp(c.rgb, overlay.rgb, overlay.a);
                }


				c.rgb *= c.a;
				return c;
			}
		ENDCG
		}
	}
}