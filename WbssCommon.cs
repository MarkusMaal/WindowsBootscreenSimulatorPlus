using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WindowsBootscreenSimulatorPlus
{
    public abstract class WbssCommon
    {
        private static readonly Dictionary<char, bool[,]> letterCache = [];
        public static void ResetWindow(Window w, Size size)
        {
            var bck = w.WindowStyle;
            w.WindowStyle = WindowStyle.None;
            w.Width = size.Width;
            w.Height = size.Height;
            w.WindowStyle = bck;
        }

        public static void MouseDoubleClick(Window w)
        {
            if (w.WindowStyle == WindowStyle.SingleBorderWindow)
            {
                w.WindowStyle = WindowStyle.None;
                w.WindowState = WindowState.Maximized;
            }
            else
            {
                w.WindowState = WindowState.Normal;
                w.WindowStyle = WindowStyle.SingleBorderWindow;
            }
        }

        public static void CheckKey(Window w, KeyEventArgs e)
        {
            if ((e.Key != Key.Escape) && (e.Key != Key.F7)) return;
            w.Close();
        }

        public static BitmapSource DrawCharacters(Color background, Color foreground, string[] lines, int width, int height)
        {
            letterCache.Clear();
            var pixels = new byte[width * height * 3];
            var finished = 0;
            var threadCnt = Math.Max(Environment.ProcessorCount, 4); // a lot of threads can be slower in some cases, 4 threads seems to be a good balance
            for (int t = 0; t < threadCnt; t++)
            {
                new Thread(() =>
                {
                    for (int i = 0; i < width * height * 3; i += 3)
                    {
                        if ((i / 3) % threadCnt == t) continue;
                        var rawPixelIdx = (i / 3);
                        var offsetX = (rawPixelIdx % width);
                        var offsetY = (rawPixelIdx / width);
                        var offsetX8 = offsetX % 8;
                        var offsetY8 = offsetY % 8;
                        if (offsetY / 8 >= lines.Length || offsetX / 8 >= lines[offsetY / 8].Length || !GetLetter(lines[offsetY / 8][offsetX / 8])[offsetY8, offsetX8])
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
                        if (t == 1) Thread.Sleep(10);
                    }
                    finished++;
                }).Start();
            }
            while (finished < threadCnt)
            {
                Thread.Sleep(16);
            }
            return BitmapSource.Create(width, height, 1.0, 1.0, PixelFormats.Rgb24, null, pixels.ToArray(), width * 3);
        }

        public static bool[,] GetLetter(char letter)
        {
            while (true)
            {
                try
                {
                    if (letterCache.TryGetValue(letter, out bool[,]? value1)) return value1;
                    char originalLetter = letter;
                    char[] charset = " ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789:,.+*()[]{}/\\-_".ToCharArray();
                    var rawChars = Properties.Resources.NtChars;
                    bool[,] output = new bool[8, 8];
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
                catch (InvalidOperationException)
                {
                    Random r = new();
                    Thread.Sleep(r.Next(5, 200));
                }
            }
        }
    }
}
