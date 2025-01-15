namespace SA.Main {
    using System;
    using System.Windows;

    static class MainClass {
        [STAThread]
        static void Main() {
            Application app = new SA.Agnostic.UI.AdvancedApplication<View.MainWindow>();
            app.Run();
        } //MainClass
    } //class MainClass

}

