Shader "Custom/SHADER_Bagic2Side" {
	Properties{
	_MainTex("Albedo (RGB)", 2D) = "white" {}
	_MainTex2("Ramp Texture", 2D) = "white" {}
	_MainTex3("빛날 부분", 2D) = "white" {}
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200


		cull front

		//1 pass

		CGPROGRAM

#pragma surface surf Lambert vertex:vert

		sampler2D _MainTex;

	void vert(inout appdata_full v) {

		v.vertex.xyz = v.vertex.xyz + v.normal.xyz*0.02;


	}

	struct Input {
		float2 uv_MainTex;
	};


	fixed4 _Color;

	void surf(Input IN, inout SurfaceOutput o) {

		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		o.Emission = c.rgb;
		o.Alpha = c.a;
	}
	ENDCG

		//2 pass

		cull off

		CGPROGRAM
#pragma surface surf mk noambient

#pragma target 3.0

		sampler2D _MainTex;
	sampler2D _MainTex2;
	sampler2D _MainTex3;


	struct Input {
		float2 uv_MainTex;
		float2 uv_MainTex3;
	};

	void surf(Input IN, inout SurfaceOutput o) {

		fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
		fixed4 d = tex2D(_MainTex3, IN.uv_MainTex3);



		o.Albedo = c.rgb*1.2;
		o.Alpha = c.a;
		o.Emission = d.rgb*0.3;
	}

	float4 Lightingmk(SurfaceOutput s , float3 lightDir, float3 viewDir , float atten) {

		float4 tt;
		float NdotL = dot(s.Normal, lightDir)*0.5 + 0.5;



		float4 ramp = tex2D(_MainTex2, float2(NdotL,0.5));



		//최종 출력

		tt.rgb = ramp.rgb*s.Albedo*atten*_LightColor0*NdotL;
		tt.a = s.Alpha;




		return tt;

	}
	ENDCG

	}
		FallBack "Diffuse"
}
