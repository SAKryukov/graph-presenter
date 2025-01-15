namespace SA.Agnostic.UI {
    using System.Reflection;
    using UIElement = System.Windows.UIElement;

    abstract class MemberPresenter : IMemberEditor {

        bool IMemberEditor.IsReadonly {
            get => isReadonly;
            set {
                if (value == isReadonly) return;
                isReadonly = value;
                ReadonlyImplementation(isReadonly);
            } //get ICustomEditor.IsReadonly
        } //ICustomEditor.IsReadonly

        bool IMemberEditor.HasFixedAccess => hasFixedAccess;

        string IMemberEditor.Name => name;
        bool IMemberEditor.UpdateCompoundObject() { return false; }
        UIElement IMemberEditor.FocusNotifier => null;
        UIElement IMemberEditor.Editor => null;
        object IMemberEditor.Value => value;
        object IMemberEditor.CompoundObject => compoundObject;
        PropertyInfo IMemberEditor.Property => property;
        FieldInfo IMemberEditor.Field => field;
        bool IMemberEditor.IsTabStop { get => IsTabStopGetter(); set { IsTabStopSetter(value); } }

        internal MemberPresenter(FlatObjectEditor owner, object compoundObject, string name, object initialValue, FieldInfo field, PropertyInfo property, bool isReadonly, bool hasFixedAccess) {
            interfaceImplementation = this;
            editor = owner;
            this.compoundObject = compoundObject;
            this.name = name;
            value = initialValue;
            this.field = field;
            this.property = property;
            this.hasFixedAccess = hasFixedAccess;
            interfaceImplementation.IsReadonly = isReadonly;
            CreateControls();
        } //MemberPresenter

        private protected virtual void ReadonlyImplementation(bool value) { }
        private protected IMemberEditor interfaceImplementation;
        private protected FieldInfo field;
        private protected PropertyInfo property;
        private protected string name;
        private protected object compoundObject;
        private protected FlatObjectEditor editor;
        private protected bool isReadonly;
        private protected bool hasFixedAccess;
        private protected object value;
        private protected abstract bool IsTabStopGetter();
        private protected abstract void IsTabStopSetter(bool value);

        internal virtual void CreateControls() { }

    } //MemberPresenter

}
