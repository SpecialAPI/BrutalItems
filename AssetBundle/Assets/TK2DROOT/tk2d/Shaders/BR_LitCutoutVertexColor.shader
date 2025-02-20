Shader "tk2d/BR_LitCutoutVertexColor" 
{
	Properties 
	{
	    _MainTex ("Base (RGB)", 2D) = "white" {}
	    _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	}
	
	SubShader
	{
		Tags {"IgnoreProjector"="True" "RenderType"="TransparentCutout"}
		Cull Off Fog { Mode Off }
		LOD 110

		CGPROGRAM
		#pragma surface surf Lambert alphatest:_Cutoff
		struct Input {
			float2 uv_MainTex;
			fixed4 color : COLOR;
		};
		sampler2D _MainTex;
		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 mainColor = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
			o.Albedo = mainColor.rgb;
			o.Alpha = mainColor.a;
		}
		ENDCG		
	}

	Fallback "tk2d/BlendVertexColor", 1
}
