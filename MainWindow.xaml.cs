using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WindowsBootscreenSimulatorPlus
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        public string[] Simulators
        {
            get { return (string[])GetValue(SimulatorProperty); }
            set { SetValue(SimulatorProperty, value); }
        }

        public static readonly DependencyProperty SimulatorProperty =
            DependencyProperty.Register("Simulators", typeof(string[]), typeof(MainWindow), new PropertyMetadata(Array.Empty<string>()));

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Simulators = ["Windows 98", "Windows ME", "Windows 2000", "Windows XP"];
        }

        public void Button_Click(object sender, RoutedEventArgs e)
        {
            switch (Mw1.OsList.SelectedItem.ToString())
            {
                case "Windows XP":
                    WinXP wxp = new()
                    {
                        WindowStyle = (Mw1.FullscreenBox.IsChecked ?? false) ? WindowStyle.None : WindowStyle.SingleBorderWindow,
                        WindowState = (Mw1.FullscreenBox.IsChecked ?? false) ? WindowState.Maximized : WindowState.Normal
                    };
                    wxp.VBox.Stretch = (Mw1.StretchBox.IsChecked ?? false) ? Stretch.Fill : Stretch.Uniform;
                    wxp.Show();
                    break;
                case "Windows 98":
                    Win98 w98 = new()
                    {
                        WindowStyle = (Mw1.FullscreenBox.IsChecked ?? false) ? WindowStyle.None : WindowStyle.SingleBorderWindow,
                        WindowState = (Mw1.FullscreenBox.IsChecked ?? false) ? WindowState.Maximized : WindowState.Normal
                    };
                    w98.VBox.Stretch = (Mw1.StretchBox.IsChecked ?? false) ? Stretch.Fill : Stretch.Uniform;
                    w98.Show();
                    break;
                case "Windows ME":
                    w98 = new Win98
                    {
                        WindowStyle = (Mw1.FullscreenBox.IsChecked ?? false) ? WindowStyle.None : WindowStyle.SingleBorderWindow,
                        WindowState = (Mw1.FullscreenBox.IsChecked ?? false) ? WindowState.Maximized : WindowState.Normal
                    };
                    w98.LogoImage.Source = GetImageResource(Properties.Resources.Windows_ME_4_90_3000_Boot);
                    w98.VBox.Stretch = (Mw1.StretchBox.IsChecked ?? false) ? Stretch.Fill : Stretch.Uniform;
                    w98.Show();
                    break;
                case "Windows 2000":
                    Win2k w2k = new()
                    {
                        WindowStyle = (Mw1.FullscreenBox.IsChecked ?? false) ? WindowStyle.None : WindowStyle.SingleBorderWindow,
                        WindowState = (Mw1.FullscreenBox.IsChecked ?? false) ? WindowState.Maximized : WindowState.Normal
                    };
                    w2k.VBox.Stretch = (Mw1.StretchBox.IsChecked ?? false) ? Stretch.Fill : Stretch.Uniform;
                    if (Mw1.ServerBox.IsChecked ?? false)
                    {
                        w2k.LogoImage.Source = GetImageResource(Properties.Resources.Windows_2000_Server);
                        w2k.SlitA.Source = GetImageResource(Properties.Resources.slit2kserver);
                        w2k.SlitB.Source = GetImageResource(Properties.Resources.slit2kserver);
                        w2k.progressColor = Color.FromArgb(0xFF, 0x30, 0x30, 0x98);
                    }
                    Random r = new();
                    for (int i = 0; i < 19; i++)
                    {
                        w2k.increments[i] = r.Next(0, 5000);
                    }
                    w2k.Show();
                    break;
                default:
                    MessageBox.Show("Not implemented!", "Simulate function", MessageBoxButton.OK, MessageBoxImage.Hand);
                    break;
            }
        }

        // Loads an image resource from bytes and returns a bitmap image
        private static BitmapImage GetImageResource(byte[] data)
        {
            BitmapImage biImg = new();
            MemoryStream ms = new(data);
            biImg.BeginInit();
            biImg.StreamSource = ms;
            biImg.EndInit();
            return biImg;
        }

        private void TabSwitch(object sender, RoutedEventArgs e)
        {
            UIMain.Visibility = Visibility.Hidden;
            Settings.Visibility = Visibility.Hidden;
            About.Visibility = Visibility.Hidden;
            if (sender is MenuItem mi) {
                switch (mi.Header)
                {
                    case "Simulator":
                        UIMain.Visibility = Visibility.Visible;
                        break;
                    case "Settings":
                        Settings.Visibility = Visibility.Visible;
                        break;
                    case "About":
                        About.Visibility = Visibility.Visible;
                        break;
                }
            }
            ToggleArrow.IsChecked = !ToggleArrow.IsChecked;
        }

        public void OsSwitch()
        {
            switch (Mw1.OsList.SelectedItem)
            {
                case "Windows 2000":
                    Mw1.ServerBox.Visibility = Visibility.Visible;
                    break;
                default:
                    Mw1.ServerBox.Visibility = Visibility.Hidden;
                    break;
            }
        }
    }
}