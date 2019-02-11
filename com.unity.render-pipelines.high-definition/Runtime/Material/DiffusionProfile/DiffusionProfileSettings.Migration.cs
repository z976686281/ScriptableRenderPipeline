using System;
using System.IO;
using UnityEngine.Rendering;

namespace UnityEngine.Experimental.Rendering.HDPipeline
{
    public sealed partial class DiffusionProfileSettings : IVersionable<DiffusionProfileSettings.Version>
    {
        enum Version
        {
            Initial,                // 16 profiles per asset
            DiffusionProfileRework, // one profile per asset
        }
        
        [Obsolete("Profiles are obsolete, only one diffusion profile per asset is allowed.")]
        public DiffusionProfile this[int index]
        {
            get => profile;
        }

        [SerializeField]
        Version m_Version;
        Version IVersionable<Version>.version { get => m_Version; set => m_Version = value; }

        [Obsolete("Profiles are obsolete, only one diffusion profile per asset is allowed.")]
        public DiffusionProfile[] profiles;

        static readonly MigrationDescription<Version, DiffusionProfileSettings> k_Migration = MigrationDescription.New(
            MigrationStep.New(Version.DiffusionProfileRework, (DiffusionProfileSettings d) =>
            {
#pragma warning disable 618
                if (d.profiles == null)
                    return;
                
                var hdAsset = GraphicsSettings.renderPipelineAsset as HDRenderPipelineAsset;
                if (hdAsset == null)
                    throw new Exception("Can't upgrade diffusion profile when the HDRenderPipeline asset is not assigned in Graphic Settings");

                var defaultProfile = new DiffusionProfile("");

                // Iterate over the diffusion profile settings and generate one new asset for each
                // diffusion profile which have been modified
                int count = 0;
                foreach (var profile in d.profiles)
                {
                    if (!profile.Equals(defaultProfile))
                        CreateNewDiffusionProfile(d, profile, count++);
                }
#if UNITY_EDITOR
                // If the diffusion profile settings we're upgrading was assigned to the HDAsset in use
                // Then we need to go over all materials and upgrade them
                if (hdAsset.diffusionProfileSettings == d)
                {
                    Debug.Log("TODO: upgrade all materials");
                }
#endif
#pragma warning restore 618
            })
        );

        static void CreateNewDiffusionProfile(DiffusionProfileSettings asset, DiffusionProfile profile, int count)
        {
            if (count == 0)
            {
                asset.profile = profile;
                return;
            }

#if UNITY_EDITOR
            var newDiffusionProfile = ScriptableObject.CreateInstance<DiffusionProfileSettings>();
            newDiffusionProfile.name = asset.name;
            newDiffusionProfile.profile = profile;
            profile.Validate();
            newDiffusionProfile.UpdateCache();

            var path = UnityEditor.AssetDatabase.GetAssetPath(asset);
            path = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(path);
            UnityEditor.AssetDatabase.CreateAsset(newDiffusionProfile, path);
#endif
        }
    }
}
