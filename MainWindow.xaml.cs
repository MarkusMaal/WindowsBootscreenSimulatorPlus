using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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
            TemplateRegistry.InitDefaultTemplates();
            Simulators = TemplateRegistry.GetTemplates();
            OsSwitch();
        }

        public void Button_Click(object sender, RoutedEventArgs e)
        {
            TemplateRegistry.Simulate(Mw1.OsList.SelectedIndex);
        }

        public void OsSwitch()
        {
            Mw1.TemplateConfig.Children.Clear();
            if (Mw1.OsList.SelectedIndex < 0) return;
            var bsT = TemplateRegistry.Templates[Mw1.OsList.SelectedIndex];
            foreach (var b in bsT.GetAllBoolKeys())
            {
                var cb = new CheckBox()
                {
                    Content = b,
                    IsChecked = bsT.GetBoolValue(b),
                };
                cb.Unchecked += (sender, e) =>
                {
                    bsT.SetValue(b, false);
                };
                cb.Checked += (sender, e) =>
                {
                    bsT.SetValue(b, true);
                };
                cb.Margin = new Thickness(0, 0, 10, 0);
                Mw1.TemplateConfig.Children.Add(cb);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            App.CheckWindows();
        }
    }
}