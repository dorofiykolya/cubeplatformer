Shader "Game/MenuTVDisplay" {
	Properties {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_EmissionTex ("Emission (RGB)", 2D) = "black" {}
		_EmissionColor ("Emission Color", Color) = (1,1,1,1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _EmissionTex;
		float4 _EmissionColor;

		struct Input {
			float2 uv_MainTex;
		};

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		// UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		// UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 mainTexture = tex2D (_MainTex, IN.uv_MainTex);
			fixed4 emissionTexture = tex2D(_EmissionTex, IN.uv_MainTex);
			o.Albedo = mainTexture.rgb;
			o.Emission = emissionTexture.rgb * _EmissionColor.rgb;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
