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
            DoubleAnimation da = new(0, -640, new Duration(TimeSpan.FromSeconds(time)));

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
        public void Window_KeyDown(object sender, KeyEventArgs e) => WbssCommon.CheckKey(this, e);
        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e) => WbssCommon.MouseDoubleClick(this);
        private void Window_MouseRightButtonUp(object sender, MouseButtonEventArgs e) => WbssCommon.ResetWindow(this, new Size(640, 429));
    }
}
