Shader "MyShader/PlayerShader" {
	Properties {
		_Color ("Color", Color)						= (1, 1, 1, 1)
		_MainTex ("Albedo (RGB)", 2D)				= "white" {}
		_BumpMap("Normal Map", 2D)					= "bump"{}
		_BumpScale("Bump Scale", Float)				= 1.0
		_Brightness("Brightness", Range(0.5, 2.0))	= 1
	}
	SubShader {
		Pass{
			Tags { "LightMode" = "ForwardBase" }
			LOD 200
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "Lighting.cginc"
			fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _BumpMap;
			float4 _BumpMap_ST;
			float _BumpScale;
			float _Brightness;
			
			struct a2v{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 texcoord : TEXCOORD0;
			};

			struct v2f{
				float4 pos : SV_POSITION;
				float4 uv : TEXCOORD0;
				float4 TtoW0 :TEXCOORD1;
				float4 TtoW1 :TEXCOORD2;
				float4 TtoW2 :TEXCOORD3;
			};

			v2f vert(a2v v){
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				o.uv.zw = v.texcoord.xy * _BumpMap_ST.xy + _BumpMap_ST.zw;

				//Get world postion, tangent, bitangent and normal vector infomation in world space
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				float3 worldNormal = UnityObjectToWorldNormal(v.normal);
				float3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
				float3 worldBitangent = cross(worldNormal, worldTangent) * v.tangent.w;
				//Transform matrix
				o.TtoW0 = float4(worldTangent.x, worldBitangent.x, worldNormal.z, worldPos.x);
				o.TtoW1 = float4(worldTangent.y, worldBitangent.y, worldNormal.y, worldPos.y);
				o.TtoW2 = float4(worldTangent.z, worldBitangent.z, worldNormal.z, worldPos.z);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target{
				float3 worldPos = float3(i.TtoW0.w, i.TtoW1.w, i.TtoW2.w);
				//Get light direction and view direction
				fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
				fixed3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos));
				fixed3 bump = UnpackNormal(tex2D(_BumpMap, i.uv.zw));
				bump.xy *= _BumpScale;
				bump.z = sqrt(1.0 - saturate(dot(bump.xy, bump.xy)));
				bump = normalize(float3(dot(i.TtoW0.xyz, bump), dot(i.TtoW1.xyz, bump), dot(i.TtoW2.xyz, bump)));
				fixed3 albedo = tex2D(_MainTex, i.uv.xy).rgb * _Color.rgb * _Brightness;
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
				fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(bump, lightDir));
				return fixed4(ambient + diffuse, 1.0);
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
