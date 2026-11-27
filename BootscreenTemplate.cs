using System.Diagnostics;
using System.IO;
using System.Text.Json.Serialization;
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

        [JsonInclude]
        private Simulator SimulatorType { get; set; }

        [JsonInclude]
        private Dictionary<string, string> Strings { get; set; } = [];

        [JsonInclude]
        private Dictionary<string, byte[]> Data { get; set; } = [];

        [JsonInclude]
        private Dictionary<string, int> Integers { get; set; } = [];

        [JsonInclude]
        private Dictionary<string, int[]> Progressors { get; set; } = [];

        [JsonInclude]
        private Dictionary<string, bool> Bools { get; set; } = [];

        [JsonInclude]
        private readonly List<string> HiddenValues = [];

        [JsonIgnore]
        public string Name => GetStringValue("FriendlyName");

        [JsonIgnore]
        public string Type => SimulatorType.ToString();

        public BootscreenTemplate() {}

        /// <summary>
        /// Create bootscreen template object
        /// </summary>
        /// <param name="simulator">Simulator</param>
        /// <param name="friendlyName">Friendly name, displayed to the user</param>
        public BootscreenTemplate(Simulator simulator, string friendlyName)
        {
            SimulatorType = simulator;
            Strings["FriendlyName"] = friendlyName;
            InitializeSimulator();
        }

        public void SetValue(string key, string value) => Strings[key] = value;
        public void SetValue(string key, byte[] value) => Data[key] = value;
        public void SetValue(string key, int value) => Integers[key] = value;
        public void SetValue(string key, int[] value) => Progressors[key] = value;
        public void SetValue(string key, bool value) => Bools[key] = value;

        public string GetStringValue(string key) => Strings.TryGetValue(key, out var value) ? value : "";
        public byte[] GetDataValue(string key) => Data.TryGetValue(key, out var value) ? value : [];
        public int GetIntValue(string key) => Integers.TryGetValue(key, out var value) ? value : -1;
        public int[] GetProgressorValue(string key) => Progressors.TryGetValue(key, out var value) ? value : [];
        public bool GetBoolValue(string key) => Bools.TryGetValue(key, out var val) && val;

        public string[] GetAllStringKeys() => [.. Strings.Where(p => !HiddenValues.Contains(p.Key)).Select(p => p.Key)];
        public string[] GetAllDataKeys() => [.. Data.Where(p => !HiddenValues.Contains(p.Key)).Select(p => p.Key)];
        public string[] GetAllIntKeys() => [.. Integers.Where(p => !HiddenValues.Contains(p.Key)).Select(p => p.Key)];
        public string[] GetAllProgressorKeys() => [.. Progressors.Where(p => !HiddenValues.Contains(p.Key)).Select(p => p.Key)];
        public string[] GetAllBoolKeys() => [.. Bools.Where(p => !HiddenValues.Contains(p.Key)).Select(p => p.Key)];

        public Simulator GetSimulatorType() => SimulatorType;
        public void SetSimulatorType(Simulator type) => SimulatorType = type;

        public void HideValue(string key) => HiddenValues.Add(key);

        /// <summary>
        /// Allows you to directly convert data value to BitmapImage
        /// </summary>
        /// <param name="key">Key of Data dictionary</param>
        /// <returns>Two part tuple, first the BitmapImage and then the unclosed memory stream, which the receiver should close when applicable</returns>
        public (BitmapImage, MemoryStream) GetImage(string key)
        {
            var bmp = new BitmapImage();
            bmp.BeginInit();
            var ms = new MemoryStream();
            try
            {
                ms.Write(Data[key]);
                bmp.StreamSource = ms;
                bmp.EndInit();
            } catch (Exception ex)
            {
                Debug.WriteLine("[DEBUG] ERROR: " + ex.Message);
            }
            return (bmp, ms);
        }

        /// <summary>
        /// Runs the simulator
        /// </summary>
        public void Show()
        {
            var formCursor = GetBoolValue("Show cursor") ? Cursors.Arrow : Cursors.None;
            Window w = new();
            switch (SimulatorType)
            {
                case Simulator.WindowsMetro:
                    w = new Win10();
                    if (w is not Win10 w10) break;
                    w10.Win11Mode = GetBoolValue("FlatLogo");
                    w10.InitialDelay = GetIntValue("Delay");
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
                    w = new Win98()
                    {
                        Win95Mode = GetBoolValue("Invert")
                    };
                    var (graphic, ms) = GetImage("Graphic");
                    var (slit, mss) = GetImage("Slit");

                    if (w is not Win98 w98) break;
                    w98.LogoImage.Source = graphic;
                    w98.Slit1.Source = slit;
                    w98.Slit2.Source = slit;
                    w98.Closed += (_, _) =>
                    {
                        ms.Close();
                        mss.Close();
                    };
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
                        w2k.LogoImage.Source = w2kgs;
                        w2k.SlitA.Source = w2kss;
                        w2k.SlitB.Source = w2kss;
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

        /// <summary>
        /// Intialize simulator with default values
        /// </summary>
        private void InitializeSimulator()
        {
            SetValue("Stretch", false);
            SetValue("Fullscreen", false);
            SetValue("Show cursor", true);
            switch (SimulatorType)
            {
                case Simulator.WindowsMetro:
                    SetValue("Delay", 4900);
                    HideValue("Delay");
                    break;
                case Simulator.Windows2000:
                    SetValue("Server boot screen", false);
                    break;
                case Simulator.WindowsXPVista:
                    SetValue("Simulate fade-in", true);
                    break;
                case Simulator.Windows9x:
                    HideValue("Invert");
                    break;
            }
        }
    }
}
