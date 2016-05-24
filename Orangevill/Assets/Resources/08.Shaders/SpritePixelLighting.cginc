#ifndef SPRITE_PIXEL_LIGHTING_INCLUDED
#define SPRITE_PIXEL_LIGHTING_INCLUDED
	
#include "ShaderShared.cginc"
#include "SpriteLighting.cginc"
#include "AutoLight.cginc"

////////////////////////////////////////
// Vertex output struct
//

struct VertexOutput
{
	float4 pos : SV_POSITION;				
	fixed4 color : COLOR;
	float2 texcoord : TEXCOORD0;
	float4 posWorld : TEXCOORD1;
	half3 normalWorld : TEXCOORD2;
	LIGHTING_COORDS(3,4)
#if defined(_NORMALMAP)
	half3 tangentWorld : TEXCOORD5;  
	half3 binormalWorld : TEXCOORD6;
#endif // _NORMALMAP
};

////////////////////////////////////////
// Light calculations
//

uniform fixed4 _LightColor0;

inline fixed3 calculateLightDiffuse(VertexOutput input)
{
	half3 viewDirection = normalize(_WorldSpaceCameraPos - input.posWorld.xyz);
	half3 lightDirection;
	float attenuation;
	
#if defined(_NORMALMAP)
	half3 normalDirection = calculateNormalFromBumpMap(input.texcoord, input.tangentWorld, input.binormalWorld, input.normalWorld);
#else
	half3 normalDirection = input.normalWorld;
#endif

	//For directional lights _WorldSpaceLightPos0.w is set to zero
	if (0.0 == _WorldSpaceLightPos0.w)
	{
		attenuation = 1.0; // no attenuation
		lightDirection = _WorldSpaceLightPos0.xyz;
	} 
	// point or spot light
	else 
	{
		float3 vertexToLightSource = _WorldSpaceLightPos0.xyz - input.posWorld.xyz;
		lightDirection = normalize(vertexToLightSource);
		attenuation = LIGHT_ATTENUATION(input);
	}
	
	float angleDot = max(0.0, dot(normalDirection, lightDirection));
	fixed3 color;
	
#if defined(_DIFFUSE_RAMP)
	color = calculateRampedDiffuse(_LightColor0.rgb * attenuation, angleDot);
#else
	color = _LightColor0.rgb * (attenuation * angleDot);
#endif // _DIFFUSE_RAMP
	
	return color;
}

////////////////////////////////////////
// Vertex program
//

uniform fixed4 _Color;
uniform sampler2D _MainTex;
uniform fixed4 _MainTex_ST;

VertexOutput vert(VertexInput v)
{
	VertexOutput output;
	
	output.pos = calculateLocalPos(v.vertex);
	output.color = v.color * _Color;
	output.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
	output.posWorld = calculateWorldPos(v.vertex);
	output.normalWorld = calculateWorldNormal(getNormal(v));
	
#if defined(_NORMALMAP)
	output.tangentWorld = calculateWorldTangent(v.tangent);
	output.binormalWorld = calculateWorldBinormal(output.normalWorld, output.tangentWorld, v.tangent.w);
#endif
	
	TRANSFER_VERTEX_TO_FRAGMENT(output)
	
	return output;
}

////////////////////////////////////////
// Fragment programs
//

fixed4 fragBase(VertexOutput input) : SV_Target
{
	fixed4 texureColor = tex2D(_MainTex, input.texcoord);	
	ALPHA_CLIP(texureColor.a)

	//Get Ambient diffuse
	fixed3 diffuseReflection = UNITY_LIGHTMODEL_AMBIENT.rgb;
	
	//Add main light diffuse
	diffuseReflection += calculateLightDiffuse(input);
	
	return calculateLitPixel(texureColor, input.color, diffuseReflection);
}

fixed4 fragAdd(VertexOutput input) : SV_Target
{
	fixed4 texureColor = tex2D(_MainTex, input.texcoord);	
	ALPHA_CLIP(texureColor.a)
	
	//Get main light diffuse
	fixed3 diffuseReflection = calculateLightDiffuse(input);
	
	return calculateLitPixel(texureColor, input.color, diffuseReflection);
}

#endif // SPRITE_PIXEL_LIGHTING_INCLUDED