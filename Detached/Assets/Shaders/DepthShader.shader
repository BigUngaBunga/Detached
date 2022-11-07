Shader "Custom/DepthShader" {
    Properties
    {
        _MainTex("-", 2D) = "white" {}
        _TargetDepth ("Target depth", Float) = 1.0
    }
    CGINCLUDE

    #include "UnityCG.cginc"

    sampler2D _MainTex;
    sampler2D _CameraDepthTexture;
    float _TargetDepth;

    half4 fragLess(v2f_img IN) : SV_Target {
        //if(tex2d(_CameraDepthTexture, IN.uv).x < _TargetDepth){
        //    return tex2D (_MainTex, IN.uv);
        //}
        //return tex2D (_MainTex, IN.uv) - tex2D (_MainTex, IN.uv);
        return (1,1,1,1);
    }

    half4 fragMore(v2f_img IN) : SV_Target {
        //if(tex2d(_CameraDepthTexture, IN.uv).x >= _TargetDepth){
        //    return tex2D (_MainTex, IN.uv);
        //}
        //return tex2D (_MainTex, IN.uv) - tex2D (_MainTex, IN.uv);
        return (1,1,1,1);
    }
    ENDCG

    SubShader {
        Tags { "RenderType"="Opaque" }
        Pass {
            CGPROGRAM

            #pragma vertex vert_img
            #pragma fragment fragLess
            ENDCG
        }

        Pass {
            CGPROGRAM

            #pragma vertex vert_img
            #pragma fragment fragMore
            ENDCG
        }
    }
}
