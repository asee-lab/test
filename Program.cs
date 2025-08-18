using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System; // for Environment
using System.Data.SqlClient; // for insecure SQL
using System.IO; // for insecure file handling
using System.Net; // for insecure HTTP calls
using System.Text; // for insecure encoding

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

            // 🚨 Vulnerability #1: SQL Injection
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

            // 🚨 Vulnerability #4: Insecure HTTP (no TLS)
            using var client = new WebClient();
            var data = client.DownloadString("http://example.com/api/secret"); 
            Console.WriteLine($"Downloaded: {data}");

            // 🚨 Vulnerability #5: Weak cryptography
            var plaintext = "super secret";
            var bytes = Encoding.ASCII.GetBytes(plaintext); // Weak encoding (ASCII)
            Console.WriteLine($"Plain bytes: {BitConverter.ToString(bytes)}");

            app.Run();
        }
    }
}
