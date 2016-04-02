using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using Microsoft.Shell;

namespace SourceLog
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : ISingleInstanceApp
    {
        private const string Unique = "SourceLog";

        /// <summary>
        /// http://blogs.microsoft.co.il/blogs/arik/archive/2010/05/28/wpf-single-instance-application.aspx
        /// </summary>
        [STAThread]
        public static void Main()
        {
            if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
            {
                var application = new App();

                application.InitializeComponent();
                application.Run();

                // Allow single instance code to perform cleanup operations
                SingleInstance<App>.Cleanup();
            }
        }

        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
            SourceLogLogger.LogInformation($"Application starting.. (version: {Assembly.GetExecutingAssembly().GetName().Version})");
        }

        static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            SourceLogLogger.Log(e.ExceptionObject);
        }

        private void ApplicationExit(object sender, ExitEventArgs e)
        {
            SourceLogLogger.LogInformation("Application closing...");
        }

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            var cmd = new ShowWindowCommand();
            cmd.Execute(MainWindow);

            return true;
        }
    }
}
