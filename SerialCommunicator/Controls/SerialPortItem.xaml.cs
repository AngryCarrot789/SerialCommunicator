using SerialCommunicator.Utilities;
using SerialCommunicator.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SerialCommunicator.Controls
{
    /// <summary>
    /// Interaction logic for SerialItem.xaml
    /// </summary>
    public partial class SerialPortItem : UserControl
    {
        public SerialPortItemViewModel ItemView
        {
            get => this.DataContext as SerialPortItemViewModel;
            set => this.DataContext = value;
        }

        public SerialPortItem()
        {
            InitializeComponent();
            Loaded += SerialPortItem_Loaded;
        }

        private void SerialPortItem_Loaded(object sender, RoutedEventArgs e)
        {
            AnimationLib.MoveToTargetX(this, -200, 0, 0.18);
            AnimationLib.OpacityControl(this, 0, 1, 0.1);
        }
    }
}
