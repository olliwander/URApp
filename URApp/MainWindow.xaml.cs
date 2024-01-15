using System;
using System.IO;
using System.Reflection;
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

            PopulateScriptComboBox();
        }

        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string ipAddress = IpTextBox.Text;
                int port = int.Parse(PortTextBox.Text); 

                await connectionManager.ConnectAsync(ipAddress, port);
                UpdateStatusLight(Colors.Green);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection failed: {ex.Message}");
                UpdateStatusLight(Colors.Red);
            }
        }

        private void SendCommandButton_Click(object sender, RoutedEventArgs e)
        {
            if (!connectionManager.IsConnected)
            {
                MessageBox.Show("No active connection. Please connect to the robot first.");
                return;
            }

            StringBuilder programCommands = new StringBuilder(); 

            programCommands.AppendLine("def programname():");

            foreach (var waypointItem in WaypointListBox.Items)
            {
                string waypointName = waypointItem.ToString();

                if (waypointCache.TryGetValue(waypointName, out string[] waypointData) && waypointData.Length == 6)
                {
                    string[] convertedValues = waypointData.Select(value => value.Replace(',', '.')).ToArray();
                    string pose = $"p[{string.Join(",", convertedValues)}]";

                    programCommands.AppendLine($"  movej({pose}, a=1.2, v=1, t=0, r=0)");
                }
                else
                {
                    MessageBox.Show($"Waypoint data for '{waypointName}' not found in cache or does not have the expected length (6).");
                    return; 
                }
            }

            programCommands.AppendLine("end");

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

        private async void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            await connectionManager.DisconnectAsync();
            UpdateStatusLight(Colors.Red);
        }
        private void UpdateStatusLight(Color color)
        {
            StatusLight.Fill = new SolidColorBrush(color);
        }
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

        private void WaypointButton1_Click(object sender, RoutedEventArgs e) => LoadWaypointData("waypoint_1");
        private void WaypointButton2_Click(object sender, RoutedEventArgs e) => LoadWaypointData("waypoint_2");
        private void WaypointButton3_Click(object sender, RoutedEventArgs e) => LoadWaypointData("waypoint_3");
        private void WaypointButton4_Click(object sender, RoutedEventArgs e) => LoadWaypointData("waypoint_4");
        private void WaypointButton5_Click(object sender, RoutedEventArgs e) => LoadWaypointData("waypoint_5");
        private void WaypointButton6_Click(object sender, RoutedEventArgs e) => LoadWaypointData("waypoint_6");
        private void WaypointButton7_Click(object sender, RoutedEventArgs e) => LoadWaypointData("waypoint_7");
        private void WaypointButton8_Click(object sender, RoutedEventArgs e) => LoadWaypointData("waypoint_8");
        private void WaypointButton9_Click(object sender, RoutedEventArgs e) => LoadWaypointData("waypoint_9");
        private void LoadWaypointData(string waypoint)
        {
            if (!waypointCache.ContainsKey(waypoint))
            {
                string[] waypointData = DatabaseHelper.GetWaypointData(waypoint);
                if (waypointData != null && waypointData.Length == 6)
                {
                    waypointCache[waypoint] = waypointData;
                }
                else
                {
                    MessageBox.Show($"Waypoint data for '{waypoint}' not found.");
                    return;
                }
            }
            string[] cachedData = waypointCache[waypoint];
            BaseTextBox.Text = cachedData[0];
            ShoulderTextBox.Text = cachedData[1];
            ElbowTextBox.Text = cachedData[2];
            Wrist1TextBox.Text = cachedData[3];
            Wrist2TextBox.Text = cachedData[4];
            Wrist3TextBox.Text = cachedData[5];

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
                string selectedWaypoint = WaypointListBox.SelectedItem.ToString();
                waypoints.Remove(selectedWaypoint);

                UpdateWaypointListBox();
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

        private void PopulateScriptComboBox()
        {
            string baseDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\.."));
            string folderPath = Path.Combine(baseDirectory, "scripts");

            Console.WriteLine($"baseDirectory: {baseDirectory}");
            Console.WriteLine($"folderPath: {folderPath}");

            Console.ReadLine();


            if (Directory.Exists(folderPath))
            {
                string[] scriptFiles = Directory.GetFiles(folderPath, "*.txt");
                foreach (string scriptFile in scriptFiles)
                {
                    string scriptName = Path.GetFileNameWithoutExtension(scriptFile);
                    ScriptComboBox.Items.Add(scriptName);
                }
            }
        }

        private void StartProgramButton_Click(object sender, RoutedEventArgs e)
        {
            if (!connectionManager.IsConnected)
            {
                MessageBox.Show("No active connection. Please connect to the robot first.");
                return;
            }

            string selectedScriptFileName = ScriptComboBox.SelectedItem as string;

            if (selectedScriptFileName != null)
            {
                try
                {
                    string scriptDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\scripts");

                    string selectedScriptFilePath = Path.Combine(scriptDirectory, selectedScriptFileName + ".txt");

                    string scriptContent = File.ReadAllText(selectedScriptFilePath);

                    bool isScriptCommandSent = connectionManager.SendCommand(scriptContent);

                    Console.Write (scriptContent);
                    Console.Read();

                    if (isScriptCommandSent)
                    {
                        MessageBox.Show($"Script '{selectedScriptFileName}' started successfully.");
                    }
                    else
                    {
                        MessageBox.Show($"Failed to start script '{selectedScriptFileName}'.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error reading or sending script: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Please select a script to start.");
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (!connectionManager.IsConnected)
            {
                MessageBox.Show("No active connection. Please connect to the robot first.");
                return;
            }

            string stopScriptCommand = "stopl(10)";
            bool isStopCommandSent = connectionManager.SendCommand(stopScriptCommand);

            if (isStopCommandSent)
            {
                MessageBox.Show("Script stopped successfully.");
            }
            else
            {
                MessageBox.Show("Failed to stop the script.");
            }
        }
    }
}