using Microsoft.AspNetCore.Mvc;
using Serilog;
using SportsComplex.API.Api.JSend;
using SportsComplex.API.Api.Requests;
using SportsComplex.Logic.Exceptions;
using SportsComplex.Logic.Interfaces;
using SportsComplex.Logic.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace SportsComplex.API.Controllers
{
    [ApiController]
    [Route("guardians")]
    [Produces("application/json")]
    public class GuardianController : ControllerBase
    {
        private readonly IGuardianLogic _logic;

        public GuardianController(IGuardianLogic logic)
        {
            _logic = logic;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Gets guardians from database")]
        public async Task<IActionResult> GetGuardiansAsync([FromQuery] GetGuardianQuery query)
        {
            var filters = new GuardianQuery
            {
                Ids = query.Ids,
                FirstName = query.FirstName,
                LastName = query.LastName,
                Count = query.Count,
                Descending = query.Descending,
                OrderBy = query.OrderBy
            };

            var data = await _logic.GetGuardiansAsync(filters);
            return Ok(new JSendResponse(data));
        }

        [HttpGet("{id:int}")]
        [SwaggerOperation(
            Summary = "Gets existing guardian from database")]
        public async Task<IActionResult> GetGuardianByIdAsync([FromRoute] int id)
        {
            try
            {
                var data = await _logic.GetGuardianByIdAsync(id);
                return Ok(new JSendResponse(data));
            }
            catch (EntityNotFoundException ex)
            {
                Log.Error(ex, "Could not find guardian with 'Id={GuardianId}' in database. See inner exception for details.", id);

                return NotFound(new JSendResponse
                {
                    Status = JSendStatus.Fail,
                    Message = ex.Message
                });
            }
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Inserts a new guardian into the database")]
        public async Task<IActionResult> AddGuardianAsync([FromBody] GuardianRequest request)
        {
            try
            {
                var guardian = Map(request);
                var data = await _logic.AddGuardianAsync(guardian);
                return Ok(new JSendResponse(data));
            }
            catch (ArgumentNullException ex)
            {
                Log.Warning(ex, "Guardian cannot be null. See exception for details.");

                return BadRequest(new JSendResponse
                {
                    Status = JSendStatus.Fail,
                    Message = ex.Message
                });
            }
        }

        [HttpPut("{id:int}")]
        [SwaggerOperation(
            Summary = "Updates an existing guardian in the database")]
        public async Task<IActionResult> UpdateGuardianAsync([FromRoute] int id, [FromBody] GuardianRequest request)
        {
            try
            {
                var guardian = Map(request, id);
                var data = await _logic.UpdateGuardianAsync(guardian);

                return Ok(new JSendResponse(data));
            }
            catch (ArgumentNullException ex)
            {
                Log.Warning(ex, "Guardian cannot be null. See exception for details.");

                return BadRequest(new JSendResponse
                {
                    Status = JSendStatus.Fail,
                    Message = ex.Message
                });
            }
        }

        [HttpDelete("{id:int}")]
        [SwaggerOperation(
            Summary = "Deletes an existing guardian in the database")]
        public async Task<IActionResult> DeleteGuardianAsync([FromRoute] int id)
        {
            try
            {
                await _logic.DeleteGuardianAsync(id);
                return Ok(new JSendResponse());
            }
            catch (EntityNotFoundException ex)
            {
                Log.Warning(ex, "Cannot delete a guardian that does not exist in the database. See exception for details.");

                return BadRequest(new JSendResponse
                {
                    Status = JSendStatus.Fail,
                    Message = ex.Message
                });
            }
        }

        private static Guardian Map(GuardianRequest request, int id=0)
        {
            return new Guardian
            {
                Id = id,
                FirstName = request.FirstName,
                LastName = request.LastName,
                BirthDate = request.BirthDate,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                Address = request.Address,
                OtherAddress = request.OtherAddress
            };
        }
    }
}