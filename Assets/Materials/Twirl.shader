Shader "Custom/Twirl" {
	Properties {
		_Value ("Rotation", Range(0, 1)) = 0.5
		_Color("Color", Color) = (0.26,0.19,0.16,0.0)
		_MainTex("Texture", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert addshadow fullforwardshadows nolightmap

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		float4 _Color;
		float _Value;
		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};


		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		float3 rotateY(float3 vertex, float rotation) {
			float4 vert = float4(vertex, 0);
			float4x4 mat;

			mat[0] = float4(cos(rotation), 0, sin(rotation), 0);
			mat[1] = float4(0, 1, 0, 0);
			mat[2] = float4(-sin(rotation), 0, cos(rotation), 0);
			mat[3] = float4(0, 0, 0, 1);
			
			return mul(mat, vert).xyz;
		}

		void vert(inout appdata_full v) {
			v.vertex.xyz = rotateY(v.vertex.xyz, _Value * v.vertex.y * sin(_Time.w));
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
