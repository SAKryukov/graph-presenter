namespace SA.Agnostic.UI {

    abstract class FloatingPointAbstractionLayer {
        internal abstract bool IsNan();
        internal abstract bool IsPositiveInfinity();
        internal abstract bool IsNegativeInfinity();
        internal abstract bool IsNan(object value);
        internal abstract bool IsPositiveInfinity(object value);
        internal abstract bool IsNegativeInfinity(object value);
        internal abstract object Parse(string value);
        internal abstract bool IsInDomain(object value, double minimum, double maximum);
        internal abstract bool IsInDomain(double minimum, double maximum);
        internal abstract bool IsNegativeInfinityInDomain(double minimum, double maximum);
        internal abstract bool IsPositiveInfinityInDomain(double minimum, double maximum);
    } //FloatingPointAbstractionLayer

    class FloatAbstractionImplementation : FloatingPointAbstractionLayer {
        internal FloatAbstractionImplementation() { }
        internal FloatAbstractionImplementation(float value) { this.value = value; }
        internal override bool IsNan() => float.IsNaN(value);
        internal override bool IsPositiveInfinity() => float.IsPositiveInfinity(value);
        internal override bool IsNegativeInfinity() => float.IsNegativeInfinity(value);
        internal override bool IsNan(object value) => float.IsNaN((float)value);
        internal override bool IsPositiveInfinity(object value) => float.IsNegativeInfinity((float)value);
        internal override bool IsNegativeInfinity(object value) => float.IsPositiveInfinity((float)value);
        internal override object Parse(string stringValue) => float.Parse(stringValue);
        internal override bool IsInDomain(object value, double minimum, double maximum) {
            if (value == null) return false;
            if (value.GetType() != typeof(float)) return false;
            float floatValue = (float)value;
            static double Convert(float value) {
                if (float.IsNaN(value)) return double.NaN;
                if (float.IsPositiveInfinity(value)) return double.PositiveInfinity;
                if (float.IsNegativeInfinity(value)) return double.NegativeInfinity;
                return value;
            } //Convert
            double doubleValue = Convert(floatValue);
            return minimum <= doubleValue && doubleValue <= maximum;
        }
        internal override bool IsInDomain(double minimum, double maximum) => IsInDomain(value, minimum, maximum);
        internal override bool IsNegativeInfinityInDomain(double minimum, double maximum) => IsInDomain(float.NegativeInfinity, minimum, maximum);
        internal override bool IsPositiveInfinityInDomain(double minimum, double maximum) => IsInDomain(float.PositiveInfinity, minimum, maximum);
        readonly float value;
    } //FloatingAbstractionImplementation

    class DoubleAbstractionImplementation : FloatingPointAbstractionLayer {
        internal DoubleAbstractionImplementation() { }
        internal DoubleAbstractionImplementation(double value) { this.value = value; }
        internal override bool IsNan() => double.IsNaN(value);
        internal override bool IsPositiveInfinity() => double.IsPositiveInfinity(value);
        internal override bool IsNegativeInfinity() => double.IsNegativeInfinity(value);
        internal override bool IsNan(object value) => double.IsNaN((double)value);
        internal override bool IsPositiveInfinity(object value) => double.IsNegativeInfinity((double)value);
        internal override bool IsNegativeInfinity(object value) => double.IsPositiveInfinity((double)value);
        internal override object Parse(string stringValue) => double.Parse(stringValue);
        internal override bool IsInDomain(object value, double minimum, double maximum) => minimum <= (double)value && (double)value <= maximum;
        internal override bool IsInDomain(double minimum, double maximum) => minimum <= value && value <= maximum;
        internal override bool IsNegativeInfinityInDomain(double minimum, double maximum) => IsInDomain(double.NegativeInfinity, minimum, maximum);
        internal override bool IsPositiveInfinityInDomain(double minimum, double maximum) => IsInDomain(double.PositiveInfinity, minimum, maximum);
        readonly double value;
    } //FloatingAbstractionImplementation

}
