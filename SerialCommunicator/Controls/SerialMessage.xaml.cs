using SerialCommunicator.ViewModels;
using System.Windows.Controls;

namespace SerialCommunicator.Controls
{
    /// <summary>
    /// Interaction logic for SerialMessage.xaml
    /// </summary>
    public partial class SerialMessage : UserControl
    {
        public SerialMessageItemViewModel MessageItem
        {
            get => this.DataContext as SerialMessageItemViewModel;
            set => this.DataContext = value;
        }

        public SerialMessage()
        {
            InitializeComponent();
        }
    }
}
