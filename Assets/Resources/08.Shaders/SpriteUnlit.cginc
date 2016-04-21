#ifndef SPRITE_UNLIT_INCLUDED
#define SPRITE_UNLIT_INCLUDED

#include "ShaderShared.cginc"

////////////////////////////////////////
// Vertex structs
//
				
struct vertexInput
{
	float4 vertex : POSITION;
	float4 texcoord : TEXCOORD0;
	fixed4 color : COLOR;
};

struct vertexOutput
{
	float4 pos : SV_POSITION;
	float2 texcoord : TEXCOORD0;
	fixed4 color : COLOR;
};

////////////////////////////////////////
// Vertex program
//

uniform fixed4 _Color;
uniform sampler2D _MainTex;
uniform fixed4 _MainTex_ST;

vertexOutput vert(vertexInput IN)
{
	vertexOutput OUT;
	
	OUT.pos = calculateLocalPos(IN.vertex);	
	OUT.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);
	OUT.color = IN.color * _Color;
	
	return OUT;
}

////////////////////////////////////////
// Fragment program
//

fixed4 frag(vertexOutput IN) : SV_Target
{
	fixed4 texureColor = tex2D(_MainTex, IN.texcoord.xy);
	ALPHA_CLIP(texureColor.a)

	return calculatePixel(texureColor, IN.color);
}

#endif // SPRITE_UNLIT_INCLUDED