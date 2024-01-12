using System;
using System.Data.SqlClient;
using System.Configuration;

namespace URApp
{
    public class User
    {
        public static bool ValidateLogin(string username, string password)
        {
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
    }

    public class DatabaseHelper
    {
        // Method to fetch waypoint data from the database
        public static string[] GetWaypointData(string waypointName)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDBConnectionString"].ConnectionString;
            string[] waypointData = new string[6]; // Hvert waypoint har 6 værdier

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "SELECT base, shoulder, elbow, wrist1, wrist2, wrist3 FROM waypoints WHERE name=@waypointName";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@waypointName", waypointName);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                waypointData[i] = reader[i].ToString();
                            }
                        }
                    }
                }
                catch
                {
                    // Handle exceptions or errors here
                }
            }
            return waypointData;
        }
    }
}
