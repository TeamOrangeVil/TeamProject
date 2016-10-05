#ifndef SHADER_SHARED_INCLUDED
#define SHADER_SHARED_INCLUDED

#include "UnityCG.cginc"

////////////////////////////////////////
// Space functions
//

inline float4 calculateWorldPos(float4 vertex)
{
	return mul(_Object2World, vertex);
}

inline float4 calculateLocalPos(float4 vertex)
{
	return mul(UNITY_MATRIX_MVP, vertex);
}

inline half3 calculateWorldNormal(float3 normal)
{
	return UnityObjectToWorldNormal(normal);
}

////////////////////////////////////////
// Normal map functions
//

#if defined(_NORMALMAP)

uniform sampler2D _BumpMap;

inline half3 calculateWorldTangent(float4 tangent)
{
	return UnityObjectToWorldDir(tangent);
}

inline half3 calculateWorldBinormal(half3 normalWorld, half3 tangentWorld, float tangentW)
{
	// For odd-negative scale transforms we need to flip the sign
	float sign = tangentW * unity_WorldTransformParams.w;
	return cross(normalWorld, tangentWorld.xyz) * sign;
}

inline half3 calculateNormalFromBumpMap(float2 texUV, half3 tangentWorld, half3 binormalWorld, half3 normalWorld)
{
	half3 localNormal = UnpackNormal(tex2D(_BumpMap, texUV));
	half3x3 rotation = half3x3(tangentWorld, binormalWorld, normalWorld);
	half3 normal = normalize(mul(localNormal, rotation));
	return normal;
}

#endif // _NORMALMAP

#if defined(_DIFFUSE_RAMP)

////////////////////////////////////////
// Diffuse ramp functions
//

uniform sampler2D _DiffuseRamp;

inline fixed3 calculateRampedDiffuse(fixed3 lightColor, float angleDot)
{
	float rampLookUp = (angleDot * 0.5) + 0.5;
	fixed4 rampColor = tex2D(_DiffuseRamp, float2(rampLookUp, rampLookUp));
	return lightColor * rampColor.rgb;
}
#endif // _DIFFUSE_RAMP
	
////////////////////////////////////////
// Blending functions
//

inline fixed4 calculateLitPixel(fixed4 texureColor, fixed4 color, fixed3 diffuseReflection) : SV_Target
{
	fixed4 finalPixel;
	
#if defined(_ALPHAPREMULTIPLY_ON)
	finalPixel = texureColor * color;
	finalPixel.rgb *= diffuseReflection * color.a;
#elif defined(_MULTIPLYBLEND)
	//Multiply
	finalPixel = color * texureColor;
	finalPixel.rgb *= diffuseReflection;
	finalPixel = lerp(fixed4(1,1,1,1), finalPixel, finalPixel.a);
#elif defined(_MULTIPLYBLEND_X2)
	//Multiply x2
	finalPixel.rgb = texureColor.rgb * color.rgb * diffuseReflection * 2.0f;
	finalPixel.a = color.a * texureColor.a;
	finalPixel = lerp(fixed4(0.5f,0.5f,0.5f,0.5f), finalPixel, finalPixel.a);
#elif defined(_ADDITIVEBLEND)
	//Additive
	finalPixel = texureColor * 2.0f * color;
	finalPixel.rgb *= diffuseReflection * color.a;
#elif defined(_ADDITIVEBLEND_SOFT)
	//Additive soft
	finalPixel = texureColor* color;
	finalPixel.rgb *= diffuseReflection * finalPixel.a;
#else
	finalPixel.a = texureColor.a * color.a;
	finalPixel.rgb = (texureColor.rgb * diffuseReflection * color.rbg) * finalPixel.a;
#endif
	
	return finalPixel;
}

inline fixed4 calculateLitPixel(fixed4 texureColor, fixed3 diffuseReflection) : SV_Target
{
	fixed4 finalPixel;
	
#if defined(_ALPHAPREMULTIPLY_ON)
	finalPixel = texureColor;
	finalPixel.rgb *= diffuseReflection;
#elif defined(_MULTIPLYBLEND)
	//Multiply
	finalPixel = texureColor;
	finalPixel.rgb *= diffuseReflection;
	finalPixel = lerp(fixed4(1,1,1,1), finalPixel, finalPixel.a);
#elif defined(_MULTIPLYBLEND_X2)
	//Multiply x2
	finalPixel.rgb = texureColor.rgb * diffuseReflection * 2.0f;
	finalPixel.a = texureColor.a;
	finalPixel = lerp(fixed4(0.5f,0.5f,0.5f,0.5f), finalPixel, finalPixel.a);
#elif defined(_ADDITIVEBLEND)
	//Additive
	finalPixel = texureColor * 2.0f;
	finalPixel.rgb *= diffuseReflection;
#elif defined(_ADDITIVEBLEND_SOFT)
	//Additive soft
	finalPixel = texureColor;
	finalPixel.rgb *= diffuseReflection * finalPixel.a;
#else
	finalPixel.a = texureColor.a;
	finalPixel.rgb = (texureColor.rgb * diffuseReflection) * finalPixel.a;
#endif
	
	return finalPixel;
}

inline fixed4 calculatePixel(fixed4 texureColor, fixed4 color) : SV_Target
{
	fixed4 finalPixel;
	
#if defined(_ALPHAPREMULTIPLY_ON)
	//Pre multiplied alpha
	finalPixel = texureColor * color;
	finalPixel.rgb *= color.a;
#elif defined(_MULTIPLYBLEND)
	//Multiply
	finalPixel = color * texureColor;
	finalPixel = lerp(fixed4(1,1,1,1), finalPixel, finalPixel.a);
#elif defined(_MULTIPLYBLEND_X2)
	//Multiply x2
	finalPixel.rgb = texureColor.rgb * color.rgb * 2.0f;
	finalPixel.a = color.a * texureColor.a;
	finalPixel = lerp(fixed4(0.5f,0.5f,0.5f,0.5f), finalPixel, finalPixel.a);
#elif defined(_ADDITIVEBLEND)
	//Additive
	finalPixel = texureColor * 2.0f * color;
#elif defined(_ADDITIVEBLEND_SOFT)
	//Additive soft
	finalPixel = color * texureColor;
	finalPixel.rgb *= finalPixel.a;
#else
	//Standard alpha
	finalPixel.a = texureColor.a * color.a;
	finalPixel.rgb = (texureColor.rgb * color.rbg) * finalPixel.a;
#endif 

	return finalPixel;
}

inline fixed4 calculatePixel(fixed4 texureColor) : SV_Target
{
	fixed4 finalPixel;
	
#if defined(_ALPHAPREMULTIPLY_ON)
	//Pre multiplied alpha
	finalPixel = texureColor;
#elif defined(_MULTIPLYBLEND)
	//Multiply
	finalPixel = texureColor;
	finalPixel = lerp(fixed4(1,1,1,1), finalPixel, finalPixel.a);
#elif defined(_MULTIPLYBLEND_X2)
	//Multiply x2
	finalPixel.rgb = texureColor.rgb * 2.0f;
	finalPixel.a = texureColor.a;
	finalPixel = lerp(fixed4(0.5f,0.5f,0.5f,0.5f), finalPixel, finalPixel.a);
#elif defined(_ADDITIVEBLEND)
	//Additive
	finalPixel = texureColor * 2.0f;
#elif defined(_ADDITIVEBLEND_SOFT)
	//Additive soft
	finalPixel = texureColor;
	finalPixel.rgb *= finalPixel.a;
#else
	//Standard alpha
	finalPixel.a = texureColor.a;
	finalPixel.rgb = texureColor.rgb * finalPixel.a;
#endif 

	return finalPixel;
}

////////////////////////////////////////
// Alpha Clipping
//

uniform fixed _DepthAlphaCutoff;

#if defined(_ALPHA_CLIP)
#define ALPHA_CLIP(alpha) clip(alpha - _DepthAlphaCutoff);
#else
#define ALPHA_CLIP(alpha)
#endif

#endif // SHADER_SHARED_INCLUDED