Shader "Custom/SpriteGradient" {
	Properties{
		_Base("Base", Color) = (1,1,1,1)
		_Color("Color", Color) = (1,1,1,1)
	}

	SubShader{
		Pass{
			CGPROGRAM

			#pragma vertex vert  
			#pragma fragment frag
			#include "UnityCG.cginc"

			fixed4 _Base;
			fixed4 _Color;

			struct v2f {
				float4 pos : SV_POSITION;
				fixed4 col : COLOR;
			};

			v2f vert(appdata_full v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);

				float2x2 rotationMatrix = float2x2(1, 0, -1, 0);
				v.texcoord.xy = mul(v.texcoord.xy, rotationMatrix);

				o.col = lerp(_Base, _Color, v.texcoord.x);

				return o;
			}

			float4 frag(v2f i) : COLOR{
				float4 c = i.col;
				c.a = 1;
				return c;
			}
			
			ENDCG
		}
	}
}