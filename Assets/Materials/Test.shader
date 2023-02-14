Shader "Custom/Test"
{
    Properties
    {
        _ColorA ("Color A", Color) = (1,1,1,1)
        _ColorB ("Color B", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Scale ("Scale", Float) = 1.0
        _ParamA ("Param A", Float) = 1.0
        _ParamB ("Param B", Integer) = 24
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        struct Input
        {
            float2 uv_MainTex;
        };
        sampler2D _MainTex;
        half _Glossiness;
        half _Metallic;
        float _Scale;
        fixed4 _ColorA;
        fixed4 _ColorB;
        float _ParamA;
        int _ParamB;

        fixed2 hash(fixed2 p) // replace this by something better
        {
            p = fixed2(dot(p, fixed2(127.1, 311.7)), dot(p, fixed2(269.5, 183.3)));
            return -1.0 + 2.0 * frac(sin(p) * 43758.5453123);
        }

        float noise(in fixed2 p)
        {
            const float K1 = 0.366025404; // (sqrt(3)-1)/2;
            const float K2 = 0.211324865; // (3-sqrt(3))/6;

            fixed2  i = floor(p + (p.x + p.y) * K1);
            fixed2  a = p - i + (i.x + i.y) * K2;
            float m = step(a.y, a.x);
            fixed2  o = fixed2(m, 1.0 - m);
            fixed2  b = a - o + K2;
            fixed2  c = a - 1.0 + 2.0 * K2;
            fixed3  h = max(0.5 - fixed3(dot(a, a), dot(b, b), dot(c, c)), 0.0);
            fixed3  n = h * h * h * h * fixed3(dot(a, hash(i + 0.0)), dot(b, hash(i + o)), dot(c, hash(i + 1.0)));
            return dot(n, fixed3(70.0, 70.0, 70.0));
        }

        float fbm(in fixed2 x, in float H)
        {
            float t = 0.0;
            for (int i = 0; i < _ParamB; i++)
            {
                float f = pow(2.0, float(i));
                float a = pow(f, -H);
                t += a * noise(f * x) * H;
            }
            return t;
        }

        float pattern(in fixed2 p)
        {
            fixed2 q = fixed2(fbm(p + fixed2(0.0, 0.0), _ParamA),
                fbm(p + fixed2(5.2, 1.3), _ParamA));

            fixed2 r = fixed2(fbm(p + 4.0 * q + fixed2(1.7, 9.2), _ParamA),
                fbm(p + 4.0 * q + fixed2(8.3, 2.8), _ParamA));

            return fbm(p + 4.0 * r, _ParamA);
        }

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float n = pattern(IN.uv_MainTex * _Scale);
            fixed4 c = _ColorA*n+(1-n)*_ColorB;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
