namespace SA.Agnostic.UI {
    using System.Windows;
    using Visual = System.Windows.Media.Visual;
    using VisualTreeHelper = System.Windows.Media.VisualTreeHelper;
    using Control = System.Windows.Controls.Control;

    public static class WindowsUtility {

        public static Target FindParent<Target>(FrameworkElement item) where Target : DependencyObject {
            if (item == null) return null;
            var parent = item.Parent;
            if (parent is Target treeView)
                return treeView;
            else
                return FindParent<Target>(parent as FrameworkElement);
        } //FindParent

        public static Rect FrameworkElementToVisualCoordinates(FrameworkElement element, Visual ancestor) {
            return element.TransformToAncestor(ancestor).TransformBounds(new Rect(0, 0, element.ActualWidth, element.ActualHeight));
        } //FrameworkElementToVisualCoordinates

        public static Target FindInVisualTree<Target>(DependencyObject parent) where Target : DependencyObject {
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int index = 0; index < count; ++index) {
                Visual childVisual = (Visual)VisualTreeHelper.GetChild(parent, index);
                Target target;
                if (childVisual is Target scrollViewer)
                    return scrollViewer;
                else
                    target = FindInVisualTree<Target>(childVisual);
                if (target != null)
                    return target;
            } //loop
            return null;
        } //FindInVisualTree

        internal static FrameworkElement FindParent(FrameworkElement start, System.Predicate<FrameworkElement> predicate) {
            if (predicate == null) return null;
            if (predicate(start)) return start;
            var parent = start.Parent;
            while (true) {
                if (parent is not FrameworkElement parentItem) break;
                if (predicate(parentItem)) return parentItem;
                parent = parentItem.Parent;
            } //loop
            return null;
        } //FindParent

    } //class WindowsUtility

}
