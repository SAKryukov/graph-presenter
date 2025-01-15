namespace SA.Agnostic.UI {
    using System.Windows.Controls;
    using System.Reflection;
    using HorizontalAlignment = System.Windows.HorizontalAlignment;
    using System.Windows.Input;

    internal partial class GraphNode : TreeViewItem {

        internal GraphNode() {
            HorizontalAlignment = HorizontalAlignment.Left;
            SetupVisualStructure(this);
        } //GraphNode

        internal virtual AssociationGraphNode AsAssociationGraphNode => null;
        internal virtual CompositionGraphNode AsCompositionGraphNode => null;

        //TODO: GraphNode: add commands
        internal bool CanInplaceEdit(bool thenAct) {
            bool yesItCan = Utility.Reflection.MemberType(titleField, titleProperty) == typeof(string) && Utility.Reflection.CanReadWrite(titleField, titleProperty);
            if (!thenAct) return yesItCan;
            InplaceTextEditorMode = true;
            return true;
        } //CanInplaceEdit

        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyDown(e);
            if (e.Key == DefinitionSet.GraphPresenter.keyInplaceEditor) {
                e.Handled = true;
                if (CanInplaceEdit(false))
                    CanInplaceEdit(true);
            } //if
        } //OnKeyDown

        internal object target, associationTarget;
        internal PropertyInfo titleProperty;
        internal FieldInfo titleField;
        internal GraphPresenter owner;

    } //class GraphNode

}
