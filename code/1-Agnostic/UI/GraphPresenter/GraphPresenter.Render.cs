namespace SA.Agnostic.UI {
    using ScrollViewer = System.Windows.Controls.ScrollViewer;
    using NodeList = System.Collections.Generic.List<GraphNode>;
    using System.Windows.Media;
    using System.Windows;

    public partial class GraphPresenter {

        public bool ShowAllAssociations {
            get => showAllAssociations;
            set {
                if (showAllAssociations == value) return;
                showAllAssociations = value;
                InvalidateVisual();
            }
        } //ShowAllAssociations
        bool showAllAssociations;

        protected override void OnRender(DrawingContext drawingContext) {
            base.OnRender(drawingContext);
            pen.Brush = ColorScheme.GraphViewScheme.Rib;
            dottedPen.Brush = ColorScheme.GraphViewScheme.Rib;
            double rightmost = double.NegativeInfinity;
            static FrameworkElement FindFrameworkElement(object node) {
                static GraphNode FindVisibleParent(GraphNode item) =>
                    (GraphNode)WindowsUtility.FindParent(item, (candidate) => (candidate is GraphNode node) && node.IsVisible);
                GraphNode graphNode = (GraphNode)node;
                graphNode = FindVisibleParent(graphNode);
                return (FrameworkElement)graphNode.Header;
            } //FindFrameworkElement
            //finding rightmost:
            void GetAllVisibleNodes(NodeList visibleNodes, GraphNode parent) {
                visibleNodes.Add(parent);
                if (parent.IsExpanded)
                    foreach (var item in parent.Items)
                        GetAllVisibleNodes(visibleNodes, (GraphNode)item);
            } //GetAllVisibleNodes
            NodeList visibleNodes = new();
            if (Items.Count > 0) GetAllVisibleNodes(visibleNodes, (GraphNode)Items[0]);
            foreach (var visibleNode in visibleNodes) {
                var visibleItem = FindFrameworkElement(visibleNode);
                var visibleItemRectangle = WindowsUtility.FrameworkElementToVisualCoordinates(visibleItem, this);
                if (rightmost < visibleItemRectangle.Right)
                    rightmost = visibleItemRectangle.Right;
            } //loop
            //end finding rightmost:
            double shift = DefinitionSet.GraphPresenter.AssociationLine.shiftX;
            void Connect(object left, object right, double x) {
                bool dottedLeft = !((GraphNode)left).IsVisible, dottedRight = !((GraphNode)right).IsVisible;
                var leftElement = FindFrameworkElement(left);
                var rightElement = FindFrameworkElement(right);
                var initialLeftRectangle = WindowsUtility.FrameworkElementToVisualCoordinates(leftElement, this);
                var initialRightRectangle = WindowsUtility.FrameworkElementToVisualCoordinates(rightElement, this);
                var leftRectangle = WindowsUtility.FrameworkElementToVisualCoordinates(leftElement, this);
                var rightRectangle = WindowsUtility.FrameworkElementToVisualCoordinates(rightElement, this);
                double sourceY = leftRectangle.Top + leftElement.ActualHeight / 2;
                double targetY = rightRectangle.Top + rightElement.ActualHeight / 2;
                if (sourceY == targetY) {
                    int sign = initialLeftRectangle.Top < initialRightRectangle.Top ? -1 : 1;
                    sourceY += sign * leftElement.ActualHeight * DefinitionSet.GraphPresenter.AssociationLine.relativeSameNodeResolutionY;
                    targetY -= sign * leftElement.ActualHeight * DefinitionSet.GraphPresenter.AssociationLine.relativeSameNodeResolutionY;
                } //if
                Point[] points = {
                        new Point(leftRectangle.Right, sourceY),
                        new Point(x, sourceY),
                        new Point(x, targetY),
                        new Point(rightRectangle.Right, targetY),
                    };
                bool[] dotted = {
                    dottedLeft,
                    false,
                    dottedRight
                };
                for (int index = 0; index < points.Length - 1; ++index)
                    drawingContext.DrawLine(dotted[index] ? dottedPen : pen, points[index], points[index + 1]);
            } //Connect
            if (showAllAssociations) {
                double x = rightmost + shift;
                associatonDictionary.ForEachReference((key, value) => {
                    Connect(key, value, x);
                    x += shift;
                });
            } else {
                double x = rightmost + shift;
                object selected = SelectedItem;
                if (selected == null) return;
                var references = associatonDictionary.FindReferences(selected);
                var target = associatonDictionary.FindTarget(selected);
                if (target != null) {
                    Connect(target, selected, x);
                    x += shift;
                } //if
                foreach (var reference in references) {
                    Connect(reference, selected, x);
                    x += shift;
                } //loop
            } //if showAllAssociations
        } //OnRender

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();
            SetVisualStyle();
            var scrollViewer = WindowsUtility.FindInVisualTree<ScrollViewer>(this);
            if (scrollViewer != null)
                scrollViewer.ScrollChanged += (_, _) => InvalidateVisual();
        } //OnApplyTemplate

        readonly Utility.AssociatonDictionary associatonDictionary = new();
        
        readonly Pen pen = new() {
            Thickness = DefinitionSet.GraphPresenter.AssociationLine.width,
            StartLineCap = DefinitionSet.GraphPresenter.AssociationLine.startLineCap,
            EndLineCap = DefinitionSet.GraphPresenter.AssociationLine.endLineCap
        };
        readonly Pen dottedPen = new() {
            DashStyle = DefinitionSet.GraphPresenter.AssociationLine.dottedDashStyle,
            Thickness = DefinitionSet.GraphPresenter.AssociationLine.width,
            StartLineCap = DefinitionSet.GraphPresenter.AssociationLine.startLineCap,
            EndLineCap = DefinitionSet.GraphPresenter.AssociationLine.endLineCap
        };

        void SetVisualStyle(GraphNode node = null) {
            if (ItemContainerStyle == null) {
                ItemContainerStyle = new Style();
                ItemContainerStyle.Setters.Add(new Setter() { Property = PaddingProperty, Value = new Thickness(0, 0, 0, 0) });
                //ItemContainerStyle.Setters.Add(new Setter() { Property = FocusVisualStyleProperty, Value = null });
            } //if
            if (node != null) {
                //TODO: Adjust FocusVisualStyle to show nothing
                //node.FocusVisualStyle = FocusVisualStyle;
                node.ItemContainerStyle = ItemContainerStyle;
            } //if            
        } //SetVisualStyle

    } //class GraphPresenter

}
