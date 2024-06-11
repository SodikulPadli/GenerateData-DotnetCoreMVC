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


        private readonly ILogger<HomeController> _logger;

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
            //Initial Datatable 
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Nama", typeof(string));
            dataTable.Columns.Add("IdGender", typeof(int));
            dataTable.Columns.Add("IdHobi", typeof(string));
            dataTable.Columns.Add("Umur", typeof(int));

            //Looping data from parameter
            foreach (var row in data)
            {
                // Add datatable
                dataTable.Rows.Add(row[0].ToString(), int.Parse(row[1]), row[2].ToString(), int.Parse(row[3]));
            }

            // Create Intial Server For Connection
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
            {
                DataSource = "DESKTOP-QDCU1GI",
                InitialCatalog = "AstekTest",
                UserID = "sa",
                Password = "sodikul1",
                IntegratedSecurity = true,
                TrustServerCertificate = true
            };

            //Connection Server
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();

                // Select Store Procedur  with sqlcommand
                using (SqlCommand command = new SqlCommand("InsertPersonalData", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    SqlParameter parameter = command.Parameters.AddWithValue("@Data", dataTable);// Add Parameter
                    parameter.SqlDbType = SqlDbType.Structured;
                    //parameter.TypeName = "dbo.PersonalUDTT";

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
