using System;

namespace DatDigger.Xml.Format
{
    public enum ModelType
    {
        PC = 0,
        Monster = 1,
        BgObject = 2,
        Weapon = 4
    }

    public class ModelData
    {
        public int RealValue { get; set; }
        public ModelType ModelType { get; set; }
        public int ModelId { get; set; }

        public override string ToString()
        {
            switch (this.ModelType)
            {
                case ModelType.PC: return String.Format("pc/c{0:D3}", this.ModelId);
                case ModelType.Monster: return String.Format("mon/m{0:D3}", this.ModelId);
                case ModelType.BgObject: return String.Format("bgobj/b{0:D3}", this.ModelId);
                case ModelType.Weapon: return String.Format("wep/w{0:D3}", this.ModelId);
                default: return String.Format("[{0}]/{1:D3}", (int)this.ModelType, this.ModelId);
            }
        }
    }

    public class ModelFormatter : IFormatCell
    {
        public Type GetStorageType(Type sourceType)
        {
            return typeof(ModelData);
        }

        public object Format(object val)
        {
            int ival = (int)val;
            ModelData result = new ModelData()
            {
                RealValue = ival,
                ModelId = ival % 10000,
                ModelType = (ModelType)(ival / 10000)
            };

            return result;
        }
    }
}
