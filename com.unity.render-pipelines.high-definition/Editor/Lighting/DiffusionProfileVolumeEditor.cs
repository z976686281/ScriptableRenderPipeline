using UnityEditor.Rendering;
using UnityEngine.Experimental.Rendering.HDPipeline;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor.Experimental.Rendering.HDPipeline
{
    [VolumeComponentEditor(typeof(DiffusionProfileVolume))]
    sealed class DiffusionProfileVolumeEditor : VolumeComponentEditor
    {
        SerializedDataParameter m_DiffusionProfiles;
        ReorderableList         m_DiffusionProfileList;

        public override void OnEnable()
        {
            var o = new PropertyFetcher<DiffusionProfileVolume>(serializedObject);

            m_DiffusionProfiles = Unpack(o.Find(x => x.diffusionProfiles));

            m_DiffusionProfileList = new ReorderableList(m_DiffusionProfiles.value.serializedObject, m_DiffusionProfiles.value);

            m_DiffusionProfileList.drawHeaderCallback = (rect) => {
                EditorGUI.LabelField(rect, "Diffusion Profile List");
            };

            m_DiffusionProfileList.drawElementCallback = (rect, index, active, focused) => {
                rect.height = EditorGUIUtility.singleLineHeight;
                // EditorGUI.BeginChangeCheck(); // not needed
                EditorGUI.ObjectField(rect, m_DiffusionProfiles.value.GetArrayElementAtIndex(index), new GUIContent("Profile " + index));
                // if (EditorGUI.EndChangeCheck())
                    // m_DiffusionProfiles.value.serializedObject.ApplyModifiedProperties();
            };
        }

        public override void OnInspectorGUI()
        {
            m_DiffusionProfileList.DoLayoutList();
            // EditorGUILayout.LabelField("Bloom", EditorStyles.miniLabel);
            // PropertyField(m_Scatter);
            // PropertyField(m_Tint);

            // EditorGUILayout.LabelField("Lens Dirt", EditorStyles.miniLabel);
            // PropertyField(m_DirtTexture, EditorGUIUtility.TrTextContent("Texture"));
            // PropertyField(m_DirtIntensity, EditorGUIUtility.TrTextContent("Intensity"));

            // if (isInAdvancedMode)
            // {
            //     EditorGUILayout.LabelField("Advanced Tweaks", EditorStyles.miniLabel);
                PropertyField(m_DiffusionProfiles);
            //     PropertyField(m_HighQualityFiltering);
            //     PropertyField(m_Prefilter);
            //     PropertyField(m_Anamorphic);
            // }
        }
    }
}
