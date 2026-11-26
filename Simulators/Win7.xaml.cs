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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WindowsBootscreenSimulatorPlus
{
    /// <summary>
    /// Interaction logic for Win7.xaml
    /// </summary>
    public partial class Win7 : Window
    {
        bool terminate = false;
        public Win7()
        {
            InitializeComponent();
        }

        private void Window_MouseRightButtonUp(object sender, MouseButtonEventArgs e) => WbssCommon.ResetWindow(this, new Size(1024, 768));
        public void Window_KeyDown(object sender, KeyEventArgs e) => WbssCommon.CheckKey(this, e);
        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e) => WbssCommon.MouseDoubleClick(this);

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var loopPoint = 60;
            var delay = 80;
            var ms = new MemoryStream(Properties.Resources.activity);
            var animationBitmap = new BitmapImage();
            CroppedBitmap?[] frames = new CroppedBitmap?[105];
            new Thread(() =>
            {
                Thread.Sleep(5000);
                if (!terminate)
                {
                    Dispatcher.Invoke(() =>
                    {
                        animationBitmap.BeginInit();
                        animationBitmap.CacheOption = BitmapCacheOption.OnLoad;
                        animationBitmap.StreamSource = ms;
                        animationBitmap.EndInit();
                        for (int i = 0; i < 105; i++)
                        {
                            frames[i] = new(animationBitmap, new Int32Rect(0, 200 * i, 200, 200));
                        }
                    });
                }
                int i;

                // beginning
                for (i = 0; i < loopPoint; i++)
                {
                    if (terminate) break;
                    Thread.Sleep(delay);
                    Dispatcher.Invoke(() =>
                    {
                        GlowFunAnimation.Source = frames[i];
                    });
                }
                // loop
                while (!terminate)
                {
                    Thread.Sleep(delay);
                    Dispatcher.Invoke(() =>
                    {
                        GlowFunAnimation.Source = frames[i];
                    });
                    i++;
                    if (i > 104) i = loopPoint;
                }

                if (!terminate)
                {
                    Dispatcher.Invoke(() =>
                    {
                        animationBitmap = null;
                        for (int i = 0; i < 104; i++)
                        {
                            frames[i] = null;
                        }
                    });
                }
                ms.Close();
                GC.Collect();
            }).Start();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            terminate = true;
        }
    }
}
