using System;
using System.Data.SqlClient;

namespace SqlDependencyBlogExample
{
    public class BetterExample
    {
        public event EventHandler<SqlNotificationEventArgs> OnChange;

        private string currentConnectionString;
        private string currentQuery;
        private bool isStarted;
        private SqlDependency currentDependency;

        public void StartListening(string connectionString, string query)
        {
            if (isStarted)
            {
                throw new InvalidOperationException("Listener already started");
            }

            currentConnectionString = connectionString;
            currentQuery = query;

            isStarted = true;
            SetupListener();
        }

        private void SetupListener()
        {
            if(currentDependency != null)
            {
                currentDependency.OnChange -= Dependency_OnChange;
            }

            using (var db = new SqlConnection(currentConnectionString))
            {
                db.Open();
                var cmd = db.CreateCommand();
                cmd.CommandText = currentQuery;

                SqlDependency.Start(currentConnectionString);
                var dependency = new SqlDependency(cmd);
                dependency.OnChange += Dependency_OnChange;
                currentDependency = dependency;

                cmd.ExecuteReader();
            }
        }

        public void StopListening()
        {
            if (!isStarted)
            {
                throw new InvalidOperationException("Listener is not running");
            }

            SqlDependency.Stop(currentConnectionString);
            isStarted = false;
        }

        private void Dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            this.OnChange?.Invoke(this, e);
            SetupListener();
        }
    }
}
