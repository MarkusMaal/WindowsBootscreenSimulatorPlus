using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WindowsBootscreenSimulatorPlus
{
    public class BootscreenTemplate
    {
        public enum Simulator
        {
            WindowsMetro,
            Windows2000,
            Windows7,
            Windows9x,
            WindowsNT,
            WindowsXPVista
        }

        private readonly Simulator _simulator;

        private Dictionary<string, string> Strings { get; set; } = [];

        private Dictionary<string, byte[]> Data { get; set; } = [];

        private Dictionary<string, int> Integers { get; set; } = [];

        private Dictionary<string, int[]> Progressors { get; set; } = [];
        private Dictionary<string, bool> Bools { get; set; } = [];

        private readonly List<string> HiddenValues = [];


        public BootscreenTemplate(Simulator simulator, string friendlyName)
        {
            _simulator = simulator;
            Strings["FriendlyName"] = friendlyName;
            InitializeSimulator();
        }

        public void SetValue(string key, string value) => Strings[key] = value;
        public void SetValue(string key, byte[] value) => Data[key] = value;
        public void SetValue(string key, int value) => Integers[key] = value;
        public void SetValue(string key, int[] value) => Progressors[key] = value;
        public void SetValue(string key, bool value) => Bools[key] = value;

        public string GetStringValue(string key) => Strings[key];
        public byte[] GetDataValue(string key) => Data[key];
        public int GetIntValue(string key) => Integers[key];
        public int[] GetProgressorValue(string key) => Progressors[key];
        public bool GetBoolValue(string key) => Bools[key];

        public string[] GetAllStringKeys() => [.. Strings.Where(p => !HiddenValues.Contains(p.Key)).Select(p => p.Key)];
        public string[] GetAllDataKeys() => [.. Data.Where(p => !HiddenValues.Contains(p.Key)).Select(p => p.Key)];
        public string[] GetAllIntKeys() => [.. Integers.Where(p => !HiddenValues.Contains(p.Key)).Select(p => p.Key)];
        public string[] GetAllProgressorKeys() => [.. Progressors.Where(p => !HiddenValues.Contains(p.Key)).Select(p => p.Key)];
        public string[] GetAllBoolKeys() => [.. Bools.Where(p => !HiddenValues.Contains(p.Key)).Select(p => p.Key)];

        public void HideValue(string key) => HiddenValues.Add(key);

        public (BitmapImage, MemoryStream) GetImage(string key)
        {
            var bmp = new BitmapImage();
            bmp.BeginInit();
            var ms = new MemoryStream(Data[key]);
            bmp.StreamSource = ms;
            bmp.EndInit();
            return (bmp, ms);
        }

        public void Show()
        {
            var formCursor = GetBoolValue("Show cursor") ? Cursors.Arrow : Cursors.None;
            Window w = new();
            switch (_simulator)
            {
                case Simulator.WindowsMetro:
                    w = new Win10();
                    break;
                case Simulator.WindowsXPVista:
                    w = new WinXP()
                    {
                        Throbber = GetDataValue("Throbber"),
                        Graphic = GetDataValue("Graphic"),
                        FadeIn = GetBoolValue("Simulate fade-in")
                    };
                    break;
                case Simulator.Windows9x:
                    w = new Win98();
                    var (graphic, ms) = GetImage("Graphic");
                    if (w is not Win98 w98) break;
                    w98.LogoImage.Source = graphic;
                    w98.Closed += (_, _) => ms.Close();
                    break;
                case Simulator.Windows2000:
                    w = new Win2k();
                    if (w is not Win2k w2k) break;
                    var (w2kgs, ms1) = GetImage("GraphicServer");
                    var (w2kss, ms2) = GetImage("SlitServer");
                    var (w2kg, ms3) = GetImage("Graphic");
                    var (w2ks, ms4) = GetImage("Slit");
                    if (GetBoolValue("Server boot screen"))
                    {
                        w2k.LogoImage.Source = w2kg;
                        w2k.SlitA.Source = w2ks;
                        w2k.SlitB.Source = w2ks;
                        w2k.progressColor = Color.FromArgb(0xFF, 0x30, 0x30, 0x98);
                    }
                    Random r = new();
                    for (int i = 0; i < 19; i++)
                    {
                        w2k.increments[i] = r.Next(0, 5000);
                    }
                    w2k.Closing += (_, _) =>
                    {
                        ms1.Close();
                        ms2.Close();
                        ms3.Close();
                        ms4.Close();
                    };
                    break;
                case Simulator.WindowsNT:
                    w = new WinNT();
                    break;
                case Simulator.Windows7:
                    w = new Win7();
                    break;
            }
            w.WindowStyle = GetBoolValue("Fullscreen") ? WindowStyle.None : WindowStyle.SingleBorderWindow;
            w.WindowState = GetBoolValue("Fullscreen") ? WindowState.Maximized : WindowState.Normal;
            if (w.FindName("VBox") is Viewbox vb)  
                vb.Stretch = GetBoolValue("Stretch") ? Stretch.Fill : Stretch.Uniform;
            w.Cursor = formCursor;
            w.Closed += (_, _) => App.CheckWindows();
            w.Show();
        }

        private void InitializeSimulator()
        {
            SetValue("Stretch", false);
            SetValue("Fullscreen", false);
            SetValue("Show cursor", true);
            switch (_simulator)
            {
                case Simulator.WindowsMetro:
                    SetValue("Delay", 4900);
                    HideValue("Delay");
                    break;
                case Simulator.Windows2000:
                    SetValue("Server boot screen", false);
                    break;
                case Simulator.WindowsXPVista:
                    SetValue("Simulate fade-in", false);
                    break;
            }
        }
    }
}
