Shader "MyShader/WaveEffectShader"
{
	Properties
	{
		_MainTex ("Base(RGB)", 2D) = "white" {}
	}
	SubShader{
		Tags { "RenderType"="Opaque" }
		CGINCLUDE
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"

		sampler2D _MainTex;
		half4 _MainTex_TexelSize;
		float4x4 _FrustumCornersRay;
		sampler2D _CameraDepthTexture;
		float _WaveStrength;
		float _WaveFactor;
		float _TimeScale;
		float _WaveRadius;
		float3 _CenterPos;
		float _CurWaveDistance;
		float _WaveWidth;

		struct v2f{
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
			float2 uv_depth : TEXCOORD1;
			float4 interpolatedRay : TEXCOORD2;
		};

		v2f vert(appdata_img v)
		{
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.uv = v.texcoord;
			o.uv_depth = v.texcoord;
			if(_MainTex_TexelSize.y < 0)
				o.uv_depth.y = 1 - o.uv_depth.y;
			int index = 0;
			if(v.texcoord.x < 0.5 && v.texcoord.y < 0.5)
				index = 0;
			else if(v.texcoord.x > 0.5 && v.texcoord.y < 0.5)
				index = 1;
			else if(v.texcoord.x > 0.5 && v.texcoord.y > 0.5)
				index = 2;
			else
				index = 3;
			if(_MainTex_TexelSize.y < 0)
				index = 3 - index;
			o.interpolatedRay = _FrustumCornersRay[index];
			return o;
		}
		fixed4 frag(v2f i) : SV_Target
		{
			float linearDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv_depth));
			float3 worldPos = _WorldSpaceCameraPos + linearDepth * i.interpolatedRay.xyz;
			fixed2 uv = i.uv;
			float dis = distance(worldPos, _CenterPos.xyz);
			fixed4 finalColor = tex2D(_MainTex, uv);
			if(worldPos.y < 0.1 && dis < _CurWaveDistance + _WaveWidth && dis > _CurWaveDistance)
			{
				fixed2 uvDir = normalize(fixed2(worldPos.x, worldPos.z) - fixed2(_CenterPos.x, _CenterPos.z));
				float sinFactor =  _WaveStrength * _WaveFactor * sin(_Time.y * _TimeScale + dis * _WaveFactor);
				float2 waveOffset = uvDir * sinFactor;
				uv = uv + waveOffset; 
				finalColor = tex2D(_MainTex, uv);
			}
			return finalColor;
		}
		ENDCG
		Pass{
			ZTEST Always Cull Off ZWrite Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}
	FallBack Off
}
