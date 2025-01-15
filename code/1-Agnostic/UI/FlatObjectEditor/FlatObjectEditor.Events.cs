namespace SA.Agnostic.UI {

    public partial class FlatObjectEditor {

        public System.EventHandler<ValidationFailureEventArgs> ValidationFailure;
        public System.EventHandler<CommittedEventArgs> Committed;
        internal GraphPresenter.ModelReflectionModeType modelReflectionMode;

    } //class FlatObjectEditor

}
