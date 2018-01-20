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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SD2_APP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
    }
}
