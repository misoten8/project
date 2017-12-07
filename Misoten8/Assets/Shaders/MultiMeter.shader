Shader "Unlit/Meter"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Base color", Color) = (1, 1, 1, 1)	// float
		//_Border("BorderArray", Vector4) = (0,0,0,0)
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

			// 0:non player 1~4:playerIndex
			uniform float _BorderValue[5];// 境界線を受け取る 例 0:0.2 1:0.4...4:1.0
			uniform float4 _MeterColor[5];
			sampler2D _MainTex;
			float4 _MainTex_ST;	// Tiling, Offsetをインスペクタに表示し、設定できるようにする！
			float4 _Color;	// fixed, harf
			

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
				fixed4 col = tex2D(_MainTex, i.uv);
	
				UNITY_APPLY_FOG(i.fogCoord, col);
				UNITY_OPAQUE_ALPHA(col.a);

				for (int count = 0; count < 5; count++)
				{
					if (i.uv.x < _BorderValue[count])
					{
						col *= _MeterColor[count];
						return col;
					}
				}
				return col;
			}
			ENDCG
		}
	}
}
