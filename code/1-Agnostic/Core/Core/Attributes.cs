namespace SA.Agnostic {
    using System;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ToolTipAttribute : Attribute {
        public ToolTipAttribute(string tooltip) { ToolTip = tooltip; }
        public string ToolTip { get; set; }
    } //ToolTipAttribute
    //TODO: implement the use of [ToolTip]

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class FloatingPointDomainAttribute : Attribute { // defines valid domain of values
        public FloatingPointDomainAttribute() {
            Minimum = double.NegativeInfinity;
            Maximum = double.PositiveInfinity;
            IsNanAccepted = true;
        } //FloatingPointDomainAttribute
        public FloatingPointDomainAttribute(double minumum, double maximum, bool isNaNAccepted = true) {
            Minimum = minumum;
            Maximum = maximum;
            IsNanAccepted = isNaNAccepted;
        } //FloatingPointDomainAttribute
        public double Minimum { get; set; }
        public double Maximum { get; set; }
        public bool IsNanAccepted { get; set; }
    } //class FloatingPointDomainAttribute

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class SignedIntegerDomainAttribute : Attribute { // defines valid domain of values
        public SignedIntegerDomainAttribute() {
            Minimum = Int64.MinValue;
            Maximum = Int64.MaxValue;
        }
        public Int64 Minimum { get; set; }
        public Int64 Maximum { get; set; }
    } //class SignedIntegerDomainAttribute

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class UnsignedIntegerDomainAttribute : Attribute { // defines valid domain of values
        public UnsignedIntegerDomainAttribute() {
            Minimum = System.UInt64.MinValue;
            Maximum = System.UInt64.MaxValue;
        }
        public UInt64 Minimum { get; set; }
        public UInt64 Maximum { get; set; }
    } //class UnsignedIntegerDomainAttribute

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class StringDomainAttribute : Attribute { // applied only to the fields/propeties of string type
        public StringDomainAttribute(Type validatorClass = null, bool isNullAccepted = true, bool isMultiline = false) {
            if (validatorClass != null) {
                var constructor = validatorClass.GetConstructor(Array.Empty<Type>());
                validator = (IStringValidator)constructor.Invoke(Array.Empty<Type>());
            } //if
            IsNullAccepted = isNullAccepted;
            IsMultiline = isMultiline;
        } //StringValidationAttribute
        public bool IsNullAccepted { get; set; }
        public bool IsMultiline { get; set; }
        public IStringValidator Validator => validator;
        readonly IStringValidator validator;
    } //class StringDomainAttribute

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class FilenameEditorAttribute : Attribute { // applied only to the fields/propeties of string type
        public string FileDialogTitle { get; set; }
        public string DialogFilter { get; set; }
        public Type BasePathProviderStaticClass {
            get => basePathProviderStaticClass;
            set {
                basePathProviderStaticClass = value;
                var constructor = basePathProviderStaticClass.GetConstructor(Array.Empty<Type>());
                basePathProvider = (IBasePathProvider)constructor.Invoke(Array.Empty<Type>());
            } //set BasePathProviderStaticClass
        } //BasePathProviderStaticClass
        public IBasePathProvider BasePathProvider => basePathProvider;
        IBasePathProvider basePathProvider;
        Type basePathProviderStaticClass;
    } //class FilenameEditorAttribute

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class TitleAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class DisplayNameAttribute : Attribute { // to mark a static field of enum class (enumeration member); note: [System.ComponentModel.DisplayName] cannot apply there
        public DisplayNameAttribute(string humanReadableName) { this.humanReadableName = humanReadableName; }
        public DisplayNameAttribute(Type resource) { // resource should implement ILocalizableDisplayNameResource
            if (resource == null) return;
            var constructor = resource.GetConstructor(Array.Empty<Type>());
            this.resource = (ILocalizableDisplayNameResource)constructor.Invoke(Array.Empty<Type>());
        } //DisplayNameAttribute
        readonly string humanReadableName = null;
        readonly ILocalizableDisplayNameResource resource = null;
        public string HumanReadableName => humanReadableName;
        public ILocalizableDisplayNameResource Resource => resource;
    } //class DisplayNameAttribute

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class CustomEditorAttribute : Attribute {
        public CustomEditorAttribute(Type editorClass) { // editorClass should implement IMemberEditor
            if (editorClass == null) return;
            var constructor = editorClass.GetConstructor(Array.Empty<Type>());
            editor = (IMemberEditor)constructor.Invoke(Array.Empty<Type>());
        } //CustomEditorAttribute
        public IMemberEditor Editor => editor;
        readonly IMemberEditor editor;
    } //class CustomEditorAttribute

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class HiddenAttribute : Attribute { } // to mark a static field of enum class (enumeration member)
    //TODO: implement the use of HiddenAttribute in other cases

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class AssociationAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class AssociationPropertySetAttribute : Attribute {
        public AssociationPropertySetAttribute(string associationMemberName = null) { AssociationMemberName = associationMemberName; }
        public string AssociationMemberName { get; set; }
    } //class AssociationPropertySetAttribute

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class GlobalOptionSetAttribute : Attribute {
        public GlobalOptionSetAttribute(Type globalOptionSetImplementation) {
            if (globalOptionSetImplementation == null) return;
            var constructor = globalOptionSetImplementation.GetConstructor(Array.Empty<Type>());
            globalOptionSet = (IGlobalOptionSet)constructor.Invoke(Array.Empty<Type>());
        } //GlobalOptionSetAttribute
        public IGlobalOptionSet GlobalOptionSet => globalOptionSet;
        readonly IGlobalOptionSet globalOptionSet;
    } //GlobalOptionSetAttribute

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class FixedAccessAttribute : Attribute { // does not depend on Editors' Readonly
        public FixedAccessAttribute(MemberAccessLevel memberAccessLevel) { MemberAccessLevel = memberAccessLevel; }
        public MemberAccessLevel MemberAccessLevel { get; init; }
    } //FixedAccessAttribute

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class NonPersistentDataMemberAttribute : Attribute { }

}
