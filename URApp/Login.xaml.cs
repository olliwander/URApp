using System.Data.SqlClient;
using System.Configuration;
using System.Windows;
using System;

namespace URApp
{
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        public bool ValidateLogin(string username, string password)
        {
            string Sasha = "sasha";
            string SashaPW = "sasha";

            // Check if the entered credentials match the hardcoded ones
            if (username == Sasha && password == SashaPW)
            {
                return true;
            }

            string connectionString = ConfigurationManager.ConnectionStrings["MyDBConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "SELECT COUNT(1) FROM users WHERE username=@username AND password=@password";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count == 1;
                }
                catch
                {
                    return false;
                }
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            if (ValidateLogin(username, password))
            {
                MessageBox.Show("Login successful!");
                
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();

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