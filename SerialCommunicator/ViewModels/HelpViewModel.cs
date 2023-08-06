using SerialCommunicator.Utilities;
using SerialCommunicator.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SerialCommunicator.ViewModels
{
    public class HelpViewModel
    {
        public ICommand ShowHelpWindowCommand { get; set; }

        public HelpWindow HelpWindow { get; set; }

        public HelpViewModel()
        {
            ShowHelpWindowCommand = new Command(ShowHelpWindow);

            HelpWindow = new HelpWindow();
        }

        public void ShowHelpWindow()
        {
            HelpWindow.Show();
        }
    }
}
