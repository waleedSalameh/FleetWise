using FleetWise.BLL.DTOs.Vehicles.Trailer;
using FleetWise.BLL.Interface.Vehicles;
using Microsoft.AspNetCore.Mvc;

namespace FleetWise.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrailerController : ControllerBase
    {
        private readonly ITrailer _trailerService;

        public TrailerController(ITrailer trailerService)
        {
            _trailerService = trailerService;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                var trailers = await _trailerService.GetAllAsync();
                return Ok(trailers);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = ex.Message });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetById(int id)
        {
            try
            {
                var trailer = await _trailerService.GetByIdAsync(id);

                if (trailer == null)
                    return NotFound(new { message = "Trailer not found." });

                return Ok(trailer);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateTrailerDto dto)
        {
            try
            {
                var result = await _trailerService.CreateAsync(dto);

                return StatusCode(StatusCodes.Status201Created, result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, UpdateTrailerDto dto)
        {
            try
            {
                var result = await _trailerService.UpdateAsync(id, dto);

                if (!result)
                    return NotFound(new { message = "Trailer not found." });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var result = await _trailerService.DeleteAsync(id);

                if (!result)
                    return NotFound(new { message = "Trailer not found." });

                return NoContent();
            }
            catch (Exception ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}