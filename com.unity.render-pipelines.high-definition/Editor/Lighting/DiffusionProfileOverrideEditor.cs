using UnityEditor.Rendering;
using UnityEngine.Experimental.Rendering.HDPipeline;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor.Experimental.Rendering.HDPipeline
{
    [VolumeComponentEditor(typeof(DiffusionProfileOverride))]
    sealed class DiffusionProfileOverrideEditor : VolumeComponentEditor
    {
        SerializedDataParameter m_DiffusionProfiles;

        public override void OnEnable()
        {
            var o = new PropertyFetcher<DiffusionProfileOverride>(serializedObject);

            m_DiffusionProfiles = Unpack(o.Find(x => x.diffusionProfiles));
        }

        public override void OnInspectorGUI()
        {
            PropertyField(m_DiffusionProfiles);
        }
    }
}
