Shader "HvA/TextureBlending_transform"
{
	Properties
	{
		_MaskTex("Mask", 2D) = "white" {}
		_GrassTex("Tex 1", 2D) = "white" {}
		_DirtTex("Tex 2", 2D) = "white" {}
		_WaterTex("Tex 3", 2D) = "white" {}
		_Speed("WaterSpeed", float) = 1
	}
	SubShader
	{
		LOD 100

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

			sampler2D _MaskTex;
			sampler2D _GrassTex;
			sampler2D _DirtTex;
			sampler2D _WaterTex;

			float4 _MaskTex_ST;
			float _Speed;
			
			v2f vert (appdata v)
			{
				v2f o;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MaskTex);

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the textures
				fixed4 mask = tex2D(_MaskTex, i.uv);
				fixed4 grass = tex2D(_GrassTex, i.uv);
				fixed4 dirt = tex2D(_DirtTex, i.uv);
				fixed4 water = tex2D(_WaterTex, i.uv + _Time.y * _Speed); //changes the uv's over time * speed propperties
				
				// mask the different texture by the corrosponding rgb channels in the mask
				grass = grass * mask.r;
				dirt = dirt * mask.g;
				water = water * mask.b;

				// combine all masked textures as output
				return grass + dirt + water;
			}
			ENDCG
		}
	}
}
