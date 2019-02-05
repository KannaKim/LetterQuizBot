using System;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using Npgsql;

namespace SQLrelated
{
    class Program
    {
        static void Main(string[] args)
        {
            var connString = "Host=localhost;Username=postgres;Password="+Environment.GetEnvironmentVariable("PostresPassword") +";Database=some_db";

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                // Insert some data
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "select * from test.client";
                    cmd.ExecuteNonQuery();
                    Console.WriteLine(cmd.CommandText);
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                            Console.WriteLine($"{reader.GetString(0)} {reader.GetString(1)} {reader.GetInt32(2)}");
                }
                
            }
        }

  
    }
}
