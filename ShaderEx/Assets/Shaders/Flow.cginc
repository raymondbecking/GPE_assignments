#if !defined(FLOW_INCLUDED)
#define FLOW_INCLUDED

float3 FlowUVW (float2 uv, float2 flowVector, float time, bool flowB, float2 jump, float tiling, float flowOffset) {
	//Give the second flow pattern an offset
	float phaseOffset = flowB ? 0.5 : 0;
	//Change UV over time
	float progress = frac(time + phaseOffset);
	float3 uvw;
	uvw.xy = uv - flowVector * (progress + flowOffset);
	//Texture tiling
	uvw.xy *= tiling;
	uvw.xy += phaseOffset;
	//Add jump for some U or V changes
	uvw.xy += (time - progress) * jump;
	uvw.z = 1 - abs(1 - 2 * progress);
	return uvw;
}

float2 DirectionalFlowUV (
	float2 uv, float2 flowVector, float tiling, float time,
	out float2x2 rotation
) {
	float2 dir = normalize(flowVector.xy);
	rotation = float2x2(dir.y, -dir.x, dir.x, dir.y);
	uv = mul(float2x2(dir.y, -dir.x, dir.x, dir.y), uv);
	uv.y -= time;
	return uv * tiling;
}
#endif