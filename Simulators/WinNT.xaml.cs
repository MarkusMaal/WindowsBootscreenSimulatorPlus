using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WindowsBootscreenSimulatorPlus
{
    /// <summary>
    /// Interaction logic for WinNT.xaml
    /// </summary>
    public partial class WinNT : Window
    {
        public readonly Color background = Color.FromArgb(255, 0, 0, 160);
        public readonly Color foreground = Color.FromArgb(255, 170, 170, 170);

        public required string OsInfo { get; init; }
        public required string HardInfo { get; init; }

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetPhysicallyInstalledSystemMemory(out long TotalMemoryInKilobytes);

        public WinNT()
        {
            InitializeComponent();
        }

        private void Window_MouseRightButtonUp(object sender, MouseButtonEventArgs e) => WbssCommon.ResetWindow(this, new Size(640, 507));
        public void Window_KeyDown(object sender, KeyEventArgs e) => WbssCommon.CheckKey(this, e);
        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e) => WbssCommon.MouseDoubleClick(this);

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                GenerateCenterText.Visibility = Visibility.Visible;
            }

            new Thread(() =>
            {
                var pixels = new byte[640 * 480 * 3];
                GetPhysicallyInstalledSystemMemory(out long memKb);
                string[] bootscreenText = [
                    OsInfo,
                    string.Format(HardInfo, memKb / 1024)
                ];
                try
                {
                    Dispatcher.Invoke(() =>
                    {
                        var bmp = WbssCommon.DrawCharacters(background, foreground, bootscreenText, 640, 480);
                        GenerateCenterText.Visibility = Visibility.Collapsed;
                        RasterImage.Source = bmp;
                        if (Cursor != Cursors.None)
                            Cursor = Cursors.Arrow;
                    });
                }
                catch (TaskCanceledException)
                {
                    Debug.WriteLine("[DEBUG] Cannot draw image - main thread has exited!");
                    return;
                }
            }).Start();
        }
    }
}
