using System;
using System.Collections.Generic; // For List<>
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace URApp
{
    public partial class MainWindow : Window
    {
        private RobotConnectionManager connectionManager;
        private List<string> waypoints = new List<string>();
        private Dictionary<string, string[]> waypointCache = new Dictionary<string, string[]>();


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
        // Send command button event handler
        private void SendCommandButton_Click(object sender, RoutedEventArgs e)
        {
            if (!connectionManager.IsConnected)
            {
                MessageBox.Show("No active connection. Please connect to the robot first.");
                return;
            }

            StringBuilder programCommands = new StringBuilder(); // To store commands for the program

            // Add the program header with def programname():
            programCommands.AppendLine("def programname():");

            // Assuming waypointsBox is a ListBox or a similar control containing your waypoints
            foreach (var waypointItem in WaypointListBox.Items)
            {
                string waypointName = waypointItem.ToString();

                if (waypointCache.TryGetValue(waypointName, out string[] waypointData) && waypointData.Length == 6)
                {
                    // Convert values from commas to periods
                    string[] convertedValues = waypointData.Select(value => value.Replace(',', '.')).ToArray();

                    // Format the pose string with the converted waypoint data
                    string pose = $"p[{string.Join(",", convertedValues)}]";

                    // Append the current movej command to the programCommands without "breakAfter"
                    programCommands.AppendLine($"  movej({pose}, a=1.2, v=1, t=0, r=0)");
                }
                else
                {
                    MessageBox.Show($"Waypoint data for '{waypointName}' not found in cache or does not have the expected length (6).");
                    return; // Stop execution if data not found or incorrect length
                }
            }

            // Add the program footer with end
            programCommands.AppendLine("end");

            // Send the programCommands to the robot
            bool isProgramCommandSent = connectionManager.SendCommand(programCommands.ToString());

            if (isProgramCommandSent)
            {
                MessageBox.Show("Program command sent successfully.");
            }
            else
            {
                MessageBox.Show("Failed to send program command.");
            }
        }


        // Disconnect button
        private async void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            await connectionManager.DisconnectAsync();
            UpdateStatusLight(Colors.Red);
        }

        // Status lampen øverst
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

        private void SaveWaypointData_Click(object sender, RoutedEventArgs e)
        {
            if (WaypointListBox.SelectedItem != null)
            {
                string selectedWaypoint = WaypointListBox.SelectedItem.ToString();
                waypointCache[selectedWaypoint] = new string[]
                {
            BaseTextBox.Text,
            ShoulderTextBox.Text,
            ElbowTextBox.Text,
            Wrist1TextBox.Text,
            Wrist2TextBox.Text,
            Wrist3TextBox.Text
                };
            }
            else
            {
                MessageBox.Show("Please select a waypoint to save.");
            }
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
            // Check if the waypoint data is already in the cache
            if (!waypointCache.ContainsKey(waypoint))
            {
                string[] waypointData = DatabaseHelper.GetWaypointData(waypoint);
                if (waypointData != null && waypointData.Length == 6)
                {
                    // Cache the waypoint data
                    waypointCache[waypoint] = waypointData;
                }
                else
                {
                    MessageBox.Show($"Waypoint data for '{waypoint}' not found.");
                    return;
                }
            }

            // Use the cached data
            string[] cachedData = waypointCache[waypoint];
            BaseTextBox.Text = cachedData[0];
            ShoulderTextBox.Text = cachedData[1];
            ElbowTextBox.Text = cachedData[2];
            Wrist1TextBox.Text = cachedData[3];
            Wrist2TextBox.Text = cachedData[4];
            Wrist3TextBox.Text = cachedData[5];

            // Add waypoint to the list and update ListBox, if not already added
            if (!waypoints.Contains(waypoint))
            {
                waypoints.Add(waypoint);
                UpdateWaypointListBox();
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

        private void WaypointListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (WaypointListBox.SelectedItem != null)
            {
                string selectedWaypoint = WaypointListBox.SelectedItem.ToString();
                LoadWaypointData(selectedWaypoint);
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