using UnityEngine;
using UnityEditor;
using UnityEngine.Experimental.Rendering.HDPipeline;
using System.Collections.Generic;

namespace UnityEditor.Experimental.Rendering.HDPipeline
{
    [InitializeOnLoad]
    public class DiffusionProfileHashTable
    {
        static List< uint >     diffusionProfileHashes = new List< uint >();

        static Queue< string >  assetsToGenerateHashes = new Queue< string >();

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

                Debug.Log("load hash: " + profile.profiles[0].hash);
                diffusionProfileHashes.Add(profile.profiles[0].hash);
            }

            EditorApplication.update += GenerateHashes;
        }

        static void GenerateHashes()
        {
            while (assetsToGenerateHashes.Count != 0)
            {
                string assetName = assetsToGenerateHashes.Dequeue();
                var diffusionProfile = AssetDatabase.LoadAssetAtPath<DiffusionProfileSettings>(assetName);
                
                if (diffusionProfile == null)
                    return ;

                uint hash = (uint)AssetDatabase.AssetPathToGUID(assetName).GetHashCode();
                diffusionProfile.profiles[0].hash = GetCollisionLessHash(hash);
                Debug.Log("Created a new Diffusion profile, updating the hash: " + diffusionProfile.profiles[0].hash);
            }
        }

        public static void  GenerateUniqueHash(string assetName)
        {
            // Enqueue the asset to process it the next editor frame, we have to wait
            // as the asset does not exists yet
            assetsToGenerateHashes.Enqueue(assetName);
        }

        static uint GetCollisionLessHash(uint hash)
        {
            while (diffusionProfileHashes.Contains(hash))
                hash++;
            return hash;
        }
    }

    public class DiffusionProfileModificationProcessor : AssetModificationProcessor
    {
        // WARNING: this callback is not called when an asset is duplicated
        static void OnWillCreateAsset(string assetName)
        {
            if (!assetName.EndsWith(".asset"))
                return ;
            
            DiffusionProfileHashTable.GenerateUniqueHash(assetName);
        }

        private static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath)
        {
            Debug.Log("Source path: " + sourcePath + ". Destination path: " + destinationPath + ".");
            AssetMoveResult assetMoveResult = AssetMoveResult.DidMove;

            // Perform operations on the asset and set the value of 'assetMoveResult' accordingly.

            return assetMoveResult;
        }
    }
}