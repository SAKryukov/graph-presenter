namespace SA.Agnostic.UI {
    using System.Reflection;
    using System.Windows.Controls;
    using Keyboard = System.Windows.Input.Keyboard;
    using System.IO;

    class FilenamePresenter : TextBoxButtonsMemberPresenter, IMemberEditor {

        internal FilenamePresenter(FlatObjectEditor editor, object compoundObject, string name, string initialValue, FieldInfo field, PropertyInfo property, bool isReadonly, FilenameEditorAttribute dialogDetail, bool hasFixedAccess) :
            base(editor, compoundObject, name, initialValue, field, property, isReadonly, hasFixedAccess) {
            this.dialogDetail = dialogDetail;
        } //FilenamePresenter

        bool IMemberEditor.UpdateCompoundObject() {
            if (!Modified) return false;
            string newValue = textBox.Text;
            Utility.Reflection.SetValue(compoundObject, field, property, newValue);
            value = newValue;
            return true;
        } //UpdateCompoundObject

        private protected override Button[] CreateButtons() {
            textBox.IsReadOnly = true;
            //TODO: think of readonly coloring:
            //textBox.Background = Brushes.LightGray; //SA???
            buttonNullify.Click += (_, _) => {
                textBox.Text = null;
                editor.InterfaceImplementation.ValidateMember(textBox);
                Keyboard.Focus(textBox);
            }; //buttonNullify.Click
            void ActOnBasePath(System.Action<string> whatToDo) {
                    if (dialogDetail.BasePathProvider != null) {
                        string basePath = dialogDetail.BasePathProvider.BasePath;
                        if (Directory.Exists(basePath))
                            whatToDo.Invoke(basePath);
                    } //if
            } //ActOnBasePath
            buttonFileDialog.Click += (_, _) => {
                fileDialog.Filter = dialogDetail.DialogFilter;
                ActOnBasePath((basePath) => {
                    fileDialog.CustomPlaces.Clear();
                    fileDialog.CustomPlaces.Add(new Microsoft.Win32.FileDialogCustomPlace(basePath));
                });
                fileDialog.Title = dialogDetail.FileDialogTitle;
                if (fileDialog.ShowDialog() == true) {
        
                    string newValue = fileDialog.FileName;
                    ActOnBasePath(basePath => {
                        newValue = Path.GetRelativePath(basePath, newValue);
                    });
                    textBox.Text = newValue;
                } //if
            }; //buttonFileDialog.Click
            buttonNullify.Background = editor.ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Background.NullifyButton;
            buttonNullify.Foreground = editor.ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Foreground.NullifyButton;
            buttonFileDialog.Background = editor.ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Background.Button;
            buttonFileDialog.Foreground = editor.ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Foreground.Button;
            return new Button[] { buttonNullify, buttonFileDialog };
        } //CreateButtons

        readonly Button buttonNullify = new() {
            ToolTip = DefinitionSet.FlatObjectEditor.toolTipNullButton,
            Content = DefinitionSet.FlatObjectEditor.buttonMakeNull,
            BorderThickness = new(0)
        };
        readonly Button buttonFileDialog = new() {
            ToolTip = DefinitionSet.FlatObjectEditor.toolTipFileDialogButton,
            Content = DefinitionSet.FlatObjectEditor.buttonEllipsis,
            BorderThickness = DefinitionSet.FlatObjectEditor.borderThicknessMemberEditButton
        };
        readonly Microsoft.Win32.OpenFileDialog fileDialog = new();
        readonly FilenameEditorAttribute dialogDetail;

    } //FilenamePresenter

}
