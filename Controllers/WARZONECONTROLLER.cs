using API_DE_WARZONE.MODELS;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace API_DE_WARZONE.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")] // También requiere rol Admin
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/Warzone")]
    public class WARZONEv1Controller : ControllerBase
    {
        private readonly string _connectionString;

        public WARZONEv1Controller(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(nameof(_connectionString));
        }

        [HttpGet]
        public ActionResult<IEnumerable<Warzone>> GetAll()
        {
            var warzones = new List<Warzone>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Warzone", conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    warzones.Add(new Warzone
                    {
                        ID = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        Tipo = reader.GetString(2)
                    });
                }
            }

            return Ok(warzones);
        }

        [HttpGet("{id}")]
        public ActionResult<Warzone> GetById(int id)
        {
            Warzone? warzone = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Warzone WHERE ID = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    warzone = new Warzone
                    {
                        ID = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        Tipo = reader.GetString(2)
                    };
                }
            }
            return warzone != null ? Ok(warzone) : NotFound();
        }

        [HttpPost]
        public IActionResult Create([FromBody] Warzone warzone)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO Warzone (Nombre, Tipo) VALUES (@nombre, @tipo)", conn);
                cmd.Parameters.AddWithValue("@nombre", warzone.Nombre);
                cmd.Parameters.AddWithValue("@tipo", warzone.Tipo);
                cmd.ExecuteNonQuery();
            }
            return CreatedAtAction(nameof(GetAll), new { warzone.Nombre }, warzone);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Warzone warzone)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Warzone SET Nombre = @nombre, Tipo = @tipo WHERE ID = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@nombre", warzone.Nombre);
                cmd.Parameters.AddWithValue("@tipo", warzone.Tipo);
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected == 0) return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Warzone WHERE ID = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected == 0) return NotFound();
            }
            return NoContent();
        }
    }
}
