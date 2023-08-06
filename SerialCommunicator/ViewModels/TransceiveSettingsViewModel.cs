using SerialCommunicator.Utilities;
using System.Collections.Generic;
using System.Text;

namespace SerialCommunicator.ViewModels
{
    public class TransceiveSettingsViewModel : BaseViewModel
    {
        private bool _sendWithNewLine;
        private bool _receiveWithNewLine;
        private string _customTag;
        private bool _sendWithCustomTag;
        private bool _receiveWithCustomTag;
        private bool _sendWithNothingElse;
        private bool _receiveWithNothingElse;
        private bool _clearTBSTAfterTransmission;
        private Encoding _serialEncoding;
        public bool SendWithNewLine
        {
            get => _sendWithNewLine;
            set => RaisePropertyChanged(ref _sendWithNewLine, value);
        }
        public bool ReceiveWithNewLine
        {
            get => _receiveWithNewLine;
            set => RaisePropertyChanged(ref _receiveWithNewLine, value);
        }

        public string CustomTag
        {
            get => _customTag;
            set => RaisePropertyChanged(ref _customTag, value);
        }
        public bool SendWithCustomTag
        {
            get => _sendWithCustomTag;
            set => RaisePropertyChanged(ref _sendWithCustomTag, value);
        }
        public bool ReceiveWithCustomTag
        {
            get => _receiveWithCustomTag;
            set => RaisePropertyChanged(ref _receiveWithCustomTag, value);
        }

        public bool SendWithNothingElse
        {
            get => _sendWithNothingElse;
            set => RaisePropertyChanged(ref _sendWithNothingElse, value);
        }
        public bool ReceiveWithNothingElse
        {
            get => _receiveWithNothingElse;
            set => RaisePropertyChanged(ref _receiveWithNothingElse, value);
        }

        public bool ClearTBSTAfterTransmission
        {
            get => _clearTBSTAfterTransmission;
            set => RaisePropertyChanged(ref _clearTBSTAfterTransmission, value);
        }

        public Encoding SerialEncoding
        {
            get => _serialEncoding;
            set => RaisePropertyChanged(ref _serialEncoding, value);
        }

        public List<Encoding> AllSerialEncodings { get; }

        public TransceiveSettingsViewModel()
        {
            SendWithNewLine = true;
            ReceiveWithNewLine = true;
            ClearTBSTAfterTransmission = true;
            CustomTag = @"\n";
            AllSerialEncodings = new List<Encoding>()
            {
                Encoding.ASCII,
                Encoding.Unicode,
                Encoding.UTF8,
                Encoding.UTF32
            };
        }
    }
}
