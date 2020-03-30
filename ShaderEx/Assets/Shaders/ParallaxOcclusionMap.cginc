#pragma once

/* 
This cginc file calculates what point of the heightmap should be seen by the camera
The texture is offset based on the camera position to show the correct vertex point on the heightmap
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
	
	// Calculate vector from eye to vertex (without taking light into consideration)
	float4 localCameraPos = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1));
	float3 eyeLocal = vertex - localCameraPos;
	float4 eyeGlobal = mul( float4(eyeLocal, 1), mW  );
	float3 E = eyeGlobal.xyz;
	
	//Transform vectors to tangent space
	float3x3 tangentToWorldSpace;
	tangentToWorldSpace[0] = mul( normalize( tangent ), mW );
	tangentToWorldSpace[1] = mul( normalize( binormal ), mW );
	tangentToWorldSpace[2] = mul( normalize( normal ), mW );
	
	//World to tangent space matrix
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
	int minSamples,
	int maxSamples
) {
	
	//Parallaxlimit Defines the max allowed length of the parallax offset
	float parallaxLimit = -length( eye.xy ) / eye.z;//Orientation of eye vector to surface
	parallaxLimit *= heightMapScale;//Determined depth of surface
	
	//Calculate direction of the offset
	float2 offsetDir = normalize( eye.xy );
	//Direction scaled by max parallax offset
	float2 maxOffset = offsetDir * parallaxLimit;
	

	int sampleAmount = (int)lerp( minSamples, maxSamples, saturate(sampleRatio) );
	float stepSize = 1.0 / (float)sampleAmount;
	
	//Implement SampleGrad instructions for texture sampling
	float2 dx = ddx( texcoord );
	float2 dy = ddy( texcoord );
	
	//Set default rayheight & offset
	float currRayHeight = 1.0;
	float2 currentOffset = float2( 0, 0 );
	float2 lastOffset = float2( 0, 0 );

	//Set default sample height
	float lastSampledHeight = 1;
	float currSampledHeight = 1;

	//Start at sample 0
	int currSample = 0;
	
	//Find the intersection of the eye vector with the heightmap
	while ( currSample < sampleAmount )
	{
	  currSampledHeight = tex2Dgrad(heightMap, texcoord + currentOffset, dx, dy ).r;
	  //Compare eye vector with heightmap
	  if ( currSampledHeight > currRayHeight )//Intersection found, set offset & break loop
	  {
		float delta1 = currSampledHeight - currRayHeight;
		float delta2 = ( currRayHeight + stepSize ) - lastSampledHeight;

		float ratio = delta1/(delta1+delta2);

		//Intersection between current & last sample
		currentOffset = (ratio) * lastOffset + (1.0-ratio) * currentOffset;

		//Break out the loop
		currSample = sampleAmount + 1;
	  }
	  else//No Intersection found, keep looping
	  {
		currSample++;

		currRayHeight -= stepSize;

		//Set current & last offset for the next loop
		lastOffset = currentOffset;
		currentOffset += stepSize * maxOffset;

		//Set last sampled height for next loop
		lastSampledHeight = currSampledHeight;
	  }
	}
	//Return offset when intersection is found
	return currentOffset;
}