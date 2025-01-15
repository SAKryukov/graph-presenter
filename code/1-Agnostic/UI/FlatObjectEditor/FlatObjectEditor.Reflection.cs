namespace SA.Agnostic.UI {
    using System.Reflection;
    using BindingFlags = System.Reflection.BindingFlags;
    using MemberEditorList = System.Collections.Generic.List<IMemberEditor>;

    public partial class FlatObjectEditor {

        void Populate(object compondObject) {
            this.memberEditors = null;
            MemberEditorList memberEditors = new();
            if (compondObject == null) { Populate(); return; }
            System.Type type = compondObject.GetType();
            PropertyInfo[] properties = Utility.Reflection.GetProperties(type, sameAssembly: modelReflectionMode == GraphPresenter.ModelReflectionModeType.BaseClassesFromTheSameAssemby, onlyReadable: true);
            FieldInfo[] fields = Utility.Reflection.GetFields(type, sameAssembly: true);
            void ProcessMember(MemberInfo member, PropertyInfo property, FieldInfo field, object value, System.Type memberType) {
                if (property != null && !property.CanRead) return;
                string memberName = Utility.Reflection.DisplayName(member);
                if (!TypeClassifier.IsEditableType(memberType)) return;
                bool effectiveReadonly = isReadonly;
                FixedAccessAttribute fixedAccessAttribute = Utility.Reflection.GetAttribute<FixedAccessAttribute>(member);
                bool hasFixedAccess = fixedAccessAttribute != null;
                if (hasFixedAccess) {
                    if (fixedAccessAttribute.MemberAccessLevel == MemberAccessLevel.Hidden) return;
                    effectiveReadonly = fixedAccessAttribute.MemberAccessLevel != MemberAccessLevel.ReadWrite;
                } //if fixedAccessAttribute
                if (property != null && !property.CanWrite)
                    effectiveReadonly = hasFixedAccess = true;
                CustomEditorAttribute customEditorAttribute = Utility.Reflection.GetAttribute<CustomEditorAttribute>(member);
                if (customEditorAttribute != null) {
                    customEditorAttribute.Editor.CustomInitializer(this, compondObject, memberName, value, field, property, effectiveReadonly, hasFixedAccess);
                    memberEditors.Add(customEditorAttribute.Editor);
                } else if (TypeClassifier.IsFloatingPointType(memberType))
                    memberEditors.Add(new FloatingPointPresenter(this, compondObject, memberName, value, field, property, effectiveReadonly, hasFixedAccess));
                else if (TypeClassifier.IsSignedIntegerType(memberType))
                    memberEditors.Add(new SignedIntegerPresenter(this, compondObject, memberName, value, field, property, effectiveReadonly, hasFixedAccess));
                else if (TypeClassifier.IsUnsignedIntegerType(memberType))
                    memberEditors.Add(new UnsignedIntegerPresenter(this, compondObject, memberName, value, field, property, effectiveReadonly, hasFixedAccess));
                else if (memberType == typeof(string)) {
                    var attribite = Utility.Reflection.GetAttribute<FilenameEditorAttribute>(member);
                    if (attribite == null)
                        memberEditors.Add(new StringPresenter(this, compondObject, memberName, value?.ToString(), field, property, effectiveReadonly, hasFixedAccess));
                    else
                        memberEditors.Add(new FilenamePresenter(this, compondObject, memberName, value?.ToString(), field, property, effectiveReadonly, attribite, hasFixedAccess));
                } else if (memberType == typeof(bool))
                    memberEditors.Add(new BooleanPresenter(this, compondObject, memberName, (bool)value, field, property, effectiveReadonly, hasFixedAccess));
                else if (memberType.IsEnum)
                    memberEditors.Add(new EnumerationPresenter(this, compondObject, memberName, value, field, property, effectiveReadonly, hasFixedAccess));
            } //ProcessMember
            foreach (var property in properties)
                ProcessMember(property, property, null, property.GetValue(compondObject), property.PropertyType);
            foreach (var field in fields)
                ProcessMember(field, null, field, field.GetValue(compondObject), field.FieldType);
            this.memberEditors = memberEditors.ToArray();
            Populate();
        } //Populate

    } //class FlatObjectEditor

}
