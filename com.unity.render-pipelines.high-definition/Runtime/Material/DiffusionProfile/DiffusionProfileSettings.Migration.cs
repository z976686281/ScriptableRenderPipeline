using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
                int index = 0;
                var newProfiles = new Dictionary<int, DiffusionProfileSettings>();
                foreach (var profile in d.profiles)
                {
                    if (!profile.Equals(defaultProfile))
                        newProfiles[index] = (CreateNewDiffusionProfile(d, profile, index));
                    index++;
                }
#if UNITY_EDITOR
                // If the diffusion profile settings we're upgrading was assigned to the HDAsset in use
                // Then we need to go over all materials and upgrade them
                if (hdAsset.diffusionProfileSettings == d)
                {
                    Debug.Log("Upgrade all materials !");
                    var materialGUIDs = AssetDatabase.FindAssets("t:Material");
                    foreach (var guid in materialGUIDs)
                    {
                        var mat = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guid));
                        UpgradeMaterial(mat, newProfiles);
                    }
                }
#endif
#pragma warning restore 618
            })
        );

#if UNITY_EDITOR
        public static void UpgradeMaterial(Material mat, Dictionary<int, DiffusionProfileSettings> replacementProfiles)
        {
            // if the material don't have a diffusion profile
            if (!mat.HasProperty("_DiffusionProfile"))
                return;
            // or if it already have been upgraded
            if (mat.HasProperty("_DiffusionProfileAsset"))
                return;

            int index = mat.GetInt("_DiffusionProfile");
            if (!replacementProfiles.ContainsKey(index))
                return;

            var newProfile = replacementProfiles[index];
            string diffusionProfileGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(newProfile));
            mat.SetVector("_DiffusionProfileAsset", HDUtils.ConvertGUIDToVector4(diffusionProfileGUID));
            mat.SetFloat("_DiffusionProfileHash", HDShadowUtils.Asfloat(newProfile.profile.hash));
        }
#endif

        static DiffusionProfileSettings CreateNewDiffusionProfile(DiffusionProfileSettings asset, DiffusionProfile profile, int index)
        {
            if (index == 0)
            {
                asset.profile = profile;
                return null;
            }

#if UNITY_EDITOR
            var newDiffusionProfile = ScriptableObject.CreateInstance<DiffusionProfileSettings>();
            newDiffusionProfile.name = asset.name;
            newDiffusionProfile.profile = profile;
            profile.Validate();
            newDiffusionProfile.UpdateCache();

            var path = AssetDatabase.GetAssetPath(asset);
            path = AssetDatabase.GenerateUniqueAssetPath(path);
            AssetDatabase.CreateAsset(newDiffusionProfile, path);
#endif
            return newDiffusionProfile;
        }
    }
}
