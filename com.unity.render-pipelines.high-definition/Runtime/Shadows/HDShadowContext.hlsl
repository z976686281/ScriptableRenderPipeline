#ifndef HD_SHADOW_CONTEXT_HLSL
#define HD_SHADOW_CONTEXT_HLSL

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "HDShadowManager.cs.hlsl"

struct HDShadowContext
{
    StructuredBuffer<HDShadowData>  shadowDatas;
    HDDirectionalShadowData         directionalShadowData;
};

// HD shadow sampling bindings
#include "HDShadowSampling.hlsl"
#include "HDShadowAlgorithms.hlsl"

TEXTURE2D(_ShadowmapAtlas);
SamplerComparisonState sampler_ShadowmapAtlas;

TEXTURE2D(_ShadowmapCascadeAtlas);
SamplerComparisonState sampler_ShadowmapCascadeAtlas;

StructuredBuffer<HDShadowData>              _HDShadowDatas;
// Only the first element is used since we only support one directional light
StructuredBuffer<HDDirectionalShadowData>   _HDDirectionalShadowData;

HDShadowContext InitShadowContext()
{
    HDShadowContext         sc;

    sc.shadowDatas = _HDShadowDatas;
    sc.directionalShadowData = _HDDirectionalShadowData[0];

    return sc;
}

#endif // HD_SHADOW_CONTEXT_HLSL
