using System;
using System.Collections.Generic;
using UnityEditor.Experimental.Rendering.HDPipeline.Drawing.Slots;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEngine.Experimental.Rendering.HDPipeline;
using UnityEngine.Rendering;

namespace UnityEditor.Experimental.Rendering.HDPipeline
{
    [Serializable]
    [FormerName("UnityEditor.ShaderGraph.DiffusionProfileInputMaterialSlot")]
    class DiffusionProfileInputMaterialSlot : Vector1MaterialSlot
    {
        [SerializeField, Obsolete("Use m_DiffusionProfileAsset instead.")]
        PopupList m_DiffusionProfile = new PopupList();

        [SerializeField]
        DiffusionProfileSettings m_DiffusionProfileAsset;

        public DiffusionProfileSettings diffusionProfile
        {
            get { return m_DiffusionProfileAsset; }
            set { m_DiffusionProfileAsset = value; }
        }

        public DiffusionProfileInputMaterialSlot()
        {
            // We can't upgrade here because we need to access the current render pipeline asset which is not
            // possible outside of unity context so we wait the next editor frame to do it
            EditorApplication.update += UpgradeIfNeeded;
        }

        public DiffusionProfileInputMaterialSlot(int slotId, string displayName, string shaderOutputName,
                                          ShaderStageCapability stageCapability = ShaderStageCapability.All, bool hidden = false)
            : base(slotId, displayName, shaderOutputName, SlotType.Input, 0.0f, stageCapability, hidden: hidden)
        {
            EditorApplication.update += UpgradeIfNeeded;
        }

        public override VisualElement InstantiateControl()
        {
            return new DiffusionProfileSlotControlView(this);
        }

        public override void AddDefaultProperty(PropertyCollector properties, GenerationMode generationMode)
        {
            var matOwner = owner as AbstractMaterialNode;
            if (matOwner == null)
                throw new Exception(string.Format("Slot {0} either has no owner, or the owner is not a {1}", this, typeof(AbstractMaterialNode)));

            // Note: Unity ShaderLab can't parse float values with exponential notation so we just can't
            // store the hash nor the asset GUID here :(
            var diffusionProfileHash = new Vector1ShaderProperty
            {
                overrideReferenceName = "_DiffusionProfileHash",
                hidden = true,
                value = 0
            };
            var diffusionProfileAsset = new Vector4ShaderProperty
            {
                overrideReferenceName = "_DiffusionProfileAsset",
                hidden = true,
                value = Vector4.zero
            };

            properties.AddShaderProperty(diffusionProfileHash);
            properties.AddShaderProperty(diffusionProfileAsset);
        }

        public override string GetDefaultValue(GenerationMode generationMode)
        {
            if (m_DiffusionProfileAsset == null)
                return "_DiffusionProfileHash";
            else
                return "((asuint(_DiffusionProfileHash) != 0) ? _DiffusionProfileHash : asfloat(uint(" + m_DiffusionProfileAsset.profile.hash + ")))";
        }

        public override void CopyValuesFrom(MaterialSlot foundSlot)
        {
            var slot = foundSlot as DiffusionProfileInputMaterialSlot;

            if (slot != null)
            {
                m_DiffusionProfileAsset = slot.m_DiffusionProfileAsset;
            }
        }

        void UpgradeIfNeeded()
        {
#pragma warning disable 618
            // Once the profile is upgraded, we set the selected entry to -1
            if (m_DiffusionProfile.selectedEntry != -1)
            {
                var hdAsset = GraphicsSettings.renderPipelineAsset as HDRenderPipelineAsset;
                diffusionProfile = hdAsset.diffusionProfileSettingsList[m_DiffusionProfile.selectedEntry];
                m_DiffusionProfile.selectedEntry = -1;
            }
#pragma warning restore 618
            EditorApplication.update -= UpgradeIfNeeded;
        }
    }
}
