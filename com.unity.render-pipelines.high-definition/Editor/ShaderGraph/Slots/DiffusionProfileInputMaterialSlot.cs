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

            string diffusionProfileGUID = "";
            if (diffusionProfile != null)
                diffusionProfileGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(diffusionProfile));
            
            // Note: Unity can't parse float values with exponential notation so we just can't
            // store the hash nor the asset GUID here :(
            var difffusionProfileHash = new Vector1ShaderProperty
            {
                overrideReferenceName = "_DiffusionProfileHash",
                // value = (diffusionProfile != null) ? diffusionProfile.profiles[0].hash : 0
                value = 0
            };
            var diffusionProfileAsset = new Vector4ShaderProperty
            {
                overrideReferenceName = "_DiffusionProfileAsset",
                value = Vector4.zero
                // value = (diffusionProfile != null) ? HDEditorUtils.ConvertGUIDToVector4(diffusionProfileGUID) : Vector4.zero
            };

            Debug.Log("Diffusion profile properties added !");
            properties.AddShaderProperty(difffusionProfileHash);
            properties.AddShaderProperty(diffusionProfileAsset);

            properties.AddShaderProperty(new ColorShaderProperty
            {
                overrideReferenceName = "_A1B2C3D4EF",
                value = Color.white
            });
        }

        public override string GetDefaultValue(GenerationMode generationMode)
        {
            if (m_DiffusionProfile == null)
                return "0";
            else
                return "asfloat(uint(" + m_DiffusionProfile.profiles[0].hash.ToString() + "))";
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
