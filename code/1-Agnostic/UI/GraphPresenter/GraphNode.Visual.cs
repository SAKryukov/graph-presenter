namespace SA.Agnostic.UI {
    using System.Windows.Controls;
    using Visibility = System.Windows.Visibility;
    using UIElement = System.Windows.UIElement;
    using KeyboardFocusChangedEventArgs = System.Windows.Input.KeyboardFocusChangedEventArgs;
    using VerticalAlignment = System.Windows.VerticalAlignment;
    using System.Windows.Media;
    using System.Windows.Input;

    internal partial class GraphNode {

        class GraphNodeVisualStructure : StackPanel {
            internal GraphNodeVisualStructure(GraphNode owner) {
                this.owner = owner;
                associationIndicator = new AssociationIndicator(owner);
                Orientation = Orientation.Horizontal;
                Children.Add(typeImage);
                Children.Add(memberName);
                Children.Add(title);
                Children.Add(inplaceEditedTitle);
                void EscapeInlineEditor(bool commit) {
                    owner.InplaceTextEditorMode = false;
                    if (!commit) return;
                    IStringValidator validator = null;
                    StringDomainAttribute attribute = Utility.Reflection.GetAttribute<StringDomainAttribute>(owner.titleField, owner.titleProperty);
                    if (attribute != null)
                        validator = attribute.Validator;
                    var exception = validator?.Validate(inplaceEditedTitle.Text);
                    if (exception == null) {
                        Utility.Reflection.SetValue(owner.target, owner.titleField, owner.titleProperty, inplaceEditedTitle.Text);
                        title.Text = inplaceEditedTitle.Text;
                        owner.owner.FlatObjectEditor.Target = null;
                        owner.owner.FlatObjectEditor.Target = owner.target;
                    } else
                        owner.owner.InvokeValidationFailure(exception);
                } //EscapeInlineEditor
                inplaceEditedTitle.KeyDown += (_, eventArgs) => {
                    bool commit = eventArgs.KeyboardDevice.Modifiers == ModifierKeys.None && eventArgs.Key == Key.Enter;
                    bool escape = eventArgs.KeyboardDevice.Modifiers == ModifierKeys.None && eventArgs.Key == Key.Escape;
                    if (escape || commit)
                        EscapeInlineEditor(commit);
                    if (commit)
                        eventArgs.Handled = true;
                }; //inplaceEditedTitle.KeyDown
                inplaceEditedTitle.LostKeyboardFocus += (_, _) => EscapeInlineEditor(false);
                Children.Add(stateImage);
                Children.Add(associationImage);
                associationImage.Child = associationIndicator.Image;
            } //GraphNodeVisualStructure
            internal Border typeImage = new() { Visibility = Visibility.Collapsed };
            //TODO: define thickness font weight for typeName and title in DefinitionSet
            internal TextBlock memberName = new() {
                Padding = DefinitionSet.GraphPresenter.NodeStructure.typeNamePadding,
                FontWeight = DefinitionSet.GraphPresenter.NodeStructure.typeNameFontWeight
            };
            internal TextBlock title = new() {
                Padding = DefinitionSet.GraphPresenter.NodeStructure.titlePadding,
                FontWeight = DefinitionSet.GraphPresenter.NodeStructure.titleFontWeight,
                Visibility = Visibility.Collapsed,
            };
            internal TextBox inplaceEditedTitle = new() {
                Visibility = Visibility.Collapsed,
                Padding = DefinitionSet.GraphPresenter.NodeStructure.titleInplacePadding,
                FontWeight = DefinitionSet.GraphPresenter.NodeStructure.titleInplaceFontWeight,
            };
            internal Border stateImage = new() { Visibility = Visibility.Collapsed };
            internal Border associationImage = new() { Visibility = Visibility.Collapsed };
            internal INodeStateIndicator associationIndicator;
            internal GraphNode owner;
        } //class GraphNodeVisualStructure

        GraphNodeVisualStructure SetupVisualStructure(GraphNode owner) {
            visualStructure = new GraphNodeVisualStructure(owner);
            Background = Background = Brushes.Transparent;
            Header = visualStructure;
            return visualStructure;
        } //SetupVisualStructur

        internal void SetupVisualStructure(UIElement typeImage = null, string memberName = null, string title = null, UIElement stateImage = null, bool isAssociation = false, object value = null) {
            visualStructure.typeImage.Child = typeImage;
            visualStructure.memberName.Text = memberName;
            Title = title;
            visualStructure.stateImage.Child = stateImage;
            //TODO: call SetupVisualStructure depending on isAssociation
            if (isAssociation)
                if (value == null) {
                    visualStructure.stateImage.Visibility = Visibility.Visible;
                    visualStructure.stateImage.Child = new TextBlock() {
                        Text = DefinitionSet.GraphPresenter.NodeStructure.emptyAssociationIndicator,
                        VerticalAlignment = VerticalAlignment.Center,
                        FontWeight = DefinitionSet.GraphPresenter.NodeStructure.emptyAssociationIndicatorFontWeight,
                        Margin = DefinitionSet.GraphPresenter.NodeStructure.emptyAssociationIndicatorMargin
                    };
                } else
                    visualStructure.associationImage.Visibility = Visibility.Visible;
        } //SetupVisualStructure

        internal string Title {
            get => visualStructure.title.Text;
            set {
                if (value == visualStructure.title.Text) return;
                visualStructure.title.Visibility = value == null ? Visibility.Collapsed : Visibility.Visible;
                visualStructure.title.Text = value;
            } //set Title
        } //Title

        internal string MemberName {
            get => visualStructure.memberName.Text;
            set {
                if (value == visualStructure.memberName.Text) return;
                visualStructure.memberName.Visibility = value == null ? Visibility.Collapsed : Visibility.Visible;
                visualStructure.memberName.Text = value;
            } //set Title
        } //MemberName

        internal bool InplaceTextEditorMode {
            set {
                if (inplaceTextEditorMode == value) return;
                inplaceTextEditorMode = value;
                visualStructure.title.Visibility = value ? Visibility.Collapsed : Visibility.Visible;
                visualStructure.inplaceEditedTitle.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                if (value) {
                    string text = Utility.Reflection.GetValue(target, titleField, titleProperty).ToString();
                    visualStructure.inplaceEditedTitle.Text = text;
                    if (text != null)
                        visualStructure.inplaceEditedTitle.SelectionStart = text.Length;
                    visualStructure.inplaceEditedTitle.SelectionLength = 0;
                    Keyboard.Focus(visualStructure.inplaceEditedTitle);
                } else
                    Keyboard.Focus(this);
            } //set InplaceTextEditorMode 
        } //InplaceTextEditorMode
        bool inplaceTextEditorMode;

        void SetColors() {
            if (owner == null) return;
            bool keyboardFocused = IsKeyboardFocused, selected = IsSelected, enabled = IsEnabled;
            visualStructure.memberName.Background = enabled
                ? (keyboardFocused
                    ? owner.ColorScheme.GraphViewScheme.TypeBackground.KeyboardFocused
                    : (selected
                        ? owner.ColorScheme.GraphViewScheme.TypeBackground.Selected
                        : owner.ColorScheme.GraphViewScheme.TypeBackground.Normal))
                : owner.ColorScheme.GraphViewScheme.TypeBackground.Disabled;
            visualStructure.memberName.Foreground = enabled
                ? (keyboardFocused
                    ? owner.ColorScheme.GraphViewScheme.TypeForeground.KeyboardFocused
                    : (selected
                        ? owner.ColorScheme.GraphViewScheme.TypeForeground.Selected
                        : owner.ColorScheme.GraphViewScheme.TypeForeground.Normal))
                : owner.ColorScheme.GraphViewScheme.TypeForeground.Disabled;
            visualStructure.title.Background = enabled
                ? (keyboardFocused
                    ? owner.ColorScheme.GraphViewScheme.TitleBackground.KeyboardFocused
                    : (selected
                        ? owner.ColorScheme.GraphViewScheme.TitleBackground.Selected
                        : owner.ColorScheme.GraphViewScheme.TitleBackground.Normal))
                : owner.ColorScheme.GraphViewScheme.TitleBackground.Disabled;
            visualStructure.title.Foreground = enabled
                ? (keyboardFocused
                    ? owner.ColorScheme.GraphViewScheme.TitleForeground.KeyboardFocused
                    : (selected
                        ? owner.ColorScheme.GraphViewScheme.TitleForeground.Selected
                        : owner.ColorScheme.GraphViewScheme.TitleForeground.Normal))
                : owner.ColorScheme.GraphViewScheme.TitleForeground.Disabled;
            visualStructure.typeImage.Background = visualStructure.memberName.Background;
            visualStructure.stateImage.Background = visualStructure.title.Background;
            visualStructure.associationImage.Background = visualStructure.title.Background;
            if (AsAssociationGraphNode != null && associationTarget != null)
                visualStructure.associationIndicator.SetState(selected, keyboardFocused, enabled, default);
        } //SetColors

        protected override void OnRender(DrawingContext drawingContext) {
            base.OnRender(drawingContext);
            visualStructure.associationImage.Height = visualStructure.memberName.ActualHeight;
            SetColors();
        } //OnRender

        protected override void OnPreviewGotKeyboardFocus(KeyboardFocusChangedEventArgs e) {
            base.OnPreviewGotKeyboardFocus(e);
            SetColors();
        } //OnPreviewGotKeyboardFocus
        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e) {
            base.OnLostKeyboardFocus(e);
            SetColors();
        } //OnLostKeyboardFocus

        protected override void OnExpanded(System.Windows.RoutedEventArgs e) {
            base.OnExpanded(e);
            owner?.InvalidateVisual();
        } //OnExpanded
        protected override void OnCollapsed(System.Windows.RoutedEventArgs e) {
            base.OnCollapsed(e);
            owner?.InvalidateVisual();
        } //OnCollapsed

        GraphNodeVisualStructure visualStructure;

    } //class GraphNode

}
