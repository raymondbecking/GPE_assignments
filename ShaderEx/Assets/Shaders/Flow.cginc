#if !defined(FLOW_INCLUDED)
#define FLOW_INCLUDED

float2 FlowUV (float2 uv, float2 flowVector, float time) {
	float progress = frac(time);
	return uv - flowVector * progress;
}

float2 DirectionalFlowUV (
	float2 uv, float3 flowVectorAndSpeed, float tiling, float time,
	out float2x2 rotation
) {
	float2 dir = normalize(flowVectorAndSpeed.xy);
	//Rotation matrix for adjusting parallax eye direction
	rotation = float2x2(dir.y, -dir.x, dir.x, dir.y);
	uv = mul(float2x2(dir.y, -dir.x, dir.x, dir.y), uv);
	uv.y -= time * flowVectorAndSpeed.z;
	return uv * tiling;
}
#endif