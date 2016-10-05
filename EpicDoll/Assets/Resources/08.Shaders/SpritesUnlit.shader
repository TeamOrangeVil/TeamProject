Shader "Game/Sprite Unlit"
{
	Properties
	{
		_MainTex ("Main Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
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
		LOD 100
		
		Pass
		{
			Blend [_SrcBlend] [_DstBlend]
			Lighting Off
			ZWrite [_ZWrite]
			ZTest LEqual
			Cull Off
			Lighting Off
			
			CGPROGRAM			
				#pragma vertex vert
				#pragma fragment frag
				#pragma shader_feature _ _ALPHAPREMULTIPLY_ON _ADDITIVEBLEND _ADDITIVEBLEND_SOFT _MULTIPLYBLEND _MULTIPLYBLEND_X2
				#pragma shader_feature _ALPHA_CLIP
				#include "SpriteUnlit.cginc"
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
	
	CustomEditor "SpriteShaderGUI"
}
