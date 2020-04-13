///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// \brief   Oil paint, based on Anisotropic Kuwahara Filtering (http://www.kyprianidis.com/p/gpupro).
// \date    10.04.2014
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Ibuprogames. All rights reserved.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// http://unity3d.com/support/documentation/Components/SL-Shader.html
Shader "Hidden/Oil Paint custom"
{
  // http://unity3d.com/support/documentation/Components/SL-Properties.html
  Properties
  {
    _MainTex("Base (RGB)", 2D) = "white" {}
  }

  CGINCLUDE
  #include "UnityCG.cginc"
  #include "OilPaint.cginc"

  #define RADIUS 2

  uniform sampler2D _MainTex;
  uniform float _Amount;
  uniform float4 _InvScreenSize;
  uniform int _Radius;

  float4 frag_gamma(v2f_img i) : COLOR
  {
    float3 m[4], s[4];
    
    m[0] = m[1] = m[2] = m[3] = 0.0f;
    s[0] = s[1] = s[2] = s[3] = 0.0f;

    float3 c;
    int u, v;

	for (int k = -_Radius; k <= 0; ++k)
	{
	  for (int j = -_Radius; j <= 0; ++j)
	  {
	    c = tex2D(_MainTex, i.uv + float2(j, k) * _InvScreenSize.xy).rgb;
        m[0] += c;
        s[0] += c * c;
	  }
	}

	for (int k = -_Radius; k <= 0; ++k)
	{
	  for (int j = 0; j <= _Radius; ++j)
	  {
	    c = tex2D(_MainTex, i.uv + float2(j, k) * _InvScreenSize.xy).rgb;
        m[1] += c;
        s[1] += c * c;
	  }
	}

	for (int k = 0; k <= _Radius; ++k)
	{
	  for (int j = 0; j <= _Radius; ++j)
	  {
	    c = tex2D(_MainTex, i.uv + float2(j, k) * _InvScreenSize.xy).rgb;
        m[2] += c;
        s[2] += c * c;
	  }
	}

	for (int k = 0; k <= _Radius; ++k)
	{
	  for (int j = -_Radius; j <= 0; ++j)
	  {
	    c = tex2D(_MainTex, i.uv + float2(j, k) * _InvScreenSize.xy).rgb;
        m[3] += c;
        s[3] += c * c;
	  }
	}

    float3 finalPixel = 0.0f;
    float minSigma2 = 1e+2;
    float n = float((_Radius + 1) * (_Radius + 1));

    for (u = 0; u < 4; ++u)
    {
      m[u] /= n;
      s[u] = abs(s[u] / n - m[u] * m[u]);
      float sigma2 = s[u].r + s[u].g + s[u].b;
      if (sigma2 < minSigma2)
      {
        minSigma2 = sigma2;
        finalPixel = m[u];
      }
    }

	float3 pixel = tex2D(_MainTex, i.uv);

#ifdef ENABLE_DEMO
    finalPixel = PixelDemo(pixel, finalPixel, i.uv);
#endif

    return float4(lerp(pixel, finalPixel, _Amount), 1.0f);
  }

  float4 frag_linear(v2f_img i) : COLOR
  {
    float3 m[4], s[4];
    
    m[0] = m[1] = m[2] = m[3] = 0.0f;
    s[0] = s[1] = s[2] = s[3] = 0.0f;

    float3 c;
    int u, v;

	for (int k = -_Radius; k <= 0; ++k)
	{
	  for (int j = -_Radius; j <= 0; ++j)
	  {
	    c = sRGB(tex2D(_MainTex, i.uv + float2(j, k) * _InvScreenSize.xy).rgb);
        m[0] += c;
        s[0] += c * c;
	  }
	}

	for (int k = -_Radius; k <= 0; ++k)
	{
	  for (int j = 0; j <= _Radius; ++j)
	  {
	    c = sRGB(tex2D(_MainTex, i.uv + float2(j, k) * _InvScreenSize.xy).rgb);
        m[1] += c;
        s[1] += c * c;
	  }
	}

	for (int k = 0; k <= _Radius; ++k)
	{
	  for (int j = 0; j <= _Radius; ++j)
	  {
	    c = sRGB(tex2D(_MainTex, i.uv + float2(j, k) * _InvScreenSize.xy).rgb);
        m[2] += c;
        s[2] += c * c;
	  }
	}

	for (int k = 0; k <= _Radius; ++k)
	{
	  for (int j = -_Radius; j <= 0; ++j)
	  {
	    c = sRGB(tex2D(_MainTex, i.uv + float2(j, k) * _InvScreenSize.xy).rgb);
        m[3] += c;
        s[3] += c * c;
	  }
	}

    float3 finalPixel = 0.0f;
    float minSigma2 = 1e+2;
    float n = float((_Radius + 1) * (_Radius + 1));

#ifdef UNROLL
    m[0] /= n;
    s[0] = abs(s[0] / n - m[0] * m[0]);

    float sigma2 = s[0].r + s[0].g + s[0].b;
    if (sigma2 < minSigma2)
    {
      minSigma2 = sigma2;
      finalPixel = m[0];
    }

    m[1] /= n;
    s[1] = abs(s[1] / n - m[1] * m[1]);

    sigma2 = s[1].r + s[1].g + s[1].b;
    if (sigma2 < minSigma2)
    {
      minSigma2 = sigma2;
      finalPixel = m[1];
    }

    m[2] /= n;
    s[2] = abs(s[2] / n - m[2] * m[2]);

    sigma2 = s[2].r + s[2].g + s[2].b;
    if (sigma2 < minSigma2)
    {
      minSigma2 = sigma2;
      finalPixel = m[2];
    }

    m[3] /= n;
    s[3] = abs(s[3] / n - m[3] * m[3]);

    sigma2 = s[3].r + s[3].g + s[3].b;
    if (sigma2 < minSigma2)
    {
      minSigma2 = sigma2;
      finalPixel = m[3];
    }
#else
    for (u = 0; u < 4; ++u)
    {
      m[u] /= n;
      s[u] = abs(s[u] / n - m[u] * m[u]);
      float sigma2 = s[u].r + s[u].g + s[u].b;
      if (sigma2 < minSigma2)
      {
        minSigma2 = sigma2;
        finalPixel = m[u];
      }
    }
#endif

    float3 pixel = sRGB(tex2D(_MainTex, i.uv).rgb);

#ifdef ENABLE_DEMO
    finalPixel = PixelDemo(pixel, finalPixel, i.uv);
#endif

    return float4(Linear(lerp(pixel, finalPixel, _Amount)), 1.0f);
  }

  ENDCG

  // Techniques (http://unity3d.com/support/documentation/Components/SL-SubShader.html).
  SubShader
  {
    // Tags (http://docs.unity3d.com/Manual/SL-CullAndDepth.html).
    ZTest Always
    Cull Off
    ZWrite Off
    Fog { Mode off }

    // Pass 0: Color Space Gamma.
    Pass
    {
      CGPROGRAM
      #pragma glsl
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma target 3.0
      #pragma vertex vert_img
      #pragma fragment frag_gamma
      ENDCG      
    }

    // Pass 1: Color Space Linear.
    Pass
    {
      CGPROGRAM
      #pragma glsl
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma target 3.0
      #pragma vertex vert_img
      #pragma fragment frag_linear
      ENDCG      
    }
  }

  Fallback off
}