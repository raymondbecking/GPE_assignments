// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/ParallaxOcclusionDepthIntersection" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo", 2D) = "white" {}
		_BumpMap ("Normal map", 2D) = "bump" {}
		_BumpAmount ("Bump scale", Range(0,1)) = 1
		_ParallaxMap ("Height map", 2D) = "white" {}
		_Parallax ("Height scale", Range(0,1)) = 0.05
		_Glossiness ("Glossiness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_ParallaxMinSamples ("Min samples", Range(2,100)) = 4
		_ParallaxMaxSamples ("Max samples", Range(2,100)) = 20

		_RegularColor("Main Color", Color) = (1, 1, 1, .5) //Color when not intersecting
        _HighlightColor("Highlight Color", Color) = (1, 1, 1, .5) //Color when intersecting
        _HighlightThresholdMax("Highlight Threshold Max", Float) = 1 //Max difference for intersections
	}
	SubShader {
		Tags { "Queue" = "Transparent" "RenderType"="Transparent"  }
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows vertex:vert
		#pragma target 5.0
		
		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _ParallaxMap;

		uniform sampler2D _CameraDepthTexture; //Depth Texture
            uniform float4 _RegularColor;
            uniform float4 _HighlightColor;
            uniform float _HighlightThresholdMax;

		struct Input {
			float2 texcoord;
			float3 eye;
			float sampleRatio;
			float4 pos;
			float4 projPos;
		};

		half _Glossiness;
		half _Metallic;
		half _BumpAmount;
		half _Parallax;
		fixed4 _Color;
		uint _ParallaxMinSamples;
		uint _ParallaxMaxSamples;
		
		#include<ParallaxOcclusionMap.cginc>
		
		void vert(inout appdata_full v, out Input o) {
			//Setup transformations
			UNITY_INITIALIZE_OUTPUT(Input, o);
			parallax_vert( v.vertex, v.normal, v.tangent, o.eye, o.sampleRatio );
			o.texcoord = v.texcoord;
			o.pos = UnityObjectToClipPos(v.vertex);
            o.projPos = ComputeScreenPos(o.pos);
		}



		void surf (Input i, inout SurfaceOutputStandard o) {
			//Set texture offset coordinates
			float2 offset = parallax_offset (_Parallax, i.eye, i.sampleRatio, i.texcoord, 
			_ParallaxMap, _ParallaxMinSamples, _ParallaxMaxSamples );
			//Change uv based on parallax offset
			float2 uv = i.texcoord + offset;

			float4 finalColor = _RegularColor;
 
                //Get the distance to the camera from the depth buffer for this point
                float sceneZ = LinearEyeDepth (tex2Dproj(_CameraDepthTexture,
                                                         UNITY_PROJ_COORD(i.projPos)).r);
 
                //Actual distance to the camera
                float partZ = i.projPos.z;
 
                //If the two are similar, then there is an object intersecting with our object
                float diff = (abs(sceneZ - partZ)) /
                    _HighlightThresholdMax;
 
                if(diff <= 1 && diff > 0.5)
                {
                    finalColor = lerp(_HighlightColor,
                                      _RegularColor,
                                      float4(diff, diff, diff, diff));
                }
 
                
 


			fixed4 c = tex2D (_MainTex, uv) * _Color;

			c.r = finalColor.r;
            c.g = finalColor.g;
            c.b = finalColor.b;
            c.a = finalColor.a;

			o.Albedo = c.rgb;
			o.Normal = UnpackScaleNormal(tex2D(_BumpMap, uv), _BumpAmount);
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
		}
		
		ENDCG
	}
	FallBack "Diffuse"
}
