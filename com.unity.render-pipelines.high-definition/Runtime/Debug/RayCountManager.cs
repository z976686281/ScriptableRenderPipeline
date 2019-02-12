using UnityEngine.Rendering;
using System.Collections.Generic; // for List
using Unity.Collections; // for NativeArray

namespace UnityEngine.Experimental.Rendering.HDPipeline
{
#if ENABLE_RAYTRACING
    public class RayCountManager
    {
        // Ray count UAV
        RTHandleSystem.RTHandle m_RayCountTex = null;
        RTHandleSystem.RTHandle m_TotalRayCountTex = null;
        static Texture2D s_DebugFontTex = null;
        static ComputeBuffer s_TotalRayCountBuffer = null;
                
        Material m_Blit; // Material used to blit the output texture into the camera render target
        Material m_DrawRayCount; // Material used to draw ray count onto colorbuffer
        MaterialPropertyBlock m_DrawRayCountProperties = new MaterialPropertyBlock();
        ComputeShader m_RayCountCompute; // Raycount shader
        float m_LatestRayCount;
        DebugDisplaySettings m_DebugDisplaySettings;

        int _TotalRayCountBuffer = Shader.PropertyToID("_TotalRayCountBuffer");
        int _FontColor = Shader.PropertyToID("_FontColor");
        static int k_NumRaySources = 3; // AO, Reflection, Area shadows

        struct RayCountReadback
        {
            public void SetRetired()
            {
                retired = true;
            }

            public ComputeBuffer rayCountBuffer;
            public AsyncGPUReadbackRequest rayCountReadback;
            public bool retired;
            public float deltaTime;
        };
        private static bool ReadbackIsRetired(RayCountReadback s)
        {
            return s.retired;
        }

        List<RayCountReadback> rayCountReadbacks = new List<RayCountReadback>();

        public void Init(RenderPipelineResources renderPipelineResources, DebugDisplaySettings currentDebugDisplaySettings)
        {
            m_Blit = CoreUtils.CreateEngineMaterial(renderPipelineResources.shaders.blitPS);
            m_DrawRayCount = CoreUtils.CreateEngineMaterial(renderPipelineResources.shaders.debugViewRayCountPS);
            m_RayCountCompute = renderPipelineResources.shaders.countTracedRays;
            s_DebugFontTex = renderPipelineResources.textures.debugFontTex;
            // UINT textures must use UINT32, since groupshared uint used to synchronize counts is allocated as a UINT32
            m_RayCountTex = RTHandles.Alloc(Vector2.one, filterMode: FilterMode.Point, colorFormat: GraphicsFormat.R32G32B32A32_UInt, enableRandomWrite: true, useMipMap: false, name: "RayCountTex");
            s_TotalRayCountBuffer = new ComputeBuffer(k_NumRaySources + 1, sizeof(uint));
            m_DebugDisplaySettings = currentDebugDisplaySettings;
        }

        public void Release()
        {
            CoreUtils.Destroy(m_Blit);
            CoreUtils.Destroy(m_DrawRayCount);

            RTHandles.Release(m_RayCountTex);
            RTHandles.Release(m_TotalRayCountTex);
            CoreUtils.SafeRelease(s_TotalRayCountBuffer);
        }

        public RTHandleSystem.RTHandle rayCountTex
        {
            get
            {
                return m_RayCountTex;
            }
        }

        public int rayCountEnabled
        {
            get
            {
                return m_DebugDisplaySettings.data.countRays ? 1 : 0;
            }
        }

        public float GetRaysPerFrame()
        {
            if (!m_DebugDisplaySettings.data.countRays)
                return 0.0f;
            else
            {
                float latestSample = 0;
                // Get the latest readback that finished
                for (int i = 0; i < rayCountReadbacks.Count; i++)
                {
                    if (rayCountReadbacks[i].rayCountReadback.hasError == false)
                    {
                        if (rayCountReadbacks[i].rayCountReadback.done)
                        {
                            NativeArray<uint> sampleCount = rayCountReadbacks[i].rayCountReadback.GetData<uint>();

                            if (sampleCount.Length > 0 && sampleCount[0] != 0)
                            {
                                latestSample = sampleCount[0] / rayCountReadbacks[i].deltaTime;
                                rayCountReadbacks[i].SetRetired();
                            }
                        }
                        else
                        {
                            rayCountReadbacks[i].rayCountReadback.Update();
                        }
                    }
                    else
                    {
                        rayCountReadbacks[i].SetRetired();
                    }
                }

                rayCountReadbacks.RemoveAll(ReadbackIsRetired);

                return latestSample;
            }
        }


        public void ClearRayCount(CommandBuffer cmd, HDCamera camera)
        {
            if (m_DebugDisplaySettings.data.countRays)
            {
                int clearBufferKernelIdx = m_RayCountCompute.FindKernel("CS_Clear");
                cmd.SetComputeBufferParam(m_RayCountCompute, clearBufferKernelIdx, _TotalRayCountBuffer, s_TotalRayCountBuffer);
                cmd.DispatchCompute(m_RayCountCompute, clearBufferKernelIdx, 1, 1, 1);

                HDUtils.SetRenderTarget(cmd, camera, m_RayCountTex, ClearFlag.Color);
            }
        }

        public void Update(CommandBuffer cmd, HDCamera camera)
        {
            ClearRayCount(cmd, camera);
        }

        public void RenderRayCount(CommandBuffer cmd, HDCamera camera, Color fontColor)
        {
            if (m_DebugDisplaySettings.data.countRays)
            {
                using (new ProfilingSample(cmd, "Raytracing Debug Overlay", CustomSamplerId.RaytracingDebug.GetSampler()))
                {
                    int width = camera.actualWidth;
                    int height = camera.actualHeight;

                    // Sum across all rays per pixel
                    int countKernelIdx = m_RayCountCompute.FindKernel("CS_CountRays");
                    uint groupSizeX = 0, groupSizeY = 0, groupSizeZ = 0;
                    m_RayCountCompute.GetKernelThreadGroupSizes(countKernelIdx, out groupSizeX, out groupSizeY, out groupSizeZ);
                    int dispatchWidth = 0, dispatchHeight = 0;
                    dispatchWidth = (int)((width + groupSizeX - 1) / groupSizeX);
                    dispatchHeight = (int)((height + groupSizeY - 1) / groupSizeY);
                    cmd.SetComputeTextureParam(m_RayCountCompute, countKernelIdx, HDShaderIDs._RayCountTexture, m_RayCountTex);
                    cmd.SetComputeBufferParam(m_RayCountCompute, countKernelIdx, _TotalRayCountBuffer, s_TotalRayCountBuffer);
                    cmd.DispatchCompute(m_RayCountCompute, countKernelIdx, dispatchWidth, dispatchHeight, 1);

                    // Read back from GPU
                    // This is necessarily out of sync, but the hope is that over enough samples, it'll average
                    // out to something close to what we want anyways. 
                    RayCountReadback singleReadBack;
                    singleReadBack.rayCountBuffer = s_TotalRayCountBuffer;
                    singleReadBack.rayCountReadback = AsyncGPUReadback.Request(s_TotalRayCountBuffer, sizeof(uint), k_NumRaySources * sizeof(uint));
                    singleReadBack.retired = false;
                    singleReadBack.deltaTime = Time.smoothDeltaTime;

                    rayCountReadbacks.Add(singleReadBack);

                    // Draw overlay
                    if (m_DebugDisplaySettings.data.showRaysPerFrame)
                    {
                        m_DrawRayCountProperties.SetTexture(HDShaderIDs._DebugFont, s_DebugFontTex);
                        m_DrawRayCountProperties.SetColor(_FontColor, fontColor);
                        m_DrawRayCount.SetBuffer(_TotalRayCountBuffer, s_TotalRayCountBuffer);
                        CoreUtils.DrawFullScreen(cmd, m_DrawRayCount, m_DrawRayCountProperties);
                    }
                }
            }
        }
    }
#endif
}
