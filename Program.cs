using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System; // for Environment
using System.Data.SqlClient; // for insecure SQL

namespace VulnerableApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            var app = builder.Build();

            app.MapControllers();

            // 🚨 Intentional vulnerability for testing CodeQL: SQL injection
            var userInput = Environment.GetEnvironmentVariable("USER_ID") ?? "1";
            var connString = "Server=localhost;Database=TestDb;Trusted_Connection=True;";
            using var connection = new SqlConnection(connString);
            var command = new SqlCommand($"SELECT * FROM Users WHERE Id = {userInput}", connection);
            connection.Open();
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine(reader["Name"]);
            }

            app.Run();
        }
    }
}
