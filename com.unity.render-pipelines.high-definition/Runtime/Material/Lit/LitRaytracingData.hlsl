#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/Raytracing/Shaders/RaytracingBuiltinData.hlsl"

void GetSurfaceDataFromIntersection(FragInputs input, float3 V, PositionInputs posInput, IntersectionVertice intersectionVertice, RayCone rayCone, out SurfaceData surfaceData, out BuiltinData builtinData)
{
    // Initial value of the material features
    surfaceData.materialFeatures = MATERIALFEATUREFLAGS_LIT_STANDARD;

#ifdef _MATERIAL_FEATURE_SUBSURFACE_SCATTERING
    surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_SUBSURFACE_SCATTERING;
#endif
#ifdef _MATERIAL_FEATURE_TRANSMISSION
    surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_TRANSMISSION;
#endif
#ifdef _MATERIAL_FEATURE_ANISOTROPY
    surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_ANISOTROPY;
#endif
#ifdef _MATERIAL_FEATURE_CLEAR_COAT
    surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_CLEAR_COAT;
#endif
#ifdef _MATERIAL_FEATURE_IRIDESCENCE
    surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_IRIDESCENCE;
#endif
#ifdef _MATERIAL_FEATURE_SPECULAR_COLOR
    surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_SPECULAR_COLOR;
#endif
    
    // Generate the primary uv coordinates
    float2 uvBase = _UVMappingMask.x * input.texCoord0.xy +
                    _UVMappingMask.y * input.texCoord1.xy +
                    _UVMappingMask.z * input.texCoord2.xy +
                    _UVMappingMask.w * input.texCoord3.xy;

    // Apply tiling and offset
    uvBase = uvBase * _BaseColorMap_ST.xy + _BaseColorMap_ST.zw;

    // The base color of the object mixed with the base color texture
    #ifdef USE_RAY_CONE_LOD
    float lod = computeTextureLOD(_BaseColorMap, _UVMappingMask, V, input.worldToTangent[2], rayCone, intersectionVertice);
    surfaceData.baseColor = SAMPLE_TEXTURE2D_LOD(_BaseColorMap, sampler_BaseColorMap, uvBase, lod).rgb * _BaseColor.rgb;
    #else
    surfaceData.baseColor = SAMPLE_TEXTURE2D_LOD(_BaseColorMap, sampler_BaseColorMap, uvBase, 0).rgb * _BaseColor.rgb;
    #endif

    // Default specular occlusion
    surfaceData.specularOcclusion = 1.0;

    #ifdef _NORMALMAP
    float3 normalTS = SAMPLE_TEXTURE2D_LOD(_NormalMap, sampler_NormalMap, uvBase, _NormalScale);
    GetNormalWS(input, normalTS, surfaceData.normalWS);
    #else
    surfaceData.normalWS = input.worldToTangent[2];
    #endif

    // Default smoothness
    #ifdef _MASKMAP
    surfaceData.perceptualSmoothness = SAMPLE_TEXTURE2D_LOD(_MaskMap, sampler_MaskMap, uvBase, 0).a;
    surfaceData.perceptualSmoothness = lerp(_SmoothnessRemapMin, _SmoothnessRemapMax, surfaceData.perceptualSmoothness);
    #else
    surfaceData.perceptualSmoothness = _Smoothness;
    #endif

    // Default Ambient occlusion
    #ifdef _MASKMAP
    surfaceData.ambientOcclusion = SAMPLE_TEXTURE2D_LOD(_MaskMap, sampler_MaskMap, uvBase, 0).g;
    surfaceData.ambientOcclusion = lerp(_AORemapMin, _AORemapMax, surfaceData.ambientOcclusion);
    #else
    surfaceData.ambientOcclusion = 1.0f;
    #endif

    // Default Metallic
    #ifdef _MASKMAP
    surfaceData.metallic = SAMPLE_TEXTURE2D_LOD(_MaskMap, sampler_MaskMap, uvBase, 0).r * _Metallic;
    #else
    surfaceData.metallic = _Metallic;
    #endif

    // Default coatMask
    surfaceData.coatMask = 0.0;

    // Default specular color
    surfaceData.specularColor = _SpecularColor.xyz;

    // Default specular color
    surfaceData.diffusionProfile = 0;

    // Default subsurface mask
    surfaceData.subsurfaceMask = 0.0;

    // Default thickness
    surfaceData.thickness = _Thickness;

    // Default tangentWS
    surfaceData.tangentWS = normalize(input.worldToTangent[0].xyz);

    // Default anisotropy
    surfaceData.anisotropy = _Anisotropy;

    // Iridesence data
    surfaceData.iridescenceThickness = 0.0;
    surfaceData.iridescenceMask = 0.0;

    // Transparency
    surfaceData.ior = 1.0;
    surfaceData.transmittanceColor = float3(1.0, 1.0, 1.0);
    surfaceData.atDistance = 1000000.0;
    surfaceData.transmittanceMask = 0.0;

    // Transparency Data
    float alpha = SAMPLE_TEXTURE2D_LOD(_BaseColorMap, sampler_BaseColorMap, uvBase, 0).a * _BaseColor.a;

    InitBuiltinData(alpha, surfaceData.normalWS, -input.worldToTangent[2], input.positionRWS, input.texCoord1, builtinData);
    PostInitBuiltinData(V, posInput, surfaceData, builtinData);
}