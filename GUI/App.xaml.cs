using System;
using System.Windows;

namespace GUI
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AppDomain.CurrentDomain.UnhandledException += UnhandledException;
        }

        private void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An unknown error occurred. Please screenshot this box and send it to Pyritie.\n\n" + e.ExceptionObject);
        }
    }
}
