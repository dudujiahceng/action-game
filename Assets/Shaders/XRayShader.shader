Shader "MyShader/XRayShader"
{
	Properties
	{
		_RimColor("Rim Color", Color) = (0.35, 0.56, 0.35, 1)
		_RimIntensity("Rim Intensity", Range(0, 2)) = 1
	}
	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType" = "Opaque"}
		LOD 200

		Pass
		{
			Blend SrcAlpha One
			ZWrite On
			Lighting Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			fixed4 _RimColor;
			float _RimIntensity;
			struct appdata{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};		
			struct v2f{
				float4 pos : SV_POSITION;
				float4 color : COLOR;
			};
			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
				float val = 1 - saturate(dot(viewDir, v.normal));
				o.color = _RimColor * val * (1 + _RimIntensity);
				return o;
			}
			fixed4 frag(v2f i) : COLOR
			{
				return i.color;
			}
			ENDCG
		}
	}
	FallBack Off
}
