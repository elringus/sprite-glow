//using System.Linq;
//using UnityEditor.Graphing;
//using UnityEditor.ShaderGraph;
//using UnityEngine;

//namespace SpriteGlow
//{
//    [Title("Sprite Glow", "Glow Outside")]
//    public class GlowOutsideNode : AbstractMaterialNode, IGeneratesBodyCode, IMayRequireMeshUV
//    {
//        public const int OutputSlotRGBAId = 0;
//        public const int OutputSlotRId = 8;
//        public const int OutputSlotGId = 9;
//        public const int OutputSlotBId = 10;
//        public const int OutputSlotAId = 11;
//        public const int TextureInputId = 1;
//        public const int UVInput = 2;
//        public const int SamplerInput = 3;
//        public const int GlowColorInput = 4;
//        public const int GlowBrightnessInput = 5;
//        public const int OutlineWidthInput = 6;
//        public const int AlphaThresholdInput = 7;

//        const string kOutputSlotRGBAName = "RGBA";
//        const string kOutputSlotRName = "R";
//        const string kOutputSlotGName = "G";
//        const string kOutputSlotBName = "B";
//        const string kOutputSlotAName = "A";
//        const string kTextureInputName = "Texture";
//        const string kUVInputName = "UV";
//        const string kSamplerInputName = "Sampler";
//        const string kGlowColorInputName = "GlowColor";
//        const string kGlowBrightnessInputName = "GlowBrightness";
//        const string kOutlineWidthInputName = "OutlineWidth";
//        const string kAlphaThresholdInputName = "AlphaThreshold";

//        public override bool hasPreview { get { return true; } }

//        public GlowOutsideNode ()
//        {
//            name = "Glow Outside";
//            UpdateNodeAfterDeserialization();
//        }

//        public override string documentationURL
//        {
//            get { return "https://github.com/Elringus/SpriteGlow"; }
//        }

//        public sealed override void UpdateNodeAfterDeserialization ()
//        {
//            AddSlot(new Vector4MaterialSlot(OutputSlotRGBAId, kOutputSlotRGBAName, kOutputSlotRGBAName, SlotType.Output, Vector4.zero));
//            AddSlot(new Vector1MaterialSlot(OutputSlotRId, kOutputSlotRName, kOutputSlotRName, SlotType.Output, 0));
//            AddSlot(new Vector1MaterialSlot(OutputSlotGId, kOutputSlotGName, kOutputSlotGName, SlotType.Output, 0));
//            AddSlot(new Vector1MaterialSlot(OutputSlotBId, kOutputSlotBName, kOutputSlotBName, SlotType.Output, 0));
//            AddSlot(new Vector1MaterialSlot(OutputSlotAId, kOutputSlotAName, kOutputSlotAName, SlotType.Output, 0));
//            AddSlot(new Texture2DInputMaterialSlot(TextureInputId, kTextureInputName, kTextureInputName));
//            AddSlot(new UVMaterialSlot(UVInput, kUVInputName, kUVInputName, UVChannel.UV0));
//            AddSlot(new SamplerStateMaterialSlot(SamplerInput, kSamplerInputName, kSamplerInputName, SlotType.Input));
//            AddSlot(new ColorRGBAMaterialSlot(GlowColorInput, kGlowColorInputName, kGlowColorInputName, SlotType.Input, Color.white));
//            AddSlot(new Vector1MaterialSlot(GlowBrightnessInput, kGlowBrightnessInputName, kGlowBrightnessInputName, SlotType.Input, 1.5f));
//            AddSlot(new Vector1MaterialSlot(OutlineWidthInput, kOutlineWidthInputName, kOutlineWidthInputName, SlotType.Input, 1));
//            AddSlot(new Vector1MaterialSlot(AlphaThresholdInput, kAlphaThresholdInputName, kAlphaThresholdInputName, SlotType.Input, 0.01f));

//            RemoveSlotsNameNotMatching(new[] { OutputSlotRGBAId, OutputSlotRId, OutputSlotGId, OutputSlotBId, OutputSlotAId, TextureInputId, UVInput, SamplerInput, GlowColorInput, GlowBrightnessInput, OutlineWidthInput, AlphaThresholdInput });
//        }

//        public virtual void GenerateNodeCode (ShaderGenerator visitor, GenerationMode generationMode)
//        {
//            var samplerSlot = FindInputSlot<MaterialSlot>(SamplerInput);
//            var edgesSampler = owner.GetEdges(samplerSlot.slotReference);

//            var result = SpriteGlow_GlowOutside(
//                  GetSlotValue(TextureInputId, generationMode)
//                , edgesSampler.Any() ? GetSlotValue(SamplerInput, generationMode) : "sampler" + GetSlotValue(TextureInputId, generationMode)
//                , GetSlotValue(UVInput, generationMode)
//                , GetSlotValue(GlowColorInput, generationMode)
//                , GetSlotValue(GlowBrightnessInput, generationMode)
//                , GetSlotValue(OutlineWidthInput, generationMode)
//                , GetSlotValue(AlphaThresholdInput, generationMode)
//                , GetVariableNameForSlot(OutputSlotRGBAId)
//                , precision);

//            visitor.AddShaderChunk(result, true);
//            visitor.AddShaderChunk(string.Format("{0} {1} = {2}.r;", precision, GetVariableNameForSlot(OutputSlotRId), GetVariableNameForSlot(OutputSlotRGBAId)), true);
//            visitor.AddShaderChunk(string.Format("{0} {1} = {2}.g;", precision, GetVariableNameForSlot(OutputSlotGId), GetVariableNameForSlot(OutputSlotRGBAId)), true);
//            visitor.AddShaderChunk(string.Format("{0} {1} = {2}.b;", precision, GetVariableNameForSlot(OutputSlotBId), GetVariableNameForSlot(OutputSlotRGBAId)), true);
//            visitor.AddShaderChunk(string.Format("{0} {1} = {2}.a;", precision, GetVariableNameForSlot(OutputSlotAId), GetVariableNameForSlot(OutputSlotRGBAId)), true);

//            Debug.Log(visitor.GetShaderString(0));
//        }

//        private static string SpriteGlow_GlowOutside (string textureName, string samplerName, string textureUVName, string glowColorName, string glowBrightnessName, string outlineWidthName, string alphaThresholdName, string outputColorName, OutputPrecision precision)
//        {
//            return string.Format(@"
//    {8}4 color = SAMPLE_TEXTURE2D({0}, {1}, {2});
//    int shouldDrawOutline = 0; 

//    float texelWidth;
//    float texelHeight;
//    {0}.GetDimensions(texelWidth, texelHeight);

//    if ({5} * color.a != 0)
//    {{
//        float2 texDdx = ddx({2});
//        float2 texDdy = ddy({2});

//        for (int i = 1; i <= 10; i++)
//        {{
//            float2 pixelUpTexCoord = {2} + float2(0, i * texelHeight);
//            float pixelUpAlpha = pixelUpTexCoord.y > 1.0 ? 0.0 : tex2Dgrad({1}, pixelUpTexCoord, texDdx, texDdy).a;
//            if (pixelUpAlpha <= {6}) {{ shouldDrawOutline = 1; break; }}

//            float2 pixelDownTexCoord = {2} - float2(0, i * texelHeight);
//            float pixelDownAlpha = pixelDownTexCoord.y < 0.0 ? 0.0 : tex2Dgrad({1}, pixelDownTexCoord, texDdx, texDdy).a;
//            if (pixelDownAlpha <= {6}) {{ shouldDrawOutline = 1; break; }}

//            float2 pixelRightTexCoord = {2} + float2(i * texelWidth, 0);
//            float pixelRightAlpha = pixelRightTexCoord.x > 1.0 ? 0.0 : tex2Dgrad({1}, pixelRightTexCoord, texDdx, texDdy).a;
//            if (pixelRightAlpha <= {6}) {{ shouldDrawOutline = 1; break; }}

//            float2 pixelLeftTexCoord = {2} - float2(i * texelWidth, 0);
//            float pixelLeftAlpha = pixelLeftTexCoord.x < 0.0 ? 0.0 : tex2Dgrad({1}, pixelLeftTexCoord, texDdx, texDdy).a;
//            if (pixelLeftAlpha <= {6}) {{ shouldDrawOutline = 1; break; }}

//            if (i > {5}) break;
//        }}
//    }}

//    {8}4 {7} = lerp(color, {3} * {4} * {3}.a, shouldDrawOutline);", 
//    textureName, samplerName, textureUVName, glowColorName, glowBrightnessName, outlineWidthName, alphaThresholdName, outputColorName, precision);
//        }

//        public bool RequiresMeshUV (UVChannel channel)
//        {
//            s_TempSlots.Clear();
//            GetInputSlots(s_TempSlots);
//            foreach (var slot in s_TempSlots)
//            {
//                if (slot.RequiresMeshUV(channel))
//                    return true;
//            }
//            return false;
//        }
//    }
//}
