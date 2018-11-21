using System;
using UnityEngine.Rendering;

namespace UnityEngine.Experimental.Rendering.HDPipeline
{
    // TODO: Tooltips
    // TODO: Move these settings to the camera component? No need for them in volumes
    [Serializable]
    public sealed class PhysicalCamera : VolumeComponent, IPostProcessComponent
    {
        [Header("Camera Body")]
        public MinIntParameter iso = new MinIntParameter(200, 1);
        public MinFloatParameter shutterSpeed = new MinFloatParameter(1f / 200f, 0f);

        [Header("Lens")]
        public ClampedFloatParameter aperture = new ClampedFloatParameter(16f, 1f, 32f);
        public MinFloatParameter focalLength = new MinFloatParameter(50f, 1f);

        [Header("Aperture Shape")]
        public ClampedIntParameter bladeCount = new ClampedIntParameter(5, 3, 11);
        public FloatRangeParameter curvature = new FloatRangeParameter(new Vector2(2f, 11f), 1f, 32f);
        public ClampedFloatParameter barrelClipping = new ClampedFloatParameter(0.25f, 0f, 1f);
        public ClampedFloatParameter anamorphism = new ClampedFloatParameter(0f, -1f, 1f);

        public bool IsActive()
        {
            return true;
        }
    }
}
