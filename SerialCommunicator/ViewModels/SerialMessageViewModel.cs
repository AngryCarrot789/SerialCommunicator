using SerialCommunicator.Utilities;
using System;

namespace SerialCommunicator.ViewModels
{
    public class SerialMessageViewModel : BaseViewModel
    {
        private string _countIndex;
        private string _rxOrTx;
        private string _message;
        private DateTime _time;

        public string CountIndex
        {
            get => _countIndex;
            set => RaisePropertyChanged(ref _countIndex, value);
        }
        public string RXorTX
        {
            get => _rxOrTx;
            set => RaisePropertyChanged(ref _rxOrTx, value);
        }
        public string Message
        {
            get => _message;
            set => RaisePropertyChanged(ref _message, value);
        }
        public DateTime Time
        {
            get => _time;
            set => RaisePropertyChanged(ref _time, value);
        }
    }
}
