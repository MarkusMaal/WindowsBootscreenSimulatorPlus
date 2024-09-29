using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace WindowsBootscreenSimulatorPlus
{
    /// <summary>
    /// Interaction logic for Win98.xaml
    /// </summary>
    public partial class Win98 : Window
    {
        public Win98()
        {
            InitializeComponent();
        }

        private void AnimateBar(int time)
        {
            DoubleAnimation da = new DoubleAnimation
                (0, -640, new Duration(TimeSpan.FromSeconds(time)));

            StackPanel[] images = [Slit];
            DoubleAnimation[] animations = [da];
            TranslateTransform[] transforms = [new TranslateTransform()];
            for (int i = 0; i < images.Length; i++)
            {
                images[i].RenderTransform = transforms[i];
                images[i].RenderTransformOrigin = new Point(0, 0);
                animations[i].RepeatBehavior = RepeatBehavior.Forever;
            }
            for (int i = 0; i < images.Length; i++) {
                transforms[i].BeginAnimation(TranslateTransform.XProperty, animations[i]);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AnimateBar(5);
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (WindowStyle == WindowStyle.SingleBorderWindow)
            {
                WindowStyle = WindowStyle.None;
                WindowState = WindowState.Maximized;
            } else
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
            
        }
    }
}
