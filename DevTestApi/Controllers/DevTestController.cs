using dev_test_api_business;
using dev_test_api_models;
using dev_test_api_models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DevTestApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class DevTestController : ControllerBase
    {
        public readonly IDevTestService _devTestService;
        public DevTestController(IDevTestService devTestService)
        {
            _devTestService = devTestService;
        }
        [HttpGet]
        [Route("GetPeople")]
        public async Task<ActionResult<List<PersonDto>>> GetPeople()
        {
            var response = await _devTestService.GetPerson();

            if(response == null || response.Count == 0)
                return NoContent();

            return Ok(response);
        }

        [HttpGet]
        [Route("GetClaims/person/{personId}")]
        public async Task<ActionResult<ClaimsResponseDto>> GetClaimsByPerson([FromRoute]int personId, int offset = 0, int rows = 50)
        {
            var response = await _devTestService.GetClaimsByPersonId(personId, offset, rows);

            if (response == null)
                return NoContent();


            return Ok(response);
        }

        [HttpGet]
        [Route("GetClaims/ExtPerson/{extPersonId}")]
        public async Task<ActionResult<List<PersonDto>>> GetClaimsByExtPerson([FromRoute] int extPersonId)
        {
            var response = await _devTestService.GetClaimsByExternalPersonId(extPersonId);

            if (response == null || response.Count() == 0)
                return NoContent();

            return Ok(response);
        }

        [HttpGet]
        [Route("GetPersonDetails/Organization/{organizationId}")]
        public async Task<ActionResult> GetPersonDetails([FromRoute]int organizationId, string? extPersonId, string? email, string? firstName, string? lastName, string? phone)
        {
            var response = await _devTestService.GetPersonDetails(organizationId, extPersonId, email, firstName, lastName, phone);

            if(response == null || response.Count == 0)
                return NoContent();

            return Ok(response);
        }
    }
}
