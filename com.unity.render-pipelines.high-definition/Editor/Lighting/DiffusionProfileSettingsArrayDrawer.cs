using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering.HDPipeline;
using UnityEditor.Rendering;
using UnityEditorInternal;

namespace UnityEditor.Experimental.Rendering.HDPipeline
{
    [VolumeParameterDrawer(typeof(DiffusionProfileSettingsParameter))]
    sealed class DiffusionProfileSettingsArrayDrawer : VolumeParameterDrawer
    {
        DiffusionProfileSettingsListUI      listUI = new DiffusionProfileSettingsListUI();

        static GUIContent m_DiffusionProfileLabel = new GUIContent("Diffusion Profile List", "Diffusion Profile List from current HDRenderPipeline Asset");

        public override bool OnGUI(SerializedDataParameter parameter, GUIContent title)
        {
            if (parameter.value.propertyType != SerializedPropertyType.Generic)
                return false;

            var a = GraphicsSettings.renderPipelineAsset as HDRenderPipelineAsset;

            // If the parameter is not overwritten, display the list inside HDRenderPipeline asset
            if (!parameter.overrideState.boolValue)
            {
                EditorGUI.BeginDisabledGroup(false);
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField(m_DiffusionProfileLabel);
                int i = 0;
                foreach (var profile in a.diffusionProfileSettingsList)
                {
                    EditorGUILayout.ObjectField("Profile " + i, profile, typeof(DiffusionProfileSettings), false);
                    i++;
                }
                EditorGUILayout.EndVertical();
                EditorGUI.EndDisabledGroup();
            }
            else
                listUI.OnGUI(parameter.value);

            return true;
        }
    }
}
