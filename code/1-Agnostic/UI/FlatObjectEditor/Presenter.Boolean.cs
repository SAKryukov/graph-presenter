namespace SA.Agnostic.UI {
    using System.Reflection;
    using System.Windows.Controls;
    using UIElement = System.Windows.UIElement;
    using HorizontalAlignment = System.Windows.HorizontalAlignment;
    using VerticalAlignment = System.Windows.VerticalAlignment;

    class BooleanPresenter : MemberPresenter, IMemberEditor {

        internal BooleanPresenter(FlatObjectEditor editor, object compoundObject, string name, bool initialValue, FieldInfo field, PropertyInfo property, bool isReadonly, bool hasFixedAccess) :
            base(editor, compoundObject, name, initialValue, field, property, isReadonly, hasFixedAccess) { }

        private protected override void ReadonlyImplementation(bool value) {
            base.ReadonlyImplementation(value);
            isReadonly = value;
            checkBox.IsEnabled = !isReadonly;
            editor.ChangeReadonly(this, value);
        } //ReadonlyImplementation

        private protected override bool IsTabStopGetter() => checkBox.IsTabStop;
        private protected override void IsTabStopSetter(bool value) {
            checkBox.IsTabStop = value;
        } //IsTabStopSetter

        internal override void CreateControls() {
            checkBox.IsChecked = (bool)value == true;
            checkBox.Click += (_, _) => editor.InterfaceImplementation.ValidateMember(checkBox);
        } //CreateControls

        bool IMemberEditor.UpdateCompoundObject() {
            if (isReadonly) return false;
            bool state = checkBox.IsChecked == true;
            Utility.Reflection.SetValue(compoundObject, field, property, state);
            value = state;
            return true;
        } //UpdateCompoundObject

        UIElement IMemberEditor.Editor => checkBox;
        UIElement IMemberEditor.FocusNotifier => checkBox;
        readonly CheckBox checkBox = new() {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = DefinitionSet.FlatObjectEditor.marginCheckBox
        };

    } //class BooleanPresenter

}
