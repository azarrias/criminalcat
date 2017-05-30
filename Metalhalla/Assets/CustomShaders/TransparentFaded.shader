Shader "CustomShaders/TransparentFaded"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
	_Color("Color", Color) = (1,1,1,1)
	}

		SubShader
	{
		Pass
	{
		ZWrite On
		ColorMask 0
	}

		Pass
	{
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

		struct appdata
	{
		float4 vertex : POSITION;
		float2 texcoord : TEXCOORD0;
	};

	struct v2f
	{
		float4 vertex : SV_POSITION;
		float2 texcoord : TEXCOORD0;
	};

	uniform sampler2D _MainTex;
	uniform float4 _MainTex_ST;
	uniform fixed4 _Color;

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
		o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		half4 texcolor = tex2D(_MainTex, i.texcoord);
		texcolor *= _Color;
		return texcolor;
	}
		ENDCG
	}
	}
}
