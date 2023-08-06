using SerialCommunicator.Utilities;
using System;
using System.Windows.Input;

namespace SerialCommunicator.ViewModels
{
    public class SerialPortItemViewModel : BaseViewModel
    {
        private SerialViewModel _serialView = new SerialViewModel();
        public SerialViewModel SerialView
        {
            get => _serialView;
            set => RaisePropertyChanged(ref _serialView, value);
        }

        public ICommand CloseItem { get; }

        public Action<SerialPortItemViewModel> Close { get; set; }

        public SerialPortItemViewModel(string name)
        {
            SerialView = new SerialViewModel(name);
            CloseItem = new Command(Remove);
        }

        public SerialPortItemViewModel()
        {
            SerialView = new SerialViewModel();
            CloseItem = new Command(Remove);
        }

        internal void Remove()
        {
            Close?.Invoke(this);
        }
    }
}
