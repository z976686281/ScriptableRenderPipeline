#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Experimental.Rendering.HDPipeline;
using System.Collections.Generic;
using UnityEditor;

namespace UnityEditor.Experimental.Rendering.HDPipeline
{
    [InitializeOnLoad]
    public class DiffusionProfileHashTable
    {
        static HashSet<uint>                    diffusionProfileHashes = new HashSet< uint >();
        static Queue<DiffusionProfileSettings>  diffusionProfileToUpdate = new Queue<DiffusionProfileSettings>();

        // Called at each domain reload to build a list of all diffusion profile hashes so we can check
        // for collisions when we create the hash for a new asset
        static DiffusionProfileHashTable()
        {
            var profileGUIDs = AssetDatabase.FindAssets("t:" + typeof(DiffusionProfileSettings));
            
            foreach (var profileGUID in profileGUIDs)
            {
                string profilePath = AssetDatabase.GUIDToAssetPath(profileGUID);
                var profile = AssetDatabase.LoadAssetAtPath<DiffusionProfileSettings>(profilePath);

                if (profile == null)
                    continue;

                Debug.Log("load hash: " + (float)(profile.profiles[0].hash));
                diffusionProfileHashes.Add(profile.profiles[0].hash);
            }

            EditorApplication.update += UpdateDiffusionProfileHashes;
        }

        static uint GenerateUniqueHash(DiffusionProfileSettings asset)
        {
            uint hash = (uint)AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(asset)).GetHashCode();
            Debug.Log("Generating new hash: " + hash);
            return GetCollisionLessHash(hash);
        }

        static void UpdateDiffusionProfileHashes()
        {
            while (diffusionProfileToUpdate.Count != 0)
            {
                var profile = diffusionProfileToUpdate.Dequeue();
                uint hash = profile.profiles[0].hash;
                Debug.Log("profile: " + profile + ", hash: " + hash);

                // If the hash is 0, then we need to generate a new one (it means that the profile was just created)
                if (hash == 0)
                    profile.profiles[0].hash = GenerateUniqueHash(profile);
                // If the hash is already in the list, it means that it was duplicated
                else if (diffusionProfileHashes.Contains(hash))
                    profile.profiles[0].hash = GenerateUniqueHash(profile);
                
                // otherwise, no issue, we don't change the hash
            }
        }

        public static void UpdateUniqueHash(DiffusionProfileSettings asset)
        {
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