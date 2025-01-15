namespace SA.Agnostic.UI {
    using System.Reflection;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Type = System.Type;
    using Enum = System.Enum;
    using Key = System.Windows.Input.Key;
    using Brush = System.Windows.Media.Brush;
    using IInputElement = System.Windows.IInputElement;
    using FrameworkElement = System.Windows.FrameworkElement;
    using Thickness = System.Windows.Thickness;
    using Brushes = System.Windows.Media.Brushes;
    using EnumList = System.Collections.Generic.List<System.Enum>;

    class EnumerationPresenter : TextBoxButtonsMemberPresenter, IMemberEditor {

        internal EnumerationPresenter(FlatObjectEditor editor, object compoundObject, string name, object initialValue, FieldInfo field, PropertyInfo property, bool isReadonly, bool hasFixedAccess) :
            base(editor, compoundObject, name, initialValue, field, property, isReadonly, hasFixedAccess) { }

        private protected override Button[] CreateButtons() {
            type = field?.FieldType ?? property?.PropertyType;
            isBitwise = System.Attribute.IsDefined(type, typeof(System.FlagsAttribute));
            buttonSelect.ToolTip = DefinitionSet.FlatObjectEditor.ToolTipEnumerationSelectionButton(isBitwise, Utility.Reflection.DisplayName(type));
            popupSelector = new(
                this,
                parent: controlContainer, focusElement: textBox,
                selectedBrushBackground: editor.ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Background.SelectedListBoxItem,
                selectedBrushForeground: editor.ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Foreground.SelectedListBoxItem,
                unselectedBrushBackground: editor.ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Background.ListBoxItem,
                unselectedBrushForeground: editor.ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Foreground.ListBoxItem,
                borderBrush: editor.ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.ListBoxBorder);
            var fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach (var field in fields) {
                if (System.Attribute.IsDefined(field, typeof(HiddenAttribute))) continue;
                string name = Utility.Reflection.DisplayName(field);
                if (isBitwise)
                    popupSelector.AddFlagListBoxItem(name, field.GetValue(null));
                else
                    popupSelector.AddListBoxItem(name, field.GetValue(null));
            } //loop
            controlContainer.Children.Add(popupSelector);
            textBox.PreviewKeyDown += (_, eventArgs) => {
                if (eventArgs.SystemKey == Key.Down) {
                    popupSelector.ToggleVisibility();
                    eventArgs.Handled = true;
                } //if
            }; //textBox.KeyDown
            textBox.PreviewTextInput += (_, eventArgs) => {
                if (eventArgs.Text.Length != 1) return;
                char id = (char)eventArgs.Text[0];
                popupSelector.FindItem(id);
            }; //textBox.PreviewTextInput
            buttonSelect.Click += (_, _) => popupSelector.ToggleVisibility();
            textBox.IsReadOnly = true;
            textBox.Text = Utility.Reflection.DisplayName((Enum)value);
            buttonSelect.Background = editor.ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Background.Button;
            buttonSelect.Foreground = editor.ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Foreground.Button;
            return new Button[] { buttonSelect };
        } //CreateButtons

        readonly Button buttonSelect = new() {
            Content = DefinitionSet.FlatObjectEditor.buttonDropDown,
            BorderThickness = DefinitionSet.FlatObjectEditor.borderThicknessMemberEditButton
        }; //buttonSelect

        bool IMemberEditor.UpdateCompoundObject() {
            Utility.Reflection.SetValue(compoundObject, field, property, value);
            return true;
        } //UpdateCompoundObject

        struct ListBoxItemStructure {
            internal ListBoxItem listBoxItem;
            internal TextBlock textBlock;
            internal Panel panel;
            internal CheckBox checkBox;
            internal Enum enumerationValue;
        } //struct ListBoxItemStructure

        class EnumerationPopupSelector : PopupSelector {
            internal EnumerationPopupSelector(
                EnumerationPresenter owner,
                FrameworkElement parent = null,
                IInputElement focusElement = null,
                Brush selectedBrushBackground = null, Brush selectedBrushForeground = null,
                Brush unselectedBrushBackground = null, Brush unselectedBrushForeground = null,
                Brush borderBrush = null)
                : base(parent, focusElement, selectedBrushBackground, selectedBrushForeground, unselectedBrushBackground, unselectedBrushForeground, borderBrush) { this.owner = owner; }
            internal void FindItem(char id) {
                string stringId = id.ToString().ToLower();
                int index = 0;
                foreach (var item in Items) {
                    var structure = GetListBoxItemStructure(item);
                    string name = Utility.Reflection.DisplayName(structure.enumerationValue).ToLower();
                    bool found = name.StartsWith(stringId);
                    if (found) {
                        ToggleVisibility(false, forceIndex: index);
                        return;
                    } //if
                    ++index;
                } //loop
            } //FindItem
            internal void AddListBoxItem(string value, object enumerationValue) =>
                Items.Add(new ListBoxItem() {
                    Content = new TextBlock {
                        Tag = enumerationValue,
                        Text = value,
                        Padding = DefinitionSet.FlatObjectEditor.paddingListBoxContent,
                        Background = owner.editor.ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Background.ListBoxItem,
                        Foreground = owner.editor.ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Foreground.ListBoxItem
                    },
                    HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch,
                    Padding = new Thickness(0),
                    Background = Brushes.Transparent
                });
            internal void AddFlagListBoxItem(string value, object enumerationValue) {
                StackPanel panel = new() {
                    Orientation = Orientation.Horizontal
                };
                CheckBox checkbox = new() {
                    VerticalAlignment = System.Windows.VerticalAlignment.Center,
                    Margin = DefinitionSet.FlatObjectEditor.paddingValue
                };
                TextBlock textBlock = new() {
                    Tag = enumerationValue,
                    VerticalAlignment = System.Windows.VerticalAlignment.Center,
                    Text = value,
                    Background = Brushes.Transparent,
                    Foreground = owner.editor.ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Foreground.ListBoxItem
                };
                panel.Children.Add(checkbox);
                panel.Children.Add(textBlock);
                ListBoxItem listBoxItem = new() {
                    Content = panel,
                    HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch,
                    Padding = new Thickness(0),
                    Background = Brushes.Transparent
                };
                Items.Add(listBoxItem);
                checkbox.Click += (_, _) => CheckBoxClickHandler();
                listBoxItem.KeyDown += (_, eventArgs) => {
                    if (eventArgs.Key == Key.Space) {
                        checkbox.IsChecked = !checkbox.IsChecked;
                        CheckBoxClickHandler();
                        eventArgs.Handled = true;
                    } //if
                }; //listBoxItem.KeyDown
            } //AddFlagListBoxItem
            void CheckBoxClickHandler() {
                EnumList list = new();
                foreach (var listItem in Items) {
                    var structure = GetListBoxItemStructure(listItem);
                    if (structure.checkBox.IsChecked == true)
                        list.Add(structure.enumerationValue);
                } //loop
                Enum result = Utility.Reflection.Or(list.ToArray());
                owner.textBox.Text = Utility.Reflection.DisplayName(result);
                owner.value = result;
                owner.editor.InterfaceImplementation.ValidateMember(owner.textBox);
            } //CheckBoxClickHandler
            ListBoxItemStructure GetListBoxItemStructure(object listBoxItem) {
                var result = new ListBoxItemStructure {
                    listBoxItem = (ListBoxItem)listBoxItem
                };
                if (owner.isBitwise) {
                    result.panel = (Panel)result.listBoxItem.Content;
                    result.checkBox = (CheckBox)result.panel.Children[0];
                    result.textBlock = (TextBlock)result.panel.Children[1];
                } else
                    result.textBlock = (TextBlock)result.listBoxItem.Content;
                result.enumerationValue = (Enum)result.textBlock.Tag;
                return result;
            } //GetListBoxItemStructure
            private protected override void HandleListBoxSelection(object item, bool selected, Brush background, Brush foreground) {
                ListBoxItemStructure structure = GetListBoxItemStructure(item);
                if (owner.isBitwise) {
                    structure.panel.Background = selected
                        ? owner.editor.ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Background.SelectedListBoxItem
                        : owner.editor.ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Background.ListBoxItem;
                    structure.textBlock.Foreground = selected
                        ? owner.editor.ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Foreground.SelectedListBoxItem
                        : owner.editor.ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Foreground.ListBoxItem;
                } else {
                    structure.textBlock.Background = selected
                        ? owner.editor.ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Background.SelectedListBoxItem
                        : owner.editor.ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Background.ListBoxItem;
                    structure.textBlock.Foreground = selected
                        ? owner.editor.ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Foreground.SelectedListBoxItem
                        : owner.editor.ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Foreground.ListBoxItem;
                } //if
                if (selected && !owner.isBitwise) {
                    Enum newValue = structure.enumerationValue;
                    owner.textBox.Text = Utility.Reflection.DisplayName(newValue);
                    owner.value = newValue;
                    owner.editor.InterfaceImplementation.ValidateMember(owner.textBox);
                } //if
            } //HandleListBoxSelection
            private protected override bool ValueToList(ListBox listBox) {
                base.ValueToList(listBox);
                return false; // return shows (using a ToolTip) if there is a choice to commit or not on closing; in this case, no choice, compoundObject is already updated
            } //ValueToList
            readonly EnumerationPresenter owner;
        } //class EnumerationPopupSelector
        EnumerationPopupSelector popupSelector;

        Type type;
        bool isBitwise;

        //TODO: design keyboard enter on the textBox; it should search for the items by key down events

    } //class EnumerationPresenter

}
