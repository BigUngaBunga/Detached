Shader "Hidden/NewImageEffectShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

         Pass {
                CGPROGRAM
                #pragma vertex vert_img
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest
                #include "UnityCG.cginc"
            
                sampler2D _MainTex;
                sampler2D _OccludeMap;
            
                half4 frag(v2f_img IN) : COLOR {
                    return tex2D (_MainTex, IN.uv) - tex2D(_OccludeMap, IN.uv);
                }
                ENDCG
        }

        Pass {
            CGPROGRAM
                #pragma vertex vert_img
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest
                #include "UnityCG.cginc"
            
                sampler2D _MainTex;
                sampler2D _OccludeMap;
            
                half4 frag(v2f_img IN) : COLOR {
                    return tex2D (_MainTex, IN.uv) + tex2D(_OccludeMap, IN.uv);
                }
            ENDCG
        }
    }
}
