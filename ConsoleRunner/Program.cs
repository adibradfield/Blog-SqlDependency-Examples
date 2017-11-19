using SqlDependencyBlogExample;
using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            var listener = new BetterExample();
            listener.OnChange += Listener_OnChange;
            var connectionString = @"Server=localhost\dev;Database=SqlDependencyBlogExample;Trusted_Connection=True;";
            var query = "SELECT Id FROM dbo.Invoices";
            listener.StartListening(connectionString, query);

            new Task(() =>
            {
                while (true)
                {
                    Thread.Sleep(1000);
                    using (var db = new SqlConnection(connectionString))
                    {
                        db.Open();
                        var cmd = db.CreateCommand();
                        cmd.CommandText = "INSERT INTO Invoices (InvoiceNumber, CustomerName) VALUES ('INV001', 'Joe Bloggs')";
                        cmd.ExecuteNonQuery();
                    }
                }
                
            }).Start();

            Console.ReadLine();
        }

        private static void Listener_OnChange(object sender, SqlNotificationEventArgs e)
        {
            var notificationText = $"Info: {e.Info.ToString()}" + Environment.NewLine +
                                    $"Source: {e.Source.ToString()}" + Environment.NewLine +
                                    $"Type: {e.Type.ToString()}" + Environment.NewLine;
            Console.WriteLine(notificationText);
        }
    }
}
