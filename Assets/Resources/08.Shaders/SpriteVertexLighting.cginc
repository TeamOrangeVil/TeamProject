#ifndef SPRITE_VERTEX_LIGHTING_INCLUDED
#define SPRITE_VERTEX_LIGHTING_INCLUDED
	
#include "ShaderShared.cginc"
#include "SpriteLighting.cginc"

//Turn off bump mapping on older shader models as they dont support needed number of outputs
#if defined(_NORMALMAP) && ((SHADER_TARGET < 30) || defined(SHADER_API_MOBILE))
#undef _NORMALMAP
#endif

////////////////////////////////////////
// Vertex output struct
//

struct VertexOutput
{
	float4 pos : SV_POSITION;				
	fixed4 color : COLOR;
	float3 texcoord : TEXCOORD0;
	half4 VertexLightInfo0 : TEXCOORD1; 
#if defined(_NORMALMAP) || defined(_DIFFUSE_RAMP)
	half4 VertexLightInfo1 : TEXCOORD2;
	half4 VertexLightInfo2 : TEXCOORD3;  
	half4 VertexLightInfo3 : TEXCOORD4;
#if defined(_NORMALMAP)
	half4 VertexLightInfo4 : TEXCOORD5;
	half4 normalWorld : TEXCOORD6;	
	half4 tangentWorld : TEXCOORD7;  
	half4 binormalWorld : TEXCOORD8;
#endif // _NORMALMAP
#endif // _NORMALMAP || _DIFFUSE_RAMP

};

////////////////////////////////////////
// Light calculations
//

struct VertexLightInfo
{
	half3 lightDirection;
	fixed3 attenuatedColor;
};

inline VertexLightInfo getVertexLightAttenuatedInfo(int index, float3 viewPos)
{
	VertexLightInfo lightInfo;
	
	//For directional lights _WorldSpaceLightPos0.w is set to zero
	lightInfo.lightDirection = unity_LightPosition[index].xyz - (viewPos.xyz * unity_LightPosition[index].w);
	float lengthSq = dot(lightInfo.lightDirection, lightInfo.lightDirection);
	lightInfo.lightDirection *= rsqrt(lengthSq);
	
	float attenuation = 1.0 / (1.0 + lengthSq * unity_LightAtten[index].z);
	
	//Spot light attenuation - for non-spot lights unity_LightAtten.x is set to -1 and y is set to 1
	if (-1 != unity_LightAtten[index].x || 1 != unity_LightAtten[index].y)
	{	
		float rho = max (0, dot(lightInfo.lightDirection, unity_SpotDirection[index].xyz));
		float spotAtt = (rho - unity_LightAtten[index].x) * unity_LightAtten[index].y;
		attenuation *= saturate(spotAtt);
	}
	
	lightInfo.attenuatedColor = unity_LightColor[index].rgb * attenuation;
	
	return lightInfo;
}

////////////////////////////////////////
// Normal map functions
//

#if defined(_NORMALMAP)

inline fixed3 getFinalLightColor(fixed3 lightColor, half3 normal, half3 lightDirection)
{
	fixed3 color;
	float angleDot = max(0.0, dot(normal, lightDirection));
	
#if defined(_DIFFUSE_RAMP)
	color = calculateRampedDiffuse(lightColor, angleDot);
#else
	color = lightColor * angleDot;
#endif // _DIFFUSE_RAMP

	return color;
}

inline VertexLightInfo getVertexLightAttenuatedInfoWorldSpace(int index, float3 viewPos)
{
	VertexLightInfo lightInfo = getVertexLightAttenuatedInfo(index, viewPos);
	
	//Convert light direction from view space to world space
	lightInfo.lightDirection = normalize(mul((float3x3)UNITY_MATRIX_V, lightInfo.lightDirection));
	
	return lightInfo;
}

#define PACK_VERTEX_LIGHT_0(vertexOutput, viewPos) \
	{ \
		VertexLightInfo lightInfo = getVertexLightAttenuatedInfoWorldSpace(0, viewPos); \
		output.VertexLightInfo0.x = lightInfo.lightDirection.x; \
		output.VertexLightInfo0.y = lightInfo.lightDirection.y; \
		output.VertexLightInfo0.z = lightInfo.lightDirection.z; \
		output.VertexLightInfo0.w = lightInfo.attenuatedColor.r; \
		output.VertexLightInfo1.x = lightInfo.attenuatedColor.g; \
		output.VertexLightInfo1.y = lightInfo.attenuatedColor.b; \
	}
	
#define PACK_VERTEX_LIGHT_1(vertexOutput, viewPos) \
	{ \
		VertexLightInfo lightInfo = getVertexLightAttenuatedInfoWorldSpace(1, viewPos); \
		output.VertexLightInfo1.z = lightInfo.lightDirection.x; \
		output.VertexLightInfo1.w = lightInfo.lightDirection.y; \
		output.VertexLightInfo2.x = lightInfo.lightDirection.z; \
		output.VertexLightInfo2.y = lightInfo.attenuatedColor.r; \
		output.VertexLightInfo2.z = lightInfo.attenuatedColor.g; \
		output.VertexLightInfo2.w = lightInfo.attenuatedColor.b; \
	}
	
#define PACK_VERTEX_LIGHT_2(vertexOutput, viewPos) \
	{ \
		VertexLightInfo lightInfo = getVertexLightAttenuatedInfoWorldSpace(2, viewPos); \
		output.VertexLightInfo3.x = lightInfo.lightDirection.x; \
		output.VertexLightInfo3.y = lightInfo.lightDirection.y; \
		output.VertexLightInfo3.z = lightInfo.lightDirection.z; \
		output.VertexLightInfo3.w = lightInfo.attenuatedColor.r; \
		output.VertexLightInfo4.x = lightInfo.attenuatedColor.g; \
		output.VertexLightInfo4.y = lightInfo.attenuatedColor.b; \
	}
	
#define PACK_VERTEX_LIGHT_3(vertexOutput, viewPos) \
	{ \
		VertexLightInfo lightInfo = getVertexLightAttenuatedInfoWorldSpace(3, viewPos); \
		output.VertexLightInfo4.z = lightInfo.lightDirection.x; \
		output.VertexLightInfo4.w = lightInfo.lightDirection.y; \
		output.texcoord.z = lightInfo.lightDirection.z; \
		output.normalWorld.w = lightInfo.attenuatedColor.r; \
		output.tangentWorld.w = lightInfo.attenuatedColor.g; \
		output.binormalWorld.w = lightInfo.attenuatedColor.b; \
	}
	
#define ADD_VERTEX_LIGHT_0(vertexOutput, normalDirection, diffuseReflection) \
	{ \
		half3 vertexLightDir = half3(vertexOutput.VertexLightInfo0.x, vertexOutput.VertexLightInfo0.y, vertexOutput.VertexLightInfo0.z); \
		fixed3 vertexLightColor = fixed3(vertexOutput.VertexLightInfo0.w, vertexOutput.VertexLightInfo1.x, vertexOutput.VertexLightInfo1.y); \
		diffuseReflection += getFinalLightColor(vertexLightColor, normalDirection, vertexLightDir); \
	}

#define ADD_VERTEX_LIGHT_1(vertexOutput, normalDirection, diffuseReflection) \
	{ \
		half3 vertexLightDir = half3(vertexOutput.VertexLightInfo1.z, vertexOutput.VertexLightInfo1.w, vertexOutput.VertexLightInfo2.x); \
		fixed3 vertexLightColor = fixed3(vertexOutput.VertexLightInfo2.y, vertexOutput.VertexLightInfo2.z, vertexOutput.VertexLightInfo2.w); \
		diffuseReflection += getFinalLightColor(vertexLightColor, normalDirection, vertexLightDir); \
	}

#define ADD_VERTEX_LIGHT_2(vertexOutput, normalDirection, diffuseReflection) \
	{ \
		half3 vertexLightDir = half3(vertexOutput.VertexLightInfo3.x, vertexOutput.VertexLightInfo3.y, vertexOutput.VertexLightInfo3.z); \
		fixed3 vertexLightColor = fixed3(vertexOutput.VertexLightInfo3.w, vertexOutput.VertexLightInfo4.x, vertexOutput.VertexLightInfo4.y); \
		diffuseReflection += getFinalLightColor(vertexLightColor, normalDirection, vertexLightDir); \
	}

#define ADD_VERTEX_LIGHT_3(vertexOutput, normalDirection, diffuseReflection) \
	{ \
		half3 vertexLightDir = half3(vertexOutput.VertexLightInfo4.z, vertexOutput.VertexLightInfo4.w, vertexOutput.texcoord.z); \
		fixed3 vertexLightColor = fixed3(vertexOutput.normalWorld.w, vertexOutput.tangentWorld.w, vertexOutput.binormalWorld.w); \
		diffuseReflection += getFinalLightColor(vertexLightColor, normalDirection, vertexLightDir); \
	}

#else

////////////////////////////////////////
// Non-Normal map functions
//

#if defined(_DIFFUSE_RAMP)

////////////////////////////////////////
// Ramped diffuse Functions
//

inline fixed3 getRampedFinalLightColor(fixed4 lightVertexInfo)
{
	return calculateRampedDiffuse(lightVertexInfo.rgb, lightVertexInfo.w);
}

inline fixed4 getRampedLightVertexInfo(int index, float3 viewPos, half3 viewNormal)
{
	fixed4 lightVertexInfo;
	VertexLightInfo lightInfo = getVertexLightAttenuatedInfo(index, viewPos);
	float angleDot = max(0.0, dot(viewNormal, lightInfo.lightDirection));
	
	lightVertexInfo.rgb = lightInfo.attenuatedColor;
	lightVertexInfo.w = angleDot;

	return lightVertexInfo;
}

#else

////////////////////////////////////////
// Normal diffuse Functions
//

inline fixed3 getLightDiffuse(int index, float3 viewPos, half3 viewNormal)
{
	VertexLightInfo lightInfo = getVertexLightAttenuatedInfo(index, viewPos);
	float angleDot = max(0.0, dot(viewNormal, lightInfo.lightDirection));
	return lightInfo.attenuatedColor * angleDot;
}

inline fixed4 getFinalLightDiffuse(float3 viewPos, half3 viewNormal)
{
	//Get Ambient diffuse
	fixed4 diffuseReflection = UNITY_LIGHTMODEL_AMBIENT;
	
	diffuseReflection.rgb += getLightDiffuse(0, viewPos, viewNormal);
	diffuseReflection.rgb += getLightDiffuse(1, viewPos, viewNormal);
	diffuseReflection.rgb += getLightDiffuse(2, viewPos, viewNormal);
	diffuseReflection.rgb += getLightDiffuse(3, viewPos, viewNormal);
	
	return diffuseReflection;
}

#endif // !_DIFFUSE_RAMP
	
#endif // _NORMALMAP
	
////////////////////////////////////////
// Vertex program
//

uniform fixed4 _Color;
uniform sampler2D _MainTex;
uniform fixed4 _MainTex_ST;

VertexOutput vert(VertexInput input)
{
	VertexOutput output;
	output.pos = calculateLocalPos(input.vertex);
	output.color = input.color * _Color;
	output.texcoord = float3(TRANSFORM_TEX(input.texcoord, _MainTex), 0);
	
	float3 viewPos = mul(UNITY_MATRIX_MV, input.vertex);
	float3 normal = getNormal(input);
	
#if defined(_NORMALMAP)
	
	output.normalWorld.xyz = calculateWorldNormal(normal);
	output.tangentWorld.xyz = calculateWorldTangent(input.tangent);
	output.binormalWorld.xyz = calculateWorldBinormal(output.normalWorld, output.tangentWorld, input.tangent.w);
	
	PACK_VERTEX_LIGHT_0(output, viewPos)
	PACK_VERTEX_LIGHT_1(output, viewPos)
	PACK_VERTEX_LIGHT_2(output, viewPos)
	PACK_VERTEX_LIGHT_3(output, viewPos)
	
#else // !_NORMALMAP
	
		half3 viewNormal = normalize(mul ((float3x3)UNITY_MATRIX_IT_MV, normal));
		
	#if defined(_DIFFUSE_RAMP)

		//Ramped - pack each vertex light
		output.VertexLightInfo0 = getRampedLightVertexInfo(0, viewPos, viewNormal);
		output.VertexLightInfo1 = getRampedLightVertexInfo(1, viewPos, viewNormal);
		output.VertexLightInfo2 = getRampedLightVertexInfo(2, viewPos, viewNormal);
		output.VertexLightInfo3 = getRampedLightVertexInfo(3, viewPos, viewNormal);
		
	#else // !_DIFFUSE_RAMP

		//Just pack full diffuse
		output.VertexLightInfo0 = getFinalLightDiffuse(viewPos, viewNormal);
		
	#endif // _DIFFUSE_RAMP	
	
#endif // _NORMALMAP

	return output;
}

////////////////////////////////////////
// Fragment program
//

fixed4 frag(VertexOutput input) : SV_Target
{
	fixed4 texureColor = tex2D(_MainTex, input.texcoord.xy);
	ALPHA_CLIP(texureColor.a)
	
#if defined(_NORMALMAP)
	
	half3 normalDirection = calculateNormalFromBumpMap(input.texcoord.xy, input.tangentWorld.xyz, input.binormalWorld.xyz, input.normalWorld.xyz);
	
	//Get Ambient diffuse
	fixed3 diffuseReflection = UNITY_LIGHTMODEL_AMBIENT.rgb;
	
	//Add each vertex light to diffuse
	ADD_VERTEX_LIGHT_0(input, normalDirection, diffuseReflection)
	ADD_VERTEX_LIGHT_1(input, normalDirection, diffuseReflection)
	ADD_VERTEX_LIGHT_2(input, normalDirection, diffuseReflection)
	ADD_VERTEX_LIGHT_3(input, normalDirection, diffuseReflection)
	
#else // !_NORMALMAP
	
	#if defined(_DIFFUSE_RAMP)
		
		//Get Ambient diffuse
		fixed3 diffuseReflection = UNITY_LIGHTMODEL_AMBIENT.rgb;
		
		diffuseReflection += getRampedFinalLightColor(input.VertexLightInfo0);
		diffuseReflection += getRampedFinalLightColor(input.VertexLightInfo1);
		diffuseReflection += getRampedFinalLightColor(input.VertexLightInfo2);
		diffuseReflection += getRampedFinalLightColor(input.VertexLightInfo3);
		
	#else // !_DIFFUSE_RAMP
		
		fixed3 diffuseReflection = input.VertexLightInfo0.rgb;
		
	#endif // _DIFFUSE_RAMP
	
#endif // _NORMALMAP
	
	return calculateLitPixel(texureColor, input.color, diffuseReflection);
}

#endif // SPRITE_VERTEX_LIGHTING_INCLUDED