Shader "Custom/LavaShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo", 2D) = "white" {}

		//Normal mapping
		_BumpMap ("Normal map", 2D) = "bump" {}
		_BumpAmount ("Bump scale", Range(0,1)) = 1

		//Parallax
		_ParallaxMap ("Height map", 2D) = "white" {}
		_Parallax ("Height scale", Range(0,1)) = 0.05
		_ParallaxMinSamples ("Min samples", Range(2,100)) = 4
		_ParallaxMaxSamples ("Max samples", Range(2,10000)) = 20

		//Other effects
		_EmissionMap ("Emission map", 2D) = "black" {}
		_Emission ("Emission", Color) = (0, 0, 0)
		_Glossiness ("Glossiness/Wetness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_FlowMap ("Flow (RG)", 2D) = "black" {}
		_Tiling ("Tiling", Float) = 1
		_GridResolution ("Grid Resolution", Float) = 10
		_Speed ("Speed", Float) = 1
		_FlowStrength ("Flow Strength", Float) = 1
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows vertex:vert
		#pragma shader_feature _EMISSION_MAP
		#pragma target 3.0
		
		#include "Flow.cginc"

		sampler2D _MainTex, _BumpMap, _ParallaxMap, _EmissionMap, _FlowMap;

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
		float _Tiling, _GridResolution, _Speed, _FlowStrength;
		
		#include<ParallaxOcclusionMap.cginc>
		
		void vert(inout appdata_full IN, out Input OUT) {
			//Setup transformations
			parallax_vert( IN.vertex, IN.normal, IN.tangent, OUT.eye, OUT.sampleRatio );
			OUT.texcoord = IN.texcoord;
		}

		float2 FlowCell (float2 uv, float2 cellOffset, float time, float3 eye, out float2x2 parallaxRotation){
			float2 uvTiled = floor(uv * _GridResolution + cellOffset) / _GridResolution;
			
			//Tile based flow
			float3 flow = tex2D(_FlowMap, uvTiled).rgb;
			flow.xy = flow.xy * 2 - 1;
			flow.z *= _FlowStrength;

			//UV Directional flow based on time & rotation
			float2 UVFlow = DirectionalFlowUV(uv, flow, _Tiling, time, parallaxRotation);
			
			return UVFlow;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {//Research on surface shader
			float2x2 parallaxRotation;
			//Time based speed
			float time = _Time.y * _Speed;
			//Change uv based on parallax offset
			float2 UVFlowA = FlowCell(IN.texcoord, float2(0, 0), time, IN.eye, parallaxRotation);
			float2 UVFlowB = FlowCell(IN.texcoord, float2(1, 0), time, IN.eye, parallaxRotation);

			float t = frac(IN.texcoord.x * _GridResolution);
			float wA = 1 - t;
			float wB = t;

			float2 UVFlow = UVFlowA * wA + UVFlowB * wB;
			
			//Rotate eye xy coordinates to rotate perspective based on rotation position
			IN.eye.xy = mul(parallaxRotation, IN.eye.xy);
			//Set texture offset coordinates
			float2 offset = parallax_offset (_Parallax, IN.eye, IN.sampleRatio, UVFlow, 
			_ParallaxMap, _ParallaxMinSamples, _ParallaxMaxSamples);
			float2 uv = UVFlow + offset;

			
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
