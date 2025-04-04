using API_DE_WARZONE.MODELS;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace API_DE_WARZONE.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")] // Solo usuarios con rol Admin pueden acceder
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/Warzonev2")]
    public class WARZONEv2Controller : ControllerBase
    {
        private readonly string _connectionString;

        public WARZONEv2Controller(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
        }

        [HttpGet]
        public ActionResult<IEnumerable<object>> GetAll()
        {
            var warzones = new List<object>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Warzone", conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    warzones.Add(new
                    {
                        ID = reader.GetInt32(0),
                        Nombre = reader.GetString(1).ToUpper(), // 🔥 Mayúsculas para destacar v2
                        Tipo = reader.GetString(2)
                    });
                }
            }

            return Ok(warzones);
        }
    }
}

