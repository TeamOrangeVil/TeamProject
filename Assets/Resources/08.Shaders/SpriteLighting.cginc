#ifndef SPRITE_LIGHTING_INCLUDED
#define SPRITE_LIGHTING_INCLUDED

////////////////////////////////////////
// Vertex structs
//

struct VertexInput
{
	float4 vertex : POSITION;
	float4 texcoord : TEXCOORD0;
	float4 color : COLOR;
#if !defined(_FIXED_NORMALS)
	float3 normal : NORMAL;
#endif // _FIXED_NORMALS
#if defined(_NORMALMAP)
	float4 tangent : TANGENT;
#endif // _NORMALMAP
};

////////////////////////////////////////
// Space functions
//

uniform float4 _FixedNormal = float4(0, 0, -1, 1);

inline float3 getNormal(VertexInput vertex)
{
#if defined(_FIXED_NORMALS)
	return _FixedNormal.xyz;
#else
	return vertex.normal;
#endif // _FIXED_NORMALS
}

#endif // SPRITE_LIGHTING_INCLUDED