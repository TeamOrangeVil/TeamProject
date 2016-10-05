Shader "Game/Sprite Vertex Lit"
{
	Properties
	{
		_MainTex ("Main Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_DiffuseRamp ("Diffuse Ramp Texture", 2D) = "gray" {}
		_FixedNormal ("Fixed Normal", Vector) = (0,0,-1,1)
		_ZWrite ("Depth Write", Float) = 0.0
		_DepthAlphaCutoff ("Depth alpha cutoff", Range(0,1)) = 0.0
		_ShadowAlphaCutoff ("Shadow alpha cutoff", Range(0,1)) = 0.1
		
		[HideInInspector] _BlendMode ("__mode", Float) = 0.0
		[HideInInspector] _SrcBlend ("__src", Float) = 1.0
		[HideInInspector] _DstBlend ("__dst", Float) = 0.0
		[HideInInspector] _RenderQueue ("__queue", Float) = 0.0
	}
	
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 150
		
		Pass
		{
			Name "Vertex" 
			Tags { "LightMode" = "Vertex" }
			Blend [_SrcBlend] [_DstBlend]
			ZWrite [_ZWrite]
			ZTest LEqual
			Cull Off
			Lighting On
			
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma shader_feature _ _ALPHAPREMULTIPLY_ON _ADDITIVEBLEND _ADDITIVEBLEND_SOFT _MULTIPLYBLEND _MULTIPLYBLEND_X2
				#pragma shader_feature _NORMALMAP
				#pragma shader_feature _FIXED_NORMALS
				#pragma shader_feature _ALPHA_CLIP
				#pragma shader_feature _DIFFUSE_RAMP
				#pragma target 3.0
				#include "SpriteVertexLighting.cginc"	
			ENDCG
		}
		Pass
		{
			Name "ShadowCaster"
			Tags { "LightMode"="ShadowCaster" }
			Offset 1, 1
			
			Fog { Mode Off }
			ZWrite On
			ZTest LEqual
			Cull Off
			Lighting Off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "SpriteShadows.cginc"
			ENDCG
		}
	}
	
	FallBack "Game/Sprite Unlit"
	CustomEditor "SpriteShaderGUI"
}
