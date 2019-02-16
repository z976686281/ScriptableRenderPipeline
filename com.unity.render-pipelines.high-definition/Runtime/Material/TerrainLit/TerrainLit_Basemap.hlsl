TEXTURE2D(_MainTex);
TEXTURE2D(_MetallicTex);

void TerrainLitShade(float2 uv, float3 tangentWS, float3 bitangentWS,
    out float3 outAlbedo, out float3 outNormalTS, out float outSmoothness, out float outMetallic, out float outAO)
{
    float4 mainTex = SAMPLE_TEXTURE2D(_MainTex, s_linear_clamp_sampler, uv);
    float4 metallicTex = SAMPLE_TEXTURE2D(_MetallicTex, s_linear_clamp_sampler, uv);
    outAlbedo = mainTex.rgb;
#ifdef SURFACE_GRADIENT
    outNormalTS = float3(0.0, 0.0, 0.0); // No gradient
#else
    outNormalTS = float3(0.0, 0.0, 1.0);
#endif
    outSmoothness = mainTex.a;
    outMetallic = metallicTex.r;
    outAO = metallicTex.g;
}

void TerrainLitDebug(float2 uv, inout float3 baseColor)
{
#ifdef DEBUG_DISPLAY
    baseColor = GetTextureDataDebug(_DebugMipMapMode, uv, _MainTex, _MainTex_TexelSize, _MainTex_MipInfo, baseColor);
#endif
}
