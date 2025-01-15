namespace SA.Agnostic.UI {
    using System.Reflection;
    using Type = System.Type;

    class SignedIntegerPresenter : TextBoxMemberPresenter, IMemberEditor {

        internal SignedIntegerPresenter(FlatObjectEditor editor, object compoundObject, string name, object initialValue, FieldInfo field, PropertyInfo property, bool isReadonly, bool hasFixedAccess) :
            base(editor, compoundObject, name, initialValue, field, property, isReadonly, hasFixedAccess) { }

        internal override void CreateControls() {
            domainAttribute = Utility.Reflection.GetAttribute<SignedIntegerDomainAttribute>(field, property);
            InputFilter = DefinitionSet.FlatObjectEditor.inputFilterNumeric;
            if (domainAttribute == null || (domainAttribute != null && domainAttribute.Minimum < 0))
                InputFilter += DefinitionSet.FlatObjectEditor.inputFilterNegativeSign;
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
            long compatibleValue = System.Convert.ToInt64(newValue);
            if (domainAttribute != null && !(domainAttribute.Minimum <= compatibleValue && compatibleValue <= domainAttribute.Maximum))
                throw new SignedIntegerDomainException(DefinitionSet.FlatObjectEditor.ErrorNumericDomain(domainAttribute.Minimum, domainAttribute.Maximum), textBox.Text, domainAttribute.Minimum, domainAttribute.Maximum);
            Utility.Reflection.SetValue(compoundObject, field, property, newValue);
            value = newValue;
            return true;
        } //IMemberEditor.UpdateCompoundObject

        SignedIntegerDomainAttribute domainAttribute;

    } //class SignedIntegerPresenter


}

