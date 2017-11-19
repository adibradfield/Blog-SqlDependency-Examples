using System;
using System.Data.SqlClient;

namespace SqlDependencyBlogExample
{
    public class SimpleExample
    {
        public void StartListening(string connectionString, string query)
        {
            using(var db = new SqlConnection(connectionString))
            {
                db.Open();
                var cmd = db.CreateCommand();
                cmd.CommandText = query;

                SqlDependency.Start(connectionString);
                var dependency = new SqlDependency(cmd);
                dependency.OnChange += Dependency_OnChange;

                cmd.ExecuteReader();
            }
        }

        private void Dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            var notificationText = $"Info: {e.Info.ToString()}" + Environment.NewLine +
                                    $"Source: {e.Source.ToString()}" + Environment.NewLine +
                                    $"Type: {e.Type.ToString()}";
            Console.WriteLine(notificationText);
        }
    }
}
