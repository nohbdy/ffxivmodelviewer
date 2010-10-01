using System.Collections.Generic;

namespace DatDigger.Sections.Shader
{
    public class ParameterData : INavigable
    {
        public bool IsPixelShaderParameter { get; set; }
        public byte Unknown2 { get; set; }
        public byte NumValues { get; set; }
        public byte Unknown3 { get; set; }
        public int Unknown4 { get; set; }
        public int Unknown5 { get; set; }
        public int Unknown6 { get; set; }

        public SlimDX.Vector4 Defaults;
        public SlimDX.Vector4 Defaults_ { get { return Defaults; } }
        public SlimDX.Direct3D9.EffectHandle EffectHandle { get; set; }

        public string Name { get; set; }

        public string DisplayName { get { return Name; } }

        public INavigable Parent { get; set; }

        public List<INavigable> Children { get { return null; } }
    }
}
