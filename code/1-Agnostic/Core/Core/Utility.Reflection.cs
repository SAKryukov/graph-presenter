namespace SA.Agnostic {
    using System.Reflection;
    using Enum = System.Enum;
    using Type = System.Type;
    using Attribute = System.Attribute;
    using InheritanceStack = System.Collections.Generic.Stack<System.Type>;
    using PropertyList = System.Collections.Generic.List<System.Reflection.PropertyInfo>;
    using FieldList = System.Collections.Generic.List<System.Reflection.FieldInfo>;
    using StringList = System.Collections.Generic.List<string>;
    using DataMemberAttribute = System.Runtime.Serialization.DataMemberAttribute;

    public static partial class Utility {

        public static class Reflection {

            public static AttributeType GetAttribute<AttributeType>(MemberInfo member, bool inherit = false) where AttributeType : Attribute =>
                (AttributeType)member.GetCustomAttribute(typeof(AttributeType), inherit);

            public static int GetAttributeCount<AttributeType>(MemberInfo member, bool inherit = false) where AttributeType : Attribute {
                var attributes = member.GetCustomAttributes(typeof(AttributeType), inherit);
                if (attributes == null) return 0;
                return attributes.Length;
            } //GetAttributeCount

            public static AttributeType GetAttribute<AttributeType>(FieldInfo field, PropertyInfo property) where AttributeType : Attribute {
                if (field != null) return GetAttribute<AttributeType>(field);
                else if (property != null) return GetAttribute<AttributeType>(property);
                else return null;
            } //GetAttribute

            public static string DisplayName(MemberInfo member) {
                var attribute = GetAttribute<DisplayNameAttribute>(member);
                if (attribute == null) return member.Name;
                if (attribute.Resource != null) {
                    string typeName = attribute.Resource.UseFullTypeName ? member.DeclaringType.FullName : member.DeclaringType.Name;
                    return attribute.Resource.DisplayName(typeName, member.Name);
                } else if (attribute.HumanReadableName != null)
                    return attribute.HumanReadableName;
                return member.Name;
            } //DisplayName

            public static string DisplayName(Enum value, string flagDelimiter = null) {
                if (value == null) return string.Empty;
                if (string.IsNullOrWhiteSpace(flagDelimiter))
                    flagDelimiter = DefinitionSet.enumerationFlageDisplayNameDelimiter;
                var fields = value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public);
                if (Attribute.IsDefined(value.GetType(), typeof(System.FlagsAttribute))) {
                    StringList list = new();
                    foreach (var field in fields) {
                        if (Attribute.IsDefined(field, typeof(HiddenAttribute))) continue;
                        var fieldValue = (Enum)field.GetValue(null);
                        if (value.HasFlag(fieldValue))
                            list.Add(DisplayName(field));
                    } //loop
                    return string.Join(flagDelimiter, list);
                } else { //not flags
                    foreach (var field in fields) {
                        if (Attribute.IsDefined(field, typeof(HiddenAttribute))) continue;
                        if (Enum.Equals(field.GetValue(null), value))
                            return DisplayName(field);
                    } //loop
                } //if not flags
                return null;
            } //DisplayName

            public static Enum Or(Enum[] values) {
                if (values == null) return null;
                if (values.Length < 1) return null;
                var type = values[0].GetType();
                var underliyingType = Enum.GetUnderlyingType(type);
                bool isSigned;
                if (TypeClassifier.IsSignedIntegerType(underliyingType))
                    isSigned = true;
                else if (TypeClassifier.IsUnsignedIntegerType(underliyingType))
                    isSigned = false;
                else
                    return null;
                if (isSigned) {
                    long result = 0;
                    foreach (var value in values)
                        result |= System.Convert.ToInt64(value);
                    return (Enum)Enum.ToObject(type, result);
                } else {
                    ulong result = 0;
                    foreach (var value in values)
                        result |= System.Convert.ToUInt64(value);
                    return (Enum)Enum.ToObject(type, result);
                } //if
            } //Or

            public static object Parse(string value, FieldInfo field, PropertyInfo property) {
                var type = field?.FieldType ?? property?.PropertyType;
                MethodInfo parseMethod = type.GetMethod(nameof(int.Parse), new Type[] { typeof(string) });
                return parseMethod.Invoke(0, new string[] { value });
            } //Parse

            public static bool CanReadWrite(FieldInfo field, PropertyInfo property) {
                if (field != null) return true;
                if (property != null)
                    return property.CanWrite && property.CanRead;
                return false;
            } //CanReadWrite

            public static Type MemberType(FieldInfo field, PropertyInfo property) {
                if (property != null) return property.PropertyType;
                else if (field != null) return field.FieldType;
                return null;
            } //MemberType

            public static object GetValue(object compoundObject, FieldInfo field, PropertyInfo property) {
                if (compoundObject == null) return null;
                if (property != null) return property.GetValue(compoundObject);
                else if (field != null) return field.GetValue(compoundObject);
                return null;
            } //GetValue

            public static void SetValue(object compoundObject, FieldInfo field, PropertyInfo property, object newValue) {
                if (compoundObject == null) return;
                if (property != null) property.SetValue(compoundObject, newValue);
                else if (field != null) field.SetValue(compoundObject, newValue);
            } //SetValue

            static void GetTypes(Type type, InheritanceStack stack, Assembly assembly) {
                stack.Push(type);
                Type baseType = type.BaseType;
                if (baseType != null && (assembly == null || baseType.Assembly == assembly))
                    GetTypes(baseType, stack, assembly);
            } //GetTypes

            public static FieldInfo[] GetFields(Type type, bool excludeValuetTypes = false, bool sameAssembly = false) {
                if (type == null) return null;
                InheritanceStack stack = new();
                GetTypes(type, stack, sameAssembly ? type.Assembly : null);
                FieldList list = new();
                while (stack.Count > 0) {
                    Type currentType = stack.Pop();
                    var members = currentType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    foreach(var member in members)
                        if (member.DeclaringType == currentType
                            && !(excludeValuetTypes && member.FieldType.IsValueType)
                            && (Attribute.IsDefined(member, typeof(DataMemberAttribute)) || Attribute.IsDefined(member, typeof(NonPersistentDataMemberAttribute)))
                            && !Attribute.IsDefined(member, typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute)))
                                list.Add(member);
                } //loop
                return list.ToArray();
            } //GetFields
            public static PropertyInfo[] GetProperties(Type type, bool excludeValuetTypes = false, bool sameAssembly = false, bool onlyReadable = false) {
                if (type == null) return null;
                InheritanceStack stack = new();
                GetTypes(type, stack, sameAssembly ? type.Assembly : null);
                PropertyList list = new();
                while (stack.Count > 0) {
                    Type currentType = stack.Pop();
                    var members = currentType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    foreach (var member in members) {
                        if (onlyReadable && !member.CanRead) continue;
                        if (member.DeclaringType == currentType
                            && !(excludeValuetTypes && member.PropertyType.IsValueType)
                            && (Attribute.IsDefined(member, typeof(DataMemberAttribute)) || Attribute.IsDefined(member, typeof(NonPersistentDataMemberAttribute))))
                            list.Add(member);
                    } //loop properties
                } //loop stack
                return list.ToArray();
            } //GetFields

        } //class Reflection

    } //class Utility

}
