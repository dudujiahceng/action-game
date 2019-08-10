Shader "MyShader/SimpleLambert" {
	Properties{
		_MainTex("Texture", 2D) = "white" {}
		_NormalTex("Normal Map", 2D) = "bump"{}
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		CGPROGRAM
	#pragma surface surf SimpleLambert

	struct Input {
		float2 uv_NormalTex;
		float2 uv_MainTex;
	};

	sampler2D _MainTex;
	sampler2D _NormalTex;
	void surf(Input IN, inout SurfaceOutput o) {
		float3 normalMap = UnpackNormal(tex2D(_NormalTex, IN.uv_NormalTex));
		o.Normal = normalMap.rgb;
		o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
	}

	half4 LightingSimpleLambert(SurfaceOutput s, half3 lightDir, half atten) {
		half NdotL = dot(s.Normal, lightDir);
		half4 c;
		c.rgb = s.Albedo * _LightColor0.rgb * (NdotL * atten * 1);
		c.a = s.Alpha;
		return c;
	}

	ENDCG
	}
		Fallback "Diffuse"
}