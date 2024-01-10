using System.Configuration;
using System.Data.SqlClient;
using System.Windows;

namespace URApp
{
    public partial class Register : Window
    {
        public Register()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDBConnectionString"].ConnectionString;
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            if (password.Length < 5)
            {
                MessageBox.Show("The password must be at least 5 characters long.");
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "INSERT INTO users (username, password) VALUES (@username, @password)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    int result = cmd.ExecuteNonQuery();
                    if (result == 1)
                    {
                        MessageBox.Show("Registration successful!");
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Registration failed. Please try again.");
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("An error occurred while registering. Please try again.");
            }
        }
    }
}
