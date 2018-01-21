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
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace SD2_APP
{
    /// <summary>
    /// Interaction logic for ClientWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        public ClientWindow()
        {
            InitializeComponent();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".txt",
                Filter = "Text Files (*.txt)|*.txt"
            };

            Nullable<bool> result = openFileDialog.ShowDialog();

            if (result == true)
            {
                Filename.Text = openFileDialog.FileName;
            }
        }

        private void SendFile_Click(object sender, RoutedEventArgs e)
        {
            Stream fileStream = File.OpenRead(Filename.Text);

            byte[] fileBuffer = new byte[fileStream.Length];

            fileStream.Read(fileBuffer, 0, (int)fileStream.Length);

            TcpClient clientSocket = new TcpClient(DestinationServer.Text, 8080);

            NetworkStream networkStream = clientSocket.GetStream();

            networkStream.Write(fileBuffer, 0, fileBuffer.GetLength(0));

            networkStream.Close();
        }
    }
}
