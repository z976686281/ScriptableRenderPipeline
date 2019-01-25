using UnityEngine;
using UnityEditor;
using UnityEngine.Experimental.Rendering.HDPipeline;
using System.Collections.Generic;

namespace UnityEditor.Experimental.Rendering.HDPipeline
{
    public class DiffusionProfileModificationProcessor : AssetModificationProcessor
    {
        // WARNING: this callback is not called when an asset is duplicated
        static void OnWillCreateAsset(string assetName)
        {
            if (!assetName.EndsWith(".asset"))
                return ;
            
            // DiffusionProfileHashTable.GenerateUniqueHash(assetName);
        }

        private static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath)
        {
            Debug.Log("Source path: " + sourcePath + ". Destination path: " + destinationPath + ".");
            AssetMoveResult assetMoveResult = AssetMoveResult.DidMove;

            // Perform operations on the asset and set the value of 'assetMoveResult' accordingly.

            return assetMoveResult;
        }

        private static string[] OnWillSaveAssets(string[] paths)
        {
            foreach (var s in paths)
                Debug.Log("Move: " + s);
            
            return paths;
        }
    }
}