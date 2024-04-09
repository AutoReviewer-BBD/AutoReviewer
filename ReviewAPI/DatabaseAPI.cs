using System;
using System.Diagnostics;
using Microsoft.Data.SqlClient;

public class DatabaseAPI
{
    private static SqlConnection? _connection;

    public static void Initialize()
    {
        Trace.WriteLine("Attempting connection to bd");
        if (_connection == null)
        {
            try {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
                {
                    DataSource = "manual-db.cwnn1s7lxjgf.eu-west-1.rds.amazonaws.com",
                    UserID = "AutoReviewer",
                    Password = "",
                    InitialCatalog = "temp",
                    TrustServerCertificate=true
                };

                _connection = new SqlConnection(builder.ConnectionString);
                _connection.Open();

                Trace.WriteLine("Connection success");

                var reader = ExecuteQuery("select * from temp");
                while (reader.Read())
                    {
                        Console.WriteLine("{0} {1}", reader.GetInt32(0), reader.GetString(1));
                    }
            }
            catch (Exception e)
            {
                Trace.WriteLine("An error occured with DB connection: " + e);
            }
        }
    }

    public static void CloseConnection()
    {
        if (_connection != null && _connection.State != System.Data.ConnectionState.Closed)
        {
            _connection.Close();
        }
    }

    public static SqlDataReader ExecuteQuery(string query)
    {
        if (_connection == null)
        {
            throw new InvalidOperationException("Database connection is not initialized or open.");
        }

        SqlCommand command = new SqlCommand(query, _connection);
        return command.ExecuteReader();
    }

    public static int ExecuteNonQuery(string query)
    {
        if (_connection == null || _connection.State != System.Data.ConnectionState.Open)
        {
            throw new InvalidOperationException("Database connection is not initialized or open.");
        }

        SqlCommand command = new SqlCommand(query, _connection);
        return command.ExecuteNonQuery();
    }
}
