using System.Diagnostics;
using System.Runtime.InteropServices;
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
        private static Dictionary<char, bool[,]> letterCache = [];

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetPhysicallyInstalledSystemMemory(out long TotalMemoryInKilobytes);

        public WinNT()
        {
            InitializeComponent();
        }

        private void Window_MouseRightButtonUp(object sender, MouseButtonEventArgs e) => WbssCommon.ResetWindow(this, new Size(640, 507));
        public void Window_KeyDown(object sender, KeyEventArgs e) => WbssCommon.CheckKey(this, e);
        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e) => WbssCommon.MouseDoubleClick(this);

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                GenerateCenterText.Visibility = Visibility.Visible;
            }
            new Thread(() =>
            {
                var pixels = new byte[640 * 480 * 3];
                GetPhysicallyInstalledSystemMemory(out long memKb);
                string[] bootscreenText = [
                    "Microsoft (R) Windows NT (TM) Version 4.0 (Build 1381: Service Pack 4).",
                    $"1 System Processor [{memKb / 1024} MB Memory]"
                ];
                var finished = 0;
                for (int t = 0; t < Environment.ProcessorCount; t++)
                {
                    new Thread(() =>
                    {
                        for (int i = 0; i < 640 * 480 * 3; i += 3)
                        {
                            if ((i / 3) % Environment.ProcessorCount == t) continue;
                            var rawPixelIdx = (i / 3);
                            var offsetX = (rawPixelIdx % 640);
                            var offsetY = (rawPixelIdx / 640);
                            var offsetX8 = offsetX % 8;
                            var offsetY8 = offsetY % 8;
                            if (offsetY / 8 >= bootscreenText.Length || offsetX / 8 >= bootscreenText[offsetY / 8].Length || !GetLetter(bootscreenText[offsetY / 8][offsetX / 8])[offsetY8, offsetX8])
                            {
                                pixels[i] = background.R;
                                pixels[i + 1] = background.G;
                                pixels[i + 2] = background.B;
                            }
                            else
                            {
                                pixels[i] = foreground.R;
                                pixels[i + 1] = foreground.G;
                                pixels[i + 2] = foreground.B;
                            }
                        }
                        finished++;
                    }).Start();
                }
                while (finished < Environment.ProcessorCount)
                {
                    Thread.Sleep(16);
                }
                try
                { 
                    Dispatcher.Invoke(() =>
                    {
                        GenerateCenterText.Visibility = Visibility.Collapsed;
                        var bmp = BitmapSource.Create(640, 480, 1.0, 1.0, PixelFormats.Rgb24, null, pixels.ToArray(), 640 * 3);
                        RasterImage.Source = bmp;
                        if (Cursor != Cursors.None)
                            Cursor = Cursors.Arrow;
                    });
                }
                catch (TaskCanceledException)
                {
                    Debug.WriteLine("[DEBUG] Cannot draw image - main thread has exited!");
                    return;
                }
            }).Start();
        }

        private static bool[,] GetLetter(char letter)
        {
            while (true)
            {
                try
                {
                    if (letterCache.TryGetValue(letter, out bool[,]? value1)) return value1;
                    break;
                }
                catch (InvalidOperationException)
                {
                    Thread.Sleep(1);
                }
            }
            char originalLetter = letter;
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
            letterCache[originalLetter] = output;
            return output;
        }
    }
}
