using System;
using System.Collections.Generic; // For List<>
using System.Windows;
using System.Windows.Media;
using URApp;

namespace URApp
{
    public partial class MainWindow : Window
    {
        private RobotConnectionManager connectionManager;
        private List<string> waypoints = new List<string>(); // List to store waypoints

        public MainWindow()
        {
            InitializeComponent();
            connectionManager = new RobotConnectionManager();
            IpTextBox.Text = "172.20.254.205"; // Robot5 IP
            PortTextBox.Text = "30002"; // Port
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
            string command = $"movej({pose}, a=1.2, v=0.25, t=0, r=0)\n";

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

        // Waypoint button click event handlers
        private void WaypointButton1_Click(object sender, RoutedEventArgs e) => LoadWaypointData("waypoint_1");
        private void WaypointButton2_Click(object sender, RoutedEventArgs e) => LoadWaypointData("waypoint_2");
        private void WaypointButton3_Click(object sender, RoutedEventArgs e) => LoadWaypointData("waypoint_3");
        private void WaypointButton4_Click(object sender, RoutedEventArgs e) => LoadWaypointData("waypoint_4");
        private void WaypointButton5_Click(object sender, RoutedEventArgs e) => LoadWaypointData("waypoint_5");
        private void WaypointButton6_Click(object sender, RoutedEventArgs e) => LoadWaypointData("waypoint_6");
        private void WaypointButton7_Click(object sender, RoutedEventArgs e) => LoadWaypointData("waypoint_7");
        private void WaypointButton8_Click(object sender, RoutedEventArgs e) => LoadWaypointData("waypoint_8");
        private void WaypointButton9_Click(object sender, RoutedEventArgs e) => LoadWaypointData("waypoint_9");

        // Load and display waypoint data
        private void LoadWaypointData(string waypoint)
        {
            string[] waypointData = DatabaseHelper.GetWaypointData(waypoint);
            if (waypointData != null && waypointData.Length == 6)
            {
                BaseTextBox.Text = waypointData[0];
                ShoulderTextBox.Text = waypointData[1];
                ElbowTextBox.Text = waypointData[2];
                Wrist1TextBox.Text = waypointData[3];
                Wrist2TextBox.Text = waypointData[4];
                Wrist3TextBox.Text = waypointData[5];

                waypoints.Add(waypoint);
                UpdateWaypointListBox();
            }
            else
            {
                // Handle case where waypoint data is not found
                MessageBox.Show($"Waypoint data for '{waypoint}' not found.");
            }
        }

        private void RemoveWaypointButton_Click(object sender, RoutedEventArgs e)
        {
            if (WaypointListBox.SelectedItem != null)
            {
                // Remove the selected waypoint from the list
                string selectedWaypoint = WaypointListBox.SelectedItem.ToString();
                waypoints.Remove(selectedWaypoint);

                // Update the ListBox
                UpdateWaypointListBox();

                // Optionally, you can also clear the corresponding fields
                // if they currently show the data of the removed waypoint
                ClearWaypointDataFields();
            }
            else
            {
                MessageBox.Show("Please select a waypoint to remove.");
            }
        }

        private void UpdateWaypointListBox()
        {
            WaypointListBox.Items.Clear();
            foreach (var wp in waypoints)
            {
                WaypointListBox.Items.Add(wp);
            }
        }

        private void ClearWaypointDataFields()
        {
            BaseTextBox.Text = "";
            ShoulderTextBox.Text = "";
            ElbowTextBox.Text = "";
            Wrist1TextBox.Text = "";
            Wrist2TextBox.Text = "";
            Wrist3TextBox.Text = "";
        }

        // ... other methods ...
    }

    // ... other methods ...
}

