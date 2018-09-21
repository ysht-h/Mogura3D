Shader "Custom/holeSurface"
{
	SubShader
	{
		Tags { "RenderType"="Opaque" "Queue" = "Geometry-1"  }

		Pass
		{
		
    		ColorMask 0
    		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			float4 vert (float4 v:POSITION) : SV_POSITION
			{
                return UnityObjectToClipPos(v);
			}
			
			fixed4 frag () : COLOR
			{
				return fixed4(0, 0, 0, 0);
			}
			ENDCG
		}
	}
}
