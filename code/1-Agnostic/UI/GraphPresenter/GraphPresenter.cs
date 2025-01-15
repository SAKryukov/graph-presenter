namespace SA.Agnostic.UI {
    using System.Reflection;
    using Type = System.Type;
    using Attribute = System.Attribute;
    using TreeView = System.Windows.Controls.TreeView;
    using System.Windows.Media;
    using IEnumerable = System.Collections.IEnumerable;
    using SelectedItemChangedEventArgs = System.Windows.RoutedPropertyChangedEventArgs<object>;
    using TargetDictionary = System.Collections.Generic.Dictionary<object, GraphNode>;
    using NodeList = System.Collections.Generic.List<GraphNode>;
    using AssociationNodeDictionary = System.Collections.Generic.Dictionary<System.Reflection.MemberInfo, AssociationGraphNode>;
    using AssociationPropertyDictionary = System.Collections.Generic.Dictionary<System.Reflection.MemberInfo, object>;

    //TODO: take into account special readonly cases: global filter and special permissions attribute
    public partial class GraphPresenter : TreeView {

        public GraphPresenter() {
            Background = Brushes.Transparent; // otherwise OnRender content is not visible!
            ClipToBounds = true; // important!
        } //GraphPresenter

        public event System.EventHandler<CommittedEventArgs> Committed;
        public event System.EventHandler<ValidationFailureEventArgs> ValidationFailure;

        public object Target {
            get => target;
            set {
                if (value == target) return;
                target = value;
                Populate(target);
            } //set Target
        } //object Target

        public IValidationFailureNavigator ValidationErrorNavigator {
            get => validationFailureNavigator;
            set {
                if (value == validationFailureNavigator) return;
                validationFailureNavigator = value;
                if (validationFailureNavigator != null)
                    validationFailureNavigator.ShowErrorSource += (sender, eventArgs) =>
                        HightlightValidationFailure(lastValidationFailure);
            } //set ValidationErrorNavigator
        } //ValidationErrorNavigator

        public void DefaultFocus() {
            if (Items.Count < 1) return;
            GraphNode node = (GraphNode)Items[0];
            node.IsSelected = true;
            System.Windows.Input.Keyboard.Focus(node);
        } //DefaultFocus

        public enum ModelReflectionModeType { BaseClassesFromTheSameAssemby, AllBaseClasses }
        public ModelReflectionModeType ModelReflectionMode {
            get => modelReflectionMode;
            set {
                if (value == modelReflectionMode) return;
                modelReflectionMode = value;
                object savedTarget = target;
                Target = null;
                Target = savedTarget;
            } //ModelReflectionMode
        } //ModelReflectionMode

        public FlatObjectEditor FlatObjectEditor {
            get => flatObjectEditor;
            set {
                if (flatObjectEditor == value) return;
                flatObjectEditor = value;
                flatObjectEditor.ColorSchemeOwner = this;
                flatObjectEditor.modelReflectionMode = this.ModelReflectionMode;
                flatObjectEditor.Committed += (sender, eventArgs) => OnCommitted(sender, eventArgs);
                flatObjectEditor.ValidationFailure += (sender, eventArgs) => OnValidationFailure(sender, eventArgs);
                GraphNode selectedNode = (GraphNode)SelectedItem;
                if (selectedNode != null)
                    flatObjectEditor.Target = selectedNode.target;
            } //set FlatObjectEditor
        } //FlatObjectEditor

        public void HightlightValidationFailure(ValidationFailureEventArgs eventArgs) {
            if (eventArgs == null) return;
            if (flatObjectEditor == null) return;
            if (eventArgs.GraphPresenterSelectedNode == null) return;
            if (eventArgs.GraphPresenterSelectedNode is not GraphNode node) return;
            node.BringIntoView();
            System.Windows.Input.Keyboard.Focus(node);
            flatObjectEditor.HightlightValidationFailure(eventArgs);
        } //HightlightValidationFailure

        protected override void OnSelectedItemChanged(SelectedItemChangedEventArgs eventInstance) {
            GraphNode node = (GraphNode)eventInstance.NewValue;
            if (node != null && FlatObjectEditor != null)
                FlatObjectEditor.Target = node.target;
            InvalidateVisual();
        } //OnSelectedItemChanged

        void Populate(object aTarget) {
            Items.Clear();
            if (aTarget == null) return;
            TargetDictionary targetDictionary = new();
            NodeList associationList = new();
            PrePopulate(aTarget, targetDictionary, associationList);
            foreach (var associationNode in associationList) {
                if (associationNode.associationTarget != null) {
                    bool found = targetDictionary.TryGetValue(associationNode.associationTarget, out GraphNode associationTargetNode);
                    if (found) {
                        associatonDictionary.Add(associationNode, associationTargetNode);
                    } //if found
                } //if
            } //loop
        } //Populate

        AssociationGraphNode PrePopulate(object aTarget, TargetDictionary targetDictionary, NodeList associationList, MemberInfo parentMember = null, GraphNode parent = null) {
            if (aTarget == null)
                System.Diagnostics.Debug.WriteLine(parentMember.Name);
            Type type = aTarget?.GetType(); // aTarget can be null for unassigned associations
            bool isComposition = false;
            bool isPrimitive = (type != null) && (type.IsEnum || type == typeof(string) || type.IsPrimitive);
            AssociationAttribute associationAttribute = null;
            if (parentMember != null) {
                isComposition = type != null && !isPrimitive && (Utility.Collection.IsList(type) || type.IsArray);
                associationAttribute = Utility.Reflection.GetAttribute<AssociationAttribute>(parentMember);
            } //if
            bool isAssociation = !isComposition && !isPrimitive && associationAttribute != null && (type == null || type.IsClass);
            AssociationGraphNode returnedAssociationGraphNode = null;
            GraphNode node;
            // Here is have a question to Anders Hejlsberg: what did you think about while trying to adopt the ideas of Delphi in .NET?
            // Where are those wonderful virtual constructors of Deplhi? Where are the metaclasses?! See here:
            if (type == null && !isAssociation) return returnedAssociationGraphNode;
            if (isComposition)
                node = new CompositionGraphNode() { owner = this, target = aTarget };
            else if (isAssociation) {
                returnedAssociationGraphNode = new AssociationGraphNode() { owner = this, associationTarget = aTarget };
                node = returnedAssociationGraphNode;
            } else
                node = new() { owner = this, target = aTarget };
            if (isAssociation)
                associationList.Add(node);
            else
                targetDictionary.Add(aTarget, node);
            //TODO: GraphPresenter.Populate: set images and pass to node.SetupVisualStructure
            SetVisualStyle(node);
            node.SetupVisualStructure(
                null,
                parentMember == null ? Utility.Reflection.DisplayName(type) : Utility.Reflection.DisplayName(parentMember),
                isPrimitive ? aTarget.ToString() : null,
                null,
                isAssociation: isAssociation, value: aTarget);
            if (parent == null)
                Items.Add(node);
            else
                parent.Items.Add(node);
            if (isComposition) {
                if (aTarget is IEnumerable enumerable)
                    foreach (var child in enumerable)
                        PrePopulate(child, targetDictionary, associationList, null, node);
            } //if composition is populated
            //TODO: GraphPresenter.Populate: from this point, there can be a mix of composition values with aggregated composite types' members, decide how to handle it
            if (type == null || isAssociation || isPrimitive) return returnedAssociationGraphNode;
            PropertyInfo[] properties = Utility.Reflection.GetProperties(type, excludeValuetTypes: true, sameAssembly: modelReflectionMode == ModelReflectionModeType.BaseClassesFromTheSameAssemby, onlyReadable: true);
            FieldInfo[] fields = Utility.Reflection.GetFields(type, excludeValuetTypes: true, sameAssembly: modelReflectionMode == ModelReflectionModeType.BaseClassesFromTheSameAssemby);
            void ProcessMember(MemberInfo member, Type memberType, object memberValue, AssociationNodeDictionary associationNodeDictionary, AssociationPropertyDictionary associationPropertyDictionary, PropertyInfo property = null, FieldInfo field = null, bool hideTypeName = false) {
                if (memberValue != null && string.IsNullOrEmpty(node.Title)) {
                    node.Title = Attribute.IsDefined(member, typeof(TitleAttribute)) ? memberValue.ToString() : null;
                    if (hideTypeName)
                        node.MemberName = null;
                    node.titleProperty = property;
                    node.titleField = field;
                } //if title
                if (memberType.IsPrimitive) return;
                if (memberType == typeof(string)) return;
                if (memberType.IsEnum) return;
                var associationPropertySetAttribute = Utility.Reflection.GetAttribute<AssociationPropertySetAttribute>(member);
                if (associationPropertySetAttribute != null && !string.IsNullOrWhiteSpace(associationPropertySetAttribute.AssociationMemberName)) {
                    MemberInfo[] associationMemberCandidates = type.GetMember(associationPropertySetAttribute.AssociationMemberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (associationMemberCandidates == null) return;
                    if (associationMemberCandidates.Length != 1) return;
                    var associationMember = associationMemberCandidates[0];
                    if (!Attribute.IsDefined(associationMember, typeof(AssociationAttribute))) return;
                    var associationProperty = associationMember as PropertyInfo;
                    var associationField = associationMember as FieldInfo;
                    if (associationField == null && associationProperty == null) return;
                    if (associationProperty != null && (!associationProperty.CanRead)) return;
                    associationPropertyDictionary.Add(associationMember, memberValue);
                } else {
                    var associationNode = PrePopulate(memberValue, targetDictionary, associationList, member, node);
                    if (associationNode != null)
                        associationNodeDictionary.Add(member, associationNode);
                } //if
            } //ProcessMember
            AssociationPropertyDictionary associationPropertyDictionary = new();
            AssociationNodeDictionary associationNodeDictionary = new();
            foreach (var property in properties)
                ProcessMember(property, property.PropertyType, property.GetValue(aTarget), associationNodeDictionary, associationPropertyDictionary, property: property);
            foreach (var field in fields)
                ProcessMember(field, field.FieldType, field.GetValue(aTarget), associationNodeDictionary, associationPropertyDictionary, field: field);
            foreach (var associationPropertyDictionaryElement in associationPropertyDictionary) {
                MemberInfo member = associationPropertyDictionaryElement.Key;
                object associationPropertySetTarget = associationPropertyDictionaryElement.Value;
                if (associationNodeDictionary.TryGetValue(member, out AssociationGraphNode associationGraphNode))
                    associationGraphNode.target = associationPropertySetTarget;
            } //loop
            return returnedAssociationGraphNode;
        } //PrePopulate

        void OnCommitted(object sender, CommittedEventArgs eventArgs) {
            if (Committed != null)
                Committed.Invoke(this, eventArgs);
            GraphNode selectedNode = (GraphNode)SelectedItem;
            if (selectedNode != null && (selectedNode.titleField == eventArgs.MemberEditor.Field || selectedNode.titleProperty == eventArgs.MemberEditor.Property))
                selectedNode.Title = Utility.Reflection.GetValue(eventArgs.MemberEditor.CompoundObject, selectedNode.titleField, selectedNode.titleProperty)?.ToString();
            if (lastValidationFailure != null 
                && validationFailureNavigator != null
                && lastValidationFailure.FlatObjectEditor == sender as FlatObjectEditor
                && eventArgs.MemberIndex == lastValidationFailure.MemberIndex)
                    validationFailureNavigator.ShowHideError(null);
        } //OnCommitted
        void OnValidationFailure(object sender, ValidationFailureEventArgs eventArgs) {
            eventArgs.GraphPresenterSelectedNode = SelectedItem;
            if (ValidationFailure != null)
                ValidationFailure.Invoke(this, eventArgs);
            lastValidationFailure = eventArgs;
            lastValidationFailure.FlatObjectEditor = sender as FlatObjectEditor;
            if (validationFailureNavigator != null)
                validationFailureNavigator.ShowHideError(eventArgs.Exception.Message);
            //TODO: GraphPresenter OnValidationFailure: show other exception data?
            //TODO: GraphPresenter OnValidationFailure: show stack of last errors and proper clean-up?
            //TODO: GraphPresenter OnValidationFailure: set error input back to member editor?
        } //OnValidationFailure

        internal void InvokeValidationFailure(System.Exception exception) {
            if (ValidationFailure != null)
                ValidationFailure.Invoke(this, new ValidationFailureEventArgs() { Exception = exception });
        } //InvokeValidationFailure

        object target;
        FlatObjectEditor flatObjectEditor;
        ModelReflectionModeType modelReflectionMode;
        IValidationFailureNavigator validationFailureNavigator;
        ValidationFailureEventArgs lastValidationFailure;

    } //class GraphPresenter

}
