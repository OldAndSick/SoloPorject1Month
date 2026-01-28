Shader "Custom/FOV_SoftMask"
{
    SubShader
    {
        // 안개보다 먼저 그려져야 하므로 Queue를 낮춥니다.
        Tags { "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" "Queue" ="Transparent-1"}
        ColorMask 0 // [핵심] 아무 색도 그리지 않음 (검은 구멍 방지)
        ZWrite OFF

        Pass
        {
            Stencil
            {
                Ref 1
                Comp Always
                Pass Replace // 이 메쉬가 닿는 곳을 1번으로 기록
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes { float4 positionOS : POSITION; };
            struct Varyings { float4 positionHCS : SV_POSITION; };

            Varyings vert(Attributes IN) {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target { return half4(0,0,0,0); }
            ENDHLSL
        }
    }
}