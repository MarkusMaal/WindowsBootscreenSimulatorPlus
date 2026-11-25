using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WindowsBootscreenSimulatorPlus
{
    /// <summary>
    /// Interaction logic for WinNT.xaml
    /// </summary>
    public partial class WinNT : Window
    {
        public readonly Color background = Color.FromArgb(255, 0, 0, 160);
        public readonly Color foreground = Color.FromArgb(255, 170, 170, 170);
        public WinNT()
        {
            InitializeComponent();
        }

        private void Window_MouseRightButtonUp(object sender, MouseButtonEventArgs e) => WbssCommon.ResetWindow(this, new Size(640, 507));
        public void Window_KeyDown(object sender, KeyEventArgs e) => WbssCommon.CheckKey(this, e);
        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e) => WbssCommon.MouseDoubleClick(this);

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GenerateCenterBar.IsIndeterminate = false;
            GenerateCenter.Visibility = Visibility.Visible;
            new Thread(() =>
            {
                var pixels = new List<byte>();
                string[] bootscreenText = [
                    "Microsoft (R) Windows NT (TM) Version 4.0 (Build 1381: Service Pack 4). õõ",
                    "1 System Processor [64 MB Memory]"
                ];
                for (int i = 0; i < 640 * 480 * 3; i += 3)
                {
                    var rawPixelIdx = (i / 3);
                    var offsetX = (rawPixelIdx % 640);
                    var offsetY = (rawPixelIdx / 640);
                    var offsetX8 = offsetX % 8;
                    var offsetY8 = offsetY % 8;
                    if (offsetY / 8 >= bootscreenText.Length || offsetX / 8 >= bootscreenText[offsetY / 8].Length || !GetLetter(bootscreenText[offsetY / 8][offsetX / 8])[offsetY8, offsetX8])
                    {
                        pixels.Add(background.R);
                        pixels.Add(background.G);
                        pixels.Add(background.B);
                    }
                    else
                    {
                        pixels.Add(foreground.R);
                        pixels.Add(foreground.G);
                        pixels.Add(foreground.B);
                    }
                    if (i % 60 == 0)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            GenerateCenterBar.Value = (int)Math.Round(i / (double)(640 * 480 * 3) * 100.0);
                        });
                    }
                }
                Dispatcher.Invoke(() =>
                {
                    GenerateCenter.Visibility = Visibility.Collapsed;
                    var bmp = BitmapSource.Create(640, 480, 1.0, 1.0, PixelFormats.Rgb24, null, pixels.ToArray(), 640 * 3);
                    RasterImage.Source = bmp;
                    Cursor = Cursors.Arrow;
                });
            }).Start();
        }

        private static bool[,] GetLetter(char letter)
        {
            char[] charset = " ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789:,.+*()[]{}/\\-_".ToCharArray();
            var rawChars = Properties.Resources.NtChars;
            bool[,] output = new bool[8,8];
            var offset = 0;
            var lowerCase = char.IsLower(letter);
            
            letter = char.ToUpper(letter);
            var found = false;
            for (int i = 0; i < charset.Length; i++)
            {
                if (charset[i] == letter)
                {
                    found = true;
                    offset = i;
                    break;
                }
            }
            if (!found)
            {
                offset = 0;
                lowerCase = true;
            }
            for (int i = offset * 8; i < offset * 8 + 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    output[i - offset * 8, j] = rawChars.Split(Environment.NewLine)[i][j + (lowerCase ? 8 : 0)] == '#';
                }
            }
            return output;
        }
    }
}
