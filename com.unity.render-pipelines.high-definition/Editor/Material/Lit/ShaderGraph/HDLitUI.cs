using UnityEngine;
using UnityEngine.Experimental.Rendering.HDPipeline;

namespace UnityEditor.Experimental.Rendering.HDPipeline
{
    public class HDLitGUI : ShaderGUI
    {
        // TODO: share code
        MaterialProperty    diffusionProfileAsset;
        MaterialProperty    diffusionProfileHash;

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            diffusionProfileAsset = FindProperty("_DiffusionProfileAsset", props);
            diffusionProfileHash = FindProperty("_DiffusionProfileHash", props);

            materialEditor.PropertiesDefaultGUI(props);
            if (materialEditor.EmissionEnabledProperty())
            {
                materialEditor.LightmapEmissionFlagsProperty(MaterialEditor.kMiniTextureFieldLabelIndentLevel, true, true);
            }

            string guid = HDEditorUtils.ConvertVector4ToGUID(diffusionProfileAsset.vectorValue);
            DiffusionProfileSettings diffusionProfile = AssetDatabase.LoadAssetAtPath<DiffusionProfileSettings>(AssetDatabase.GUIDToAssetPath(guid));

            // is it okay to do this every frame ?
            EditorGUI.BeginChangeCheck();
            diffusionProfile = (DiffusionProfileSettings)EditorGUILayout.ObjectField("TEST", diffusionProfile, typeof(DiffusionProfileSettings), false);
            if (EditorGUI.EndChangeCheck())
            {
                Vector4 newGuid = Vector4.zero;
                float    hash = 0;

                if (diffusionProfile != null)
                {
                    guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(diffusionProfile));
                    newGuid = HDEditorUtils.ConvertGUIDToVector4(guid);
                    hash = HDShadowUtils.Asfloat(diffusionProfile.profiles[0].hash);
                }

                // encode back GUID and it's hash
                diffusionProfileAsset.vectorValue = newGuid;
                Debug.Log("Update diffusion profile diffusionProfile hash from: " + diffusionProfileHash.floatValue + " to " + hash);
                diffusionProfileHash.floatValue = hash;
            }

            // Make sure all selected materials are initialized.
            string materialTag = "MotionVector";
            foreach (var obj in materialEditor.targets)
            {
                var material = (Material)obj;
                string tag = material.GetTag(materialTag, false, "Nothing");
                if (tag == "Nothing")
                {
                    material.SetShaderPassEnabled(HDShaderPassNames.s_MotionVectorsStr, false);
                    material.SetOverrideTag(materialTag, "User");
                }
            }

            {
                // If using multi-select, apply toggled material to all materials.
                bool enabled = ((Material)materialEditor.target).GetShaderPassEnabled(HDShaderPassNames.s_MotionVectorsStr);
                EditorGUI.BeginChangeCheck();
                enabled = EditorGUILayout.Toggle("Motion Vector For Vertex Animation", enabled);
                if (EditorGUI.EndChangeCheck())
                {
                    foreach (var obj in materialEditor.targets)
                    {
                        var material = (Material)obj;
                        material.SetShaderPassEnabled(HDShaderPassNames.s_MotionVectorsStr, enabled);
                    }
                }
            }
        }
    }
}
