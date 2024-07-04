using AutoMapper;
using MagicVilla_API.Datos;
using MagicVilla_API.Modelos;
using MagicVilla_API.Modelos.Dto;
using MagicVilla_API.Repositorio.IRepositorio;
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
        private readonly IVillaRepositorio _villaRepo;
        //private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public VillaController(ILogger<VillaController> logger, IVillaRepositorio  villaRepo, IMapper mapper)
        {
             _logger = logger;
            _villaRepo = villaRepo;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VillaDto>>> GetVillas() 
        {
            _logger.LogInformation("Obtener las villas");

            IEnumerable<Villa> villaList = await _villaRepo.ObtenerTodos();

            return Ok(_mapper.Map<IEnumerable<VillaDto>>(villaList));
            
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
            var villa = await _villaRepo.Obtener(x => x.Id == id);

            if (villa == null) 
            {
                _logger.LogError("Error no encontado la  Villa con id " + id);
                return NotFound();
            }

            return Ok(_mapper.Map<VillaDto>(villa));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaCreateDto>> CrearVilla([FromBody] VillaCreateDto crearDto) 
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }

            if (await _villaRepo.Obtener(x=>x.Nombre.ToLower() == crearDto.Nombre.ToLower()) != null)
            {
                ModelState.AddModelError("Nombre Existe", "La villa con ese nombre ya existe!");
                return BadRequest(ModelState);
            }

            if (crearDto == null) 
            {
                return BadRequest(crearDto);
            }

            //Esta linea de código reemplaza todo el mapeo de abajo         
            Villa modelo = _mapper.Map<Villa>(crearDto);

            //se reemplaza este mapeo 
            //Villa modelo = new()
            //{
            //    //Id = villaDto.Id,
            //    Nombre = villaDto.Nombre,
            //    Detalle = villaDto.Detalle,
            //    Tarifa = villaDto.Tarifa,
            //    Ocupantes = villaDto.Ocupantes,
            //    MetrosCuadrados = villaDto.MetrosCuadrados,
            //    ImageUrl = villaDto.ImageUrl,
            //    Amenidad = villaDto.Amenidad

            //};
            await _villaRepo.Crear(modelo);
            //await _villaRepo.Grabar();

            return CreatedAtRoute("GetVillaById", new { id = modelo.Id}, modelo);
        
        
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDto updateDto) 
        {
            if (updateDto == null || id != updateDto.Id)
            {
                return BadRequest();
            }   
          
            Villa modelo = _mapper.Map<Villa>(updateDto);
          
            _villaRepo.Actualizar(modelo);
            
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
            var villa = await _villaRepo.Obtener(x=>x.Id == id, tracked:false);
            
            VillaUpdateDto villaDto = _mapper.Map<VillaUpdateDto>(villa);            

            if (villa == null) 
            {
                return BadRequest();
            }

            patchDto.ApplyTo(villaDto, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }

            Villa modelo = _mapper.Map<Villa>(villaDto);
           
           _villaRepo.Actualizar(modelo);            

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

            var villa = await _villaRepo.Obtener(x=>x.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            //VillaStore.VillaList.Remove(villa);
             _villaRepo.Remover(villa);            

            return NoContent();
        }
    }
}
