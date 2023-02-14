Shader "Unlit/Sun"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ColorA("Color A", Color) = (1,1,1,1)
        _ColorB("Color B", Color) = (1,1,1,1)
        _Scale("Scale", Float) = 1.0
        _ParamA("Param A", Float) = 1.0
        _ParamB("Param B", Integer) = 24
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float n = pattern(i.uv * _Scale);
                fixed4 col = _ColorA * n + (1 - n) * _ColorB;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
