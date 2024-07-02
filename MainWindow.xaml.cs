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
            Simulators = ["Windows 98", "Windows ME", "Windows XP"];
        }

        public void Button_Click(object sender, RoutedEventArgs e)
        {
            switch (Mw1.OsList.SelectedItem.ToString())
            {
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
                    BitmapImage biImg = new();
                    MemoryStream ms = new(Properties.Resources.Windows_ME_4_90_3000_Boot);
                    biImg.BeginInit();
                    biImg.StreamSource = ms;
                    biImg.EndInit();
                    w98.LogoImage.Source = biImg;
                    w98.VBox.Stretch = (Mw1.StretchBox.IsChecked ?? false) ? Stretch.Fill : Stretch.Uniform;
                    w98.Show();
                    break;
                default:
                    MessageBox.Show("Not implemented!", "Simulate function", MessageBoxButton.OK, MessageBoxImage.Hand);
                    break;
            }
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
    }
}