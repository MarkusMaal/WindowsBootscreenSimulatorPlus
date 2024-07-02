using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        MainWindow? parent;

        public MainView()
        {
            InitializeComponent();
        }
        public string[] Simulators
        {
            get { return (string[])GetValue(SimulatorProperty); }
            set { SetValue(SimulatorProperty, value); }
        }

        public static readonly DependencyProperty SimulatorProperty =
            DependencyProperty.Register("Simulators", typeof(string[]), typeof(MainView), new PropertyMetadata(Array.Empty<string>()));

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            parent?.Button_Click(sender, e);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            parent = ((MainWindow)Window.GetWindow(this.Parent));
            RootGrid.Background = parent.Background;
        }
    }
}
