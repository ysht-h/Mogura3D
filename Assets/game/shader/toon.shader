Shader "Custom/toon" {
	Properties {
		_MainTex("MainTex(RGB)", 2D) = "white" {}
		_RampTex("Shadow(RGB)", 2D ) = "white" {}
		_OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
		_Outline("Outline width", Range(0.0001, 0.5)) = 0.005
		_shadow("Diffuse border", Range(0.01, 1)) = 0.2
		_shadowBlur("Diffuse border blur", Range(0.01, 0.2)) = 0.01
		

	}
	
	SubShader {
	
	
	    pass
	    {
	        Tags { "LightMode" = "ForwardBase" }
	        
	        CGPROGRAM
	        #pragma vertex vert
	        #pragma fragment frag
	        #pragma multi_compile_fog
	        #include "UnityCG.cginc"
	        
	        sampler2D _MainTex;
	        sampler2D _RampTex;
	        float     _shadow;
	        float     _shadowBlur;
	        
	        struct vinput
	        {
	            float4 vertex   : POSITION;
	            float2 uv       : TEXCOORD0;
	            float3 normal   : NORMAL;
	        };
	    
	        struct interp
	        {
	            float2 uv       : TEXCOORD0;
	            UNITY_FOG_COORDS(4)
	            float4 pos      : SV_POSITION;
	            float3 L        : TEXCOORD1;
	            float3 N        : TEXCOORD2;
	        };
	        
	        interp vert(vinput i)
	        {
	            interp o;
	            
	            o.pos = UnityObjectToClipPos(i.vertex);
	            
	            UNITY_TRANSFER_FOG(o, o.pos);
	            
	            // UV
	            o.uv = i.uv;
	            
	            // 法線ベクトル
	            o.N = normalize(i.normal);
	            
                // ライトベクトル
                o.L = normalize(-ObjSpaceLightDir(i.vertex));
                
                return o;	            
	            
	        }
	        
	        
	        float4 frag(interp i) : COLOR
	        {
	            
	            // 拡散反射の場合
	            half i_d = dot(i.L, i.N);
	            
	            fixed4 col = tex2D(_MainTex, i.uv);
	            fixed4 shadow = tex2D(_RampTex, i.uv);
	            
	            // 影 (i_d > _shadow であれば影色で塗る / __SpecularBorderBlurで影をぼかす
	            fixed t_d = smoothstep( _shadow - _shadowBlur, _shadow + _shadowBlur, i_d);
	            fixed4 c = lerp(col, col * shadow, t_d);
	            
	            UNITY_APPLY_FOG(i.fogCoord, c);
	            
	            return c;
	        }
	        ENDCG
	        
	    }
	    
	    pass
	    {
	    
	        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
	        
	        Cull Front
	        ZWrite On
	        
	        CGPROGRAM
	        #pragma vertex vert
	        #pragma fragment frag
	        #pragma multi_compile_fog
	        #include "UnityCG.cginc"
	        
	        struct vinput
	        {
	            float4 vertex       : POSITION;
	            float3 normal       : NORMAL;
	            fixed4 color        : COLOR;
	        };
	        
	        struct interp
	        {
	            float4 pos          : SV_POSITION;
	            UNITY_FOG_COORDS(1)
	        };
	        
	        float _Outline;
	        float4 _OutlineColor;
	        
	        interp vert(vinput i)
	        {
	            interp o;
	            o.pos = UnityObjectToClipPos(i.vertex);
	            
	            float3 norm = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, i.normal));
	            
	            float2 offset = TransformViewToProjection(norm.xy);
	            
	            o.pos.xy += offset * _Outline;
	            UNITY_TRANSFER_FOG(o, o.pos);
	            return o;
	        }
	        
	        fixed4 frag(interp i) : SV_Target
	        {
	            UNITY_APPLY_FOG(i.fogCoord, _OutlineColor);
	            return _OutlineColor;
	        }
	        ENDCG
	    }

	}
	FallBack "Diffuse"
}
