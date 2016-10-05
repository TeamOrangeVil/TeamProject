Shader "Spine/Bumped Diffuse with Shadow" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_BumpMap ("Normalmap", 2D) = "bump" {}
	_Cutoff("Cutoff", Float) = 0.01
}

SubShader {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="TransparentCutOut" }
	LOD 300

		CGPROGRAM
	#pragma surface surf Lambert alpha vertex:vert addshadow alphatest:_Cutoff
			#pragma multi_compile DUMMY PIXELSNAP_ON 

	
	sampler2D _MainTex;
	sampler2D _BumpMap;
	fixed4 _Color;
	
	struct Input {
		float2 uv_MainTex;
		float2 uv_BumpMap;
		fixed4 color; 
	};
	
void vert (inout appdata_full v, out Input o)
		{
			#if defined(PIXELSNAP_ON) && !defined(SHADER_API_FLASH)
			v.vertex = UnityPixelSnap (v.vertex);
			#endif
			v.normal = float3(0,0,-1);
			v.tangent =  float4(1, 0, 0, 1);
			
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.color = _Color;
		}
	void surf (Input IN, inout SurfaceOutput o) {
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
		o.Albedo = c.rgb;
		o.Alpha = c.a;
	}
	ENDCG
}

Fallback "Transparent/Cutout/Diffuse"
}