Shader "Hidden/Custom/Distortion"
{
    HLSLINCLUDE
        
        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
		//Maintex here is basically the screen
        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        TEXTURE2D_SAMPLER2D(_GlobalDistortionTex, sampler_GlobalDistortionTex);
        float4 _MainTex_TexelSize;
        float _Magnitude;

        float4 Frag(VaryingsDefault i) : SV_Target
        {
			//
            float2 mag = _Magnitude * _MainTex_TexelSize.xy;
			//Grab unity post processing default distortion calculations (Distortion.hlsl, accessed via StdLib.hlsl) Amount of distortion is changed based on the defined magnitude
            float2 distortion = SAMPLE_TEXTURE2D(_GlobalDistortionTex, sampler_GlobalDistortionTex, i.texcoord).xy * mag;
			//Change texcoords 
            float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord + distortion);
            return color;
        }

    ENDHLSL
	//Subshader to change culling & Zwrite
    SubShader
    {
        Cull Off
        ZWrite Off
        ZTest Always

        Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment Frag

            ENDHLSL
        }
    }
}
