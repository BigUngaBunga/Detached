Shader "Custom/MagnetizationShader"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _Max("Max alpha value", Float) = 0.2
        _Min("Minimum alpha value", Float) = 0.05
        _Alpha("Alpha", Float) = 0.1
        _Frequency("Frequency of the cosine wave", Float) = 1
        _UseObjectSpace("Use object space bool", Int) = 1
        _WaveSpeed("Speed of the magnetic waves", Float) = 10
        _TargetPosition("Point of effect origin", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Fade"}
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass {
            

            CGPROGRAM
            #pragma vertex vert alpha
            #pragma fragment frag alpha

            fixed4 _Color;
            float _Max;
            float _Min;
            float _Alpha;
            float _Frequency;
            float _UseObjectSpace;
            Vector _TargetPosition;

            float _GameTime;
            float _WaveSpeed;

            float putInRange(float value) {
                return value * (_Max - _Min) + _Min;
            }

            float putInPositiveRange(float value){
                float positiveValue = (value + 1)/2;
                return putInRange(positiveValue);
            }

            float4 vert(float4 v:POSITION) : POSITION {
                return UnityObjectToClipPos(v);
            }
            fixed4 frag(float4 v:POSITION) : SV_Target{
                
                float3 objectPosition = _TargetPosition.xyz - mul(unity_WorldToObject, v).xyz;
                float value = cos(length(objectPosition) * _Frequency + _GameTime * _WaveSpeed);
                _Alpha = putInPositiveRange(value);
                return fixed4(_Color * _Alpha);
            }
            ENDCG
        }
    }
}
