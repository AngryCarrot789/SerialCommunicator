using SerialCommunicator.Graph;
using SerialCommunicator.Graph.DataPoints;
using SerialCommunicator.Utilities;
using SerialCommunicator.ViewModels;
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
using System.Windows.Shapes;

namespace SerialCommunicator.Windows
{
    /// <summary>
    /// Interaction logic for GraphWindow.xaml
    /// </summary>
    public partial class GraphWindow : Window
    {
        public ICommand ShowWindowCommand { get; set; }

        public GraphViewModel GraphView { get; set; }
        public GraphWindow()
        {
            InitializeComponent();
            GraphView = new GraphViewModel();
            DataContext = GraphView;

            ShowWindowCommand = new Command(ShowWindow);

            this.Closing += GraphWindow_Closing;
        }

        private void GraphWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            GraphView.StopPlotting();
            this.Hide();
        }

        public void ShowWindow()
        {
            Show();
            GraphView.StartPlotting();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            GraphView.Controller.RequestVirtualRangeChange();
        }

        private void Slider_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            GraphView.Controller.RequestVirtualRangeChange();
        }
    }
}
