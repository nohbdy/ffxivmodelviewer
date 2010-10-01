using System;
using System.Collections.Generic;

namespace DatDigger.Sound
{
    public struct AdpcmCoefficient
    {
        public Int16 A;
        public Int16 B;
    }

    public class AdpcmWaveFormat : SlimDX.Multimedia.WaveFormat
    {
        public Int16 SamplesPerBlock { get; set; }
        public Int16 NumCoef { get; set; }
        public List<AdpcmCoefficient> Coefficients { get; set; }

        public override byte[] GetBytes()
        {
            this.Size = 0x20;

            List<byte> result = new List<byte>(50);

            byte[] baseBytes = base.GetBytes();
            result.AddRange(baseBytes);
            result.AddRange(BitConverter.GetBytes(this.SamplesPerBlock));
            result.AddRange(BitConverter.GetBytes(this.NumCoef));
            foreach (AdpcmCoefficient coef in this.Coefficients)
            {
                result.AddRange(BitConverter.GetBytes(coef.A));
                result.AddRange(BitConverter.GetBytes(coef.B));
            }

            return result.ToArray();
        }
    }
}
