using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace WindowsBootscreenSimulatorPlus
{
    /// <summary>
    /// Interaction logic for WinXP.xaml
    /// </summary>
    public partial class WinXP : Window
    {

        internal bool FadeIn { get; init; } = false;
        public WinXP()
        {
            InitializeComponent();
        }
        int pos = 1;
        private Image? throbberA;
        private Image? throbberB;
        private Image? throbberC;

        private void AnimateBar()
        {
            DispatcherTimer dispatcherTimer = new()
            {
                Interval = new TimeSpan(0, 0, 0, 0, 50)
            };
            dispatcherTimer.Tick += (sender, e) =>
            {
                if ((pos - 4 > 0) && (pos - 4 < 16))
                {
                    ((Image)this.FindName("Throbber" + (pos - 4).ToString())).Source = null;
                }
                if ((pos - 3 > 0) && (pos - 3 < 16))
                {
                    ((Image)this.FindName("Throbber" + (pos - 3).ToString())).Source = throbberA?.Source;
                }
                if ((pos - 2 > 0) && (pos - 2 < 16))
                {
                    ((Image)this.FindName("Throbber" + (pos - 2).ToString())).Source = throbberB?.Source;
                }
                if ((pos - 1 > 0) && (pos - 1 < 16))
                {
                    ((Image)this.FindName("Throbber" + (pos - 1).ToString())).Source = throbberC?.Source;
                }
                if ((pos > 0) && (pos < 16))
                {
                    ((Image)this.FindName("Throbber" + (pos).ToString())).Source = null;
                }

                pos++;
                if (pos == 20)
                {
                    pos = 1;
                }
            };
            dispatcherTimer.Start();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            new Thread(() =>
            {
                if (FadeIn)
                {
                    Thread.Sleep(500);
                    var levels = 16.0;
                    for (int i = 0; i < (int)levels; i++)
                    {
                        Dispatcher.Invoke(() => RootGrid.Opacity += 1.0 / levels);
                        Thread.Sleep(100);
                    }
                    Thread.Sleep(100);
                }
                Dispatcher.Invoke(() =>
                {
                    RootGrid.Opacity = 1.0;
                    throbberA = GetCroppedImageResource(Properties.Resources.xp_blue_throbber, new Int32Rect(0, 0, 6, 9));
                    throbberB = GetCroppedImageResource(Properties.Resources.xp_blue_throbber, new Int32Rect(8, 0, 6, 9));
                    throbberC = GetCroppedImageResource(Properties.Resources.xp_blue_throbber, new Int32Rect(16, 0, 6, 9));
                    AnimateBar();
                });
            }).Start();
        }

        private static Image GetCroppedImageResource(byte[] data, Int32Rect crop)
        {
            BitmapImage biImg = new();
            MemoryStream ms = new(data);

            biImg.BeginInit();
            biImg.StreamSource = ms;
            biImg.EndInit();
            Image cropped = new()
            {
                Source = CropBitmapImage(biImg, crop)
            };
            return cropped;
        }

        public static CroppedBitmap CropBitmapImage(BitmapImage source, Int32Rect cropArea)
        {
            CroppedBitmap cb = new(source, cropArea);
            return cb;
        }


        private void Window_Closed(object sender, EventArgs e)
        {
            if (throbberA == null || throbberB == null || throbberC == null) return;
            throbberA.Source = null;
            throbberB.Source = null;
            throbberC.Source = null;
        }

        private void Window_MouseRightButtonUp(object sender, MouseButtonEventArgs e) => WbssCommon.ResetWindow(this, new Size(640, 480));
        public void Window_KeyDown(object sender, KeyEventArgs e) => WbssCommon.CheckKey(this, e);
        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e) => WbssCommon.MouseDoubleClick(this);
    }
}
