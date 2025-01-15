namespace SA.Agnostic.UI {
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Shapes;
    using System.Windows.Media;
    using UIElement = System.Windows.UIElement;

    internal class AssociationIndicator : Viewbox, INodeStateIndicator {
        internal AssociationIndicator(GraphNode owner) {
            this.owner = owner;
            Stretch = Stretch.Fill;
            StretchDirection = StretchDirection.Both;
            Canvas canvas = new() { Width = 1, Height = 1, Visibility = Visibility.Visible, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
            polygon = new() { StrokeThickness = DefinitionSet.GraphPresenter.NodeStructure.Association.strokeThickness };
            foreach (var point in DefinitionSet.GraphPresenter.NodeStructure.Association.points)
                polygon.Points.Add(point);
            canvas.Children.Add(polygon);
            Child = canvas;
        } //AssociationIndicator

        UIElement INodeImage.Image => this;
        double INodeImage.Height { get => ActualHeight; set { Height = value; } }

        void INodeStateIndicator.SetState(bool isSelected, bool isKeyboardFocused, bool isEnabled, System.Enum state) {
            if (owner == null) return;
            if (owner.owner == null) return;
            //owner.owner.ColorScheme
            polygon.Fill = isEnabled
                ? (isKeyboardFocused
                    ? owner.owner.ColorScheme.GraphViewScheme.AssociationIndicatorBackground.KeyboardFocused
                    : (isSelected
                        ? owner.owner.ColorScheme.GraphViewScheme.AssociationIndicatorBackground.Selected
                        : owner.owner.ColorScheme.GraphViewScheme.AssociationIndicatorBackground.Normal))
                : owner.owner.ColorScheme.GraphViewScheme.AssociationIndicatorBackground.Disabled;
            polygon.Stroke = isEnabled
                ? (isKeyboardFocused
                    ? owner.owner.ColorScheme.GraphViewScheme.TypeBackground.KeyboardFocused
                    : (isSelected
                        ? owner.owner.ColorScheme.GraphViewScheme.AssociationIndicatorForeground.Selected
                        : owner.owner.ColorScheme.GraphViewScheme.AssociationIndicatorForeground.Normal))
                : owner.owner.ColorScheme.GraphViewScheme.AssociationIndicatorForeground.Disabled;
            //TODO: implement AssociationIndicator INodeStateIndicator.SetState
        } //INodeStateIndicator.SetState

        readonly GraphNode owner;
        readonly Polygon polygon;

    } //class AssociationIndicator

}
