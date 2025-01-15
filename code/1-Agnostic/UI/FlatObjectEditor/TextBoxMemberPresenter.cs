namespace SA.Agnostic.UI {
    using System.Reflection;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Brushes = System.Windows.Media.Brushes;
    using UIElement = System.Windows.UIElement;

    abstract class TextBoxMemberPresenter : MemberPresenter, IMemberEditor {

        internal TextBoxMemberPresenter(FlatObjectEditor editor, object compoundObject, string name, object initialValue, FieldInfo field, PropertyInfo property, bool isReadonly, bool hasFixedAccess) :
            base(editor, compoundObject, name, initialValue, field, property, isReadonly, hasFixedAccess) { }

        private protected override void ReadonlyImplementation(bool value) {
            textBox.IsReadOnly = value;
            editor.ChangeReadonly(this, value);
        } //ReadonlyImplementation

        private protected readonly FilteredTextBox textBox = new();
        private protected string InputFilter { get; set; }
        private protected bool Modified { get; set; }

        private protected override bool IsTabStopGetter() => textBox.IsTabStop;
        private protected override void IsTabStopSetter(bool value) {
            textBox.IsTabStop = value;
        } //IsTabStopSetter

        private protected void RethrowStringInputException(System.Exception exception, bool customStringValidatorException = false) {
            System.Exception reportedException = exception;
            if (exception.GetType() == typeof(TargetInvocationException))
                reportedException = exception.InnerException;
            if (customStringValidatorException)
                throw new CustomStringValidatorException(reportedException.Message, textBox.Text);
            else
                throw new ParseException(reportedException.Message, textBox.Text);
        } //RethrowStringInputException

        UIElement IMemberEditor.FocusNotifier => textBox;
        UIElement IMemberEditor.Editor => textBox;

        internal override void CreateControls() {
            textBox.InputFilter = InputFilter;
            textBox.modified += (_, _) => { Modified = true; };
            textBox.Text = value?.ToString();
            textBox.BorderThickness = new System.Windows.Thickness(0);
            textBox.Padding = DefinitionSet.FlatObjectEditor.paddingValue;
            textBox.Background = Brushes.Transparent;
            textBox.Foreground = editor.ColorSchemeOwner.ColorScheme.FlatObjectEditorScheme.FocusControlForeground;
        } //CreateControls

        internal class FilteredTextBox : TextBox {
            internal FilteredTextBox() { ContextMenu = null; }
            internal System.EventHandler modified;
            internal string InputFilter { get; set; }
            protected override void OnPreviewTextInput(TextCompositionEventArgs eventArgs) {
                base.OnPreviewTextInput(eventArgs);
                bool isModified = true;
                if (!string.IsNullOrEmpty(InputFilter))
                    isModified = InputFilter.Contains(eventArgs.Text);
                eventArgs.Handled = !isModified;
                if (isModified && modified != null)
                    modified.Invoke(this, new System.EventArgs());
            } //OnPreviewTextInput
            protected override void OnPreviewKeyDown(KeyEventArgs eventArgs) {
                base.OnPreviewKeyDown(eventArgs);
                if (string.IsNullOrEmpty(InputFilter)) return;
                if (eventArgs.Key == Key.Space && !InputFilter.Contains(DefinitionSet.FlatObjectEditor.space)) { // special case not filtered by OnPreviewTextInput
                    eventArgs.Handled = true;
                    return;
                } //if
                if ((eventArgs.Key == Key.V && eventArgs.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control)) || (eventArgs.Key == Key.Insert && eventArgs.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Shift))) { //Paste                    
                    eventArgs.Handled = true;
                    var clipboardText = System.Windows.Clipboard.GetText();
                    if (string.IsNullOrEmpty(clipboardText))
                        return;
                    foreach (var c in clipboardText)
                        if (!InputFilter.Contains(c))
                            return;
                    var savedSelection = SelectionStart;
                    var newText = Text;
                    newText = newText.Remove(SelectionStart, SelectionLength);
                    newText = newText.Insert(SelectionStart, clipboardText);
                    Text = newText;
                    SelectionStart = savedSelection + clipboardText.Length;
                    SelectionLength = 0;
                } //if clipboard keystroke
            } //OnPreviewKeyDown
        } //class FilteredTextBox

    } //TextBoxMemberPresenter

}
