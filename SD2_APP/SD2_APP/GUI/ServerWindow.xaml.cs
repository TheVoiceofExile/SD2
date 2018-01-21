using System;
using System.Collections;
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
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace SD2_APP
{
    /// <summary>
    /// Interaction logic for ServerWindow.xaml
    /// </summary>

    public partial class ServerWindow : Window
    {
        private ArrayList nSockets;
        private IPHostEntry Host;

        public ServerWindow()
        {
            InitializeComponent();
        }

        private void ServerLoad(object sender, System.EventArgs e)
        {
            Host = Dns.GetHostEntry(Dns.GetHostName());

            NetworkIPDisplay.Text = "Network IP: " + Host.AddressList[0].ToString();

            nSockets = new ArrayList();

            Thread listener = new Thread(new ThreadStart(ListenerThread));

            listener.Start();

            AppConfig.Text = "Server";
        }

        public void ListenerThread()
        {
            TcpListener tcpListener = new TcpListener(Host.AddressList[0], 8080);

            tcpListener.Start();

            while (true)
            {
                Socket handlerSocket = tcpListener.AcceptSocket();

                if (handlerSocket.Connected)
                {
                    ConnectedClients.Items.Add(handlerSocket.RemoteEndPoint.ToString() + " connected");

                    lock (this)
                    {
                        nSockets.Add(handlerSocket);
                    }

                    ThreadStart thdstHandler = new ThreadStart(HandlerThread);
                    Thread thdHandler = new Thread(thdstHandler);
                    thdHandler.Start();
                }
            }
        }

        public void HandlerThread()
        {
            Socket handlerSocket = (Socket)nSockets[nSockets.Count - 1];

            NetworkStream networkStream = new NetworkStream(handlerSocket);

            int thisRead = 0;
            int blockSize = 1024;

            Byte[] dataByte = new Byte[blockSize];

            lock (this)
            {
                Stream fileStream = File.OpenWrite("C:\\");

                while (true)
                {
                    thisRead = networkStream.Read(dataByte, 0, blockSize);

                    fileStream.Write(dataByte, 0, thisRead);

                    if (thisRead == 0)
                    {
                        break;
                    }

                    fileStream.Close();
                }
            }

            ConnectedClients.Items.Add("File Written");
            handlerSocket = null;
        }
    }
}
