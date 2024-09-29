using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WindowsBootscreenSimulatorPlus
{
    /// <summary>
    /// Interaction logic for WinXP.xaml
    /// </summary>
    public partial class WinXP : Window
    {
        public WinXP()
        {
            InitializeComponent();
        }
        int pos = 1;
        private Image throbberA;
        private Image throbberB;
        private Image throbberC;

        private void AnimateBar()
        {
            DispatcherTimer dispatcherTimer = new()
            {
                Interval = new TimeSpan(0, 0, 0, 0, 50)
            };
            dispatcherTimer.Tick += (object? sender, EventArgs e) =>
            {
                if ((pos - 4 > 0) && (pos - 4 < 16))
                {
                    ((Image)this.FindName("Throbber" + (pos - 4).ToString())).Source = null;
                }
                if ((pos - 3 > 0) && (pos - 3 < 16))
                {
                    ((Image)this.FindName("Throbber" + (pos - 3).ToString())).Source = throbberA.Source;
                }
                if ((pos - 2 > 0) && (pos - 2 < 16))
                {
                    ((Image)this.FindName("Throbber" + (pos - 2).ToString())).Source = throbberB.Source;
                }
                if ((pos - 1 > 0) && (pos - 1 < 16))
                {
                    ((Image)this.FindName("Throbber" + (pos - 1).ToString())).Source = throbberC.Source;
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
            throbberA = GetCroppedImageResource(Properties.Resources.xp_blue_throbber, new Int32Rect(0, 0, 6, 9));
            throbberB = GetCroppedImageResource(Properties.Resources.xp_blue_throbber, new Int32Rect(8, 0, 6, 9));
            throbberC = GetCroppedImageResource(Properties.Resources.xp_blue_throbber, new Int32Rect(16, 0, 6, 9));
            AnimateBar();
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

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (WindowStyle == WindowStyle.SingleBorderWindow)
            {
                WindowStyle = WindowStyle.None;
                WindowState = WindowState.Maximized;
            }
            else
            {
                WindowState = WindowState.Normal;
                WindowStyle = WindowStyle.SingleBorderWindow;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Escape) || (e.Key == Key.F7))
            {
                Close();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            throbberA.Source = null;
            throbberB.Source = null;
            throbberC.Source = null;
        }
    }
}
