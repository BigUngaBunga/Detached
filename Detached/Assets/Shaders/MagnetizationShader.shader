Shader "Custom/MagnetizationShader"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Pass {
            Tags {"RenderType" = "Fade"}
            ZWrite On ZTest Always Fog { Mode Off }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            fixed4 _Color;
            float _Max;
            float _Min;
            float _Length;
            float _Frequency;

            float putInRange(float value, float min, float max) {
                return value * (max - min) + min;
            }

            float4 vert(float4 v:POSITION) : POSITION {
                return UnityObjectToClipPos(v);
            }
            fixed4 frag(float4 v:POSITION) : SV_Target{

                float value = cos(length(v) + _Length);
                float alpha = putInRange(value, _Min, _Max);
                return fixed4(_Color.rgb * alpha, 1.0);
            }
            ENDCG
        }
    }
}
