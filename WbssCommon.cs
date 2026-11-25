using System.Windows;
using System.Windows.Input;

namespace WindowsBootscreenSimulatorPlus
{
    public abstract class WbssCommon
    {
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
    }
}
