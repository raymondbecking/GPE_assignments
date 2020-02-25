#pragma once

/* 
This cginc file calculates what point of the heightmap should be seen by the camera
The texture is offset based on the camera position to show the correct vertex

*/

void parallax_vert(
	float4 vertex,
	float3 normal,
	float4 tangent,
	out float3 eye,
	out float sampleRatio
) {
	float4x4 mW = unity_ObjectToWorld;
	float3 binormal = cross( normal, tangent.xyz ) * tangent.w;
	
	// Calculate vector from eye to vertex, not taking light into consideration
	float4 localCameraPos = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1));
	float3 eyeLocal = vertex - localCameraPos;
	float4 eyeGlobal = mul( float4(eyeLocal, 1), mW  );
	float3 E = eyeGlobal.xyz;
	
	//Transform vectors to tangent space
	float3x3 tangentToWorldSpace;
	tangentToWorldSpace[0] = mul( normalize( tangent ), mW );
	tangentToWorldSpace[1] = mul( normalize( binormal ), mW );
	tangentToWorldSpace[2] = mul( normalize( normal ), mW );
	
	//World to tangent spoace matrix
	float3x3 worldToTangentSpace = transpose(tangentToWorldSpace);
	
	//Transform eye vector to tangent space
	eye	= mul( E, worldToTangentSpace );
	sampleRatio = 1-dot( normalize(E), -normal );
}

float2 parallax_offset (
	float heightMapScale,
	float3 eye,
	float sampleRatio,
	float2 texcoord,
	sampler2D heightMap,
	int nMinSamples,
	int nMaxSamples
) {
	
	//Parallaxlimit Defines the max allowed length of the parallax offset
	float parallaxLimit = -length( eye.xy ) / eye.z;//Orientation of eye vector to surface
	parallaxLimit *= heightMapScale;//Determined depth of surface
	
	//Calculate direction of the offset
	float2 offsetDir = normalize( eye.xy );
	//Direction scaled by max parallax offset
	float2 maxOffset = offsetDir * parallaxLimit;
	
	int nNumSamples = (int)lerp( nMinSamples, nMaxSamples, saturate(sampleRatio) );
	float fStepSize = 1.0 / (float)nNumSamples;
	
	//Implement SampleGrad instructions for texture sampling
	float2 dx = ddx( texcoord );
	float2 dy = ddy( texcoord );
	

	float fCurrRayHeight = 1.0;
	float2 vCurrOffset = float2( 0, 0 );
	float2 vLastOffset = float2( 0, 0 );

	//
	float fLastSampledHeight = 1;
	float fCurrSampledHeight = 1;

	//Start at sample 0
	int nCurrSample = 0;
	
	while ( nCurrSample < nNumSamples )
	{
	  fCurrSampledHeight = tex2Dgrad(heightMap, texcoord + vCurrOffset, dx, dy ).r;
	  if ( fCurrSampledHeight > fCurrRayHeight )
	  {
		float delta1 = fCurrSampledHeight - fCurrRayHeight;
		float delta2 = ( fCurrRayHeight + fStepSize ) - fLastSampledHeight;

		float ratio = delta1/(delta1+delta2);

		vCurrOffset = (ratio) * vLastOffset + (1.0-ratio) * vCurrOffset;

		nCurrSample = nNumSamples + 1;
	  }
	  else
	  {
		nCurrSample++;

		fCurrRayHeight -= fStepSize;

		vLastOffset = vCurrOffset;
		vCurrOffset += fStepSize * maxOffset;

		fLastSampledHeight = fCurrSampledHeight;
	  }
	}
	
	return vCurrOffset;
}