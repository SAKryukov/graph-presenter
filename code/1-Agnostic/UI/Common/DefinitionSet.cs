namespace SA.Agnostic.UI {
    using Thickness = System.Windows.Thickness;
    using PenLineCap = System.Windows.Media.PenLineCap;
    using DashStyle = System.Windows.Media.DashStyle;
    using DashStyles = System.Windows.Media.DashStyles;
    using FontWeight = System.Windows.FontWeight;
    using FontWeights = System.Windows.FontWeights;
    using Point = System.Windows.Point;
    using Key = System.Windows.Input.Key;

    static class DefinitionSet {

        internal const double splitterWidth = 6;
        const double horizontalGap = 8;
        const double listBoxItemVerticalGap = 2;

        internal static class FlatObjectEditor {
            internal const char space = ' ';
            internal const string toolTipNullButton = "Click to make null";
            internal const string toolTipFloatingPointDropdownButton = "Click or hit Alt+Down to enter an infinite or NaN value";
            internal const string toolTipListSelectionCommit = "Press Enter or double-click to commit, Escape to cancel";
            internal const string toolTipListSelectionClose = "Press Enter, Escape, or double-click to close";
            internal const string toolTipFileDialogButton = "Click to select a file";
            internal static string ToolTipEnumerationSelectionButton(bool isBitwise, string typeName) =>
                isBitwise ? $"Select {typeName}" : $"Select one of {typeName}";
            internal static readonly Thickness marginCheckBox = new(horizontalGap, 0, 0, 0);
            internal static readonly Thickness paddingName = new(0, 0, horizontalGap, 0);
            internal static readonly Thickness paddingValue = new(horizontalGap, 0, horizontalGap, 0);
            internal static readonly Thickness paddingListBoxContent = new (horizontalGap, listBoxItemVerticalGap, horizontalGap, listBoxItemVerticalGap);
            internal static readonly Thickness borderThicknessMemberEditButton = new(1, 0, 0, 0);
            internal static readonly Thickness borderThicknessSplitter = new(1, 0, 1, 0);
            internal static readonly string buttonDropDown = char.ConvertFromUtf32(0x25BC); // Geometric Shapes > Black Down-Pointing Triangle
            internal static readonly string buttonMakeNull = char.ConvertFromUtf32(0x232B); // Miscellaneous Technical > Erase to the Left (0x232B); char.ConvertFromUtf32(0x274C); // Dingbats > cross mark
            internal static readonly string buttonEllipsis = char.ConvertFromUtf32(0x22EF); // Mathematical Operations > Midline Horizontal Ellipsis
            internal static readonly string stringFloatingPointNull = double.NaN.ToString();
            internal static readonly string stringFloatingPointPositiveInfinity = double.PositiveInfinity.ToString();
            internal static readonly string stringFloatingPointNegativeInfinity = double.NegativeInfinity.ToString();
            internal static string ErrorNumericDomain(object minimum, object maximum) =>
                $"Valid value should be between {minimum} and {maximum}, inclusively";
            internal const string inputFilterNumeric = "0123456789";
            internal static readonly string inputFilterFloatingPoint = $"{inputFilterNumeric}eE{System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator}";
            internal static readonly string inputFilterNegativeSign = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NegativeSign;
        } //class FlatObjectEditor

        internal static class GraphPresenter {

            internal const Key keyInplaceEditor = Key.F2;

            internal static class AssociationLine {
                internal const double width = 1.7;
                internal const PenLineCap startLineCap = PenLineCap.Square;
                internal const PenLineCap endLineCap = PenLineCap.Square;
                internal static readonly DashStyle dottedDashStyle = DashStyles.Dot;
                internal const double shiftX = 10;
                internal const double relativeSameNodeResolutionY = 0.26; // should be less then 0.5, see the usage
            } //AssociationLine

            internal static class NodeStructure {
                internal static readonly Thickness typeNamePadding = new(splitterWidth, 0, splitterWidth / 2, 0);
                internal static readonly Thickness titlePadding = new(splitterWidth / 2, 0, splitterWidth, 0);
                internal static readonly Thickness titleInplacePadding = titlePadding;
                internal static readonly FontWeight titleFontWeight = FontWeights.Normal;
                internal static readonly FontWeight titleInplaceFontWeight = FontWeights.SemiBold;
                internal static readonly FontWeight typeNameFontWeight = FontWeights.SemiBold;
                internal static readonly FontWeight emptyAssociationIndicatorFontWeight = FontWeights.Normal;
                internal static readonly Thickness emptyAssociationIndicatorMargin = new(0, 0, horizontalGap / 2, 0);
                internal static readonly string emptyAssociationIndicator = char.ConvertFromUtf32(0x2205);
                internal static class Association {
                    internal const double strokeThickness = 0.04;
                    const double headWidth = 0.7;
                    const double stemWidth = 0.3;
                    const double stemLength = 0.8;
                    internal static Point[] points = new Point[] {
                        new Point() { X = 1, Y = 0.5 },
                        new Point() { X = 1 - headWidth / 2, Y = (1 - headWidth) / 2 },
                        new Point() { X = 1 - headWidth / 2, Y = (1 - stemWidth) / 2 },
                        new Point() { X = 1 - stemLength, Y = (1 - stemWidth) / 2 },
                        new Point() { X = 1 - stemLength, Y = 1 - (1 - stemWidth) / 2 },
                        new Point() { X = 1 - headWidth / 2, Y = 1 - (1 - stemWidth) / 2 },
                        new Point() { X = 1 - headWidth / 2, Y = 1 - (1 - headWidth) / 2 },
                    }; //points
                } //class Association
            } //class NodeStructure

        } //class GraphPresenter

        /*
            internal const char prohibitedSpace = (char)0x20;
            internal static class FileFilterSyntax {
                internal const string delimiter = "|";
                internal const string fileTypeSeparator = ";";
            }

        internal static class GraphView {
            internal const double associationLineGapToggleButton = 0.1;
            internal const double associationLineGapImage = -0.3;
            internal const double associationLineToSelfHalfGap = 0.25;
            internal const double associationLineWidth = 1.6;
            internal static readonly Brush associationLineColor = Brushes.Navy;
        } //class GraphView

        internal static class GraphViewItem {
            internal const double horizontalMargin = 1.0 / 4;
            internal static readonly Key stateCycleKey = Key.Space;
            internal static readonly Key[] reverseStateCycleKey = new Key[] { Key.LeftCtrl, Key.RightShift };
            internal static readonly Key inplaceEditorLeaveKey = Key.Escape;
            internal static readonly Key inplaceEditorCommitKey = Key.Enter;
            internal const string inplaceEditorToolTip = "Press [Enter] to commit changes or [Escape]";
        } //class GraphViewItem

        internal static class WindowTargetChoice {
            internal const string associationButton = "_Associate";
            internal const string compositionButton = "_Create";
            internal const string treePathSeparator = " > ";
            internal static string FormatAssociationTitle(string baseType) => $" Choose {baseType} for new Association Target";
            internal static string FormatCompositionTitle(string baseType) => $" Choose {baseType} Type";
            internal static string FormatNewGraphViewItemTitle(string displayName) => $"New {displayName}";
            internal const string exceptionInvalidSelectionRuleProviderClass = "Invalid SelectionRuleProvider class; should implement IAssociationSelectionRuleProvider";
            internal static string SelectorNodeName(string typeName, string name) {
                var delimiter = string.IsNullOrEmpty(name) ? string.Empty : ":";
                return $"{typeName}{delimiter} {name}";
            }
    } //class WindowTargetChoice
    */

    } //class DefinitionSet

}
