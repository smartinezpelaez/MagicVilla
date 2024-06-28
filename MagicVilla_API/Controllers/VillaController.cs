using MagicVilla_API.Datos;
using MagicVilla_API.Modelos;
using MagicVilla_API.Modelos.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaController : ControllerBase
    {
        private readonly ILogger<VillaController> _logger;
        private readonly ApplicationDbContext _db;
        public VillaController(ILogger<VillaController> logger, ApplicationDbContext db)
        {
             _logger = logger;
            _db = db;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VillaDto>>> GetVillas() 
        {
            _logger.LogInformation("Obtener las villas");
            return Ok(await _db.Villas.ToListAsync());
            
        }

        [HttpGet("id:int", Name= "GetVillaById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VillaDto>> GetVillaById(int id)
        {
            if (id == 0) 
            {
                _logger.LogError("Error al traer  Villa con id " + id);
                return BadRequest();
            } 

            //var villa = VillaStore.VillaList.FirstOrDefault(x => x.Id == id);
            var villa = await _db.Villas.FirstOrDefaultAsync(x => x.Id == id);

            if (villa == null) 
            {
                _logger.LogError("Error no encontado la  Villa con id " + id);
                return NotFound();
            }

            return Ok(villa);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaCreateDto>> CrearVilla([FromBody] VillaCreateDto villaDto) 
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }

            if (await _db.Villas.FirstOrDefaultAsync(x=>x.Nombre.ToLower() == villaDto.Nombre.ToLower()) != null)
            {
                ModelState.AddModelError("Nombre Existe", "La villa con ese nombre ya existe!");
                return BadRequest(ModelState);
            }

            if (villaDto == null) 
            {
                return BadRequest(villaDto);
            }
           
            //villaDto.Id = _db.Villas.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
            //VillaStore.VillaList.Add(villaDto);
            Villa modelo = new()
            {
                //Id = villaDto.Id,
                Nombre = villaDto.Nombre,
                Detalle = villaDto.Detalle,
                Tarifa = villaDto.Tarifa,
                Ocupantes = villaDto.Ocupantes,
                MetrosCuadrados = villaDto.MetrosCuadrados,
                ImageUrl = villaDto.ImageUrl,
                Amenidad = villaDto.Amenidad

            };
            await _db.Villas.AddAsync(modelo);
            await _db.SaveChangesAsync();

            return CreatedAtRoute("GetVillaById", new { id = modelo.Id}, modelo);
        
        
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDto villaDto) 
        {
            if (villaDto == null || id != villaDto.Id)
            {
                return BadRequest();
            }
            
            //Forma cuando no se tenia la BD
            //var villa = VillaStore.VillaList.FirstOrDefault(x => x.Id == id);
            //villa.Nombre = villaDto.Nombre;
            //villa.Ocupantes = villaDto.Ocupantes;
            //villa.MetrosCuadrados = villaDto.MetrosCuadrados;

            Villa modelo = new()
            {
                Id = villaDto.Id,
                Nombre = villaDto.Nombre,
                Detalle = villaDto.Detalle,
                Tarifa = villaDto.Tarifa,
                Ocupantes = villaDto.Ocupantes,
                MetrosCuadrados = villaDto.MetrosCuadrados,
                ImageUrl = villaDto.ImageUrl,
                Amenidad = villaDto.Amenidad

            };
            _db.Villas.Update(modelo);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto> patchDto)
        {
            if (patchDto == null || id == 0)
            {
                return BadRequest();
            }
            //var villa = VillaStore.VillaList.FirstOrDefault(x => x.Id == id);
            var villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(x=>x.Id == id);
            
            VillaUpdateDto villaDto = new()
            {
                Id = villa.Id,
                Nombre = villa.Nombre,
                Detalle = villa.Detalle,
                Tarifa = villa.Tarifa,
                Ocupantes = villa.Ocupantes,
                MetrosCuadrados = villa.MetrosCuadrados,
                ImageUrl = villa.ImageUrl,
                Amenidad = villa.Amenidad

            };

            if (villa == null) 
            {
                return BadRequest();
            }

            patchDto.ApplyTo(villaDto, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }

            Villa modelo = new()
            {
                Id = villaDto.Id,
                Nombre = villaDto.Nombre,
                Detalle = villaDto.Detalle,
                Tarifa = villaDto.Tarifa,
                Ocupantes = villaDto.Ocupantes,
                MetrosCuadrados = villaDto.MetrosCuadrados,
                ImageUrl = villaDto.ImageUrl,
                Amenidad = villaDto.Amenidad

            };
            _db.Villas.Update(modelo);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteVilla(int id) 
        {
            if (id ==0)
            {
                return BadRequest();
            }

            var villa = await _db.Villas.FirstOrDefaultAsync(x=>x.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            //VillaStore.VillaList.Remove(villa);
             _db.Villas.Remove(villa);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
