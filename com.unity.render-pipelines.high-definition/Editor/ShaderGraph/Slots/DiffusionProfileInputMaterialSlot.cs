using System;
using System.Collections.Generic;
using UnityEditor.Experimental.Rendering.HDPipeline.Drawing.Slots;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEngine.Experimental.Rendering.HDPipeline;

namespace UnityEditor.Experimental.Rendering.HDPipeline
{
    [Serializable]
    [FormerName("UnityEditor.ShaderGraph.DiffusionProfileInputMaterialSlot")]
    class DiffusionProfileInputMaterialSlot : Vector1MaterialSlot
    {
        [SerializeField]
        DiffusionProfileSettings m_DiffusionProfile;

        public DiffusionProfileSettings diffusionProfile
        {
            get { return m_DiffusionProfile; }
            set { m_DiffusionProfile = value; }
        }

        public DiffusionProfileInputMaterialSlot()
        {
        }

        public DiffusionProfileInputMaterialSlot(int slotId, string displayName, string shaderOutputName,
                                          ShaderStageCapability stageCapability = ShaderStageCapability.All, bool hidden = false)
            : base(slotId, displayName, shaderOutputName, SlotType.Input, 0.0f, stageCapability, hidden: hidden)
        {
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
            if (m_DiffusionProfile == null)
                return "_DiffusionProfileHash";
            else
                return "((asuint(_DiffusionProfileHash) != 0) ? _DiffusionProfileHash : asfloat(uint(" + m_DiffusionProfile.profiles[0].hash + ")))";
        }

        public override void CopyValuesFrom(MaterialSlot foundSlot)
        {
            var slot = foundSlot as DiffusionProfileInputMaterialSlot;

            if (slot != null)
            {
                m_DiffusionProfile = slot.m_DiffusionProfile;
            }
        }
    }
}
