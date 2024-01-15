using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows;

namespace URApp
{
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            if (User.ValidateLogin(username, password))
            {
                MessageBox.Show("Login successful!");

                if (username.Equals("admin", StringComparison.OrdinalIgnoreCase))
                {
                    MainWindow mainWindow = new MainWindow();
                    UserOverview userOverview = new UserOverview(); // Assuming you have a UserOverview window

                    mainWindow.Show();
                    userOverview.Show();
                }
                else
                {
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                }

                this.Close();
            }
            else
            {
                MessageBox.Show("Login failed. Please check your username and password.");
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            Register registerWindow = new Register();
            registerWindow.Show();
        }
    }
}