using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;
using System.Configuration;


namespace URApp
{
    public partial class UserOverview : Window
    {
        public ObservableCollection<Appuser> Users { get; set; }

        public UserOverview()
        {
            InitializeComponent();
            LoadUsers();
            DataContext = this;
        }

        private void LoadUsers()
        {
            Users = new ObservableCollection<Appuser>();
            string connectionString = ConfigurationManager.ConnectionStrings["MyDBConnectionString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT username, trusted FROM Users WHERE username <> 'admin'";
                SqlCommand command = new SqlCommand(sql, connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Users.Add(new Appuser
                        {
                            Username = reader.GetString(0),
                            IsTrusted = reader.GetBoolean(1)
                        });
                    }
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDBConnectionString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                foreach (var user in Users)
                {
                    string sql = "UPDATE Users SET trusted = @trusted WHERE username = @username";
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@trusted", user.IsTrusted);
                    command.Parameters.AddWithValue("@username", user.Username);
                    command.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Users updated successfully.");
        }
    }

    public class Appuser
    {
        public string Username { get; set; }
        public bool IsTrusted { get; set; }
    }
}
