Shader "Hidden/NewImageEffectShader"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        //_MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        //Cull Off ZWrite Off ZTest Always

        Pass {
            Tags {"RenderType" = "Opaque"}
            ZWrite On ZTest Always Fog { Mode Off }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            fixed4 _Color;
            float4 vert(float4 v:POSITION) : POSITION {
                return UnityObjectToClipPos(v);
            }
            fixed4 frag() : SV_Target {
                return fixed4(_Color.rgb * _Color.a, 1.0);
            }
            ENDCG
        }
    }
}
