Shader "Custom/ParallaxOcclusionMap" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo", 2D) = "white" {}
		_BumpMap ("Normal map", 2D) = "bump" {}
		_BumpAmount ("Bump scale", Range(0,1)) = 1
		_ParallaxMap ("Height map", 2D) = "white" {}
		_Parallax ("Height scale", Range(0,1)) = 0.05
		_EmissionMap ("Emission map", 2D) = "black" {}
		_Emission ("Emission", Color) = (0, 0, 0)
		_Glossiness ("Glossiness/Wetness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_ParallaxMinSamples ("Min samples", Range(2,100)) = 4
		_ParallaxMaxSamples ("Max samples", Range(2,10000)) = 20
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows vertex:vert
		#pragma shader_feature _EMISSION_MAP
		#pragma target 3.0
		
		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _ParallaxMap;
		sampler2D _EmissionMap;

		struct Input {
			float2 texcoord;
			float3 eye;
			float sampleRatio;
		};
		
		float3 _Emission;
		half _Glossiness;
		half _Metallic;
		half _BumpAmount;
		half _Parallax;
		fixed4 _Color;
		uint _ParallaxMinSamples;
		uint _ParallaxMaxSamples;
		
		#include<ParallaxOcclusionMap.cginc>
		
		void vert(inout appdata_full IN, out Input OUT) {
			//Setup transformations
			parallax_vert( IN.vertex, IN.normal, IN.tangent, OUT.eye, OUT.sampleRatio );
			OUT.texcoord = IN.texcoord;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			//Set texture offset coordinates
			float2 offset = parallax_offset (_Parallax, IN.eye, IN.sampleRatio, IN.texcoord, 
			_ParallaxMap, _ParallaxMinSamples, _ParallaxMaxSamples );
			//Change uv based on parallax offset
			float2 uv = IN.texcoord + offset;

			fixed4 c = tex2D (_MainTex, uv);
			c.rgb += tex2D(_EmissionMap, uv.xy) * _Emission;
			o.Albedo = c.rgb;
			o.Normal = UnpackScaleNormal(tex2D(_BumpMap, uv), _BumpAmount);
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
		}
		
		ENDCG
	}
	FallBack "Diffuse"
}
