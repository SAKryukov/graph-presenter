namespace SA.Agnostic {

    static class DefinitionSet {

        internal const string enumerationFlageDisplayNameDelimiter = ", ";
        internal const string xmlIndentChars = "\t";

        internal static class StaticValidationEngine {
            internal static string InvalidFloatingPointDomainAttribute(string memberTypeName, string compoundTypeName, string memberName) =>
                $"[FloatingPointDomain] attribute can only be applied to a members of the type double or float, applied to {memberTypeName} {compoundTypeName}.{memberName}";
            internal static string InvalidFloatingPointDomainAttributeMinimum(string memberTypeName, string compoundTypeName, string memberName) =>
                $"[FloatingPointDomain] attribute cannot have minimum equal to NaN or positive infinity, applied to {memberTypeName} {compoundTypeName}.{memberName}";
            internal static string InvalidFloatingPointDomainAttributeMaximum(string memberTypeName, string compoundTypeName, string memberName) =>
                $"[FloatingPointDomain] attribute cannot have maximum equal to NaN or negative infinity, applied to {memberTypeName} {compoundTypeName}.{memberName}";
            internal static string InvalidFloatingPointDomainMargins(string memberTypeName, string compoundTypeName, string memberName, double minimum, double maximum) =>
                $"[FloatingPointDomain] attribute cannot have maximum less then minimum, applied to {memberTypeName} {compoundTypeName}.{memberName}, minimum: {minimum}, maximum: {maximum}";
            internal static string InvalidCustomEditor(string interfaceTypeName, string memberTypeName, string compoundTypeName, string memberName) =>
                $"[CustomEditor] attribute should define EditorClass implementing the interface {interfaceTypeName}, applied to {memberTypeName} {compoundTypeName}.{memberName}";
        } //class StaticValidationEngine

    } //class DefinitionSet

}

