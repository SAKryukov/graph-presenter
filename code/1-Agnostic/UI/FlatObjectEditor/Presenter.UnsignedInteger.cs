namespace SA.Agnostic.UI {
    using System.Reflection;

    class UnsignedIntegerPresenter : TextBoxMemberPresenter, IMemberEditor {

        internal UnsignedIntegerPresenter(FlatObjectEditor editor, object compoundObject, string name, object initialValue, FieldInfo field, PropertyInfo property, bool isReadonly, bool hasFixedAccess) :
            base(editor, compoundObject, name, initialValue, field, property, isReadonly, hasFixedAccess) { }

        internal override void CreateControls() {
            domainAttribute = Utility.Reflection.GetAttribute<UnsignedIntegerDomainAttribute>(field, property);
            InputFilter = DefinitionSet.FlatObjectEditor.inputFilterNumeric;
            base.CreateControls();
        } //CreateControls

        bool IMemberEditor.UpdateCompoundObject() {
            if (!Modified) return false;
            object newValue = null;
            try {
                newValue = Utility.Reflection.Parse(textBox.Text, field, property);
            } catch (System.Exception exception) {
                RethrowStringInputException(exception);
            } //exception
            ulong compatibleValue = System.Convert.ToUInt64(newValue);
            if (domainAttribute != null && !(domainAttribute.Minimum <= compatibleValue && compatibleValue <= domainAttribute.Maximum))
                throw new UnsignedIntegerDomainException(DefinitionSet.FlatObjectEditor.ErrorNumericDomain(domainAttribute.Minimum, domainAttribute.Maximum), textBox.Text, domainAttribute.Minimum, domainAttribute.Maximum);
            Utility.Reflection.SetValue(compoundObject, field, property, newValue);
            value = newValue;
            return true;
        } //IMemberEditor.UpdateCompoundObject

        UnsignedIntegerDomainAttribute domainAttribute;

    } //class UnsignedIntegerPresenter

}
