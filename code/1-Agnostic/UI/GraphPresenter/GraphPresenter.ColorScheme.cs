namespace SA.Agnostic.UI {
    using TreeView = System.Windows.Controls.TreeView;

    public interface IColorScheme {
        ColorScheme ColorScheme { get; }
    } //IColorScheme

    public partial class GraphPresenter : IColorScheme {

        public ColorScheme ColorScheme { get => colorScheme; }
        internal ColorScheme colorScheme = ColorScheme.Default;

    } //class GraphPresenter

}
