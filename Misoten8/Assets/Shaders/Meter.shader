Shader "Unlit/Meter"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Base color", Color) = (1, 1, 1, 1)	// float
		_Border("Border", Float) = 1.0
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex VS
			#pragma fragment FS
			#pragma target 2.0
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			struct VS_INPUT
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct FS_INPUT 
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(0)
				UNITY_VERTEX_OUTPUT_STEREO
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;	// Tiling, Offsetをインスペクタに表示し、設定できるようにする！
			float4 _Color;	// fixed, harf
			float _Border;

			FS_INPUT VS(VS_INPUT v)
			{
				FS_INPUT o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			fixed4 FS(FS_INPUT i) : COLOR
			{
				fixed4 col = _Color * tex2D(_MainTex, i.uv);;
	
				UNITY_APPLY_FOG(i.fogCoord, col);
				UNITY_OPAQUE_ALPHA(col.a);
				// 閾値を超えたら、色を暗くする
				if (i.uv.x > _Border)
				{
					col *= 0.25;
				}
				return col;
			}
			ENDCG
		}
	}
}
