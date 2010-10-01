namespace DatDigger.Sections.Animation.Curves
{
    public abstract class Curve<T> : CurveBase where T : struct
    {
        public abstract T GetValue(float animationTimer);
    }

    public abstract class CurveBase
    {
        public System.Collections.ICollection CurveValues { get; protected set; }
        public AnimatedComponent AnimatedComponent { get; protected set; }
    }
}
