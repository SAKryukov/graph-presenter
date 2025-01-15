namespace SA.Agnostic.UI {
    /*
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Input;
    using Debug = System.Diagnostics.Debug;

    internal partial class GraphNode {

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e) {
            base.OnGotKeyboardFocus(e);
            Research(true);
        } //OnGotKeyboardFocus

        protected override void OnLostFocus(RoutedEventArgs e) {
            base.OnLostFocus(e);
            Research(false);
        } //OnLostFocus

        void Research(bool on) {
            Debug.WriteLine($"================= On: {on}");
            void Report(DependencyObject @object, int level) {
                string prefix = new(' ', (level + 1) * 3);
                string background = string.Empty;
                string foreground = string.Empty;
                string backgroundPropertyName = nameof(Background);
                PropertyInfo backgroundProperty = @object.GetType().GetProperty(backgroundPropertyName);
                if (backgroundProperty != null) {
                    Brush backroundBrush = (Brush)backgroundProperty.GetValue(@object);
                    if (backroundBrush != null)
                        background = $"Background: {backroundBrush},";
                }
                if (@object is Control control && control.Visibility == Visibility.Visible) {
                    background = $"Background: {control.Background},";
                    foreground = $"Foreground: {control.Foreground} ";
                } //if
                Debug.WriteLine($"{prefix}{@object.GetType().FullName} {background} {foreground}");
            } //Report
            void Track(DependencyObject parent, int level) {
                if (parent == null) return;
                if (parent is GraphNode && level > 0) return;
                Report(parent, level);
                int count = VisualTreeHelper.GetChildrenCount(parent);
                for (int index = 0; index < count; ++index) {
                    var child = VisualTreeHelper.GetChild(parent, index);
                    Track(child, level + 1);
                } //loop
            } //Track
            Track(this, 0);
        } //Research

    } //class GraphNode
*/
}
