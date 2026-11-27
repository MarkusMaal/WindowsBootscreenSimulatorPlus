using System.Data;
using System.Windows;

namespace WindowsBootscreenSimulatorPlus
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Terminates the app if every window is closed (i.e. kills orphaned threads).
        /// This ensures the application doesn't stay running in the background.
        /// </summary>
        public static void CheckWindows()
        {
            if (Current.Windows.Cast<Window>().Where(win => win.IsLoaded).ToArray().Length == 0)
                Environment.Exit(0);
        }
    }


}
