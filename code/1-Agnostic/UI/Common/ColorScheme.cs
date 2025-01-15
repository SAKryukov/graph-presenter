namespace SA.Agnostic.UI {
    using System.Windows.Media;

    public struct ColorScheme {

        public struct NodeScheme {
            public Brush Normal;
            public Brush Selected;
            public Brush KeyboardFocused; // automatically means selected
            public Brush Disabled;
        } //struct NodeScheme

        public struct МemberScheme {
            public Brush Label;
            public Brush LabelError;
            public Brush ListBoxItem;
            public Brush SelectedListBoxItem;
            public Brush Button;
            public Brush NullifyButton;
        } //struct МemberScheme

        public struct BackgroundМemberScheme {
            public Brush Normal;
            public Brush Selected;
            public Brush Focused;
            public Brush Readonly;
            public double ReadonlyOpacity;
        } //struct BackgroundМemberScheme

        public struct FlatObjectEditor {
            public BackgroundМemberScheme Highligting;
            public МemberScheme Background;
            public МemberScheme Foreground;
            public Brush FocusControlForeground;
            public Brush Border;
            public Brush SplitterBackground;
            public Brush SplitterHoverBackground;
            public Brush ListBoxBorder;
        } //struct FlatObjectEditor

        public struct GraphView {
            public NodeScheme TypeBackground;
            public NodeScheme TypeForeground;
            public NodeScheme TitleBackground;
            public NodeScheme TitleForeground;
            public NodeScheme AssociationIndicatorBackground;
            public NodeScheme AssociationIndicatorForeground;
            public Brush Rib;
        } //struct GraphView

        public FlatObjectEditor FlatObjectEditorScheme;
        public GraphView GraphViewScheme;

        public static ColorScheme Default {
            get {
                ColorScheme result = new();
                //FlatObjectEditor:
                var selectedMemberEditorFocusedBackground = new SolidColorBrush(new Color() { A = 0xFF, R = 0x72, G = 0xD8, B = 0xFF });
                var selectedMemberEditorUnfocusedBackground = new SolidColorBrush(new Color() { A = 0xFF, R = 0xCA, G = 0xE4, B = 0xEE });
                result.FlatObjectEditorScheme.FocusControlForeground = Brushes.Black;
                result.FlatObjectEditorScheme.Border = Brushes.Gray;
                result.FlatObjectEditorScheme.SplitterHoverBackground = Brushes.PaleTurquoise;
                result.FlatObjectEditorScheme.SplitterBackground = Brushes.White;
                result.FlatObjectEditorScheme.ListBoxBorder = Brushes.SteelBlue;
                result.FlatObjectEditorScheme.Highligting.Normal = Brushes.White;
                result.FlatObjectEditorScheme.Highligting.Selected = selectedMemberEditorUnfocusedBackground;
                result.FlatObjectEditorScheme.Highligting.Focused = selectedMemberEditorFocusedBackground;
                result.FlatObjectEditorScheme.Highligting.Readonly = Brushes.Black;
                result.FlatObjectEditorScheme.Highligting.ReadonlyOpacity = 0.1;
                result.FlatObjectEditorScheme.Background.Label = Brushes.White;
                result.FlatObjectEditorScheme.Background.LabelError = Brushes.Red;
                result.FlatObjectEditorScheme.Foreground.Label = Brushes.Black;
                result.FlatObjectEditorScheme.Foreground.LabelError = Brushes.Yellow;
                result.FlatObjectEditorScheme.Background.Button = Brushes.GhostWhite;
                result.FlatObjectEditorScheme.Background.NullifyButton = Brushes.Transparent;
                result.FlatObjectEditorScheme.Foreground.Button = Brushes.Black;
                result.FlatObjectEditorScheme.Foreground.NullifyButton = Brushes.Black;
                result.FlatObjectEditorScheme.Background.ListBoxItem = Brushes.SteelBlue; // if dark, better be the same as result.FlatObjectEditorScheme.ListBoxBorder
                result.FlatObjectEditorScheme.Background.SelectedListBoxItem = Brushes.Azure;
                result.FlatObjectEditorScheme.Foreground.ListBoxItem = Brushes.LemonChiffon; 
                result.FlatObjectEditorScheme.Foreground.SelectedListBoxItem = Brushes.Sienna;
                //GraphView, type name:
                var selectedGraphViewUnfocusedBackground = new SolidColorBrush(new Color() { A = 0xFF, R = 0xA8, G = 0xD0, B = 0xE0 });
                result.GraphViewScheme.TypeBackground.Normal = Brushes.White;
                result.GraphViewScheme.TypeForeground.Normal = Brushes.DarkMagenta;
                result.GraphViewScheme.TypeBackground.Selected = selectedGraphViewUnfocusedBackground;
                result.GraphViewScheme.TypeForeground.Selected = Brushes.Magenta;
                result.GraphViewScheme.TypeBackground.KeyboardFocused = Brushes.Navy;
                result.GraphViewScheme.TypeForeground.KeyboardFocused = Brushes.Pink;
                result.GraphViewScheme.TypeBackground.Disabled = Brushes.White;
                result.GraphViewScheme.TypeForeground.Disabled = Brushes.Gray;
                //GraphView, title:
                result.GraphViewScheme.TitleBackground.Normal = Brushes.White;
                result.GraphViewScheme.TitleForeground.Normal = Brushes.Black;
                result.GraphViewScheme.TitleBackground.Selected = selectedGraphViewUnfocusedBackground;
                result.GraphViewScheme.TitleForeground.Selected = Brushes.Black;
                result.GraphViewScheme.TitleBackground.KeyboardFocused = Brushes.Navy;
                result.GraphViewScheme.TitleForeground.KeyboardFocused = Brushes.Yellow;
                result.GraphViewScheme.TitleBackground.Disabled = Brushes.White;
                result.GraphViewScheme.TitleForeground.Disabled = Brushes.Gray;
                //association indicator:
                result.GraphViewScheme.AssociationIndicatorBackground.Normal = selectedGraphViewUnfocusedBackground;
                result.GraphViewScheme.AssociationIndicatorForeground.Normal = Brushes.Black;
                result.GraphViewScheme.AssociationIndicatorBackground.Selected = Brushes.PapayaWhip;
                result.GraphViewScheme.AssociationIndicatorForeground.Selected = Brushes.Navy;
                result.GraphViewScheme.AssociationIndicatorBackground.KeyboardFocused = Brushes.LightGreen;
                result.GraphViewScheme.AssociationIndicatorForeground.KeyboardFocused = Brushes.Navy;
                result.GraphViewScheme.AssociationIndicatorBackground.Disabled = Brushes.LightGray;
                result.GraphViewScheme.AssociationIndicatorForeground.Disabled = Brushes.Gray;
                //GraphView, rib:
                result.GraphViewScheme.Rib = Brushes.Black;
                return result;
            } //get Default
        } //Default

    } //struct ColorScheme

}
