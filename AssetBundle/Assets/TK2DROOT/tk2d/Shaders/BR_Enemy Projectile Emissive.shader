// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// unlit, vertex colour, alpha blended
// cull off

Shader "Brave/BR_Enemy Projectile Emissive" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" { }
		_Perpendicular ("Is Perpendicular Tilt", Float) = 1
		_Cutoff ("Alpha cutoff", Range(0, 1)) = 0.5
		_EmissivePower ("Emissive Power", Float) = 0
		_EmissiveColorPower ("Emissive Color Power", Float) = 7
		_EmissiveColor ("Emissive Color", Color) = (1,1,1,1)
		_BlackBullet ("Black Bulletness", Float) = 0
	}

	SubShader
	{
		Tags {"IgnoreProjector"="True" "RenderType"="TransparentCutout"}
		Lighting Off Cull Off Fog { Mode Off } AlphaTest Greater 0
		LOD 110
		
		Pass 
		{
			CGPROGRAM
			#pragma vertex vert_vct
			#pragma fragment frag_mult
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;

			struct vin_vct 
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f_vct
			{
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			v2f_vct vert_vct(vin_vct v)
			{
				v2f_vct o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.texcoord = v.texcoord;
				return o;
			}

			fixed4 frag_mult(v2f_vct i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.texcoord) * i.color;
				return col;
			}
						
			ENDCG
		} 
	}
}
