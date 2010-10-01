using System.Collections.Generic;

namespace DatDigger.Sections.Animation.Curves
{
    public struct LinearCurveValue
    {
        public float Time;
        public float Value;

        public override string ToString()
        {
            return Value.ToString() + " @ " + Time.ToString();
        }
    }

    public class LinearCurve : Curve<float>
    {
        public List<LinearCurveValue> Values { get; protected set; }

        public LinearCurve(AnimatedComponent component)
        {
            this.AnimatedComponent = component;
            this.Values = new List<LinearCurveValue>();
            this.CurveValues = this.Values;
        }

        public override float GetValue(float animationTimer)
        {
            if (Values.Count == 0)
            {
                return 0;
            }

            if (animationTimer <= Values[0].Time)
            {
                return Values[0].Value;
            }

            if (animationTimer >= Values[Values.Count - 1].Value)
            {
                return Values[Values.Count - 1].Value;
            }

            // Find the values before and after the given time
            for (var i = 1; i < Values.Count; i++)
            {
                if (animationTimer < Values[i].Time)
                {
                    // Perform a Linear Interpolation between this value and the previous value
                    float totalTimeDiff = Values[i].Time - Values[i - 1].Time;
                    float weightB = (Values[i].Time - animationTimer) / totalTimeDiff;
                    float weightA = 1 - weightB;
                    float valA = Values[i - 1].Value;
                    float valB = Values[i].Value;

                    return weightA * valA + weightB * valB;
                }
            }

            return Values[Values.Count - 1].Value; // This shouldn't ever happen but whatever...
        }
    }
}
