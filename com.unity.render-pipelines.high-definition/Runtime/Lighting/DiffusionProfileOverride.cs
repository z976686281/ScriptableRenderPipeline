using System;
using UnityEngine.Rendering;

namespace UnityEngine.Experimental.Rendering.HDPipeline
{
    [Serializable, VolumeComponentMenu("Diffusion Profile Override")]
    public sealed class DiffusionProfileOverride : VolumeComponent
    {
        public BoolArrayParameter ovearrideStates = new BoolArrayParameter(new bool[16]); // TODO: constant
        [Tooltip("List of diffusion profiles used inside the volume.")]
        public DiffusionProfileSettingsParameter diffusionProfiles = new DiffusionProfileSettingsParameter(default(DiffusionProfileSettings[]));
    }

    [Serializable]
    public sealed class DiffusionProfileSettingsParameter : VolumeParameter<DiffusionProfileSettings[]>
    {
        public DiffusionProfileSettingsParameter(DiffusionProfileSettings[] value, bool overrideState = true)
            : base(value, overrideState) { }
    }

    [Serializable]
    public sealed class BoolArrayParameter : VolumeParameter<bool[]>
    {
        public BoolArrayParameter(bool[] value, bool overrideState = true) : base(value, overrideState) { }
    }
}
