using API_DE_WARZONE.MODELS;
using Microsoft.AspNetCore.Mvc;

namespace API_DE_WARZONE.Controllers
{
    [ApiController]
    [Route("api/Warzone")]
    public class WARZONECONTROLLER : ControllerBase
    {
        private static List<Warzone> warzoneObjects = new List<Warzone>
        {
            new Warzone { ID = 1, Nombre = "MP5", Tipo = "SUBFUSIL" },
            new Warzone { ID = 2, Nombre = "Tanque", Tipo = "Blindado" },
            new Warzone { ID = 3, Nombre = "Placas", Tipo = "Blindado" },
            new Warzone { ID = 4, Nombre = "SNIPER", Tipo = "Francotirador" }
        };

        // GET: api/Warzone
        [HttpGet]
        public ActionResult<IEnumerable<Warzone>>GetAll() => Ok(warzoneObjects);


        // GET: api/Warzone/{id}
        [HttpGet("{id}")]
        public ActionResult<Warzone> GetById(int id)
        {
            var warzoneObject = warzoneObjects.FirstOrDefault(o => o.ID == id);
            return warzoneObject != null ? Ok(warzoneObject) : NotFound();
        }

        // POST: api/Warzone
        [HttpPost]
        public ActionResult<Warzone> Create(Warzone warzone)
        {
            warzone.ID = warzoneObjects.Any()? warzoneObjects.Max(p=>p.ID)+1:1;
            warzoneObjects.Add(warzone);
            return CreatedAtAction(nameof(GetById), new { id = warzone.ID }, warzone);
        }

        // PUT: api/warzoneobjects/{id}
        [HttpPut("{id}")]
        public IActionResult Update(int id, Warzone updatedObject)
        {
            var warzoneObject = warzoneObjects.FirstOrDefault(o => o.ID == id);
            if (warzoneObject == null) return NotFound();

            warzoneObject.Nombre = updatedObject.Nombre;
            warzoneObject.Tipo = updatedObject.Tipo;
            return NoContent();
        }

        // DELETE: api/warzoneobjects/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var warzoneObject = warzoneObjects.FirstOrDefault(o => o.ID == id);
            if (warzoneObject == null) return NotFound();

            warzoneObjects.Remove(warzoneObject);
            return NoContent();
        }

    }
}
