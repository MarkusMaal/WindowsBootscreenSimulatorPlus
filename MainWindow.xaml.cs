using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TaskDialogInterop;
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
            Mw1.TemplateStrings.Children.Clear();
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

            foreach (var s in bsT.GetAllStringKeys())
            {
                var sp = new Grid()
                {
                    Margin = new Thickness(10),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                };
                sp.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                sp.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                var lbl = new Label()
                {
                    Content = s,
                };
                lbl.SetValue(Grid.RowProperty, 0);
                sp.Children.Add(lbl);
                var txt = new TextBox()
                {
                    Text = bsT.GetStringValue(s),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Margin = new Thickness(5, 0, 5, 0)
                };
                txt.TextChanged += (sender, e) =>
                {
                    bsT.SetValue(s, txt.Text);
                };
                txt.SetValue(Grid.RowProperty, 1);
                sp.Children.Add(txt);
                Mw1.TemplateStrings.Children.Add(sp);
            }
            if (bsT.GetDataValue("Graphic").Length > 0)
            {
                var sp = new Grid()
                {
                    Margin = new Thickness(10),
                    HorizontalAlignment = HorizontalAlignment.Left,
                };
                sp.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                sp.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                var lbl = new Label()
                {
                    Content = "Graphic",
                };
                lbl.SetValue(Grid.RowProperty, 0);
                sp.Children.Add(lbl);
                BitmapImage bmp = new();
                var ms = new MemoryStream(bsT.GetDataValue("Graphic"));
                bmp.BeginInit();
                bmp.StreamSource = ms;
                bmp.EndInit();
                var img = new Image()
                {
                    Width = 300,
                    Margin = new Thickness(5, 5, 0, 0)
                };
                if (bmp.PixelHeight > 800)
                {
                    CroppedBitmap cbmp = new(bmp, new Int32Rect(0, bmp.PixelHeight - 200, Math.Min(bmp.PixelWidth, 200), bmp.PixelWidth));
                    img.Source = cbmp;
                    lbl.Content = $"Graphic (frame: {cbmp.PixelWidth}x{cbmp.PixelHeight}, full: {bmp.PixelWidth}x{bmp.PixelHeight})";
                } else
                {
                    img.Source = bmp;
                    lbl.Content = $"Graphic ({bmp.PixelWidth}x{bmp.PixelHeight})";
                }
                img.SetValue(Grid.RowProperty, 1);
                img.Unloaded += (_, _) =>
                {
                    ms.Close();
                };
                sp.Children.Add(img);
                var btn = new Button()
                {
                    Content = "Replace",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                btn.SetValue(Grid.RowProperty, 1);
                btn.Click += (_, _) =>
                {

                    OpenFileDialog ofd = new()
                    {
                        Filter = "Image files (*.png, *.bmp, *.jpeg)|*.png;*.bmp;*.jpeg;*.jpg;*.jpe",
                        Title = "Choose replacement graphic",
                        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
                    };
                    if (ofd.ShowDialog() == true)
                    {
                        var backup = bsT.GetDataValue("Graphic");
                        bsT.SetValue("Graphic", File.ReadAllBytes(ofd.FileName));
                        try
                        {
                            ms.Close();
                            BitmapImage bmp = new();
                            ms = new MemoryStream(bsT.GetDataValue("Graphic"));
                            bmp.BeginInit();
                            bmp.StreamSource = ms;
                            bmp.EndInit();
                            if (bmp.PixelHeight > 800)
                            {
                                CroppedBitmap cbmp = new(bmp, new Int32Rect(0, bmp.PixelHeight - 200, Math.Min(bmp.PixelWidth, 200), bmp.PixelWidth));
                                img.Source = cbmp;
                                lbl.Content = $"Graphic (frame: {cbmp.PixelWidth}x{cbmp.PixelHeight}, full: {bmp.PixelWidth}x{bmp.PixelHeight})";
                            }
                            else
                            {
                                img.Source = bmp;
                                lbl.Content = $"Graphic ({bmp.PixelWidth}x{bmp.PixelHeight})";
                            }
                        }
                        catch when (!Debugger.IsAttached)
                        {

                            TaskDialogOptions config = new()
                            {
                                Owner = this,
                                MainInstruction = "Graphic replacement",
                                Content = "Failed to replace graphic. Make sure that the imported image is within the expected dimensions and that it's not corrupted!",
                                CustomButtons = ["OK"],
                                MainIcon = TaskDialogIcon.Error,
                                Title = "Windows Bootscreen Simulator Plus"
                            };
                            TaskDialog.Show(config);
                            bsT.SetValue("Graphic", backup);
                        }
                    }
                };
                sp.Children.Add(btn);
                Mw1.TemplateStrings.Children.Add(sp);
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