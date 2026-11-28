using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WindowsBootscreenSimulatorPlus.Simulators
{
    /// <summary>
    /// Interaction logic for Win3.xaml
    /// </summary>
    public partial class Win3 : Window
    {
        public required string Copyright { get; init; }
        public required string Version { get; init; }
        public required string OS { get; init; }
        public required byte[] Graphic { get; init; }
        public Win3()
        {
            InitializeComponent();
        }

        private void Window_MouseRightButtonUp(object sender, MouseButtonEventArgs e) => WbssCommon.ResetWindow(this, new Size(640, 507));
        public void Window_KeyDown(object sender, KeyEventArgs e) => WbssCommon.CheckKey(this, e);
        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e) => WbssCommon.MouseDoubleClick(this);

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            // generate dither pattern
            var cols = new List<Color>
            {
                Color.FromArgb(255, 85, 85, 170),
                Colors.Black
            };
            var pixels = new byte[4];
            pixels[0] = 1;
            pixels[3] = 1;
            RootGrid.Background = new ImageBrush()
            {
                TileMode = TileMode.Tile,
                ViewportUnits = BrushMappingMode.Absolute,
                Viewport = new Rect(0, 0, 2, 2),
                ImageSource = BitmapSource.Create(2, 2, 1.0, 1.0, PixelFormats.Indexed8, new BitmapPalette(cols), pixels, 2)
            };

            var ms = new MemoryStream(Graphic);
            var logoBitmap = new BitmapImage();
            logoBitmap.BeginInit();
            logoBitmap.StreamSource = ms;
            logoBitmap.EndInit();
            MsftLogo.Source = logoBitmap;
            CenterText.Text = OS;
            VersionText.Text = Version;
            BottomText.Content = Copyright;
        }
    }
}
