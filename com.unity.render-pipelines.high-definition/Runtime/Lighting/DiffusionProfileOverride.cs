using System;
using UnityEngine.Rendering;

namespace UnityEngine.Experimental.Rendering.HDPipeline
{
    [Serializable, VolumeComponentMenu("Diffusion Profile Override")]
    public sealed class DiffusionProfileOverride : VolumeComponent
    {
        public BoolArrayParameter overrideStates = new BoolArrayParameter(new bool[16]); // TODO: constant
        [Tooltip("List of diffusion profiles used inside the volume.")]
        public DiffusionProfileSettingsParameter diffusionProfiles = new DiffusionProfileSettingsParameter(default(DiffusionProfileSettings[]));

        [NonSerialized]
        DiffusionProfileSettings[] mergedDiffusionProfiles = new DiffusionProfileSettings[16]; // TODO: constan

        public DiffusionProfileSettings[]   GetMergedDiffusionProfileSettings()
        {
            var hdAsset = GraphicsSettings.renderPipelineAsset as HDRenderPipelineAsset;

            if (diffusionProfiles.value == null)
                return hdAsset.diffusionProfileSettingsList;

            for (int i = 0; i < 16; i++)
            {
                mergedDiffusionProfiles[i] = null;

                if (i < diffusionProfiles.value.Length)
                {
                    if (overrideStates.value[i] && diffusionProfiles.value[i] != null)
                        mergedDiffusionProfiles[i] = diffusionProfiles.value[i];
                }
                if (i < hdAsset.diffusionProfileSettingsList.Length)
                {
                    if (hdAsset.diffusionProfileSettingsList[i] != null && mergedDiffusionProfiles[i] == null)
                        mergedDiffusionProfiles[i] = hdAsset.diffusionProfileSettingsList[i];
                }
            }

            return mergedDiffusionProfiles;
        }
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
