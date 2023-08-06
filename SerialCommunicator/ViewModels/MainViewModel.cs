using SerialCommunicator.Utilities;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Windows.Input;

namespace SerialCommunicator.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public ObservableCollection<SerialPortItemViewModel> SerialPortItems { get; set; }
        public ObservableCollection<string> AvaliableCOMPorts { get; set; }

        private SerialPortItemViewModel _selectedItem;
        public SerialPortItemViewModel SelectedItem
        {
            get => _selectedItem;
            set => RaisePropertyChanged(ref _selectedItem, value);
        }

        private int _selectedIndex;
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                RaisePropertyChanged(ref _selectedIndex, value);
            }
        }

        public HelpViewModel Help { get; set; }

        public ICommand NewSerialItemCommand { get; set; }
        public ICommand RemoveSerialItemCommand { get; set; }
        public ICommand RefreshCOMPortsCommand { get; set; }
        public ICommand ResetSerialViewCommand { get; set; }

        public MainViewModel()
        {
            SerialPortItems = new ObservableCollection<SerialPortItemViewModel>();
            NewSerialItemCommand = new Command(CreateSerialItem);
            RemoveSerialItemCommand = new Command(RemoveSelectedItem);
            AvaliableCOMPorts = new ObservableCollection<string>();
            RefreshCOMPortsCommand = new Command(RefreshCOMPorts);
            ResetSerialViewCommand = new Command(ResetSerialView);

            Help = new HelpViewModel();
            RefreshCOMPorts();
        }

        public void CreateSerialItem()
        {
            SerialPortItemViewModel item = new SerialPortItemViewModel($"Serial {SerialPortItems.Count}");
            item.Close = this.RemoveSerialPortItem;
            AddSerialPortItem(item);
        }

        public void RemoveSelectedItem()
        {
            if (SelectedItem != null)
                RemoveSerialPortItem(SelectedItem);
        }

        public void AddSerialPortItem(SerialPortItemViewModel item)
        {
            SerialPortItems.Add(item);
        }

        public void RemoveSerialPortItem(SerialPortItemViewModel item)
        {
            if (item.SerialView != null)
                item.SerialView.ShutdownEverything();
            SerialPortItems.Remove(item);
        }

        public void RefreshCOMPorts()
        {
            AvaliableCOMPorts.Clear();
            foreach(string port in SerialPort.GetPortNames())
            {
                AvaliableCOMPorts.Add(port);
            }
        }

        public void ResetSerialView()
        {
            if (SelectedItem != null)
            {
                SelectedItem.SerialView.ShutdownEverything();
                string oldName = SelectedItem.SerialView.SerialItemName;
                SelectedItem.SerialView = new SerialViewModel(oldName);
            }
        }
    }
}
