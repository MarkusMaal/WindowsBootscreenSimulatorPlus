using System.Windows;

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
    }
}
