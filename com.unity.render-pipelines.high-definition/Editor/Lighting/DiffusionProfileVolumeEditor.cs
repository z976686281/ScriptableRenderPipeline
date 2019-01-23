using UnityEditor.Rendering;
using UnityEngine.Experimental.Rendering.HDPipeline;

namespace UnityEditor.Experimental.Rendering.HDPipeline
{
    [VolumeComponentEditor(typeof(DiffusionProfileVolume))]
    sealed class DiffusionProfileVolumeEditor : VolumeComponentEditor
    {
        SerializedDataParameter m_DiffusionProfile0;
        SerializedDataParameter m_DiffusionProfile1;
        SerializedDataParameter m_DiffusionProfile2;
        SerializedDataParameter m_DiffusionProfile3;
        SerializedDataParameter m_DiffusionProfile4;
        SerializedDataParameter m_DiffusionProfile5;
        SerializedDataParameter m_DiffusionProfile6;
        SerializedDataParameter m_DiffusionProfile7;

        public override void OnEnable()
        {
            var o = new PropertyFetcher<DiffusionProfileVolume>(serializedObject);

            m_DiffusionProfile0 = Unpack(o.Find(x => x.diffusionProfile0));
            m_DiffusionProfile1 = Unpack(o.Find(x => x.diffusionProfile1));
            m_DiffusionProfile2 = Unpack(o.Find(x => x.diffusionProfile2));
            m_DiffusionProfile3 = Unpack(o.Find(x => x.diffusionProfile3));
            m_DiffusionProfile4 = Unpack(o.Find(x => x.diffusionProfile4));
            m_DiffusionProfile5 = Unpack(o.Find(x => x.diffusionProfile5));
            m_DiffusionProfile6 = Unpack(o.Find(x => x.diffusionProfile6));
            m_DiffusionProfile7 = Unpack(o.Find(x => x.diffusionProfile7));
        }

        public override void OnInspectorGUI()
        {
            // EditorGUILayout.LabelField("Bloom", EditorStyles.miniLabel);
            PropertyField(m_DiffusionProfile0);
            PropertyField(m_DiffusionProfile1);
            PropertyField(m_DiffusionProfile2);
            PropertyField(m_DiffusionProfile3);
            PropertyField(m_DiffusionProfile4);
            PropertyField(m_DiffusionProfile5);
            PropertyField(m_DiffusionProfile6);
            PropertyField(m_DiffusionProfile7);
            // PropertyField(m_Scatter);
            // PropertyField(m_Tint);

            // EditorGUILayout.LabelField("Lens Dirt", EditorStyles.miniLabel);
            // PropertyField(m_DirtTexture, EditorGUIUtility.TrTextContent("Texture"));
            // PropertyField(m_DirtIntensity, EditorGUIUtility.TrTextContent("Intensity"));

            // if (isInAdvancedMode)
            // {
            //     EditorGUILayout.LabelField("Advanced Tweaks", EditorStyles.miniLabel);
            //     PropertyField(m_Resolution);
            //     PropertyField(m_HighQualityFiltering);
            //     PropertyField(m_Prefilter);
            //     PropertyField(m_Anamorphic);
            // }
        }
    }
}
