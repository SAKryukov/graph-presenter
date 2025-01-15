namespace SA.Agnostic.UI {
    using System.Reflection;
    using Button = System.Windows.Controls.Button;
    using Keyboard = System.Windows.Input.Keyboard;
    using Visibility = System.Windows.Visibility;

    class StringPresenter : TextBoxButtonsMemberPresenter, IMemberEditor {

        internal StringPresenter(FlatObjectEditor editor, object compoundObject, string name, string initialValue, FieldInfo field, PropertyInfo property, bool isReadonly, bool hasFixedAccess) :
            base(editor, compoundObject, name, initialValue, field, property, isReadonly, hasFixedAccess) { }

        private protected override Button[] CreateButtons() {
            StringDomainAttribute attribute = Utility.Reflection.GetAttribute<StringDomainAttribute>(field, property);
            bool isMultiline = false;
            if (attribute != null) {
                validator = attribute.Validator;
                isNullAccepted = attribute.IsNullAccepted;
                isMultiline = attribute.IsMultiline;
            } //if
            textBox.AcceptsReturn = isMultiline;
            buttonNullify.Visibility = isNullAccepted ? Visibility.Visible : Visibility.Collapsed;
            buttonNullify.Background = editor.ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Background.NullifyButton;
            buttonNullify.Foreground = editor.ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Foreground.NullifyButton;
            buttonNullify.Click += (_, _) => {
                textBox.Text = null;
                editor.InterfaceImplementation.ValidateMember(textBox);
                Keyboard.Focus(textBox);
            }; //buttonNullify.Click
            return new Button[] { buttonNullify };
        } //CreateButtons

        bool IMemberEditor.UpdateCompoundObject() {
            if (!Modified) return false;
            string newValue = textBox.Text;
            if (validator != null) {
                System.Exception exception = validator.Validate(newValue);
                if (exception != null) RethrowStringInputException(exception);
            } //if
            Utility.Reflection.SetValue(compoundObject, field, property, newValue);
            value = newValue;
            return true;
        } //UpdateCompoundObject

        readonly Button buttonNullify = new() {
            ToolTip = DefinitionSet.FlatObjectEditor.toolTipNullButton,
            Content = DefinitionSet.FlatObjectEditor.buttonMakeNull,
            BorderThickness = new(0)
        };
        IStringValidator validator;
        bool isNullAccepted = true;

    } //StringPresenter

}
