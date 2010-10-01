using System.ComponentModel;

namespace DatDigger.Sections.Animation
{
    public class AnimatedBone
    {
        public int BoneId { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Curves.QuaternionCurve RotationCurve { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Curves.Curve<float> TranslationX { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Curves.Curve<float> TranslationY { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Curves.Curve<float> TranslationZ { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Curves.Curve<float> ScaleX { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Curves.Curve<float> ScaleY { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Curves.Curve<float> ScaleZ { get; set; }
    }
}
