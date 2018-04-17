using System.Collections.Generic;
using System.Windows;
using System.IO;

namespace ServerApplication
{
    /// <summary>
    /// Interaction logic for LogWindow.xaml
    /// </summary>
    public partial class LogWindow : Window
    {
        List<LogEntry> logEntries = new List<LogEntry>();

        public LogWindow()
        {
            InitializeComponent();
            LoggedInAsLabel.Text = "Current User: " + AppBrain.brain.Username;
            AccessLevelTextBlock.Text = "Access Level: " + AppBrain.brain.AccessLevel;
            PopulateLog();
            LogTable.ItemsSource = logEntries;
        }

        // Return back to the control panel window
        private void ControlPanelWindow(object sender, RoutedEventArgs e)
        {
            MainControlWindow controlPanelWindow = new MainControlWindow();
            controlPanelWindow.Show();
            AppBrain.brain.Msgw = null;
            this.Close();
        }

        // Log out from current user
        private void Logout(object sender, RoutedEventArgs e)
        {
            MainWindow loginWindow = new MainWindow();
            loginWindow.Show();
            AppBrain.brain.Msgw = null;
            this.Close();
        }

        private void PopulateLog()
        {
            var log = File.ReadAllLines(AppBrain.brain.LogFile);

            if (log.Length == 0)
                return;

            for (int i = 0; i < log.Length; i++)
            {
                string[] fileLogEntry = log[i].Split(',');

                LogEntry logEntry = new LogEntry
                {
                    User = fileLogEntry[0],
                    Command = fileLogEntry[1],
                    Time = fileLogEntry[2],
                    Substation = fileLogEntry[3],
                    Switchgear = fileLogEntry[4],
                    Frame = fileLogEntry[5],
                    Breaker = fileLogEntry[6]
                };

                logEntries.Add(logEntry);
            }
        }

        public class LogEntry
        {
            public string User { get; set; }
            public string Command { get; set; }
            public string Time { get; set; }
            public string Substation { get; set; }
            public string Switchgear { get; set; }
            public string Frame { get; set; }
            public string Breaker { get; set; }
        }
    }
}
