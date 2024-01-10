using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace URApp
{
    public partial class MainWindow : Window
    {
        private RobotConnectionManager connectionManager;
        private DispatcherTimer dataUpdateTimer;

        public MainWindow()
        {
            InitializeComponent();
            connectionManager = new RobotConnectionManager();
            IpTextBox.Text = "172.20.254.205"; // Default IP
            PortTextBox.Text = "30002"; // Default Port

            // giver timer til at opdatere data fordi den ikke skal opdatere hver time jo
            dataUpdateTimer = new DispatcherTimer();
            dataUpdateTimer.Tick += DataUpdateTimer_Tick;
            dataUpdateTimer.Interval = TimeSpan.FromMilliseconds(500);
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

        // Send command button event handler
        private void SendCommandButton_Click(object sender, RoutedEventArgs e)
        {
            if (!connectionManager.IsCommandConnected)
            {
                MessageBox.Show("No active connection. Please connect to the robot first.");
                return;
            }

            string pose = "p[0.22,0.24,0.5,0.1,-0.5,-0.1]"; // Example pose
            string command = $"movej({pose}, a=1.2, v=0.25, t=0, r=0)\n"; // Example command

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

        // Event handler to update data
        private async void DataUpdateTimer_Tick(object sender, EventArgs e)
        {
            string data = await connectionManager.ReceiveDataAsync();
            if (data != null)
            {
                // Parse the actual TCP pose from the received data
                // Assuming the received string is in the format "x,y,z,rx,ry,rz"
                string[] poseData = data.Split(',');
                if (poseData.Length == 6)
                {
                    UpdatePoseDisplay(poseData);
                }
            }
        }

        private void UpdatePoseDisplay(string[] poseData)
        {
            // Update the UI elements with the received pose data
            // Make sure to invoke this on the UI thread
            Dispatcher.Invoke(() =>
            {
                TcpPoseX.Text = poseData[0];
                TcpPoseY.Text = poseData[1];
                TcpPoseZ.Text = poseData[2];
                TcpPoseRX.Text = poseData[3];
                TcpPoseRY.Text = poseData[4];
                TcpPoseRZ.Text = poseData[5];
            });
        }

        // Connect button event handler and other events...

        // Override OnClosed to ensure network resources are released
        protected override async void OnClosed(EventArgs e)
        {
            dataUpdateTimer?.Stop();
            await connectionManager.DisconnectAsync();
            base.OnClosed(e);
        }
    }
}
