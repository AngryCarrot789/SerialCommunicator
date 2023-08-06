using SerialCommunicator.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialCommunicator.SerialFileLogger
{
    public static class MessageWriter
    {
        public static void WriteToFile(string filePath, List<SerialMessageViewModel> messages)
        {
            List<string> formattedMessages = new List<string>();

            foreach (SerialMessageViewModel message in messages)
            {
                formattedMessages.Add($"{message.RXorTX} >> {message.Message} || {message.Time}");
            }

            File.WriteAllLines(filePath, formattedMessages.ToArray());
        }
    }
}
