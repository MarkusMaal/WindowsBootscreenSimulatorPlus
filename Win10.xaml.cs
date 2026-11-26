using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace WindowsBootscreenSimulatorPlus
{
    /// <summary>
    /// Interaction logic for Win10.xaml
    /// </summary>
    public partial class Win10 : Window
    {
        public Win10()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            new Thread(() =>
            {
                Thread.Sleep(4900); // show dots after one animation cycle
                try
                {
                    Dispatcher.Invoke(() =>
                    {
                        Dot1.Visibility = Visibility.Visible;
                        Dot2.Visibility = Visibility.Visible;
                        Dot3.Visibility = Visibility.Visible;
                        Dot4.Visibility = Visibility.Visible;
                        Dot5.Visibility = Visibility.Visible;
                    });
                } catch (TaskCanceledException)
                {
                    Debug.WriteLine("[DEBUG] Cancelled metro bootscreen");
                    return;
                }
            }).Start();
        }

        private void Window_MouseRightButtonUp(object sender, MouseButtonEventArgs e) => WbssCommon.ResetWindow(this, new Size(1024, 768));
        public void Window_KeyDown(object sender, KeyEventArgs e) => WbssCommon.CheckKey(this, e);
        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e) => WbssCommon.MouseDoubleClick(this);

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // fade out on close
            if (RootGrid.Opacity != 1.0)
            {
                e.Cancel = false;
                return;
            }
            e.Cancel = true;
            new Thread(() =>
            {
                for (int i = 0; i < 25; i++)
                {
                    Dispatcher.Invoke(() => RootGrid.Opacity -= 0.128);
                    Thread.Sleep(16);
                }
                Dispatcher.Invoke(Close); // this is fine, because we do an opacity check at the beginning
            }).Start();
        }
    }
}
