using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WindowsBootscreenSimulatorPlus.Simulators
{
    /// <summary>
    /// Interaction logic for Win1.xaml
    /// </summary>
    public partial class Win1 : Window
    {
        public required string OSName { get; init; }
        public required string Version { get; init; }
        public required string Copyright { get; init; }
        public required string Trademark { get; init; }
        public required byte[] LogoData { get; init; }
        public Win1()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            new Thread(() =>
            {
                for (int i = 0; i < 8; i++)
                {
                    Dispatcher.Invoke(() => InterleaveLogo(i));
                    Thread.Sleep(64);
                }
            }).Start();
            string[] lines = [OSName, Version];
            var bgCol = ((SolidColorBrush)Background).Color;
            var fgCol = ((SolidColorBrush)Foreground).Color;
            CenterText.Source = WbssCommon.DrawCharacters(bgCol, fgCol, lines, 8 * lines[0].Length, lines.Length * 8);
            CenterText.Width = lines[0].Length * 4;
            lines = [Copyright, Trademark];
            BottomText.Source = WbssCommon.DrawCharacters(bgCol, fgCol, lines, 8 * lines[0].Length, lines.Length * 8);
            BottomText.Width = lines[0].Length * 4;
        }

        private void InterleaveLogo(int shift)
        {

            var outputWidth = 81;
            var pixels = new byte[outputWidth * 36];
            var scanLine1 = LogoData.Take(67 * 36 / 2).ToArray();
            var scanLine2 = LogoData.Skip(67 * 36 / 2).Take(67 * 36 / 2).ToArray();

            List<Color> colorPalette = [];
            colorPalette.Add(((SolidColorBrush)Background).Color);
            colorPalette.Add(Colors.Black);
            colorPalette.Add(Colors.White);
            colorPalette.Add(((SolidColorBrush)Foreground).Color);

            for (int y = 0; y < 36; y++)
            {
                for (int x = 0; x < 67; x++)
                {
                    if (y % 2 == 0)
                    {
                        pixels[y * outputWidth + x + shift] = scanLine1[67 * (y / 2) + x];
                    }
                    else
                    {
                        pixels[(y+1) * outputWidth - 67 + x - shift] = scanLine2[67 * (y / 2) + x];
                    }
                }
            }


            LogoTest.Source = BitmapSource.Create(outputWidth * 4, 36, 1.0, 1.0, PixelFormats.Indexed2, new BitmapPalette(colorPalette), pixels.ToArray(), outputWidth);
        }

        private void Window_MouseRightButtonUp(object sender, MouseButtonEventArgs e) => WbssCommon.ResetWindow(this, new Size(640, 400));
        public void Window_KeyDown(object sender, KeyEventArgs e) => WbssCommon.CheckKey(this, e);
        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e) => WbssCommon.MouseDoubleClick(this);
    }
}
