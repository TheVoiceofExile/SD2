using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;



namespace SD2_APP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NetComSender sendNet = new NetComSender();
        NetComListener listenNet = new NetComListener();

        public MainWindow()
        {
            InitializeComponent();
        }

        /**
         * Check which radio button is selected when Send Command is clicked.
         * Open a message box displaying sent command when clicked based
         * on the selected radio button. Throw error if no command is selected.
         * Uncheck the selected command after the command is sent.
         */
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (RackIn.IsChecked == true)
            {
                MessageBox.Show("Racking In");
                RackIn.IsChecked = false;
            }
            else if (RackOut.IsChecked == true)
            {
                MessageBox.Show("Racking Out");
                RackOut.IsChecked = false;
            }
            else if (MotorOff.IsChecked == true)
            {
                MessageBox.Show("Motor Off");
                MotorOff.IsChecked = false;
            }
            else if (MotorOn.IsChecked == true)
            {
                MessageBox.Show("Motor On");
                MotorOn.IsChecked = false;
            }
            else
            {
                MessageBox.Show("No command selected, please select a command.");
                return;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            sendNet.SendText(ComText.Text, NetworkIP.Text);
        }

        private void InitializeNetworking_Click(object sender, RoutedEventArgs e)
        {
            if (ClientSelected.IsChecked == true)
            {
                ClientWindow clientWindow = new ClientWindow();
                App.Current.MainWindow = new ClientWindow();
                this.Close();
                clientWindow.Show();
            }
            else if (ServerSelected.IsChecked == true)
            {
                ServerWindow serverWindow = new ServerWindow();
                App.Current.MainWindow = new ServerWindow();
                this.Close();
                serverWindow.Show();
            }
            else
            {
                MessageBox.Show("Before initializing networking select the network type for this instance.");
            }
        }
    }
}
