Shader "Custom/Removal"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Strength ("Colour strength", Float) = 1.0
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
                float _Strength;
                fixed4 _Color;

                half4 frag(v2f_img IN) : COLOR {
                    half4 mainTexture = tex2D (_MainTex, IN.uv);
                    half4 addedTexture = tex2D(_OccludeMap, IN.uv) * _Strength;

                    bool xIsBright = mainTexture.x > 0.5;
                    bool yIsBright = mainTexture.y > 0.5;
                    bool zIsBright = mainTexture.z > 0.5;
                    if( (xIsBright && yIsBright) || (yIsBright && zIsBright) || (xIsBright && zIsBright) )
                    {
                        addedTexture = addedTexture * -1;
                    }
                    
                    half4 result = mainTexture + addedTexture;
                    

                    return result;
                }
            ENDCG
        }
    }
}