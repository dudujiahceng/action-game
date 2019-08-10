// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MyShader/EmissionShader"
{
	Properties
	{
		_MainTex ("Texture", 2D)							 = "white" {}
		//Weapon emission light color
		_EmissionColor("Emission Color", Color)				 = (1, 0, 0, 1)
		_EmissionSize("Emission Size", Float)				 = 0.1
		_EmissionStrength("Emission Strength", Float)		 = 10
		_EmissionPower("Emission Power", Float)				 = 5	
	}
	SubShader
	{
		Pass{
			Tags{"LightMode" = "Always"}
			Cull Back
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			half4 _EmissionColor;
			float _EmissionSize;
			float _EmissionStrength;
			float _EmissionPower;
			struct v2f{
				float4 pos : SV_POSITION;
				float3 normal : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				float2 texcoord : TEXCOORD2;
			};	
			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.normal = v.normal;
				o.worldPos = UnityObjectToClipPos(v.vertex).xyz;
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}

			half4 frag(v2f i) : COLOR
			{
				float4 color = tex2D(_MainTex, i.texcoord);
				return color;
			}
			
			ENDCG
		}
		Pass{
			Tags{"LightMode" = "Always"}
			Cull Back
			Blend SrcAlpha One
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			half4 _EmissionColor;
			float _EmissionSize;
			float _EmissionStrength;
			float _EmissionPower;

			struct v2f{
				float4 pos : SV_POSITION;
				float3 normal : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				v.vertex.xyz += v.normal * _EmissionSize;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.normal = v.normal;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				return o;
			}

			half4 frag(v2f i) : COLOR
			{
				fixed3 normal = normalize(i.normal);
				fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
				half4 color = _EmissionColor;
				color.a = pow(saturate(dot(viewDir, normal)), _EmissionPower);
				color.a *= _EmissionStrength * dot(viewDir, i.normal);
				return color;
			}
			ENDCG
		}
	}
}
