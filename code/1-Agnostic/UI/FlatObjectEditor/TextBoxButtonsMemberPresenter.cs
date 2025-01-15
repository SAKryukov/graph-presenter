namespace SA.Agnostic.UI {
    using System.Reflection;
    using System.Windows.Controls;
    using GridLength = System.Windows.GridLength;
    using GridUnitType = System.Windows.GridUnitType;
    using UIElement = System.Windows.UIElement;
    using Visibility = System.Windows.Visibility;
    using VerticalAlignment = System.Windows.VerticalAlignment;

    abstract class TextBoxButtonsMemberPresenter : TextBoxMemberPresenter, IMemberEditor {

        internal TextBoxButtonsMemberPresenter(FlatObjectEditor editor, object compoundObject, string name, object initialValue, FieldInfo field, PropertyInfo property, bool isReadonly, bool hasFixedAccess) :
            base(editor, compoundObject, name, initialValue, field, property, isReadonly, hasFixedAccess) { }

        UIElement IMemberEditor.Editor => controlContainer;

        private protected override void ReadonlyImplementation(bool value) {
            base.ReadonlyImplementation(value);
            textBox.IsReadOnly = value;
            if (buttons == null) return;
            foreach (var button in buttons)
                button.Visibility = value ? Visibility.Collapsed : Visibility.Visible;
        } //ReadonlyImplementation

        private protected override void IsTabStopSetter(bool value) {
            base.IsTabStopSetter(value);
            foreach (var button in buttons)
                button.IsTabStop = value;
        } //IsTabStopSetter

        internal override void CreateControls() {
            base.CreateControls();
            Button[] buttons = CreateButtons();
            if (buttons == null || buttons.Length < 1) return;
            controlContainer.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            controlContainer.Children.Add(textBox);
            Grid.SetColumn(textBox, 0);
            for (int index = 0; index < buttons.Length; ++index) {
                controlContainer.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                controlContainer.Children.Add(buttons[index]);
                buttons[index].VerticalAlignment = VerticalAlignment.Bottom;
                buttons[index].SizeChanged += (sender, eventArgs) => { ((Button)sender).Width = ((Button)sender).ActualHeight; };
                if (buttons[index].Visibility == Visibility.Visible)
                    buttons[index].Visibility = isReadonly ? Visibility.Collapsed : Visibility.Visible;
                Grid.SetColumn(buttons[index], index + 1);
            } //loop buttons
            this.buttons = buttons;
        } //CreateControls

        private protected abstract Button[] CreateButtons();
        Button[] buttons;

        private protected readonly Grid controlContainer = new();

    } //TextBoxMemberPresenter

}
