using Astek.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Astek.Controllers
{
    public class HomeController : Controller
    {
        public class Astek
        {
            public string Nama { get; set; }
            public int Gender { get; set; }
            public String Hobi { get; set; }
            public int Umur { get; set; }
        }

        private readonly ILogger<HomeController> _logger;

        private string connectionString = @"Server=DESKTOP-QDCU1GI;Database=AstekTest;User Id=sa;Password=sodikul1;Trusted_Connection=True";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SubmitData([FromBody] List<List<string>> data)
        {

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Nama", typeof(string));
            dataTable.Columns.Add("IdGender", typeof(int));
            dataTable.Columns.Add("IdHobi", typeof(string));
            dataTable.Columns.Add("Umur", typeof(int));

            foreach (var row in data)
            {
                dataTable.Rows.Add(row[0].ToString(), int.Parse(row[1]), row[2].ToString(), int.Parse(row[3]));
            }

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
            {
                DataSource = "DESKTOP-QDCU1GI",
                InitialCatalog = "AstekTest",
                UserID="sa",
                Password="sodikul1",
                IntegratedSecurity=true,
                TrustServerCertificate=true
            };

            // Panggil stored procedure untuk menyimpan data
            //using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            //{
            //    connection.Open();

            //    SqlCommand command = new SqlCommand("InsertPersonalData", connection);
            //    command.CommandType = CommandType.StoredProcedure;
            //    SqlParameter parameter = command.Parameters.AddWithValue("@Data", dataTable);
            //    parameter.SqlDbType = SqlDbType.Structured;
            //    command.ExecuteNonQuery();
            //}

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("InsertPersonalData", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Tentukan struktur tabel parameter secara eksplisit
                    SqlParameter parameter = command.Parameters.AddWithValue("@Data", dataTable);
                    parameter.SqlDbType = SqlDbType.Structured;
                    parameter.TypeName = "dbo.PersonalUDTT"; // Sesuaikan dengan nama jenis tabel Anda

                    command.ExecuteNonQuery();
                }
            }
            return Ok();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
