namespace SA.Agnostic.UI {

    public class EditorEventArgs : System.EventArgs {
        public IFlatObjectEditor FlatObjectEditor { get; internal set; }
        public IMemberEditor MemberEditor { get; internal set; }
        public int MemberIndex { get; internal set; }
    } //EditorEventArgs

    public class ValidationFailureEventArgs : EditorEventArgs {
        public System.Exception Exception { get; internal set; }
        public object GraphPresenterSelectedNode { get; internal set; }
    } //ValidationFailureEventArgs

    public class CommittedEventArgs : EditorEventArgs {
    } //CommittedEventArgs

}
