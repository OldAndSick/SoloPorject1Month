Shader "Custom/FOV_Fog"
{
    Properties
    {
        _BaseColor("Fog Color", Color) = (0, 0, 0, 0.8)
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" "Queue"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite OFF

        Pass
        {
            Stencil
            {
                Ref 1
                Comp NotEqual // 1번(시야)이 아닌 곳에만 안개를 그립니다.
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes { float4 positionOS : POSITION; };
            struct Varyings { float4 positionHCS : SV_POSITION; };

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                return _BaseColor;
            }
            ENDHLSL
        }
    }
}