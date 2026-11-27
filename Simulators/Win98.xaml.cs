using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

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
            DoubleAnimation da1 = new(0, -Slit1.ActualWidth, new Duration(TimeSpan.FromSeconds(time)));
            DoubleAnimation da2 = new(0, -Slit2.ActualWidth, new Duration(TimeSpan.FromSeconds(time)));

            Image[] images = [Slit1, Slit2];
            DoubleAnimation[] animations = [da1, da2];
            TranslateTransform[] transforms = [new TranslateTransform(), new TranslateTransform()];
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
        public void Window_KeyDown(object sender, KeyEventArgs e) => WbssCommon.CheckKey(this, e);
        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e) => WbssCommon.MouseDoubleClick(this);
        private void Window_MouseRightButtonUp(object sender, MouseButtonEventArgs e) => WbssCommon.ResetWindow(this, new Size(640, 429));
    }
}
