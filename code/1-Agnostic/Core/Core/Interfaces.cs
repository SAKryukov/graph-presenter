namespace SA.Agnostic {
    using UIElement = System.Windows.UIElement;
    using System.Reflection;
    using FormatDictionary = System.Collections.Generic.IDictionary<System.Type, string>;

    public enum MemberAccessLevel { ReadWrite, Readonly, Disabled, Hidden, }

    public interface INodeImage {
        UIElement Image { get; }
        double Height { get; set; }
    } //interface INodeImage
    public interface INodeTypedicator : INodeImage {
        void SetState(bool isSelected, bool isKeyboardFocused, bool isEnabled);
    } //interface INodeTypedicator
    public interface INodeStateIndicator : INodeImage {
        void SetState(bool isSelected, bool isKeyboardFocused, bool isEnabled, System.Enum state);
    } //interface INodeStateIndicator

    public interface IFlatObjectEditor { // to be used by custom member presenter via IMemberEditor CustomInitializer
        void ValidateMember(UIElement sender); // handles sender and reports validation result to UI in case of any problems
    } //IFlatObjectEditor

    public interface IMemberEditor {
        void CustomInitializer(IFlatObjectEditor owner, object compoundObject, string name, object initialValue, FieldInfo field, PropertyInfo property, bool isReadonly, bool hasFixedAccess) {
            // Only used by a custom implementation passed to CustomEditorAttribute applied to a member of a custom type using a custom member editor
            // The custom implementation needs to have a parameterless constructor
        }
        UIElement Editor { get; }
        object Value { get; }
        UIElement FocusNotifier { get; }
        bool IsReadonly { get; set; }
        bool HasFixedAccess { get; }
        string Name { get; }
        bool UpdateCompoundObject();
        object CompoundObject { get; }
        PropertyInfo Property { get; }
        FieldInfo Field { get; }
        bool IsTabStop { get; set; }
    } //interface IMemberEditor

    public interface ILocalizableDisplayNameResource {
        string DisplayName(string typeName, string memberName);
        bool UseFullTypeName { get; }
    } //ILocalizableDisplayNameResource

    public interface IStringValidator {
        System.ApplicationException Validate(string value);
    } //IStringValidator

    public interface IBasePathProvider {
        string BasePath { get; }
    } //IBasePathProvider

    public enum GlobalFitleringResult { Normal, Readonly, Hidden, Disabled }

    public interface IGlobalOptionSet {
        GlobalFitleringResult GlobalFitleringResult { get; }
        FormatDictionary FormatDictionary { get; }
    } //IGlobalOptionSet

    public interface IValidationFailureNavigator {
        void ShowHideError(string message); // use null to hide
        event System.EventHandler ShowErrorSource;
    } //interface IIValidationFailureNavigator

}
