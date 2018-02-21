Shader "VFX/MainMenu/Neon"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float blink : COLOR;
				UNITY_FOG_COORDS(1)
			};

			sampler2D _MainTex;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				UNITY_TRANSFER_FOG(o,o.vertex);

				float phase_01, phase_02, phase_03;

				phase_01 = saturate(20 * frac(_Time.b * 20) - 15);
				phase_02 = saturate(16 * frac(_Time.r * 2) - 14);

				o.blink = phase_01 * phase_02;
				//o.blink = phase_02;

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);

				// apply fog
				
				col.rgb += .1 * i.blink;
				col = saturate(col);
				
				UNITY_APPLY_FOG(i.fogCoord, col);
				
				return col;
				//return i.blink;
			}
			ENDCG
		}
	}
}
