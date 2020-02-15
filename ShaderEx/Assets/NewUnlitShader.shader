Shader "Unlit/NewUnlitShader"
{
    Properties
    {
	
		_Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
		_HeightTex ("Heightmap (R)", 2D) = "gray" {}
		_Normal ("Normal", 2D) = "bump" {}
		_Parallax("Height Amount", float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        //Pass
        //{
            CGPROGRAM
			#pragma surface surf Standard fullforwardshadows
			#pragma target 3.0
			#pragma shader_feature HEIGHT_MAP

            #include "UnityCG.cginc"

			
			float _Amount;
			fixed4 _Color;
			
            //struct appdata
            //{
            //    float4 vertex : POSITION;
            //    float2 uv : TEXCOORD0;
            //};

            //struct v2f
            //{
            //    float2 uv : TEXCOORD0;
            //    float4 vertex : SV_POSITION;
            //};

			struct Input{
			    float2 uv_MainTex;
				float2 uv_HeightTex;
				float2 uv_Normal;
				float3 viewDir;
			};

            
			sampler2D _MainTex;
			sampler2D _HeightTex;
			sampler2D _Normal;
			float _Parallax;


			void surf (Input IN, inout SurfaceOutputStandard o)
			{
			// Displacement
			float heightTex = tex2D(_HeightTex, IN.uv_HeightTex).r;
            float2 parallaxOffset = ParallaxOffset(heightTex, _Parallax, IN.viewDir);
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex + parallaxOffset) * _Color;
			o.Normal = UnpackNormal(tex2D(_Normal, IN.uv_Normal + parallaxOffset));
            o.Albedo = c.rgb;
			}
			
            //v2f vert (appdata v)
            //{
            //    v2f o;
            //    o.vertex = UnityObjectToClipPos(v.vertex);
            //    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
            //    return o;
            //}
            ENDCG
        
    }
}
