using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEngine.Experimental.Rendering.HDPipeline;

namespace UnityEditor.Experimental.Rendering.HDPipeline
{
    [Title("Input", "High Definition Render Pipeline", "Diffusion Profile")]
    [FormerName("UnityEditor.ShaderGraph.DiffusionProfileNode")]
    class DiffusionProfileNode : AbstractMaterialNode, IGeneratesBodyCode
    {
        public DiffusionProfileNode()
        {
            name = "Diffusion Profile";
            UpdateNodeAfterDeserialization();
        }

        public override string documentationURL
        {
            // This still needs to be added.
            get { return "https://github.com/Unity-Technologies/ShaderGraph/wiki/Diffusion-Profile-Node"; }
        }

        [SerializeField]
        DiffusionProfileSettings    m_DiffusionProfileAsset;

        [ObjectControl]
        public DiffusionProfileSettings diffusionProfile
        {
            get
            {
                return m_DiffusionProfileAsset;
            }
            set
            {
                m_DiffusionProfileAsset = value;
                Dirty(ModificationScope.Node);
            }
        }

        private const int kOutputSlotId = 0;
        private const string kOutputSlotName = "Out";

        public override bool hasPreview { get { return false; } }

        public sealed override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new Vector1MaterialSlot(kOutputSlotId, kOutputSlotName, kOutputSlotName, SlotType.Output, 0.0f));
            RemoveSlotsNameNotMatching(new[] { kOutputSlotId });
        }

        public void GenerateNodeCode(ShaderGenerator visitor, GraphContext graphContext, GenerationMode generationMode)
        {
            uint hash = 0;
            
            if (m_DiffusionProfileAsset != null)
                hash = (m_DiffusionProfileAsset.profiles[0].hash);
            
            visitor.AddShaderChunk(precision + " " + GetVariableNameForSlot(0) + " = asfloat(uint(" + hash + "));", true);
        }
    }
}
