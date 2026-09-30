Shader "EmberTail/UIBurn"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _BurnAmount ("Burn Amount", Range(0,1)) = 0
        _NoiseScale ("Noise Scale", Float) = 6

        _CharWidth ("Char Width", Range(0.001,0.3)) = 0.12
        _FireWidth ("Fire Width", Range(0.001,0.2)) = 0.07
        _HotWidth ("Hot Edge Width", Range(0.001,0.1)) = 0.025

        _CharColor ("Char Color", Color) = (0.08,0.015,0.005,1)
        _FireColor ("Fire Color", Color) = (1,0.12,0,1)
        _HotColor ("Hot Edge Color", Color) = (1,0.85,0.15,1)

        _GlowIntensity ("Glow Intensity", Range(0,5)) = 2
        _FlickerSpeed ("Flicker Speed", Range(0,20)) = 7
        _FlickerStrength ("Flicker Strength", Range(0,1)) = 0.25

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                float2 localPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            fixed4 _Color;

            float _BurnAmount;
            float _NoiseScale;

            float _CharWidth;
            float _FireWidth;
            float _HotWidth;

            fixed4 _CharColor;
            fixed4 _FireColor;
            fixed4 _HotColor;

            float _GlowIntensity;
            float _FlickerSpeed;
            float _FlickerStrength;

            float Random(float2 p)
            {
                return frac(
                    sin(
                        dot(
                            p,
                            float2(12.9898, 78.233)
                        )
                    ) * 43758.5453
                );
            }

            float Noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);

                f =
                    f * f *
                    (3.0 - 2.0 * f);

                float a =
                    Random(i);

                float b =
                    Random(
                        i +
                        float2(1.0, 0.0)
                    );

                float c =
                    Random(
                        i +
                        float2(0.0, 1.0)
                    );

                float d =
                    Random(
                        i +
                        float2(1.0, 1.0)
                    );

                return lerp(
                    lerp(
                        a,
                        b,
                        f.x
                    ),
                    lerp(
                        c,
                        d,
                        f.x
                    ),
                    f.y
                );
            }

            float FractalNoise(float2 p)
            {
                float value = 0.0;
                float amplitude = 0.5;

                value +=
                    Noise(p) *
                    amplitude;

                p *= 2.03;
                amplitude *= 0.5;

                value +=
                    Noise(p) *
                    amplitude;

                p *= 2.01;
                amplitude *= 0.5;

                value +=
                    Noise(p) *
                    amplitude;

                return value;
            }

            v2f vert(appdata_t v)
            {
                v2f o;

                o.vertex =
                    UnityObjectToClipPos(
                        v.vertex
                    );

                o.color =
                    v.color *
                    _Color;

                o.texcoord =
                    v.texcoord;

                o.localPos =
                    v.texcoord;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 originalColor =
                    tex2D(
                        _MainTex,
                        i.texcoord
                    ) *
                    i.color;

                if (originalColor.a <= 0.001)
                {
                    discard;
                }

                float2 noiseUV =
                    i.localPos *
                    _NoiseScale;

                float noise =
                    FractalNoise(
                        noiseUV
                    );

                float flickerNoise =
                    Noise(
                        noiseUV * 2.5 +
                        float2(
                            _Time.y *
                            _FlickerSpeed,
                            -_Time.y *
                            _FlickerSpeed *
                            0.7
                        )
                    );

                float flicker =
                    (flickerNoise - 0.5) *
                    _FlickerStrength;

                float burnThreshold =
                    _BurnAmount;

                if (noise < burnThreshold)
                {
                    discard;
                }

                float distanceFromBurn =
                    noise -
                    burnThreshold;

                float animatedDistance =
                    distanceFromBurn +
                    flicker *
                    _HotWidth;

                fixed4 finalColor =
                    originalColor;

                if (
                    animatedDistance <
                    _HotWidth
                )
                {
                    float hotProgress =
                        saturate(
                            animatedDistance /
                            _HotWidth
                        );

                    fixed3 hotColor =
                        _HotColor.rgb *
                        _GlowIntensity;

                    finalColor.rgb =
                        lerp(
                            hotColor,
                            _FireColor.rgb *
                            _GlowIntensity,
                            hotProgress
                        );
                }
                else if (
                    animatedDistance <
                    _HotWidth +
                    _FireWidth
                )
                {
                    float fireProgress =
                        saturate(
                            (
                                animatedDistance -
                                _HotWidth
                            ) /
                            _FireWidth
                        );

                    finalColor.rgb =
                        lerp(
                            _FireColor.rgb *
                            _GlowIntensity,
                            _CharColor.rgb,
                            fireProgress
                        );
                }
                else if (
                    animatedDistance <
                    _HotWidth +
                    _FireWidth +
                    _CharWidth
                )
                {
                    float charProgress =
                        saturate(
                            (
                                animatedDistance -
                                _HotWidth -
                                _FireWidth
                            ) /
                            _CharWidth
                        );

                    finalColor.rgb =
                        lerp(
                            _CharColor.rgb,
                            originalColor.rgb,
                            charProgress
                        );
                }

                return finalColor;
            }

            ENDCG
        }
    }
}