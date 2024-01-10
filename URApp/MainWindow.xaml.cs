using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace URApp
{
    public partial class MainWindow : Window
    {
        private RobotConnectionManager connectionManager;

        public MainWindow()
        {
            InitializeComponent();
            connectionManager = new RobotConnectionManager();
            IpTextBox.Text = "172.20.254.205"; // Default IP
            PortTextBox.Text = "30002"; // Default Port
        }

        // Connect button event handler
        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string ipAddress = IpTextBox.Text;
                int port = int.Parse(PortTextBox.Text); // Let the exception be caught if not a valid number

                await connectionManager.ConnectAsync(ipAddress, port);
                UpdateStatusLight(Colors.Green);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection failed: {ex.Message}");
                UpdateStatusLight(Colors.Red);
            }
        }
        // send kommando til armen
        private void SendCommandButton_Click(object sender, RoutedEventArgs e)
        {
            if (!connectionManager.IsConnected)
            {
                MessageBox.Show("No active connection. Please connect to the robot first.");
                return;
            }

            // Read values from TextBoxes
            double baseValue = double.Parse(BaseTextBox.Text);
            double shoulderValue = double.Parse(ShoulderTextBox.Text);
            double elbowValue = double.Parse(ElbowTextBox.Text);
            double wrist1Value = double.Parse(Wrist1TextBox.Text);
            double wrist2Value = double.Parse(Wrist2TextBox.Text);
            double wrist3Value = double.Parse(Wrist3TextBox.Text);

            // Format the pose string with the input values
            string pose = $"p[{baseValue},{shoulderValue},{elbowValue},{wrist1Value},{wrist2Value},{wrist3Value}]";
            string command = $"movej({pose}, a=1, v=0.25, t=0, r=0)\n";

            bool isCommandSent = connectionManager.SendCommand(command);
            if (isCommandSent)
                MessageBox.Show("Command sent successfully.");
            else
                MessageBox.Show("Failed to send command.");
        }

    

    // Disconnect button event handler
    private async void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            await connectionManager.DisconnectAsync();
            UpdateStatusLight(Colors.Red);
        }

        // Update the status light based on connection status
        private void UpdateStatusLight(Color color)
        {
            StatusLight.Fill = new SolidColorBrush(color);
        }

        // Override OnClosed to ensure network resources are released
        protected override async void OnClosed(EventArgs e)
        {
            await connectionManager.DisconnectAsync();
            base.OnClosed(e);
        }
    }
}
