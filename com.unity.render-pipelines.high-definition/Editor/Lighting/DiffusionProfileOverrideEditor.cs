using UnityEditor.Rendering;
using UnityEngine.Experimental.Rendering.HDPipeline;
using UnityEditorInternal;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using System.Linq;

namespace UnityEditor.Experimental.Rendering.HDPipeline
{
    [VolumeComponentEditor(typeof(DiffusionProfileOverride))]
    sealed class DiffusionProfileOverrideEditor : VolumeComponentEditor
    {
        SerializedDataParameter m_DiffusionProfiles;
        SerializedDataParameter m_OverrideStates;
        Volume                  m_Volume;
        DiffusionProfileSettings[]  m_DefaultDiffusionProfiles;

        DiffusionProfileSettingsListUI      listUI = new DiffusionProfileSettingsListUI();

        const int k_OverrideStateCheckboxWidth = 50;

        static GUIContent m_DiffusionProfileLabel = new GUIContent("Diffusion Profile List", "Diffusion Profile List from current HDRenderPipeline Asset");

        public override void OnEnable()
        {
            var o = new PropertyFetcher<DiffusionProfileOverride>(serializedObject);

            m_Volume = (m_Inspector.target as Volume);
            m_DiffusionProfiles = Unpack(o.Find(x => x.diffusionProfiles));
            m_OverrideStates = Unpack(o.Find(x => x.overrideStates));
            var hdAsset = GraphicsSettings.renderPipelineAsset as HDRenderPipelineAsset;
            m_DefaultDiffusionProfiles = hdAsset.diffusionProfileSettingsList;
        }

        public override void OnInspectorGUI()
        {
            listUI.drawElement = DrawDiffusionProfileElement;

            listUI.OnGUI(m_DiffusionProfiles.value);

            // If the volume is null it means that we're editing the component from the asset
            // So we can't access the bounds of the volume to fill diffusion profiles used in the volume
            if (m_Volume != null)
            {
                if (GUILayout.Button("Fill profiles with scene materials"))
                    FillProfileListWithScene();
            }
        }

        void DrawDiffusionProfileElement(SerializedProperty element, Rect rect, int index)
        {
            Rect overrideStateRect = rect;
            overrideStateRect.width = k_OverrideStateCheckboxWidth;
            rect.xMin += k_OverrideStateCheckboxWidth;
            var toggle = m_OverrideStates.value.GetArrayElementAtIndex(index);
            toggle.boolValue = EditorGUI.Toggle(overrideStateRect, toggle.boolValue);
            if (!toggle.boolValue)
            {
                DiffusionProfileSettings defaultValue = (index < m_DefaultDiffusionProfiles.Length) ? m_DefaultDiffusionProfiles[index] : null;
                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.ObjectField(rect, new GUIContent("Profile " + index), defaultValue, typeof(DiffusionProfileSettings), false);
                EditorGUI.EndDisabledGroup();
            }
            else
                EditorGUI.ObjectField(rect, element, new GUIContent("Profile " + index));
        }

        void FillProfileListWithScene()
        {
            var profiles = new HashSet<DiffusionProfileSettings>();
            if (m_Volume.isGlobal)
            {
                Debug.Log("TODO !");
            }
            else
            {
                var volumeCollider = m_Volume.GetComponent<Collider>();

                // Get all mesh renderers that are within the current volume
                var diffusionProfiles = new List<DiffusionProfileSettings>();
                foreach (var meshRenderer in Object.FindObjectsOfType<MeshRenderer>())
                {
                    var colliders = Physics.OverlapBox(meshRenderer.bounds.center, meshRenderer.bounds.size / 2);
                    if (colliders.Contains(volumeCollider))
                    {
                        foreach (var mat in meshRenderer.sharedMaterials)
                        {
                            var profile = GetMaterialDiffusionProfile(mat);

                            Debug.Log("Add profile: " + profile);
                            if (profile != null)
                                profiles.Add(profile);
                        }
                    }
                }
            }

            m_DiffusionProfiles.value.arraySize = profiles.Count;
            int i = 0;
            foreach (var profile in profiles)
            {
                m_DiffusionProfiles.value.GetArrayElementAtIndex(i).objectReferenceValue = profile;
                m_OverrideStates.value.GetArrayElementAtIndex(i).boolValue = true;
                i++;
            }
        }

        DiffusionProfileSettings GetMaterialDiffusionProfile(Material mat)
        {
            if (!mat.HasProperty(HDShaderIDs._MaterialID))
            {
                Debug.Log("Material " + mat + "has no MaterialID");
                return null;
            }
            
            var materialID = (MaterialId)mat.GetInt(HDShaderIDs._MaterialID);

            if (materialID != MaterialId.LitSSS && materialID != MaterialId.LitTranslucent)
                return null;

            string guid = HDEditorUtils.ConvertVector4ToGUID(mat.GetVector(HDShaderIDs._DiffusionProfileAsset));
            return AssetDatabase.LoadAssetAtPath<DiffusionProfileSettings>(AssetDatabase.GUIDToAssetPath(guid));
        }
    }
}
