using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using TaskDialogInterop;

namespace WindowsBootscreenSimulatorPlus.Fragments
{
    /// <summary>
    /// Interaction logic for TemplatesView.xaml
    /// </summary>
    public partial class TemplatesView : UserControl
    {
        public TemplatesView()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public string[] BaseTemplates => [.. new TemplateRegistry().Templates.Select(s => s.GetStringValue("FriendlyName"))];
        public string[] Systems => Enum.GetNames(typeof(BootscreenTemplate.Simulator));

        public ObservableCollection<BootscreenTemplate> Templates
        {
            get { return (ObservableCollection<BootscreenTemplate>)GetValue(TemplatesProperty); }
            set { SetValue(TemplatesProperty, value); }
        }

        public static readonly DependencyProperty TemplatesProperty =
            DependencyProperty.Register("Templates", typeof(ObservableCollection<BootscreenTemplate>), typeof(TemplatesView), new PropertyMetadata(new ObservableCollection<BootscreenTemplate>()));

        private void NewPresetComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AddPresetButton.IsEnabled = NewPresetComboBox.SelectedIndex != -1;
            if (!AddPresetButton.IsEnabled) return;
            List<string> systems = [.. Systems];
            var baseSimulator = new TemplateRegistry().Templates.First(s => s.GetStringValue("FriendlyName").Equals(NewPresetComboBox.SelectedItem));
            var simulatorType = baseSimulator.GetSimulatorType().ToString();
            NewPresetOsComboBox.SelectedIndex = systems.FindIndex(p => p == simulatorType);
            NewPresetName.Text = baseSimulator.GetStringValue("FriendlyName");
        }

        private void NewPresetName_TextChanged(object sender, TextChangedEventArgs e)
        {
            AddPresetButton.IsEnabled = NewPresetName.Text != null && NewPresetName.Text.Length > 0 && NewPresetComboBox.SelectedIndex != -1;
        }

        private void AddPresetButton_Click(object sender, RoutedEventArgs e)
        {
            var newPreset = new TemplateRegistry().Templates[NewPresetComboBox.SelectedIndex];
            newPreset.SetSimulatorType((BootscreenTemplate.Simulator)NewPresetOsComboBox.SelectedIndex);
            newPreset.SetValue("FriendlyName", NewPresetName.Text);
            ((App)Application.Current).TemplateRegistry.Templates.Add(newPreset);
            NewPresetName.Text = "";
            NewPresetOsComboBox.SelectedIndex = -1;
            NewPresetComboBox.SelectedIndex = -1;
            Templates = new ObservableCollection<BootscreenTemplate>(((App)Application.Current).TemplateRegistry.Templates);
            if (Window.GetWindow(this) is MainWindow mw) mw.Refresh();
        }

        private void Remove_Click(object sender, RoutedEventArgs? e)
        {
            if (TemplatesListView.SelectedItems.Count == 0) return;
            ((App)Application.Current).TemplateRegistry.Templates.RemoveAt(TemplatesListView.SelectedIndex);
            Templates = new ObservableCollection<BootscreenTemplate>(((App)Application.Current).TemplateRegistry.Templates);

            if (Window.GetWindow(this) is MainWindow mw) mw.Refresh();
        }

        private void RestoreDefaultsButton_Click(object sender, RoutedEventArgs e)
        {
            ((App)Application.Current).TemplateRegistry.InitDefaultTemplates();
            Templates = new ObservableCollection<BootscreenTemplate>(((App)Application.Current).TemplateRegistry.Templates);
            if (Window.GetWindow(this) is MainWindow mw) mw.Refresh();
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new()
            {
                Filter = "Javascript Object Notation (*.json)|*.json",
                Title = "Specify output filename",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
            };
            var w = Window.GetWindow(this);
            if (!(sfd.ShowDialog(w) ?? false)) return;
            try
            {
                File.WriteAllText(sfd.FileName, ((App)Application.Current).TemplateRegistry.Serialize());
                TaskDialogOptions config = new()
                {
                    Owner = w,
                    MainInstruction = "Export presets",
                    Content = "The file was saved successfuly",
                    CustomButtons = ["OK"],
                    MainIcon = TaskDialogIcon.Information
                };

                TaskDialog.Show(config);
            } catch (Exception ex)
            {
                ShowError(w, ex);
            }
        }

        private void ShowError(Window w, Exception ex)
        {
            TaskDialogOptions config = new()
            {
                Owner = w,
                MainInstruction = "An error ocurred!",
                Content = ex.Message,
                FooterText = "Source: " + ex.Source,
                CustomButtons = ["OK"],
                MainIcon = TaskDialogIcon.Error
            };
            TaskDialog.Show(config);
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new()
            {
                Filter = "Javascript Object Notation (*.json)|*.json",
                Title = "Specify output filename",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
            };
            var w = Window.GetWindow(this);
            var dr = ofd.ShowDialog(w);
            if (!(dr ?? false)) return;
            try
            {
                var fileHandle = File.OpenText(ofd.FileName);
                var content = fileHandle.ReadToEnd();
                fileHandle.Close();
                ((App)Application.Current).TemplateRegistry.Deserialize(content);
                Templates = new ObservableCollection<BootscreenTemplate>(((App)Application.Current).TemplateRegistry.Templates);
                if (Window.GetWindow(this) is MainWindow mw) mw.Refresh();
                TaskDialogOptions config = new()
                {
                    Owner = w,
                    MainInstruction = "Import presets",
                    Content = "The file was loaded successfuly",
                    CustomButtons = ["OK"],
                    MainIcon = TaskDialogIcon.Information
                };
                TaskDialog.Show(config);
            } catch (Exception ex)
            {
                ShowError(w, ex);
            }
        }

        private void TemplatesListView_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Delete && TemplatesListView.SelectedItems.Count > 0)
                Remove_Click(sender, null);
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ((App)Application.Current).TemplateRegistry.Templates.Clear();
            Templates = new ObservableCollection<BootscreenTemplate>(((App)Application.Current).TemplateRegistry.Templates);
            if (Window.GetWindow(this) is MainWindow mw) mw.Refresh();
        }
    }
}
