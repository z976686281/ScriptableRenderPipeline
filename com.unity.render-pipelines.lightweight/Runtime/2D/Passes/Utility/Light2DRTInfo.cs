using System;
using UnityEngine;


namespace UnityEngine.Experimental.Rendering.LightweightPipeline
{
    [Serializable]
    public class Light2DRTInfo
    {
        public enum BlendMode
        {
            Additive,
            Modulate,
            Modulate2X,
            Substractive,
            Custom
        }

        const int k_DefaultPixelWidth = 1;
        const int k_DefaultPixelHeight = 1;

        public int m_PixelWidth;
        public int m_PixelHeight;
        public FilterMode m_FilterMode;
        public BlendMode m_BlendMode;
        public Vector2 m_CustomBlendFactors;

        public Vector2 blendFactors
        {
            get
            {
                switch(m_BlendMode)
                {
                    case BlendMode.Additive:
                        return new Vector2(0.0f, 1.0f);
                    case BlendMode.Substractive:
                        return new Vector2(0.0f, -1.0f);
                    case BlendMode.Modulate:
                        return new Vector2(1.0f, 0.0f);
                    case BlendMode.Modulate2X:
                        return new Vector2(2.0f, 0.0f);
                    case BlendMode.Custom:
                        return m_CustomBlendFactors;
                    default:
                        return Vector2.zero;
                }
            }
        }

        public RenderTexture GetRenderTexture(RenderTextureFormat format)
        {
            int width = m_PixelWidth > 0 ? m_PixelWidth : k_DefaultPixelWidth;
            int height = m_PixelHeight > 0 ? m_PixelHeight : k_DefaultPixelHeight;

            RenderTextureDescriptor renderTextureDescriptor = new RenderTextureDescriptor(width, height, format);
            renderTextureDescriptor.sRGB = false;
            renderTextureDescriptor.useMipMap = false;
            renderTextureDescriptor.autoGenerateMips = false;
            renderTextureDescriptor.depthBufferBits = 0;

            RenderTexture retTexture = RenderTexture.GetTemporary(renderTextureDescriptor);
            retTexture.wrapMode = TextureWrapMode.Clamp;
            retTexture.filterMode = m_FilterMode;

            retTexture.DiscardContents(true, true);

            return retTexture;
        }

        public Light2DRTInfo(int pixelWidth, int pixelHeight, FilterMode filterMode)
        {
            m_PixelWidth = pixelWidth;
            m_PixelHeight = pixelHeight;
            m_FilterMode = filterMode;
        }
    }
}
