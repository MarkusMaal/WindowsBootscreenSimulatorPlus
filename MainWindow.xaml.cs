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
            // set items of OsList here
            OsList.ItemsSource = new List<string> { "Windows 98", "Windows XP" };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch (OsList.SelectedItem.ToString()) {
                case "Windows 98":
                    Win98 w98 = new Win98();
                    w98.WindowStyle = (FullscreenBox.IsChecked ?? false) ? WindowStyle.None : WindowStyle.SingleBorderWindow;
                    w98.WindowState = (FullscreenBox.IsChecked ?? false) ? WindowState.Maximized : WindowState.Normal;
                    w98.VBox.Stretch = (StretchBox.IsChecked ?? false) ? Stretch.Fill : Stretch.Uniform;
                    w98.Show();
                    break;
                default:
                    MessageBox.Show("Not implemented!", "Simulate function", MessageBoxButton.OK, MessageBoxImage.Hand);
                    break;
            }
        }
    }
}