using System;
using UnityEngine.Rendering;

namespace UnityEngine.Experimental.Rendering.HDPipeline
{
    [Serializable, VolumeComponentMenu("Diffusion Profile")]
    public sealed class DiffusionProfileVolume : VolumeComponent
    {
        [Tooltip("Diffusion profile test.")]
        public DiffusionProfileSettingsParameter diffusionProfiles = new DiffusionProfileSettingsParameter(default(DiffusionProfileSettings[]));
    }

    [Serializable]
    public sealed class DiffusionProfileSettingsParameter : VolumeParameter<DiffusionProfileSettings[]>
    {
        public DiffusionProfileSettingsParameter(DiffusionProfileSettings[] value, bool overrideState = false)
            : base(value, overrideState) { }
    }
}
