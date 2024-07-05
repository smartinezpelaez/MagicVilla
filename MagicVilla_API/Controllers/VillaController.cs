using AutoMapper;
using MagicVilla_API.Datos;
using MagicVilla_API.Modelos;
using MagicVilla_API.Modelos.Dto;
using MagicVilla_API.Repositorio.IRepositorio;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

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
        protected APIResponse _response;
        


        public VillaController(ILogger<VillaController> logger, IVillaRepositorio  villaRepo, IMapper mapper)
        {
             _logger = logger;
            _villaRepo = villaRepo;
            _mapper = mapper;
            _response = new();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillas() 
        {
            try
            {
                _logger.LogInformation("Obtener las villas");

                IEnumerable<Villa> villaList = await _villaRepo.ObtenerTodos();

                _response.Resultado = _mapper.Map<IEnumerable<VillaDto>>(villaList);
                _response.statusCode = HttpStatusCode.OK;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string>() { ex.ToString()};
            }

            return _response;
        }

        [HttpGet("id:int", Name= "GetVillaById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVillaById(int id)
        {
            try
            {
                if (id == 0)
                {
                    _logger.LogError("Error al traer  Villa con id " + id);
                    _response.statusCode = HttpStatusCode.BadRequest;
                    return BadRequest (_response);
                }

                //var villa = VillaStore.VillaList.FirstOrDefault(x => x.Id == id);
                var villa = await _villaRepo.Obtener(x => x.Id == id);

                if (villa == null)
                {
                    _logger.LogError("Error no encontado la  Villa con id " + id);
                    _response.statusCode = HttpStatusCode.NotFound;
                    _response.IsExitoso = false;
                    return NotFound(_response);
                }

                _response.statusCode = HttpStatusCode.OK;
                _response.Resultado = _mapper.Map<VillaDto>(villa);
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return _response;
            
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CrearVilla([FromBody] VillaCreateDto crearDto) 
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _response.statusCode=HttpStatusCode.BadRequest;
                    return BadRequest(_response);                   

                }

                if (await _villaRepo.Obtener(x => x.Nombre.ToLower() == crearDto.Nombre.ToLower()) != null)
                {
                    ModelState.AddModelError("Nombre Existe", "La villa con ese nombre ya existe!");
                    _response.statusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                if (crearDto == null)
                {
                    _response.statusCode=HttpStatusCode.BadRequest;
                    _response.IsExitoso = false;
                    return BadRequest(_response);
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
                _response.Resultado = modelo;
                _response.statusCode = HttpStatusCode.Created;
                //await _villaRepo.Grabar();

                return CreatedAtRoute("GetVillaById", new { id = modelo.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return _response;
        
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDto updateDto)
        {
            try
            {
                if (updateDto == null || id != updateDto.Id)
                {
                    _response.IsExitoso = false;
                    _response.statusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                Villa modelo = _mapper.Map<Villa>(updateDto);

                await _villaRepo.Actualizar(modelo);

                _response.statusCode = HttpStatusCode.NoContent;

                return NoContent();
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };

            }

            return Ok(_response);
           
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto> patchDto)
        {
            try
            {
                if (patchDto == null || id == 0)
                {
                    _response.IsExitoso = false;
                    _response.statusCode = HttpStatusCode.BadRequest;
                    return BadRequest();
                }
                //var villa = VillaStore.VillaList.FirstOrDefault(x => x.Id == id);
                var villa = await _villaRepo.Obtener(x => x.Id == id, tracked: false);

                VillaUpdateDto villaDto = _mapper.Map<VillaUpdateDto>(villa);

                if (villa == null)
                {
                    _response.IsExitoso=false;
                    return BadRequest(_response);
                }

                patchDto.ApplyTo(villaDto, ModelState);

                if (!ModelState.IsValid)
                {
                    
                    return BadRequest(ModelState);

                }

                Villa modelo = _mapper.Map<Villa>(villaDto);

               await _villaRepo.Actualizar(modelo);

                _response.statusCode = HttpStatusCode.NoContent;

                return NoContent();
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
                
            }
           return Ok(_response);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteVilla(int id) 
        {
            try
            {
                if (id == 0)
                {
                    _response.IsExitoso = false;
                    _response.statusCode = HttpStatusCode.BadRequest;
                    return BadRequest();
                }

                var villa = await _villaRepo.Obtener(x => x.Id == id);
                if (villa == null)
                {
                    _response.IsExitoso=false;
                    _response.statusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }
                //VillaStore.VillaList.Remove(villa);
                await _villaRepo.Remover(villa);

                _response.statusCode = HttpStatusCode.NoContent;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };

            }

            return  BadRequest(_response);
        }
    }
}
