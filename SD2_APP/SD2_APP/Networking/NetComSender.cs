using System;
using System.Windows;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;

namespace SD2_APP
{
    public class NetComSender
    {
        public NetComSender()
        {
        }

        public void SendText(string textInput, string serverIP)
        {
            MessageBox.Show(textInput);

            byte[] textBuffer = new byte[textInput.Length];

            for (int i = 0; i < textInput.Length; i++)
            {
                textBuffer[i] = Convert.ToByte(textInput[i]);
            }

            TcpClient serverSocket = new TcpClient(serverIP, 8080);
            NetworkStream networkStream = serverSocket.GetStream();

            networkStream.Write(textBuffer, 0, textBuffer.GetLength(0));

            networkStream.Close();
        }
    }
}