#ifdef SHADER_VARIABLES_INCLUDE_CB
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/ScreenSpaceLighting/ShaderVariablesScreenSpaceLighting.cs.hlsl"
#else
    TEXTURE2DX(_DepthPyramidTexture);
    TEXTURE2DX(_AmbientOcclusionTexture);
    TEXTURE2DX(_CameraMotionVectorsTexture);
    TEXTURE2DX(_SsrLightingTexture);
#endif
