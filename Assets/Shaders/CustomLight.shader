Shader "Custom/CustomLight" {
	Properties {
		_MainTint("Diffuse Tint", Color) = (1, 1, 1, 1)
		_MainTex("Base(RGB)", 2D) = "white"{}
		_SpecularColor("Specular Color", Color) = (1, 1, 1, 1)
		_SpecPower("Specular Power", Range(0, 30)) = 1
	}
	SubShader {
		CGPROGRAM
		#pragma surface surf BlinnPhone
		float4 _MainTint;
		sampler2D _MainTex;
		float4 _SpecularColor;
		float _SpecPower;
		struct Input{
			float2 uv_MainTex;
		};
		fixed4 LightingBlinnPhone(SurfaceOutput s, fixed3 lightDir, half3 viewDir, fixed atten)
		{
			float NdotL = dot(s.Normal, lightDir);
			float3 halfVector = normalize(lightDir + viewDir);
			float NdotH = max(0, dot(s.Normal, halfVector));
			float spec = pow(NdotH, _SpecPower);
			float3 finalSpec = spec * _SpecularColor.rgb;
			float4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * max(0, NdotL) + finalSpec * _LightColor0.rgb * atten;
			c.a = s.Alpha;
			return c;
		}
		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _MainTint;
			o.Alpha = c.a;
			o.Albedo = c.rgb;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
