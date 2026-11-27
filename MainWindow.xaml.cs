using System.IO;
using System.Windows;
using System.Windows.Controls;
using WindowsBootscreenSimulatorPlus.Fragments;

namespace WindowsBootscreenSimulatorPlus
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string? cfgFile = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WBSSP", "Config.json");
        private readonly string? cfgDir;

        public MainWindow()
        {
            cfgDir = new FileInfo(cfgFile).Directory?.FullName;
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
            var tr = ((App)Application.Current).TemplateRegistry;
            /*if (cfgFile != null)
            {
                if (!Directory.Exists(cfgDir) && cfgDir != null)
                {
                    Directory.CreateDirectory(cfgDir);
                }
                if (!File.Exists(cfgFile))
                {
                    tr.InitDefaultTemplates();
                    File.WriteAllText(cfgFile, tr.Serialize());
                }
                else
                {
                    tr.Deserialize(File.ReadAllText(cfgFile));
                }
            }*/
            if (FindName("TmpView") is TemplatesView tv)
            {
                tv.Templates = new System.Collections.ObjectModel.ObservableCollection<BootscreenTemplate>(tr.Templates);
            }
            tr.InitDefaultTemplates();
            Refresh();
        }

        public void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Mw1.OsList.SelectedIndex == -1) return;
            ((App)Application.Current).TemplateRegistry.Simulate(Mw1.OsList.SelectedIndex);
        }

        public void OsSwitch()
        {
            Mw1.TemplateConfig.Children.Clear();
            if (Mw1.OsList.SelectedIndex < 0) return;
            var bsT = ((App)Application.Current).TemplateRegistry.Templates[Mw1.OsList.SelectedIndex];
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
            if (cfgFile == null) return;
            //File.WriteAllText(cfgFile, ((App)Application.Current).TemplateRegistry.Serialize());
        }

        public void Refresh()
        {
            var tr = ((App)Application.Current).TemplateRegistry;
            Simulators = tr.GetTemplates();
            OsSwitch();
            if (Mw1.OsList.Items.Count > 0) 
                Mw1.OsList.SelectedIndex = 0;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            App.CheckWindows();
        }
    }
}