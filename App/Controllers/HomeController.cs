using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

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
#pragma warning disable SYSLIB0011
                var formatter = new BinaryFormatter();
                using var ms = new MemoryStream(bytes);
                var obj = formatter.Deserialize(ms);
#pragma warning restore SYSLIB0011
            }

            // Path Traversal
            string fileName = HttpContext.Request.Query["file"];
            string content = System.IO.File.ReadAllText("/var/app/uploads/" + fileName);

            // XSS
            return Content("<html><body>User input: " + userInput + "</body></html>", "text/html");
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
            string dbPassword = "HardcodedPassword123!";

            return Content("Secrets are here (for scanning test).");
        }

        [HttpGet("/redirect")]
        public IActionResult RedirectTo(string returnUrl)
        {
            // Open Redirect
            return Redirect(returnUrl);
        }

        [HttpGet("/random")]
        public IActionResult GetRandom()
        {
            // Insecure Random Number
            Random rng = new Random();
            int token = rng.Next(100000, 999999); // Predictable
            return Content("Generated token: " + token);
        }

        [HttpGet("/ssl")]
        public IActionResult CallUntrustedHost(string host)
        {
            // Improper Certificate Validation
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            var client = new HttpClient(handler);
            var response = client.GetAsync("https://" + host).Result;
            return Content("Fetched content from: " + host);
        }

        [HttpGet("/hash")]
        public IActionResult WeakHash(string input)
        {
            // Weak Hash Algorithm (MD5)
            MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Content("MD5 hash: " + Convert.ToBase64String(hash));
        }

        [HttpGet("/encrypt")]
        public IActionResult WeakEncryption(string secret)
        {
            // Weak Symmetric Encryption (DES)
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            des.GenerateKey();
            des.GenerateIV();

            ICryptoTransform encryptor = des.CreateEncryptor();
            byte[] inputBytes = Encoding.UTF8.GetBytes(secret);
            byte[] encrypted = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
            return Content("Encrypted (DES): " + Convert.ToBase64String(encrypted));
        }

        [HttpGet("/clickjack")]
        public IActionResult Clickjack()
        {
            // Clickjacking (No X-Frame-Options header)
            Response.Headers.Remove("X-Frame-Options");
            return Content("<html><body>This page is vulnerable to clickjacking</body></html>", "text/html");
        }

        [HttpGet("/error")]
        public IActionResult ShowError()
        {
            // Exposed Stack Trace
            try
            {
                throw new Exception("This is a test exception");
            }
            catch (Exception ex)
            {
                return Content("Error occurred: " + ex.ToString());
            }
        }

        [HttpGet("/timing")]
        public IActionResult TimingAttack(string password)
        {
            // 🚨 Added Vulnerability: Insecure Timing Leak
            string correctPassword = "SuperSecret123";
            for (int i = 0; i < Math.Min(password.Length, correctPassword.Length); i++)
            {
                if (password[i] != correctPassword[i])
                {
                    // Leaks timing info because it returns at first mismatch
                    return Content("Incorrect password");
                }
            }

            // Artificial delay to simulate processing
            System.Threading.Thread.Sleep(50); 

            return Content("Access granted");
        }

        [HttpGet("/timing1")]
        public IActionResult TimingAttack1(string password)
        {
            // 🚨 Added Vulnerability: Insecure Timing Leak
            string correctPassword = "SuperSecret123";
            for (int i = 0; i < Math.Min(password.Length, correctPassword.Length); i++)
            {
                if (password[i] != correctPassword[i])
                {
                    // Leaks timing info because it returns at first mismatch
                    return Content("Incorrect password");
                }
            }

            // Artificial delay to simulate processing
            System.Threading.Thread.Sleep(50); 

            return Content("Access granted");
        }

        [HttpGet("/timing2")]
        public IActionResult TimingAttack2(string password)
        {
            // 🚨 Added Vulnerability: Insecure Timing Leak
            string correctPassword = "SuperSecret123";
            for (int i = 0; i < Math.Min(password.Length, correctPassword.Length); i++)
            {
                if (password[i] != correctPassword[i])
                {
                    // Leaks timing info because it returns at first mismatch
                    return Content("Incorrect password");
                }
            }

            // Artificial delay to simulate processing
            System.Threading.Thread.Sleep(50); 

            return Content("Access granted");
        }

        [HttpGet("/timing3")]
        public IActionResult TimingAttack3(string password)
        {
            // 🚨 Added Vulnerability: Insecure Timing Leak
            string correctPassword = "SuperSecret123";
            for (int i = 0; i < Math.Min(password.Length, correctPassword.Length); i++)
            {
                if (password[i] != correctPassword[i])
                {
                    // Leaks timing info because it returns at first mismatch
                    return Content("Incorrect password");
                }
            }

            // Artificial delay to simulate processing
            System.Threading.Thread.Sleep(50); 

            return Content("Access granted");
        }
    }
}
