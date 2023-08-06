using SerialCommunicator.Utilities;
using System.Windows.Media;

namespace SerialCommunicator.ViewModels
{
    public class SerialMessageItemViewModel : BaseViewModel
    {
        private SerialMessageViewModel _message;
        public SerialMessageViewModel Message
        {
            get => _message;
            set => RaisePropertyChanged(ref _message, value);
        }

        private SolidColorBrush _background;
        public SolidColorBrush Background
        {
            get => _background;
            set => RaisePropertyChanged(ref _background, value);
        }

        public SerialMessageItemViewModel() { }
        public SerialMessageItemViewModel(SerialMessageViewModel message, SolidColorBrush background)
        {
            Message = message;
            Background = background;
        }
    }
}
