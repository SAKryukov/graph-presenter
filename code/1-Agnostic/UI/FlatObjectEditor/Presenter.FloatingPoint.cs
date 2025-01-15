namespace SA.Agnostic.UI {
    using System.Reflection;
    using System.Windows.Controls;
    using Key = System.Windows.Input.Key;
    using Brush = System.Windows.Media.Brush;
    using IInputElement = System.Windows.IInputElement;
    using FrameworkElement = System.Windows.FrameworkElement;
    using Visibility = System.Windows.Visibility;
    using Thickness = System.Windows.Thickness;
    using Brushes = System.Windows.Media.Brushes;

    class FloatingPointPresenter : TextBoxButtonsMemberPresenter, IMemberEditor {

        internal FloatingPointPresenter(FlatObjectEditor editor, object compoundObject, string name, object initialValue, FieldInfo field, PropertyInfo property, bool isReadonly, bool hasFixedAccess) :
            base(editor, compoundObject, name, initialValue, field, property, isReadonly, hasFixedAccess) { }

        internal override void CreateControls() {
            var attribite = Utility.Reflection.GetAttribute<FloatingPointDomainAttribute>(field, property);
            if (attribite != null) {
                System.Type type = field?.FieldType ?? property?.PropertyType;
                numericAbstractionLayer = type == typeof(double) ? new DoubleAbstractionImplementation() : new FloatAbstractionImplementation();
                isNaNAccepted = attribite.IsNanAccepted;
                minimum = attribite.Minimum;
                maximum = attribite.Maximum;
                isNegativeInfinityAccepted = numericAbstractionLayer.IsNegativeInfinityInDomain(minimum, maximum);
                isPositiveInfinityAccepted = numericAbstractionLayer.IsPositiveInfinityInDomain(minimum, maximum);
            } //if
            InputFilter = DefinitionSet.FlatObjectEditor.inputFilterFloatingPoint;
            if (minimum < 0)
                InputFilter += DefinitionSet.FlatObjectEditor.inputFilterNegativeSign;
            base.CreateControls();
        } //CreateControls

        private protected override Button[] CreateButtons() {
            popupSelector = new(
                this,
                parent: controlContainer, focusElement: textBox,
                selectedBrushBackground: editor.ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Background.SelectedListBoxItem,
                selectedBrushForeground: editor.ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Foreground.SelectedListBoxItem,
                unselectedBrushBackground: editor.ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Background.ListBoxItem,
                unselectedBrushForeground: editor.ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Foreground.ListBoxItem,
                borderBrush: editor.ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.ListBoxBorder);
            if (isPositiveInfinityAccepted)
                popupSelector.AddListBoxItem(DefinitionSet.FlatObjectEditor.stringFloatingPointPositiveInfinity);
            if (isNegativeInfinityAccepted)
                popupSelector.AddListBoxItem(DefinitionSet.FlatObjectEditor.stringFloatingPointNegativeInfinity);
            if (isNaNAccepted)
                popupSelector.AddListBoxItem(DefinitionSet.FlatObjectEditor.stringFloatingPointNull);
            controlContainer.Children.Add(popupSelector);
            textBox.PreviewKeyDown += (_, eventArgs) => {
                if (eventArgs.SystemKey == Key.Down) {
                    popupSelector.ToggleVisibility();
                    eventArgs.Handled = true;
                } //if
            }; //textBox.KeyDown
            buttonSpecialValue.Click += (_, _) => popupSelector.ToggleVisibility();
            if (popupSelector.Items.Count < 1)
                buttonSpecialValue.Visibility = Visibility.Collapsed;
            buttonSpecialValue.Background = editor.ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Background.Button;
            buttonSpecialValue.Foreground = editor.ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Foreground.Button;
            return new Button[] { buttonSpecialValue };
        } //CreateButtons

        bool IMemberEditor.UpdateCompoundObject() {
            if (!Modified) return false;
            object newValue = null;
            try {
                newValue = Utility.Reflection.Parse(textBox.Text, field, property);
            } catch (System.Exception exception) {
                RethrowStringInputException(exception);
            } //exception
            if (numericAbstractionLayer != null && !numericAbstractionLayer.IsInDomain(newValue, minimum, maximum))
                throw new FloatingPointDomainException(DefinitionSet.FlatObjectEditor.ErrorNumericDomain(minimum, maximum), textBox.Text, minimum, maximum);
            Utility.Reflection.SetValue(compoundObject, field, property, newValue);
            value = newValue;
            return true;
        } //UpdateCompoundObject

        class FloatingPointPopupSelector : PopupSelector{
            internal FloatingPointPopupSelector(
                FloatingPointPresenter owner,
                FrameworkElement parent = null,
                IInputElement focusElement = null,
                Brush selectedBrushBackground = null, Brush selectedBrushForeground = null,
                Brush unselectedBrushBackground = null, Brush unselectedBrushForeground = null,
                Brush borderBrush = null)
                : base(parent, focusElement, selectedBrushBackground, selectedBrushForeground, unselectedBrushBackground, unselectedBrushForeground, borderBrush)
                { this.owner = owner; }
            internal void AddListBoxItem(string value) =>
                Items.Add(new ListBoxItem() {
                    Content = new TextBlock {
                        Text = value,
                        Padding = DefinitionSet.FlatObjectEditor.paddingListBoxContent,
                        Background = owner.editor.ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Background.ListBoxItem,
                        Foreground = owner.editor.ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Foreground.ListBoxItem
                    },
                    HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch,
                    Padding = new Thickness(0),
                    Background = Brushes.Transparent
                });
            private protected override void HandleListBoxSelection(object item, bool selected, Brush background, Brush foreground) {
                ListBoxItem listBoxItem = (ListBoxItem)item;
                TextBlock textBlock = (TextBlock)listBoxItem.Content;
                textBlock.Background = selected
                    ? owner.editor.ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Background.SelectedListBoxItem
                    : owner.editor.ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Background.ListBoxItem;
                textBlock.Foreground = selected
                    ? owner.editor.ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Foreground.SelectedListBoxItem
                    : owner.editor.ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Foreground.ListBoxItem;
            } //HandleListBoxSelection
            private protected override void UpdateChoiceToSelectedContent(ListBox listBox) =>
                owner.textBox.Text = ((TextBlock)(((ListBoxItem)listBox.SelectedItem).Content)).Text;
            private protected override void HandleValidation() =>
                owner.editor.InterfaceImplementation.ValidateMember(owner.textBox);
            readonly FloatingPointPresenter owner;
        } //class FloatingPointPopupSelector
        FloatingPointPopupSelector popupSelector;

        FloatingPointAbstractionLayer numericAbstractionLayer;
        double minimum = double.NegativeInfinity;
        double maximum = double.PositiveInfinity;
        bool isNaNAccepted = true, isNegativeInfinityAccepted = true, isPositiveInfinityAccepted = true;
        readonly Button buttonSpecialValue = new() {
            Content = DefinitionSet.FlatObjectEditor.buttonDropDown,
            ToolTip = DefinitionSet.FlatObjectEditor.toolTipFloatingPointDropdownButton,
            BorderThickness = DefinitionSet.FlatObjectEditor.borderThicknessMemberEditButton
        };

    } //class FloatingPointPresenter

}
