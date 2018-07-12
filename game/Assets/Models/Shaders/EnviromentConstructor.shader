Shader "Custom/EnviromentConstructor" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_WorldTexture ("WorldTexture (RGB)", 2D) = "white" {}
		_LocalTexture ("LocalTexture (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_WorldSize ("WorldSize", Vector) = (1,1,0,0)
		_WorldOffset ("WorldOffset", Vector) = (0,0,0,0)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _WorldTexture;
		sampler2D _LocalTexture;

		struct Input {
			float2 uv_WorldTexture;
			float2 uv_LocalTexture;
			float3 worldPos;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		float4 _WorldSize;
		float4 _WorldOffset;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			float3 uv = (IN.worldPos + _WorldOffset.xyz) / _WorldSize.xyz;
			fixed4 c = tex2D (_WorldTexture, uv) * _Color;
			fixed4 mask = tex2D(_LocalTexture, IN.uv_LocalTexture);
			c.rgb = lerp(c.rgb, c.rgb * mask.r, mask.r);
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
