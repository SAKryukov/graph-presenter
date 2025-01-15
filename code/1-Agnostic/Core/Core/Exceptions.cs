namespace SA.Agnostic {

    public class DomainException : System.ApplicationException {
        public DomainException(string message, string input) : base(message) { this.input = input; }
        public string Input => input;
        private protected readonly string input;
    } //DomainException

    public class ParseException : DomainException {
        public ParseException(string message, string input) : base(message, input) { }
    } //ParseException
    public class CustomStringValidatorException : DomainException {
        public CustomStringValidatorException(string message, string input) : base(message, input) { }
    } //CustomStringValidatorException

    public class FloatingPointDomainException : DomainException {
        public FloatingPointDomainException(string message, string input, double minimum, double maximum) : base(message, input) { this.minimum = minimum; this.maximum = maximum; }
        public double Minimum => minimum;
        public double Maximum => maximum;
        readonly double minimum, maximum;
    } //FloatingPointDomainException

    public class SignedIntegerDomainException : DomainException {
        public SignedIntegerDomainException(string message, string input, long minimum, long maximum) : base(message, input) { this.minimum = minimum; this.maximum = maximum; }
        public long Minimum => minimum;
        public long Maximum => maximum;
        readonly long minimum, maximum;
    } //SignedIntegerDomainException

    public class UnsignedIntegerDomainException : DomainException {
        public UnsignedIntegerDomainException(string message, string input, ulong minimum, ulong maximum) : base(message, input) { this.minimum = minimum; this.maximum = maximum; }
        public ulong Minimum => minimum;
        public ulong Maximum => maximum;
        readonly ulong minimum, maximum;
    } //UnsignedIntegerDomainException

}
