namespace SA.Agnostic {
    using Type = System.Type;
    using ITypeSet = System.Collections.Generic.ISet<System.Type>;
    using TypeSet = System.Collections.Generic.HashSet<System.Type>;
    using BindingFlags = System.Reflection.BindingFlags;
    using Convert = System.Convert;

    public static class TypeClassifier {

        public static ITypeSet SignedIntegerTypeSet {
            get {
                if (signedIntegerTypeSet == null)
                    ClassifyTypes();
                return signedIntegerTypeSet;
            } //get SignedIntegerTypeSet
        } //SignedIntegerTypeSet

        public static ITypeSet UnsignedIntegerTypeSet {
            get {
                if (unsignedIntegerTypeSet == null)
                    ClassifyTypes();
                return unsignedIntegerTypeSet;
            } //get UnsignedIntegerTypeSet 
        } //UnsignedIntegerTypeSet 

        public static ITypeSet FloatingPointTypeSet {
            get {
                if (floatingPointTypeSet == null)
                    ClassifyTypes();
                return floatingPointTypeSet;
            } //get FloatingPointTypeSet 
        } //FloatingPointTypeSet

        public static ITypeSet NumericTypeSet {
            get {
                if (numericTypeSet == null)
                    ClassifyTypes();
                return numericTypeSet;
            } //get NumericTypeSet
        } //NumericTypeSet

        public static ITypeSet EditableTypeSet {
            get {
                if (editableTypeSet == null)
                    ClassifyTypes();
                return editableTypeSet;
            } //get EditableTypeSet
        } //EditableTypeSet

        public static bool IsEditableType(Type type) => type.IsEnum || EditableTypeSet.Contains(type);
        public static bool IsNumericType(Type type) => NumericTypeSet.Contains(type);
        public static bool IsFloatingPointType(Type type) => FloatingPointTypeSet.Contains(type);
        public static bool IsSignedIntegerType(Type type) => SignedIntegerTypeSet.Contains(type);
        public static bool IsUnsignedIntegerType(Type type) => UnsignedIntegerTypeSet.Contains(type);

        static void ClassifyTypes() {
            numericTypeSet = new TypeSet();
            var stringType = typeof(string);
            var wantedTypes = stringType.Assembly.GetTypes();
            foreach (var type in wantedTypes)
                if (type.IsPrimitive && type != typeof(char) && type != typeof(System.IntPtr) && type != typeof(System.UIntPtr))
                    numericTypeSet.Add(type);
            editableTypeSet = new TypeSet(numericTypeSet);
            numericTypeSet.Remove(typeof(bool));
            editableTypeSet.Add(typeof(string)); // now there are two more types in this set, bool and string
            ITypeSet types = numericTypeSet;
            signedIntegerTypeSet = new TypeSet();
            unsignedIntegerTypeSet = new TypeSet();
            floatingPointTypeSet = new TypeSet { typeof(double), typeof(float) };
            foreach (Type type in types) {
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
                if (fields.Length < 2) continue;
                var minField = fields[^1];
                object minimalValue = minField.GetValue(null);
                try {
                    if (Convert.ToInt64(minimalValue) < 0)
                        signedIntegerTypeSet.Add(type);
                } catch { }
                try {
                    if (Convert.ToUInt64(minimalValue) == 0)
                        unsignedIntegerTypeSet.Add(type);
                } catch { }
            } //loop
        } //ClassifyTypes

        static ITypeSet editableTypeSet;
        static ITypeSet numericTypeSet;
        static ITypeSet signedIntegerTypeSet;
        static ITypeSet unsignedIntegerTypeSet;
        static ITypeSet floatingPointTypeSet;

    } //class TypeClassifier

}
