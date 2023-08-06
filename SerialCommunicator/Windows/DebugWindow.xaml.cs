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
    /// Interaction logic for DebugWindow.xaml
    /// </summary>
    public partial class DebugWindow : Window
    {
        public DebugWindow()
        {
            InitializeComponent();

            Closing += DebugWindow_Closing;
        }

        private void DebugWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        public void WriteSlot(int slot, string name, string value)
        {
            switch (slot)
            {
                case 1: _1.Content = name; _1val.Content = value; break;
                case 2: _2.Content = name; _2val.Content = value; break;
                case 3: _3.Content = name; _3val.Content = value; break;
                case 4: _4.Content = name; _4val.Content = value; break;
                case 5: _5.Content = name; _5val.Content = value; break;
                case 6: _6.Content = name; _6val.Content = value; break;
                case 7: _7.Content = name; _7val.Content = value; break;
                case 8: _8.Content = name; _8val.Content = value; break;
                case 9: _9.Content = name; _9val.Content = value; break;
                case 10: _10.Content = name; _10val.Content = value; break;
                case 11: _11.Content = name; _11val.Content = value; break;
                case 12: _12.Content = name; _12val.Content = value; break;
                case 13: _13.Content = name; _13val.Content = value; break;
                case 14: _14.Content = name; _14val.Content = value; break;
                case 15: _15.Content = name; _15val.Content = value; break;
            }
        }
    }
}
