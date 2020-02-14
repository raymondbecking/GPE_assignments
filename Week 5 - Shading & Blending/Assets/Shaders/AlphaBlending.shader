Shader "HvA_PostFX_Examples/AlphaBlending"
{
	Properties
	{
		_MaskTex("Alpha Mask Texture", 2D) = "white" {}
		_MainTex("Main Texture", 2D) = "white" {}
	}
	SubShader
	{
		// inside SubShader
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" }

		LOD 100

		Pass
		{
			// inside Pass to apply blending
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

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

			sampler2D _MaskTex;
			sampler2D _MainTex;

			float4 _MaskTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MaskTex);

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 mask = tex2D(_MaskTex, i.uv);
				fixed4 main = tex2D(_MainTex, i.uv);

				return float4(main.r, main.g, main.b, mask.g);
			}
			ENDCG
		}
	}
}
