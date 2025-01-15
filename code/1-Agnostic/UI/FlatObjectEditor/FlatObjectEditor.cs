namespace SA.Agnostic.UI {
    using System.Windows.Controls;
    using System.Windows.Input;
    using GridLength = System.Windows.GridLength;
    using GridUnitType = System.Windows.GridUnitType;
    using Visibility = System.Windows.Visibility;
    using UIElement = System.Windows.UIElement;
    using BorderList = System.Collections.Generic.List<System.Windows.Controls.Border>;
    using FocusNotificationDictionary = System.Collections.Generic.Dictionary<System.Windows.UIElement, int>;
    using VerticalAlignment = System.Windows.VerticalAlignment;
    using HorizontalAlignment = System.Windows.HorizontalAlignment;
    using TextAlignment = System.Windows.TextAlignment;
    using Thickness = System.Windows.Thickness;

    public partial class FlatObjectEditor : Border, IFlatObjectEditor {

        public object Target {
            get { return target; }
            set {
                if (target == value) return;
                target = value;
                Populate(target);
            } //set Target
        } //Target

        public bool IsReadonly {
            get => isReadonly;
            set {
                if (value == isReadonly) return;
                isReadonly = value;
                if (memberEditors != null)
                    foreach (var editor in memberEditors)
                        if (!editor.HasFixedAccess)
                            editor.IsReadonly = isReadonly; 
            } //set IsReadonly
        } //IsReadonly

        public void FocusMember(int index) {
            if (index < 0 || index >= memberEditors.Length) return;
            var focusNotifier = memberEditors[index].FocusNotifier;
            if (focusNotifier != null)
                Keyboard.Focus(focusNotifier);
        } //FocusMember

        public int SelectedIndex {
            get => selectedIndex;
            set {
                if (memberEditors.Length < 1) {
                    selectedIndex = -1;
                    return;
                } //if
                if (value < 0) return;
                if (value >= memberEditors.Length) return;
                if (value == selectedIndex) return;
                selectedIndex = value;
                for (int index = 0; index < memberEditors.Length; ++index) {
                    memberEditors[index].IsTabStop = index == selectedIndex;
                    GetGridContainer(index, isValue: true).Background = index == selectedIndex
                        ? ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Highligting.Selected
                        : ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Highligting.Normal;
                }
            } //set SelectedIndex
        } //SelectedIndex

        public FlatObjectEditor() {
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0, GridUnitType.Auto) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            splitter.Initialized += (sender, _) => {
                var instance = (GridSplitter)sender;
                instance.BorderBrush = ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Border;
                instance.Background = ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.SplitterBackground;
            }; //splitter.Initialized
            splitter.MouseEnter += (sender, _) => {
                var instance = (GridSplitter)sender;
                instance.Background = ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.SplitterHoverBackground;
            }; //splitter.MouseEnter
            splitter.MouseLeave += (sender, _) => {
                var instance = (GridSplitter)sender;
                instance.Background = ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.SplitterBackground;
            }; //splitter.MouseLeave
            scrollViewer.Content = grid;
            Child = scrollViewer;
            VerticalAlignment = VerticalAlignment.Top;
            BorderBrush = ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Border;
        } //FlatObjectEditor

        void Populate() {
            focusNotificationDictionary.Clear();
            grid.Children.Clear();
            if (target == null) return;
            if (memberEditors == null || memberEditors.Length < 1) {
                Visibility = Visibility.Collapsed;
                return;
            } //if
            BorderList valueReadonlyFilters = new();
            for (int index = 0; index < memberEditors.Length; ++index) {
                bool last = index == memberEditors.Length - 1;
                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0, GridUnitType.Auto) });
                TextBlock nameTextBlock = new() {
                    TextAlignment = TextAlignment.Right,
                    Text = memberEditors[index].Name,
                    Foreground = ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Foreground.Label
                };
                Border nameBorder = new() {
                    Background = ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Background.Label,
                    BorderBrush = ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Border,
                    BorderThickness = new Thickness(0, 0, 0, last ? 0 : 1),
                    Padding = DefinitionSet.FlatObjectEditor.paddingName,
                    Child = nameTextBlock
                };
                Grid valueGrid = new();
                Grid.SetColumn(nameBorder, 0);
                Grid.SetRow(nameBorder, index);
                grid.Children.Add(nameBorder);
                Border valueBorder = new() {
                    BorderThickness = new Thickness(0, 0, 0, last ? 0 : 1),
                    BorderBrush = ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Border,
                    Background = ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Highligting.Normal,
                    Child = valueGrid
                };
                UIElement editorControl = memberEditors[index].Editor;
                UIElement focusNotifier = memberEditors[index].FocusNotifier;
                focusNotificationDictionary.Add(focusNotifier, index);
                void HandleSelectionUpDown(bool up = false) {
                    if (up && selectedIndex > 0)
                        Keyboard.Focus(memberEditors[selectedIndex - 1].FocusNotifier);
                    else if (!up && selectedIndex < memberEditors.Length - 1)
                        Keyboard.Focus(memberEditors[selectedIndex + 1].FocusNotifier);
                } //HandleSelectionUpDown
                focusNotifier.PreviewKeyDown += (_, eventArgs) => {
                    if (eventArgs.Key == Key.Down) {
                        HandleSelectionUpDown(up: false);
                        eventArgs.Handled = true;
                    } else if (eventArgs.Key == Key.Up) {
                        HandleSelectionUpDown(up: true);
                        eventArgs.Handled = true;
                    } //if
                }; //focusNotifier.PreviewKeyDown
                focusNotifier.GotKeyboardFocus += (sender, _) => {
                    if (!focusNotificationDictionary.TryGetValue((UIElement)sender, out int indexInList))
                        return;
                    SelectedIndex = indexInList;
                    GetGridContainer(indexInList, isValue: true).Background = ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Highligting.Focused;
                }; //focusNotifier.GotKeyboardFocus
                focusNotifier.LostKeyboardFocus += (sender, _) => HandleMemberValidation((UIElement)sender);
                Border valueReadonlyFilter = new() {
                    Background = ColorSchemeOwner.ColorScheme.FlatObjectEditorScheme.Highligting.Readonly,
                    Opacity = 0,
                    IsHitTestVisible = false };
                valueGrid.Children.Add(editorControl);
                valueGrid.Children.Add(valueReadonlyFilter);
                valueReadonlyFilters.Add(valueReadonlyFilter);
                ShowReadonly(valueReadonlyFilter, memberEditors[index].IsReadonly);
                Grid.SetColumn(valueBorder, 2);
                Grid.SetRow(valueBorder, index);
                grid.Children.Add(valueBorder);
            } //loop
            this.valueReadonlyFilters = valueReadonlyFilters.ToArray();
            grid.Children.Add(splitter);
            Grid.SetColumn(splitter, 1);
            Grid.SetRowSpan(splitter, memberEditors.Length);
            Height = splitter.Height;
            Visibility = Visibility.Visible;
            SelectedIndex = 0;
        } //Populate

        void IFlatObjectEditor.ValidateMember(UIElement sender) => HandleMemberValidation(sender);

        void HandleMemberValidation(UIElement sender) {
            if (!focusNotificationDictionary.TryGetValue(sender, out int indexInList))
                return;
            GetGridContainer(indexInList, isValue: true).Background = indexInList == selectedIndex
                ? ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Highligting.Selected
                : ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Highligting.Normal;
            try {
                bool actuallyUpdated = memberEditors[indexInList].UpdateCompoundObject();
                SetError(null, indexInList);
                if (actuallyUpdated && Committed != null)
                    Committed.Invoke(this, new CommittedEventArgs() {
                        MemberIndex = indexInList,
                        MemberEditor = memberEditors[indexInList] });
            } catch (System.Exception exception) {
                System.Exception reportedException = exception;
                if (exception.GetType() == typeof(System.Reflection.TargetInvocationException))
                    reportedException = exception.InnerException;
                SetError(reportedException, indexInList);
                if (ValidationFailure != null)
                    ValidationFailure.Invoke(this, new ValidationFailureEventArgs() {
                        MemberIndex = indexInList,
                        MemberEditor = memberEditors[indexInList],
                        Exception = reportedException });
            } //exception
        } //HandleMemberValidation

        internal void HightlightValidationFailure(ValidationFailureEventArgs eventArgs) {
            if (eventArgs == null) return;
            if (eventArgs.MemberIndex >= memberEditors.Length) return;
            Keyboard.Focus(memberEditors[eventArgs.MemberIndex].FocusNotifier);
        } //HightlightValidationFailure

        Border GetGridContainer(int index, bool isValue = false) => (Border)grid.Children[index * 2 + (isValue ? 1 : 0)];

        void SetError(System.Exception exception, int index) {
            var container = GetGridContainer(index, isValue: false);
            container.Background = exception == null
                ? ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Background.Label
                : ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Background.LabelError;
            ((TextBlock)container.Child).Foreground = exception == null
                ? ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Foreground.Label
                : ColorSchemeOwner?.ColorScheme.FlatObjectEditorScheme.Foreground.LabelError;
            container.ToolTip = exception?.Message; //SA!!! important: not string.Empty but null
        } //SetError

        //TODO: take into account special readonly cases: global filter and special permissions attribute

        public IColorScheme ColorSchemeOwner {
            get {
                if (colorSchemeOwner == null) colorSchemeOwner = new EmbeddedColorSchemeOwner();
                return colorSchemeOwner;
            } //get ColorSchemeOwner
            set {
                if (value == colorSchemeOwner) return;
                colorSchemeOwner = value;
            } //set ColorSchemeOwner
        } //ColorSchemeOwner
        IColorScheme colorSchemeOwner;
        class EmbeddedColorSchemeOwner : IColorScheme {
            public ColorScheme ColorScheme { get => colorScheme; }
            internal ColorScheme colorScheme = ColorScheme.Default;
        } //class EmbeddedColorSchemeOwner

        internal void ChangeReadonly(IMemberEditor editor, bool makeReadonly) {
            if (!focusNotificationDictionary.TryGetValue((UIElement)editor.FocusNotifier, out int indexInList))
                return;
            if (valueReadonlyFilters != null && valueReadonlyFilters.Length > indexInList)
                ShowReadonly(valueReadonlyFilters[indexInList], makeReadonly);
        } //ChangeReadonly

        void ShowReadonly(Border border, bool value) {
            border.Opacity = value ? ColorSchemeOwner.ColorScheme.FlatObjectEditorScheme.Highligting.ReadonlyOpacity : 0;
        } //ShowReadonly

        readonly ScrollViewer scrollViewer = new() { HorizontalScrollBarVisibility = ScrollBarVisibility.Auto, VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
        readonly Grid grid = new();
        readonly GridSplitter splitter = new() {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            BorderThickness = DefinitionSet.FlatObjectEditor.borderThicknessSplitter,
            Width = DefinitionSet.splitterWidth,
            ResizeDirection = GridResizeDirection.Columns
        };
        object target;
        bool isReadonly;
        IMemberEditor[] memberEditors;
        readonly FocusNotificationDictionary focusNotificationDictionary = new();
        Border[] valueReadonlyFilters;
        int selectedIndex = -1;

        internal IFlatObjectEditor InterfaceImplementation => this;

    } //class FlatObjectEditor

}
