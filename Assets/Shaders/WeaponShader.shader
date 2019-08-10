// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MyShader/WeaponShader"
{
	Properties
	{
		_MainTex ("Texture", 2D)							 = "white" {}
		_NormalMap("Normal Map", 2D)						 = "bump"{}
		//Weapon dissolve
		_BurnMap("Burn Map", 2D)							 = "white"{}
		_BurnAmount("Burn Amount", Range(0.0, 1.0))			 = 1.0
		_LineWidth("Burn Line Width", Range(0.0, 0.2))		 = 0.1
		_BurnFirstColor("Burn First Color", Color)			 = (1, 0, 0, 1)
		_BurnSecondColor("Burn Second Color", Color)		 = (1, 0, 0, 1)
		//Weapon emission light color
		_EmissionColor("Emission Color", Color)				 = (1, 0, 0, 1)
		_EmissionSize("Emission Size", Float)				 = 0.1
		_EmissionStrength("Emission Strength", Float)		 = 10
		_EmissionPower("Emission Power", Float)				 = 5	
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "Queue"="Geometry"}

		Pass
		{
			Tags{"LightMode" = "ForwardBase"}
			Name "WeaponBase"
			Cull Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase			
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _NormalMap;
			float4 _NormalMap_ST;
			sampler2D _BurnMap;
			float4 _BurnMap_ST;
			float _BurnAmount;
			float _LineWidth;
			half4 _BurnFirstColor;
			half4 _BurnSecondColor;

			half4 _EmissionColor;
			float _EmissionSize;
			float _EmissionStrength;
			float _EmissionPower;

			struct a2v
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uvMainTex : TEXCOORD0;
				float2 uvNormalMap : TEXCOORD1;
				float2 uvBurnMap : TEXCOORD2;
				float3 lightDir : TEXCOORD3;
				float3 worldPos : TEXCOORD4;
				SHADOW_COORDS(5)
			};

			v2f vert(a2v v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);			
				o.uvMainTex = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uvNormalMap = TRANSFORM_TEX(v.texcoord, _NormalMap);
				o.uvBurnMap = TRANSFORM_TEX(v.texcoord, _BurnMap);			
				TANGENT_SPACE_ROTATION;
  				o.lightDir = mul(rotation, ObjSpaceLightDir(v.vertex)).xyz; 			
  				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;			
  				TRANSFER_SHADOW(o);	
				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target
			{
				fixed3 burn = tex2D(_BurnMap, i.uvBurnMap).rgb;
				clip(burn.r - _BurnAmount);
				float3 tangentLightDir = normalize(i.lightDir);
				fixed3 tangentNormal = UnpackNormal(tex2D(_NormalMap, i.uvNormalMap));
				fixed3 albedo = tex2D(_MainTex, i.uvMainTex).rgb;
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
				fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(tangentNormal, tangentLightDir));
				fixed t = 1 - smoothstep(0.0, _LineWidth, burn.r - _BurnAmount);
				fixed3 burnColor = lerp(_BurnFirstColor, _BurnSecondColor, t);
				burnColor = pow(burnColor, 5);
				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
				fixed3 finalColor = lerp(ambient + diffuse * atten, burnColor, t * step(0.0001, _BurnAmount));
				return fixed4(finalColor, 1);
			}
			ENDCG
		}
		Pass{
			Tags{"LightMode" = "ShadowCaster"}
			Name "Shadow"
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#pragma multi_complie_shadowcaster

			fixed _BurnAmount;
			sampler2D _BurnMap;
			float4 _BurnMap_ST;
			struct v2f{
				V2F_SHADOW_CASTER;
				float2 uvBurnMap : TEXCOORD1;
			};
			v2f vert(appdata_base v)
			{
				v2f o;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
				o.uvBurnMap = TRANSFORM_TEX(v.texcoord, _BurnMap);
				return o;
			}
			fixed4 frag(v2f i) : SV_Target
			{
				fixed3 burn = tex2D(_BurnMap, i.uvBurnMap).rgb;
				clip(burn.r - _BurnAmount);
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}
		Pass{
			Tags{"LightMode" = "Always"}
			Name "EmissionBase"
			Cull Back
			Blend SrcAlpha One
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			int  _OpenEmission;
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
				//v.vertex.xyz += v.normal * _EmissionSize;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.normal = v.normal;
				o.worldPos = UnityObjectToClipPos(v.vertex);
				return o;
			}

			half4 frag(v2f i) : COLOR
			{
				half4 color = _EmissionColor;
				float3 normal = normalize(i.normal);
				float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
				half rim = 1.0 - saturate(dot(viewDir, normal));
				color.rgb = color.rgb * pow(rim, _EmissionPower);
				color.a = pow(rim, _EmissionPower);
				return color;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
