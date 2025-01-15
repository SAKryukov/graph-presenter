namespace SA.Agnostic.UI {
    using System.Windows.Controls;
    using Popup = System.Windows.Controls.Primitives.Popup;
    using Brush = System.Windows.Media.Brush;
    using IList = System.Collections.IList;
    using IInputElement = System.Windows.IInputElement;
    using FrameworkElement = System.Windows.FrameworkElement;
    using Keyboard = System.Windows.Input.Keyboard;
    using Key = System.Windows.Input.Key;
    using Thickness = System.Windows.Thickness;

    abstract class PopupSelector : Popup {

        internal PopupSelector(
            FrameworkElement parent = null,
            IInputElement focusElement = null,
            Brush selectedBrushBackground = null, Brush selectedBrushForeground = null,
            Brush unselectedBrushBackground = null, Brush unselectedBrushForeground = null,
            Brush borderBrush = null) {
            this.parent = parent;
            this.focusElement = focusElement;
            StaysOpen = false;
            IsOpen = false;
            listBox.BorderThickness = new Thickness(1);
            listBox.BorderBrush = borderBrush;
            listBox.Background = unselectedBrushBackground;
            void ColorSelection(IList list, bool selected) {
                foreach (var item in list)
                    HandleListBoxSelection(item, selected,
                        selected ? selectedBrushBackground : unselectedBrushBackground,
                        selected ? selectedBrushForeground : unselectedBrushForeground);
            } //RecolorSelection
            listBox.SelectionChanged += (sender, eventArgs) => {
                ListBox instance = (ListBox)sender;
                ColorSelection(eventArgs.RemovedItems, false);
                ColorSelection(eventArgs.AddedItems, true);
            }; //listBox.SelectionChanged
            listBox.KeyDown += (sender, eventArg) => {
                if (eventArg.Key == Key.Enter) {
                    ToggleVisibility(updateContent: true);
                    eventArg.Handled = true;
                } else if (eventArg.Key == Key.Escape || eventArg.Key == Key.System || eventArg.Key == Key.LeftCtrl || eventArg.Key == Key.RightCtrl) {
                    ToggleVisibility(updateContent: false);
                    eventArg.Handled = true;
                } //if
            }; //listBox.KeyDown
            listBox.KeyDown += (_, eventArg) => {
                if (eventArg.Key == Key.Enter) {
                    ToggleVisibility(updateContent: true);
                    eventArg.Handled = true;
                } else if (eventArg.Key == Key.Escape || eventArg.Key == Key.System || eventArg.Key == Key.LeftCtrl || eventArg.Key == Key.RightCtrl) {
                    ToggleVisibility(updateContent: false);
                    eventArg.Handled = true;
                } //if
            }; //listBox.KeyDown
            listBox.MouseDoubleClick += (_, _) => {
                ToggleVisibility(updateContent: true);
            }; //listBox.MouseDoubleClick
        } //PopupSelector

        internal void ToggleVisibility(bool updateContent = false, int forceIndex = -1) {
            if (popupIsVisible) {
                if (updateContent) {
                    UpdateChoiceToSelectedContent(listBox);
                    HandleValidation();
                } //if
                IsOpen = false;
                Keyboard.Focus(focusElement);
                lastSelectedIndex = listBox.SelectedIndex;
            } else {
                listBox.Width = parent.ActualWidth;
                Child = listBox;
                IsOpen = true;
                listBox.Focus();
                string toolTipText = ValueToList(listBox)
                    ? DefinitionSet.FlatObjectEditor.toolTipListSelectionCommit
                    : DefinitionSet.FlatObjectEditor.toolTipListSelectionClose;
                listItemToolTip.Content = toolTipText;
                listItemToolTip.Placement = System.Windows.Controls.Primitives.PlacementMode.Right;
                //TODO: create a separate ToolTip utility
                foreach (var child in listBox.Items)
                    ((ListBoxItem)child).ToolTip = listItemToolTip;
                if (listBox.SelectedItem != null)
                    listItemToolTip.PlacementTarget = (ListBoxItem)listBox.SelectedItem;
                if (forceIndex >= 0) {
                    Control focusControl = GetFocusControl(listBox.Items[forceIndex]);
                    if (focusControl != null) {
                        listBox.SelectedIndex = forceIndex;
                        Keyboard.Focus(focusControl);
                    } //if
                } //if
                listItemToolTip.IsOpen = true;
            } //if
            popupIsVisible = !popupIsVisible;
        } //ToggleVisibility

        private protected virtual Control GetFocusControl(object item) => (Control)item;
        private protected virtual bool ValueToList(ListBox instance) {
            int indexToSelect = lastSelectedIndex;
            if (indexToSelect < 0)
                indexToSelect = 0;
            instance.SelectedIndex = indexToSelect;
            if (instance.Items.Count > indexToSelect)
                Keyboard.Focus((ListBoxItem)instance.Items[indexToSelect]);
            return true; // return shows (using a ToolTip) if there is a choice to commit or not on closing; in this case, there is the choice
        } //ValueToList

        private protected abstract void HandleListBoxSelection(object item, bool selected, Brush background, Brush foreground);
        private protected virtual void UpdateChoiceToSelectedContent(ListBox listBox) { }
        private protected virtual void HandleValidation() { }

        internal ItemCollection Items => listBox.Items;

        readonly ListBox listBox = new();
        readonly ToolTip listItemToolTip = new();
        readonly FrameworkElement parent;
        readonly IInputElement focusElement;
        bool popupIsVisible;
        int lastSelectedIndex = -1;

    } //class PopupSelector

}
