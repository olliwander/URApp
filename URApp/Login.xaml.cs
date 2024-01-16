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
                // Check if user is trusted
                bool isTrusted = IsUserTrusted(username);
                if (!isTrusted)
                {
                    MessageBox.Show("Permission denied.");
                    return;
                }

                MessageBox.Show("Login successful!");

                if (username.Equals("admin", StringComparison.OrdinalIgnoreCase))
                {
                    // Open Admin specific windows
                    MainWindow mainWindow = new MainWindow(username);
                    UserOverview userOverview = new UserOverview();
                    mainWindow.Show();
                    userOverview.Show();
                }
                else
                {
                    MainWindow mainWindow = new MainWindow(username);
                    mainWindow.Show();
                }

                this.Close();
            }
            else
            {
                MessageBox.Show("Login failed. Please check your username and password.");
            }
        }

        private bool IsUserTrusted(string username)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDBConnectionString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT trusted FROM Users WHERE username = @username";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@username", username);

                object result = command.ExecuteScalar();
                if (result != null && result is bool)
                {
                    return (bool)result;
                }
                return false; // default to not trusted if user not found or any other issue
            }
        }


        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            Register registerWindow = new Register();
            registerWindow.Show();
        }
    }
}