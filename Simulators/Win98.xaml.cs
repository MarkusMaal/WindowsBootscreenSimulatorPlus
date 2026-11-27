using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace WindowsBootscreenSimulatorPlus
{
    /// <summary>
    /// Interaction logic for Win98.xaml
    /// </summary>
    public partial class Win98 : Window
    {
        internal bool Win95Mode { get; init; }

        public Win98()
        {
            InitializeComponent();
        }

        private void AnimateBar(int time)
        {
            var shift = Win95Mode ? 0 : -640;
            new Thread(() =>
            {
                while (true)
                {
                    Dispatcher.Invoke(() =>
                    {
                        Slit1.RenderTransform = new TranslateTransform(shift, 0);
                        Slit2.RenderTransform = new TranslateTransform(shift, 0);
                        shift += Win95Mode ? -32 : 32;
                        if (shift > 0 && !Win95Mode) shift = -640;
                        if (shift < -640 && Win95Mode) shift = 0;
                    });
                    Thread.Sleep(100);
                }
            }).Start();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AnimateBar(5);
        }
        public void Window_KeyDown(object sender, KeyEventArgs e) => WbssCommon.CheckKey(this, e);
        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e) => WbssCommon.MouseDoubleClick(this);
        private void Window_MouseRightButtonUp(object sender, MouseButtonEventArgs e) => WbssCommon.ResetWindow(this, new Size(640, 429));
    }
}
