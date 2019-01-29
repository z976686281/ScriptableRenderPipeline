#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Experimental.Rendering.HDPipeline;
using System.Collections.Generic;
using UnityEditor;

namespace UnityEditor.Experimental.Rendering.HDPipeline
{
    // This class keep track of every diffusion profile in the project so it can generate unique uint hashes
    // for every asset, which are used to differentiate diffusion profiles in the shader
    [InitializeOnLoad]
    public class DiffusionProfileHashTable
    {
        static HashSet<uint>                    diffusionProfileHashes = new HashSet< uint >();
        static Queue<DiffusionProfileSettings>  diffusionProfileToUpdate = new Queue<DiffusionProfileSettings>();

        // Called at each domain reload to build a list of all diffusion profile hashes so we can check
        // for collisions when we create the hash for a new asset
        static DiffusionProfileHashTable()
        {
            EditorApplication.update += UpdateDiffusionProfileHashes;
        }

        static uint GetDiffusionProfileHash(DiffusionProfileSettings asset)
        {
            return (uint)AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(asset)).GetHashCode();
        }

        static uint GenerateUniqueHash(DiffusionProfileSettings asset)
        {
            uint hash = GetDiffusionProfileHash(asset);
            Debug.Log("Generating new hash for asset " + asset);
            return GetCollisionLessHash(hash);
        }

        static void UpdateDiffusionProfileHashes()
        {
            while (diffusionProfileToUpdate.Count != 0)
            {
                var profile = diffusionProfileToUpdate.Dequeue();
                uint hash = profile.profiles[0].hash;

                // If the hash is 0, then we need to generate a new one (it means that the profile was just created)
                if (hash == 0)
                {
                    Debug.Log("Empty hash in asset: " + profile);
                    profile.profiles[0].hash = GenerateUniqueHash(profile);
                    EditorUtility.SetDirty(profile);
                }
                // If the hash is already in the list, it means that it was duplicated
                else if (diffusionProfileHashes.Contains(hash))
                {
                    foreach (var h in diffusionProfileHashes)
                        Debug.Log("h: " + h);
                    profile.profiles[0].hash = GenerateUniqueHash(profile);
                    EditorUtility.SetDirty(profile);
                }
                
                // otherwise, no issue, we don't change the hash
            }
        }

        public static void UpdateUniqueHash(DiffusionProfileSettings asset)
        {
            diffusionProfileHashes.Add(asset.profiles[0].hash);
            // Defere the generation of the hash because we can't call AssetDatabase functions outside of editor scope
            diffusionProfileToUpdate.Enqueue(asset);
        }

        static uint GetCollisionLessHash(uint hash)
        {
            while (diffusionProfileHashes.Contains(hash))
            {
                Debug.Log("Collision found !!!!, generating a new hash");
                hash++;
            }
            return hash;
        }
    }
}
#endif