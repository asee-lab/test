using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace app.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet("/")]
        public IActionResult Index(string userInput)

        {

            // SQL Injection
            var conn = new SqlConnection("Data Source=(local);Initial Catalog=TestDb;Integrated Security=True");
            var cmd = new SqlCommand("SELECT * FROM Users WHERE Name = '" + userInput + "'", conn);
            conn.Open();
            var reader = cmd.ExecuteReader();
            conn.Close();

            // Command Injection
            var output = new ProcessStartInfo()
            {
                FileName = "/bin/bash",
                Arguments = "-c \"ls " + userInput + "\"",
                RedirectStandardOutput = true
            };
            Process.Start(output);

            // Insecure Deserialization
            var base64 = HttpContext.Request.Query["payload"];
            if (!string.IsNullOrEmpty(base64))
            {
                byte[] bytes = Convert.FromBase64String(base64);
                var formatter = new BinaryFormatter();
                using var ms = new MemoryStream(bytes);
                var obj = formatter.Deserialize(ms);
            }

            // Path Traversal
            string fileName = HttpContext.Request.Query["file"];
            string content = System.IO.File.ReadAllText("/var/app/uploads/" + fileName);

            return Content("Processed input: " + userInput);
        }
        [HttpGet("/secrets")]
        public IActionResult GetSecrets()
        {
            // Fake hardcoded secrets to trigger GitHub Secret Scanning

            string githubToken = "ghp_FAKEGitHubTokenForTest1234567890";
            string aws_access_key = "AKIAIOSFODNN7EXAMPLE";
            string aws_secret_key = "wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY";
            string sendGridApiKey = "SG.DemoSecretKeyTriggerScan.123456";
            string slackToken = "xoxb-111111111111-222222222222-EXAMPLESECRETKEY";
            string twilioAuthToken = "1234567890abcdef1234567890abcdef";

            return Content("Secrets are here (for scanning test).");
        }
    }
}
