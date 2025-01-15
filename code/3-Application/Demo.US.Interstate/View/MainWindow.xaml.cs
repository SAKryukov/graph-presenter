namespace SA.View {
    using System;
    using System.Runtime.Serialization;
    using System.Windows;
    using System.Windows.Controls;
    using Agnostic;
    using Agnostic.UI;

    class ValidationFailureNavigator : IValidationFailureNavigator {
        internal ValidationFailureNavigator(Button buttonShowErrorSource, TextBlock errorMessageSink, Panel errorPanel) {
            this.buttonShowErrorSource = buttonShowErrorSource;
            this.errorMessageSink = errorMessageSink;
            this.errorPanel = errorPanel;
        } //ValidationErrorNavigator
        void IValidationFailureNavigator.ShowHideError(string message) {
            errorPanel.Visibility = message == null ? Visibility.Collapsed : Visibility.Visible;
            errorMessageSink.Text = message;
        } //IValidationErrorNavigator.ShowHideError
        event EventHandler IValidationFailureNavigator.ShowErrorSource {
            add { if (buttonShowErrorSource != null) buttonShowErrorSource.Click += (sender, eventArgs) => { value(sender, new EventArgs()); }; }
            remove { if (buttonShowErrorSource != null) buttonShowErrorSource.Click += (sender, eventArgs) => { value(sender, new EventArgs()); }; }
        } //IValidationErrorNavigator.ShowErrorSource
        readonly Button buttonShowErrorSource;
        readonly TextBlock errorMessageSink;
        readonly Panel errorPanel;
    } //class ValidationFailureNavigator

    class BasePathProvider : IBasePathProvider {
        string IBasePathProvider.BasePath => System.IO.Path.GetDirectoryName(AdvancedApplicationBase.ExecutableDirectory);
    } //class BasePathProvider

    class StringValidator : IStringValidator {
        System.ApplicationException IStringValidator.Validate(string value) {
            if (value.Contains("$"))
                return new System.ApplicationException("Value should not contain $");
            return null;
        } //IStringValidator.Validate
    } //StringValidator

    enum Numbers { One, Two, Three, Four, }

    [System.Flags]
    enum Bits { [Hidden]None = 0, A = 1, B = 2, C = 16, D = 64, }

    [DataContract]
    [KnownType(typeof(AAA))]
    sealed class AAA {
        [DataMember, Title]
        internal string Name { get; set; }
        [DataMember, StringDomain(validatorClass: typeof(StringValidator), IsMultiline = true), DisplayName("No-$ string")]
        internal string String { get; set; }
    } //class AAA

    class ExtendedComposition : System.Collections.Generic.List<AAA> {
        [DataMember, Title, DisplayName("Composition Name")]
        internal string compositionName = "Some composition name";
    } //class ExtendedComposition

    [DataContract]
    class SampleChild {
        internal SampleChild() {
            list.Add(new AAA() { Name = "first" });
            var associated = new AAA() { Name = "second" };
            list.Add(associated);
            list.Add(new AAA() { Name = "third very very very very long name, to fix association arrows" });
            association1 = associated;
            association2 = associated;
            Name = "Sample child name";
        }
        public override string ToString() => "!First child!";
        [DataMember, DisplayName("Grandchildren")]
        internal ExtendedComposition list = new ();
        [DataMember]
        internal object[] composition = { new AAA() { Name = "first" }, new AAA() { Name = "second" } };
        [DataMember, Title]
        internal string Name { get; set; }
        [DataMember]
        internal Numbers Number { get; set; }
        [DataMember]
        internal Bits Bits { get; set; }
        [DataMember, FilenameEditor(FileDialogTitle = "Open Damn Demo File", DialogFilter = "C# files|*.cs", BasePathProviderStaticClass = typeof(BasePathProvider))]
        internal string File { get; set; }
        [DataMember]
        internal int Integer { get; set; }
        [DataMember, AssociationPropertySet(associationMemberName: nameof(association0))]
        internal AAA associationProperySet = new() { Name = "association property set" };
        [DataMember, Association, DisplayName("AAA A")]
        internal AAA association0 = null;
        [DataMember, Association, DisplayName("AAA B")]
        internal AAA association1;
        [DataMember, Association, DisplayName("AAA C")]
        internal AAA association2;
    } //class SampleChild

    [DataContract]
    class Point {
        [NonPersistentDataMember]
        internal double X {
            get => Back.X;
            set { Back = new (value, Back.Y); }
        }
        [NonPersistentDataMember]
        internal double Y {
            get => Back.Y;
            set { Back = new(Back.X, value); }
        }
        [DataMember]
        internal System.Windows.Point Back { get; set; }
    } //class Point

    [DataContract]
    class Geometry {
        internal Geometry() {
            Point = new() { X = 13, Y = 14 };
        }
        [DataMember]
        internal Point Point { get; set; }
    } //class Geometry

    [DataContract]
    [DisplayName("Target")]
    class SampleTarget {
        public override string ToString() => "Top!";
        internal SampleTarget() {
            Integer = -12;
            UnsignedInteger = 101;
            Double = 13.11;
            String = "Some value";
            Boolean = true;
            File = "Some file";
            Geometry = new();
        } //Target
        [DataMember, DisplayName("Child")]
        readonly SampleChild child = new();
        public SampleChild Child => child;
        [DataMember, Title]
        internal string name = "My Name";
        [DataMember]
        internal Numbers Number { get; set; }
        [DataMember]
        internal Bits Bits { get; set; }
        [DataMember, FilenameEditor(FileDialogTitle = "Open Damn Demo File", DialogFilter = "C# files|*.cs", BasePathProviderStaticClass = typeof(BasePathProvider))]
        internal string File { get; set; }
        [DataMember]
        internal int Integer { get; set; }
        [DataMember, DisplayName("Unsigned Integer"), FixedAccess(MemberAccessLevel.Disabled)]
        internal uint UnsignedInteger { get; set; }
        [DataMember, FixedAccess(MemberAccessLevel.ReadWrite)]
        internal float Float { get; set; }
        [DataMember, FloatingPointDomain(Minimum = double.NegativeInfinity, Maximum = 3, IsNanAccepted = false), DisplayName("Float in domain")]
        internal float FloatInDomain { get; set; }
        [DataMember, FloatingPointDomain(1, double.PositiveInfinity, true), DisplayName("Double in domain")]
        internal double DoubleInDomain { get; set; }
        [DataMember]
        internal double Double { get; set; }
        [DataMember, StringDomain(typeof(StringValidator), false), DisplayName("No-$ string")]
        internal string String { get; set; }
        [DataMember, StringDomain(null, true), DisplayName("Nullable string")]
        internal string NullableString { get; set; }
        [DataMember, DisplayName("Nullable string by default")]
        internal string NullableStringByDefault { get; set; }
        [DataMember]
        internal bool Boolean { get; set; }
        [DataMember, DisplayName("Primitive-type Array")]
        internal int[] aPrimitiveOne = new int[] { 13, 18, 312 };
        [DataMember, DisplayName("Value-type Value")]
        internal ValueType valueValue = new();
        [NonPersistentDataMember]
        internal Geometry Geometry { get; set; }
    } //class Target

    [DataContract]
    struct ValueType {
        [DataMember]
        public int A { get; set; }
        [DataMember]
        public ulong B { get; set; }
        [DataMember]
        public string C { get; set; }
    } //struct ValueType

    public partial class MainWindow : Window {

        public MainWindow() {
            InitializeComponent();
            SetupGraphPresenter();
            checkboxReadonly.Click += (sender, _) => {
                var checkbox = (CheckBox)sender;
                editor.IsReadonly = checkbox.IsChecked == true;
            };
            editor.BorderBrush = ((Agnostic.UI.IColorScheme)graphPresenter).ColorScheme.FlatObjectEditorScheme.Border; //sic!
            menuItemAllAssociations.Click += (sender, _) => {
                MenuItem item = (MenuItem)sender;
                graphPresenter.ShowAllAssociations = item.IsChecked;
            }; //menuItemAllAssociations.Click
        } //MainWindow

        void SetupGraphPresenter() {
            var target = new SampleTarget();
            graphPresenter.Target = target;
            graphPresenter.FlatObjectEditor = editor;
            graphPresenter.ValidationErrorNavigator = new ValidationFailureNavigator(buttonSeeError, textBlockError, panelFailure);
            editor.Committed += (_, eventArgs) => {
                System.Diagnostics.Debug.WriteLine("================= committed");
            };
            buttonExamineTarget.Click += (_, _) => {
                object saveTarget = graphPresenter.Target;
                graphPresenter.Target = null;
                graphPresenter.Target = saveTarget;
                //Agnostic.Persistence<SampleTarget>.Store(target, "sampleStore.xml");
            };
        } //SetupGraphPresenter

        protected override void OnContentRendered(EventArgs e) {
            base.OnContentRendered(e);
            graphPresenter.DefaultFocus();
        } //OnContentRendered

    } //class MainWindow

}
