namespace DatDigger.Sections.Animation.Curves
{
    public class ConstantCurve : Curve<float>
    {
        public ConstantCurve(AnimatedComponent component, float value)
        {
            this.AnimatedComponent = component;
            this.Value = value;
            this.CurveValues = new float[1] { value };
        }

        public float Value { get; private set; }

        public override float GetValue(float animationTimer)
        {
            return Value;
        }

        public override string ToString()
        {
            return "ConstantCurve [" + this.Value.ToString() + "]";
        }
    }
}