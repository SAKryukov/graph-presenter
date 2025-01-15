namespace SA.Agnostic {
    using System.Reflection;
    using System.Runtime.Serialization;
    using Type = System.Type;
    using Attribute = System.Attribute;
    using Console = System.Console;

    public static class StaticValidationEngine {

        public static bool Validate(Assembly[] assemblies) {
            bool hasFailures = false;
            foreach (Assembly assembly in assemblies)
                hasFailures = hasFailures || Validate(assembly);
            return !hasFailures;
        } //Validate

        static bool Validate(Assembly assembly) {
            Type[] types = assembly.GetTypes();
            bool hasFailures = false;
            foreach (Type type in types)
                hasFailures = hasFailures || Validate(type);
            return !hasFailures;
        } //Validate assembly

        static bool Validate(Type type) {
            if (!Attribute.IsDefined(type, typeof(DataContractAttribute))) return true;
            MemberInfo[] members = type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Instance);
            bool hasFailures = false;
            foreach (MemberInfo member in members) {
                Type memberType = null;
                if (member.MemberType == MemberTypes.Property)
                    memberType = ((PropertyInfo)member).PropertyType;
                else if (member.MemberType == MemberTypes.Field)
                    memberType = ((FieldInfo)member).FieldType;
                hasFailures = hasFailures || Validate(member, memberType, type);
            }
            return !hasFailures;
        } //Validate type

        static bool Validate(MemberInfo member, Type memberType, Type compoundType) {
            if (!Attribute.IsDefined(member, typeof(DataMemberAttribute))) return true;
            if (Attribute.IsDefined(member, typeof(IgnoreDataMemberAttribute))) return true;
            bool hasFailures = false;
            hasFailures = hasFailures || ValidateFloatingPoint(member, memberType, compoundType);
            //TODO: decide if it is needed
            //hasFailures = hasFailures || ValidateCustomEditor(member, memberType, compoundType); //SA???
            return !hasFailures;
        } //Validate member

        static bool ValidateFloatingPoint(MemberInfo member, Type memberType, Type compoundType) {
            var attribute = member.GetCustomAttribute(typeof(FloatingPointDomainAttribute));
            if (attribute == null) return true;
            FloatingPointDomainAttribute floatingPointDomainAttribute = (FloatingPointDomainAttribute)attribute;
            if (memberType != null && memberType != typeof(double) && memberType != typeof(float)) {
                Console.WriteLine(DefinitionSet.StaticValidationEngine.InvalidFloatingPointDomainAttributeMinimum(memberType.Name, compoundType.FullName, member.Name));
                return false;
            } //if
            if (double.IsNaN(floatingPointDomainAttribute.Maximum) || double.IsNegativeInfinity(floatingPointDomainAttribute.Maximum)) {
                Console.WriteLine(DefinitionSet.StaticValidationEngine.InvalidFloatingPointDomainAttributeMaximum(memberType.Name, compoundType.FullName, member.Name));
                return false;
            } //if
            if (floatingPointDomainAttribute.Minimum < floatingPointDomainAttribute.Maximum) {
                Console.WriteLine(DefinitionSet.StaticValidationEngine.InvalidFloatingPointDomainMargins(memberType.Name, compoundType.FullName, member.Name, floatingPointDomainAttribute.Minimum, floatingPointDomainAttribute.Maximum));
                return false;
            } //if
            return true;
        } //ValidateFloatingPoint

        //static bool ValidateInterfaceAttribute() {
            //TODO: design/implement ValidateInterfaceAttribute
        //} //ValidateInterfaceAttribute

        //TODO: implement validation of custom editor:
        /* //SA???
        static bool ValidateCustomEditor(MemberInfo member, Type memberType, Type compoundType) {
            var attribute = member.GetCustomAttribute(typeof(CustomEditorAttribute));
            if (attribute == null) return true;
            CustomEditorAttribute customEditorAttribute = (CustomEditorAttribute)attribute;
            Type correctInterfaceType = typeof(IMemberEditor);
            if (customEditorAttribute.Editor is not IMemberEditor) {
                Console.WriteLine(DefinitionSet.StaticValidationEngine.InvalidCustomEditor(correctInterfaceType.Name, memberType.Name, compoundType.FullName, member.Name));
                return false;
            } //if
            return true;
        } //bool ValidateCustomEditor
        */

    } //class StaticValidationEngine

}
