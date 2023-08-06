using Microsoft.Win32;
using SerialCommunicator.Controls;
using SerialCommunicator.SerialFileLogger;
using SerialCommunicator.Utilities;
using SerialCommunicator.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace SerialCommunicator.ViewModels
{
    public class SerialViewModel : BaseViewModel
    {
        #region Commands
        public ICommand ConnectDisconnedCommand { get; set; }
        public ICommand ClearReceivedMessagesCommand { get; set; }
        public ICommand ClearSentMessagesCommand { get; set; }
        public ICommand ClearBuffersCommand { get; set; }
        public ICommand SendMessageCommand { get; set; }
        public ICommand ResetSerialPortCommand { get; set; }
        public ICommand WriteMessagesToFileCommand { get; set; }
        #endregion

        #region Private Fields

        private string _serialItemName;
        private bool _isConnected;

        private double _sendTimeout;
        private double _receiveTimeout;
        private double _bufferSize;
        private double _maximumAllowedMessages;

        private string _comName;
        private string _activeComName;
        private string _baudRate;
        private string _dataBits;
        private string _stopBits;
        private string _parity;
        private string _handShake;
        private bool _dataTerminalReady;
        private string _cnctDcnctBtnContent;

        private string _toBeSentText;
        private bool _waitingStatus;

        #endregion

        #region Public Fields

        public string SerialItemName
        {
            get => _serialItemName;
            set
            {
                RaisePropertyChanged(ref _serialItemName, value);
                if (GraphWindow != null && !string.IsNullOrEmpty(value))
                {
                    GraphWindow.Title = value;
                }
            }
        }
        public bool IsConnected
        {
            get => _isConnected;
            set => RaisePropertyChanged(ref _isConnected, value);
        }
        public double SendTimeout
        {
            get => _sendTimeout;
            set { RaisePropertyChanged(ref _sendTimeout, value); UpdateSendTimeout(int.Parse(Math.Round(value, 0).ToString())); }
        }
        public double ReceiveTimeout
        {
            get => _receiveTimeout;
            set { RaisePropertyChanged(ref _receiveTimeout, value); UpdateReceiveTimeout(int.Parse(Math.Round(value, 0).ToString())); }
        }
        public double BufferSize
        {
            get => _bufferSize;
            set { RaisePropertyChanged(ref _bufferSize, value); UpdateBufferSize(int.Parse(Math.Round(value, 0).ToString())); }
        }
        public double MaximumAllowedMessages
        {
            get => _maximumAllowedMessages;
            set => RaisePropertyChanged(ref _maximumAllowedMessages, value);
        }
        public string COMName
        {
            get => _comName;
            set { RaisePropertyChanged(ref _comName, value); UpdateCOMName(value); }
        }
        public string ActiveCOMName
        {
            get => _activeComName;
            set => RaisePropertyChanged(ref _activeComName, value);
        }
        public string BaudRate
        {
            get => _baudRate;
            set { RaisePropertyChanged(ref _baudRate, value); UpdateBaudRate(int.Parse(value)); }
        }
        public string DataBits
        {
            get => _dataBits;
            set { RaisePropertyChanged(ref _dataBits, value); UpdateDataBits(int.Parse(value)); }
        }
        public string StopBits
        {
            get => _stopBits;
            set { RaisePropertyChanged(ref _stopBits, value); UpdateStopBits(value); }
        }
        public string Parity
        {
            get => _parity;
            set { RaisePropertyChanged(ref _parity, value); UpdateParity(value); }
        }
        public string HandShake
        {
            get => _handShake;
            set { RaisePropertyChanged(ref _handShake, value); UpdateHandShake(value); }
        }
        public bool DataTerminalReady
        {
            get => _dataTerminalReady;
            set { RaisePropertyChanged(ref _dataTerminalReady, value); UpdateDTR(value); }
        }
        public string ConnectDisconnectButtonContent
        {
            get => _cnctDcnctBtnContent;
            set => RaisePropertyChanged(ref _cnctDcnctBtnContent, value);
        }
        public ObservableCollection<SerialMessageItemViewModel> ReceivedMessages { get; set; }
        public ObservableCollection<SerialMessageItemViewModel> SentMessages { get; set; }
        public string ToBeSentText
        {
            get => _toBeSentText;
            set => RaisePropertyChanged(ref _toBeSentText, value);
        }
        public bool WaitingStatus
        {
            get => _waitingStatus;
            set => RaisePropertyChanged(ref _waitingStatus, value);
        }

        public string ReceivedDataBuffer { get; set; }

        #endregion

        public TransceiveSettingsViewModel Settings { get; set; }
        public SerialPort SerialPort { get; set; }
        public GraphWindow GraphWindow { get; set; }

        #region Constructor and Destructor

        public SerialViewModel()
        {
            ReceivedMessages             = new ObservableCollection<SerialMessageItemViewModel>();
            SentMessages                 = new ObservableCollection<SerialMessageItemViewModel>();
            ConnectDisconnedCommand      = new Command(AutoConnectDisconnect);
            ClearSentMessagesCommand     = new Command(ClearSentMessages);
            SendMessageCommand           = new Command(SendMessage);
            ClearReceivedMessagesCommand = new Command(ClearReceivedMessages);
            ClearBuffersCommand          = new Command(ClearBuffers);
            ResetSerialPortCommand       = new Command(RestartSerialPort);
            WriteMessagesToFileCommand   = new Command(WriteMessagesToFile);
            Settings                     = new TransceiveSettingsViewModel();

            RestartSerialPort();
            SetDefaultValues();
            UpdateSerialValues();
        }

        //executes the constructor above first, then this one.
        public SerialViewModel(string name) : this()
        {
            SerialItemName = name;
            GraphWindow = new GraphWindow
            {
                Title = SerialItemName
            };
        }

        ~SerialViewModel()
        {
            ShutdownEverything();
        }

        #endregion

        #region Graphs

        public void PlotMessageToGraph(string message)
        {
            int sizeCounter = 0;
            if (IsDigitsOnly(message) && int.TryParse(message, out sizeCounter))
            {
                Application.Current.Dispatcher.Invoke(() => { GraphWindow.GraphView.PlotGraph(Convert.ToDouble(sizeCounter)); });
            }
            else
            {
                foreach (char letter in message)
                {
                    sizeCounter += CharAlphabeticalPositions.CharToAlphabeticalPosition(letter);
                }
                Application.Current.Dispatcher.Invoke(() => { GraphWindow.GraphView.PlotGraph(Convert.ToDouble(sizeCounter)); });
            }
        }

        private bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region File logging/writing of messages

        public void WriteMessagesToFile()
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                FileName = "serialLog1",
                Title = "Select a location to save every sent/received message to (as a txt)",
                Filter = "Text File|*.txt"
            };
            if (sfd.ShowDialog() == true)
            {
                List<SerialMessageViewModel> messages = new List<SerialMessageViewModel>();
                foreach (SerialMessageItemViewModel item in ReceivedMessages)
                {
                    if (item.Message != null)
                    {
                        messages.Add(item.Message);
                    }
                }

                foreach (SerialMessageItemViewModel item in SentMessages)
                {
                    if (item.Message != null)
                    {
                        messages.Add(item.Message);
                    }
                }

                MessageWriter.WriteToFile(sfd.FileName, messages);
            }
        }

        #endregion

        #region SerialPort connect/disconnect methods

        public void AutoConnectDisconnect()
        {
            if (IsConnected)
            {
                Disconnect();
            }
            else
            {
                Connect();
            }
        }

        public void Connect()
        {
            if (ActiveCOMName == "COM1")
            {
                AlertMessage("Cannot use COM1 because it crashes the program");
                return;
            }
            try
            {
                UpdateSerialValues();
                SetWaitingStatus(true);
                SerialPort.Open();
                IsConnected = true;
                SetWaitingStatus(false);
                AlertMessage(
                    $"Successfully connected to {ActiveCOMName}. " +
                    $"Baud: {SerialPort.BaudRate} | " +
                    $"Encoding: {SerialPort.Encoding.EncodingName}");
                ConnectDisconnectButtonContent = "Disconnect";
                WriteExtraBufferData();
            }
            catch (Exception e)
            {
                ErrorMessage($"Failed to connect to port: {e.Message}");
            }
            finally
            {
                SetWaitingStatus(false);
            }
        }

        public void Disconnect()
        {
            try
            {
                SetWaitingStatus(true);
                Task.Run(() =>
                {
                    SerialPort.Close();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        SetWaitingStatus(false);
                        AlertMessage($"Successfully disconnected from {ActiveCOMName}.");
                        IsConnected = false;
                        ConnectDisconnectButtonContent = "Connect";
                    });
                });
            }
            catch (Exception e)
            {
                ErrorMessage($"Failed to disconnect from port: {e.Message}");
            }
            finally
            {
                SetWaitingStatus(false);
            }
        }

        public void WriteExtraBufferData()
        {
            if (SerialPort != null && SerialPort.IsOpen && SerialPort.BytesToRead > 0)
            {
                BufferMessage(SerialPort.ReadExisting());
            }
        }

        public void RestartSerialPort()
        {
            if (SerialPort != null)
            {
                SerialPort.Dispose();
            }

            SerialPort = new SerialPort
            {
                NewLine = GlobalSettings.DEFAULT_SERIAL_END
            };
            SerialPort.DataReceived += SerialPort_DataReceived;
            SerialPort.ErrorReceived += SerialPort_ErrorReceived;
        }

        #endregion

        #region SerialPort Events

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Task.Run(() =>
            {
                if (SerialPort.IsOpen)
                {
                    try
                    {
                        if (Settings.ReceiveWithNewLine)
                        {
                            string dataReceived = ReadLine();
                            if (!string.IsNullOrEmpty(dataReceived))
                            {
                                MessageReceived(dataReceived);
                                PlotMessageToGraph(dataReceived);
                            }
                        }
                        else if (Settings.ReceiveWithCustomTag)
                        {
                            string dataReceived = SerialPort.ReadTo(Settings.CustomTag);
                            MessageReceived(dataReceived);
                            PlotMessageToGraph(dataReceived);
                        }
                        else if (Settings.ReceiveWithNothingElse)
                        {
                            string dataReceived = ReadRawText();
                            MessageReceived(dataReceived);
                            PlotMessageToGraph(dataReceived);
                        }
                    }
                    catch (TimeoutException t)
                    {
                        ErrorMessage($"Received message has timed out: {t.Message}");
                    }
                    catch (Exception ee)
                    {
                        ErrorMessage($"Error trying to parse received data: {ee.Message}");
                    }
                }
                else
                {
                    ErrorMessage($"SerialPort is not connected.");
                }
            });
        }


        private void SerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            switch (e.EventType)
            {
                case SerialError.Frame:
                    ErrorMessage(
                        "Framing error detected: attempted to read from wrong starting point of data. " +
                        "solution: [RESTART SERIALPORT]");
                    RestartSerialPort(); break;
                case SerialError.Overrun:
                    ErrorMessage(
                        "Overrun error detected: data arrived before previous data could be processed. " +
                        "solution: [RESTART SERIALPORT.]");
                    RestartSerialPort(); break;
                case SerialError.RXOver:
                    ErrorMessage(
                        "RXOver error detected: the receive buffer is full, or data was received after end-of-file marker. " +
                        "solution: [CLEAR BUFFERS]"); break;
                case SerialError.RXParity:
                    ErrorMessage(
                        "RXParity error detected: parity might not have been applied, or data was corrupted. " +
                        "solution: [none]"); break;
                case SerialError.TXFull:
                    ErrorMessage(
                        "TXFull error detected: attempted to transmit data when output buffer was full." +
                        " solution: [CLEAR BUFFERS]"); break;
            }
        }

        #endregion

        #region GUI Messaging Methods

        //all thread safe
        public void BufferMessage(string message)
        {
            AddAutomaticMessage("Buffer", message);
        }

        public void AlertMessage(string message)
        {
            AddAutomaticMessage("Alert", message);
        }

        public void ErrorMessage(string message)
        {
            AddAutomaticMessage("Error", message);
        }

        public void MessageReceived(string message)
        {
            AddAutomaticMessage("RX", message);
        }

        public void MessageSent(string message)
        {
            AddAutomaticMessage("TX", message);
        }

        //thread safe
        public void AddAutomaticMessage(string transmissionDirection, string message)
        {
            AddAutomaticMessage(new SerialMessageViewModel() { Message = message, RXorTX = transmissionDirection, Time = DateTime.Now });
        }
        //thread safe
        public void AddAutomaticMessage(SerialMessageViewModel message)
        {
            switch (message.RXorTX)
            {
                case "Buffer": AddSentMessage(message); break;
                case "Alert": AddAlertMessage(message); break;
                case "Error": AddErrorMessage(message); break;
                case "RX": AddReceivedMessage(message); break;
                case "TX": AddSentMessage(message); break;
                default: AddReceivedMessage(message); break;
            }
        }
        //thread safe
        public void AddSentMessage(SerialMessageViewModel message)
        {
            if (Application.Current != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (SentMessages.Count >= MaximumAllowedMessages)
                    {
                        SentMessages.Clear();
                    }
                    message.CountIndex = SentMessages.Count.ToString();
                    InsertSent(0, new SerialMessageItemViewModel(message, GlobalSettings.NORM_BRUSH));
                });
            }
        }
        //thread safe
        public void AddReceivedMessage(SerialMessageViewModel message)
        {
            if (Application.Current != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (ReceivedMessages.Count >= MaximumAllowedMessages)
                    {
                        ReceivedMessages.Clear();
                    }

                    message.CountIndex = SentMessages.Count.ToString();
                    InsertReceived(0, new SerialMessageItemViewModel(message, GlobalSettings.NORM_BRUSH));
                });
            }
        }
        //thread safe
        public void AddErrorMessage(SerialMessageViewModel message)
        {
            if (Application.Current != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (ReceivedMessages.Count >= MaximumAllowedMessages)
                    {
                        ReceivedMessages.Clear();
                    }

                    message.CountIndex = SentMessages.Count.ToString();
                    InsertReceived(0, new SerialMessageItemViewModel(message, GlobalSettings.ERR_BRUSH));
                });
            }
        }
        //thread safe
        public void AddAlertMessage(SerialMessageViewModel message)
        {
            if (Application.Current != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (ReceivedMessages.Count >= MaximumAllowedMessages)
                    {
                        ReceivedMessages.Clear();
                    }

                    message.CountIndex = SentMessages.Count.ToString();
                    InsertReceived(0, new SerialMessageItemViewModel(message, GlobalSettings.ALERT_BRUSH));
                });
            }
        }

        public void InsertReceived(int index, SerialMessageItemViewModel msg)
        {
            ReceivedMessages.Insert(index, msg);
        }
        public void InsertSent(int index, SerialMessageItemViewModel msg)
        {
            SentMessages.Insert(index, msg);
        }

        #endregion

        #region SerialPort Transmit Methods

        public void SendMessage()
        {
            if (!string.IsNullOrEmpty(ToBeSentText))
            {
                SendMessage(ToBeSentText);
            }
        }

        public void SendMessage(string message)
        {
            if (SerialPort.IsOpen)
            {
                //async due to timeouts halting main thread
                Task.Run(() =>
                {
                    try
                    {
                        SetWaitingStatus(true);

                        if (Settings.SendWithNewLine)
                            SerialPort.WriteLine(message);

                        else if (Settings.SendWithCustomTag)
                        {
                            //idc this is messy but it might work
                            string originalTag = SerialPort.NewLine;
                            SerialPort.NewLine = Settings.CustomTag;
                            SerialPort.WriteLine(message);
                            SerialPort.NewLine = originalTag;
                        }
                        else if (Settings.SendWithNothingElse)
                            SerialPort.Write(message);

                        MessageSent(message);

                        if (Settings.ClearTBSTAfterTransmission)
                            ToBeSentText = "";

                        SetWaitingStatus(false);
                    }
                    catch (TimeoutException t)
                    {
                        ErrorMessage($"Send Message Timeout: {t.Message}");
                    }
                    catch (Exception e)
                    {
                        ErrorMessage($"Error sending message: {e.Message}");
                    }
                    finally
                    {
                        SetWaitingStatus(false);
                    }
                });
            }
            else
            {
                ErrorMessage($"SerialPort is not connected.");
            }
        }

        public string ReadRawText()
        {
            int nBytesToRead = SerialPort.BytesToRead;
            byte[] buffer = new byte[nBytesToRead];
            if (SerialPort.Read(buffer, 0, nBytesToRead) > nBytesToRead)
            {
                ErrorMessage("More data was read than what was supposed to be read.");
            }

            return SerialPort.Encoding.GetString(buffer);
        }

        public string ReadLine()
        {
            int nBytesToRead = SerialPort.BytesToRead;
            byte[] buffer = new byte[nBytesToRead];

            if (SerialPort.Read(buffer, 0, nBytesToRead) > nBytesToRead)
                ErrorMessage("More data was read than what was supposed to be read.");

            string receivedData = (SerialPort.Encoding).GetString(buffer);
            ReceivedDataBuffer += receivedData;

            if (ReceivedDataBuffer.EndsWith("\n"))
            {
                string received = ReceivedDataBuffer.Split('\n')[0];
                ReceivedDataBuffer = "";
                return !string.IsNullOrEmpty(received) ? received : null;
            }

            return null;
        }

        public string ReadWithCustomTag()
        {
            int nBytesToRead = SerialPort.BytesToRead;
            byte[] buffer = new byte[nBytesToRead];

            if (SerialPort.Read(buffer, 0, nBytesToRead) > nBytesToRead)
            {
                ErrorMessage("More data was read than what was supposed to be read.");
            }

            string receivedData = (SerialPort.Encoding).GetString(buffer);
            ReceivedDataBuffer += receivedData;

            if (ReceivedDataBuffer.EndsWith(Settings.CustomTag))
            {
                string received = ReceivedDataBuffer;
                ReceivedDataBuffer = "";
                return received;
            }

            return "";
        }

        #endregion

        #region Buffer/Message clearing

        public void ClearReceivedMessages()
        {
            ReceivedMessages.Clear();
        }

        public void ClearSentMessages()
        {
            SentMessages.Clear();
        }

        public void ClearBuffers()
        {
            if (SerialPort.IsOpen)
            {
                SerialPort.DiscardInBuffer();
                SerialPort.DiscardOutBuffer();
            }
            else
            {
                ErrorMessage("Conenct to port to clear buffers.");
            }
        }

        #endregion

        #region Updating field and serialport values
        //Thread safe
        public void SetWaitingStatus(bool waiting)
        {
            Application.Current.Dispatcher.Invoke(() => WaitingStatus = waiting);
        }

        public void SetDefaultValues()
        {
            // 0.5 seconds, is relatively reasonable.
            SendTimeout = 500;
            ReceiveTimeout = 500;
            BufferSize = 4096;
            // too many could slow down the program
            MaximumAllowedMessages = 150;
            COMName = "COM1";
            ActiveCOMName = COMName;
            BaudRate = "9600";
            DataBits = "8";
            StopBits = "One";
            Parity = "None";
            HandShake = "None";
            ConnectDisconnectButtonContent = "Connect";
        }

        public void UpdateSerialValues()
        {
            try
            {
                UpdateSendTimeout(int.Parse(Math.Round(SendTimeout, 0).ToString()));
                UpdateReceiveTimeout(int.Parse(Math.Round(ReceiveTimeout, 0).ToString()));
                UpdateBufferSize(int.Parse(Math.Round(BufferSize, 0).ToString()));
                UpdateCOMName(COMName);
                UpdateBaudRate(int.Parse(Math.Round(double.Parse(BaudRate), 0).ToString()));
                UpdateDataBits(int.Parse(Math.Round(double.Parse(DataBits), 0).ToString()));
                UpdateStopBits(StopBits);
                UpdateParity(Parity);
                UpdateHandShake(HandShake);
                UpdateDTR(DataTerminalReady);
                UpdateEncoding(Settings.SerialEncoding);
            }
            catch (IOException io)
            {
                ErrorMessage($"IOException while setting serial values: {io.Message}");
            }
            catch (Exception e)
            {
                ErrorMessage($"General error while setting serial values: {e.Message}");
            }
        }
        private void UpdateSendTimeout(int newVal)
        {
            if (!SerialPort.IsOpen && newVal > 0)
            {
                SerialPort.WriteTimeout = newVal;
            }
        }
        private void UpdateReceiveTimeout(int newVal)
        {
            if (!SerialPort.IsOpen && newVal > 0)
            {
                SerialPort.ReadTimeout = newVal;
            }
        }
        private void UpdateBufferSize(int newVal)
        {
            if (!SerialPort.IsOpen && newVal > 0)
            {
                SerialPort.ReadBufferSize = newVal;
                SerialPort.WriteBufferSize = newVal;
            }
        }
        private void UpdateCOMName(string newVal)
        {
            if (!SerialPort.IsOpen)
            {
                if (!string.IsNullOrEmpty(newVal))
                {
                    SerialPort.PortName = newVal;
                    ActiveCOMName = newVal;
                }
            }
        }
        private void UpdateBaudRate(int newVal)
        {
            if (!SerialPort.IsOpen && newVal > 0)
            {
                SerialPort.BaudRate = newVal;
            }
        }
        private void UpdateDataBits(int newVal)
        {
            if (!SerialPort.IsOpen && newVal > 0)
            {
                SerialPort.DataBits = newVal;
            }
        }
        private void UpdateStopBits(string newVal)
        {
            if (!SerialPort.IsOpen)
            {
                SerialPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), newVal);
            }
        }
        private void UpdateParity(string newVal)
        {
            if (!SerialPort.IsOpen)
            {
                SerialPort.Parity = (Parity)Enum.Parse(typeof(Parity), newVal);
            }
        }
        private void UpdateHandShake(string newVal)
        {
            if (!SerialPort.IsOpen)
            {
                SerialPort.Handshake = (Handshake)Enum.Parse(typeof(Handshake), newVal);
            }
        }
        private void UpdateDTR(bool newVal)
        {
            SerialPort.DtrEnable = newVal;
        }
        private void UpdateEncoding(Encoding newEncoding)
        {
            if (newEncoding != null && SerialPort != null)
            {
                SerialPort.Encoding = newEncoding;
            }
        }

        #endregion

        /// <summary>
        /// Use when deleting SerialItems. if you dont use this, an open SerialPort would still remain open.
        /// </summary>
        public void ShutdownEverything()
        {
            if (SerialPort != null)
            {
                SerialPort.Dispose();
            }
        }
    }
}
