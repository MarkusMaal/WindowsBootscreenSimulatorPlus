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
using static System.Net.Mime.MediaTypeNames;

namespace WindowsBootscreenSimulatorPlus
{
    /// <summary>
    /// Interaction logic for Win2k.xaml
    /// </summary>
    public partial class Win2k : Window
    {
        public Color progressColor = Color.FromArgb(0xFF, 0x00, 0x28, 0x9C);
        public int[] increments = new int[19];

        private int increment = 0;

        public Win2k()
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
            for (int i = 0; i < images.Length; i++)
            {
                transforms[i].BeginAnimation(TranslateTransform.XProperty, animations[i]);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetDots();
            AnimateBar(5);
        }

        private void SetDots()
        {
            var dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler((s, x) =>
            {
                DrawDot(increment);
                increment++;
                if (increment == increments.Length)
                {
                    dispatcherTimer.Stop();
                    return;
                }
                dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, increments[increment]);

            });

            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, increments[0]);

            dispatcherTimer.Start();
        }

        // draws a rectangle to the progress bar
        private void DrawDot(int increment)
        {
            if (increment > 18) { return; }
            Point startPoint;
            Rectangle rect;
            startPoint = new Point(274 + (9 * increment), 11);

            rect = new Rectangle
            {
                Fill = new SolidColorBrush(progressColor),
                Stroke = null,
                Width = increment < 18 ? 8 : 1,
                Height = 8
            };
            Canvas.SetLeft(rect, startPoint.X);
            Canvas.SetTop(rect, startPoint.Y);
            BottomLoader.Children.Add(rect);
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

        }
    }
}
