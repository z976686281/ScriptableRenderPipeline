using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering.HDPipeline;
using UnityEditor.Rendering;
using UnityEditorInternal;

namespace UnityEditor.Experimental.Rendering.HDPipeline
{
    public class DiffusionProfileSettingsListUI
    {
        ReorderableList         m_DiffusionProfileList;
        SerializedProperty      m_Property;
        string                  m_ListName;

        const string            k_DefaultListName = "Diffusion Profile List";

        public DiffusionProfileSettingsListUI(string listName = k_DefaultListName)
        {
            m_ListName = listName;
        }

        public void OnGUI(SerializedProperty parameter)
        {
            if (m_DiffusionProfileList == null || m_Property != parameter)
                CreateReorderableList(parameter);

            EditorGUILayout.BeginVertical();
            m_DiffusionProfileList.DoLayoutList();
            EditorGUILayout.EndVertical();
        }

        void CreateReorderableList(SerializedProperty parameter)
        {
            m_Property = parameter;
            m_DiffusionProfileList = new ReorderableList(parameter.serializedObject, parameter);

            m_DiffusionProfileList.drawHeaderCallback = (rect) => {
                EditorGUI.LabelField(rect, m_ListName);
            };

            m_DiffusionProfileList.drawElementCallback = (rect, index, active, focused) => {
                rect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.ObjectField(rect, parameter.GetArrayElementAtIndex(index), new GUIContent("Profile " + index));
            };
        }
    }
}
