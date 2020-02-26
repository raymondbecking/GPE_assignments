Shader "ParallaxMap"
{
    Properties
    {
	
		_Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
		_HeightTex ("Heightmap (R)", 2D) = "gray" {}
		_Normal ("Normal", 2D) = "bump" {}
		_Parallax("Height Amount", Range(0, 1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

            CGPROGRAM
			#pragma surface surf Standard fullforwardshadows
			#pragma target 3.0
			#pragma shader_feature HEIGHT_MAP

            #include "UnityCG.cginc"

			
			float _Amount;
			fixed4 _Color;
			
         

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
			float _ParallaxStrength;



			void surf (Input IN, inout SurfaceOutputStandard o)
			{
			// Displacement
			float heightTex = tex2D(_HeightTex, IN.uv_HeightTex).r;
            float2 parallaxOffset = ParallaxOffset(heightTex, _Parallax, IN.viewDir);

            fixed4 c = tex2D (_MainTex, IN.uv_MainTex + parallaxOffset) * _Color;

			//Normal mapping
			o.Normal = UnpackNormal(tex2D(_Normal, IN.uv_Normal + parallaxOffset));

            o.Albedo = c.rgb;
			}
			
            ENDCG
        
    }
}
