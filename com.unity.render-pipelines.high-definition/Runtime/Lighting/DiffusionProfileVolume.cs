using System;
using UnityEngine.Rendering;

namespace UnityEngine.Experimental.Rendering.HDPipeline
{
    [Serializable, VolumeComponentMenu("Diffusion Profile")]
    public sealed class DiffusionProfileVolume : VolumeComponent
    {
        [Tooltip("Diffusion profile test.")]
        public DiffusionProfileSettingsParameter diffusionProfile0 = new DiffusionProfileSettingsParameter(null);
        public DiffusionProfileSettingsParameter diffusionProfile1 = new DiffusionProfileSettingsParameter(null);
        public DiffusionProfileSettingsParameter diffusionProfile2 = new DiffusionProfileSettingsParameter(null);
        public DiffusionProfileSettingsParameter diffusionProfile3 = new DiffusionProfileSettingsParameter(null);
        public DiffusionProfileSettingsParameter diffusionProfile4 = new DiffusionProfileSettingsParameter(null);
        public DiffusionProfileSettingsParameter diffusionProfile5 = new DiffusionProfileSettingsParameter(null);
        public DiffusionProfileSettingsParameter diffusionProfile6 = new DiffusionProfileSettingsParameter(null);
        public DiffusionProfileSettingsParameter diffusionProfile7 = new DiffusionProfileSettingsParameter(null);
    }

    [Serializable]
    public sealed class DiffusionProfileSettingsParameter : VolumeParameter<DiffusionProfileSettings>
    {
        public DiffusionProfileSettingsParameter(DiffusionProfileSettings value, bool overrideState = false)
            : base(value, overrideState) { }
    }
}
