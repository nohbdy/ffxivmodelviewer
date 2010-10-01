using System.Collections.Generic;

namespace DatDigger.Sections.Animation.Curves
{
    public struct QuaternionCurveValue
    {
        public float Time;
        public SlimDX.Quaternion Value;

        public override string ToString()
        {
            return Value.ToString() + " @ " + Time.ToString();
        }
    }

    public class QuaternionCurve : Curve<SlimDX.Quaternion>
    {
        public List<QuaternionCurveValue> Values { get; protected set; }

        public QuaternionCurve()
        {
            this.AnimatedComponent = Animation.AnimatedComponent.Quaternion;
            this.Values = new List<QuaternionCurveValue>();
            this.CurveValues = this.Values;
        }

        public override SlimDX.Quaternion GetValue(float animationTimer)
        {
            if (this.Values.Count == 0)
            {
                return SlimDX.Quaternion.Identity;
            }

            if (animationTimer <= this.Values[0].Time)
            {
                return this.Values[0].Value;
            }

            int lastIndex = this.Values.Count - 1;
            if (animationTimer >= this.Values[lastIndex].Time)
            {
                return this.Values[lastIndex].Value;
            }

            // Find the values before and after the given time
            for (var i = 1; i < Values.Count; i++)
            {
                if (animationTimer < Values[i].Time)
                {
                    // Perform a Linear Interpolation between this value and the previous value
                    float totalTimeDiff = Values[i].Time - Values[i - 1].Time;
                    float weight = (animationTimer - Values[i-1].Time) / totalTimeDiff;

                    return SlimDX.Quaternion.Normalize(SlimDX.Quaternion.Lerp(Values[i - 1].Value, Values[i].Value, weight));
                }
            }

            return this.Values[lastIndex].Value;
        }
    }
}
