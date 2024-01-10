using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace URApp
{
    public partial class MainWindow : Window
    {
        private TcpClient client;
        private NetworkStream stream;

        public MainWindow()
        {
            InitializeComponent();
            IpTextBox.Text = "172.20.254.201";
            PortTextBox.Text = "30002";
            client = new TcpClient(); 
        }

        // Connectknap
        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            string ipAddress = IpTextBox.Text; 
            int port;

            if (!IPAddress.TryParse(ipAddress, out IPAddress ip))
            {
                MessageBox.Show("Please enter a valid IP address.");
                return;
            }

            if (!int.TryParse(PortTextBox.Text, out port) || port < 1 || port > 65535)
            {
                MessageBox.Show("Please enter a valid port number.");
                return;
            }
            // Attempt to connect if not already connected
            if (!client.Connected)
            {
                try
                {
                    client.Connect(ipAddress, port);
                    stream = client.GetStream();
                    StatusLight.Fill = new SolidColorBrush(Colors.Green);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Connection failed: {ex.Message}");
                    StatusLight.Fill = new SolidColorBrush(Colors.Red);
                }
            }
        }

        // Send kommando
        private void SendCommandButton_Click(object sender, RoutedEventArgs e)
        {
            string command = CommandTextBox.Text;

            if (!client.Connected)
            {
                MessageBox.Show("No active connection. Please connect to the robot first.");
                return;
            }

            if (!string.IsNullOrWhiteSpace(command))
            {
                // Append the necessary newline character to the command as required by the robot
                command += "\n";
                bool isCommandSent = SendCommand(command);

                if (isCommandSent)
                {
                    MessageBox.Show("Command sent successfully.");
                }
                else
                {
                    MessageBox.Show("Failed to send the command.");
                }
            }
            else
            {
                MessageBox.Show("Please enter a command.");
            }
        }

        // Send kommando når der er forbindelse men virker det når jeg opdaterer herigennem? 
        private bool SendCommand(string command)
        {
            if (stream != null && client.Connected)
            {
                byte[] data = Encoding.ASCII.GetBytes(command);
                stream.Write(data, 0, data.Length);
                return true;
            }
            return false;
        }

        // Method to disconnect the client
        private void Disconnect()
        {
            if (client != null && client.Connected)
            {
                stream.Close();
                client.Close();
            }
        }

        // Event handler for disconnecting
        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            Disconnect();
            StatusLight.Fill = new SolidColorBrush(Colors.Red);
        }

        // Override OnClosed to ensure the network resources are released
        protected override void OnClosed(EventArgs e)
        {
            Disconnect();
            base.OnClosed(e);
        }
    }
}
