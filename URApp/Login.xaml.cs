using System;
using System.Windows;

namespace URApp
{
    public partial class Login : Window //fuck dig med dit grimme fjæs sasha
    {
        public Login()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            if (IsValidCredentials(username, password))
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid username or password");
            }
        }

        private bool IsValidCredentials(string username, string password)
        {
            // Skal ændres til at hive fra database
            string dummyusername = "admin";
            string dummypassword = "admin";

            return username == dummyusername && password == dummypassword;
        }
    }
}
