Shader "Hidden/TexturePreview" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "gray" {}
		_InfoTex("Info (RGB)", 2D) = "gray" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard fullforwardshadows nolightmap

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

#pragma multi_compile UVSET1 UVSET2

			sampler2D _MainTex;
			sampler2D _InfoTex;

			struct Input {
				float2 uv_MainTex;
				float2 uv2_MainTex;
				float2 uv_InfoTex;
				float2 uv2_InfoTex;
			};			

			half _Glossiness;
			half _Metallic;
			fixed4 _Color;

			void surf(Input IN, inout SurfaceOutputStandard o) {
				// Albedo comes from a texture tinted by color
				float2 uv = IN.uv2_MainTex;
				float2 uvInfo = IN.uv2_InfoTex;
#if UVSET1
				uv = IN.uv_MainTex;
				uvInfo = IN.uv_InfoTex;
#endif
				fixed4 base = tex2D(_MainTex, uv);
				fixed4 info = tex2D(_InfoTex, uvInfo);
				fixed4 c = base+(info - 0.5);
				c.a = 1;
				o.Albedo = c*1.75;
				//o.Emission = 0.75*c.rgb;
				// Metallic and smoothness come from slider variables
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
				o.Alpha = c.a;
			}
			ENDCG
		}
			FallBack "Diffuse"
}
