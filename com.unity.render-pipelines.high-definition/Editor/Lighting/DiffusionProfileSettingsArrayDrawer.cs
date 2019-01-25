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
        ReorderableList         m_DiffusionProfileList;

        public override bool OnGUI(SerializedDataParameter parameter, GUIContent title)
        {
            if (parameter.value.propertyType != SerializedPropertyType.Generic)
                return false;

            if (m_DiffusionProfileList == null)
                CreateReorderableList(parameter);

            EditorGUILayout.BeginVertical();
            m_DiffusionProfileList.DoLayoutList();
            EditorGUILayout.EndVertical();

            return true;
        }

        void CreateReorderableList(SerializedDataParameter parameter)
        {
            m_DiffusionProfileList = new ReorderableList(parameter.value.serializedObject, parameter.value);

            m_DiffusionProfileList.drawHeaderCallback = (rect) => {
                EditorGUI.LabelField(rect, "Diffusion Profile List");
            };

            m_DiffusionProfileList.drawElementCallback = (rect, index, active, focused) => {
                rect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.ObjectField(rect, parameter.value.GetArrayElementAtIndex(index), new GUIContent("Profile " + index));
            };
        }
    }
}
