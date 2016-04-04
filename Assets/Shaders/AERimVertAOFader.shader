Shader "AE/Rimlit Vertex AO Fader" {
	
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_RimFalloff ("Rimlight Falloff", Range(0.1, 10)) = 4.0
		_RimStrength ("Rimlight Strength", Range(0, 1)) = 1.0
		_Color ("Color Tint", Color) = (1,1,1,1)
	}
	
	SubShader {
		
		Tags {
			"Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
		}

		
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert
		//#pragma vertex vert
        //#pragma fragment frag
		
		
		sampler2D _MainTex;
		float _RimFalloff;
		float _RimStrength;
		half4 _Color;
		
		struct Input {
			float2 uv_MainTex : TEXCOORD0;
			//float4 color      : COLOR;
			half4 color : COLOR;
		};
		
		
		void vert(inout appdata_full v)
		{
			v.color.rgb = 1;
			half rim = 1 - saturate( dot( normalize( ObjSpaceViewDir(v.vertex) ), v.normal ) );
			v.color.rgb *= pow(rim, _RimFalloff);
		}
		
		
		
		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			//o.Albedo = c.rgb * IN.color.a;
			o.Albedo = _Color;
			o.Emission = IN.color.rgb * _RimStrength;
			//o.Alpha = c.a;
			o.Alpha = _Color.a;
		}
		
		ENDCG
	}
	
	FallBack "Diffuse"
	
}
