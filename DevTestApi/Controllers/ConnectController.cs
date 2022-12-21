using dev_test_api_business;
using dev_test_api_data_access;
using dev_test_api_models;
using dev_test_api_models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DevTestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConnectController : ControllerBase
    {
        private ILogger<ConnectController> _logger;
        private IConfiguration _configuration;
        private IConnectService _connectService;

        public ConnectController(ILogger<ConnectController> logger, IConfiguration configuration, IConnectService connectService)
        {
            _logger = logger;
            _configuration = configuration;
            _connectService = connectService;
        }

        [HttpPost]
        [Route("authenticate")]
        public async Task<ActionResult<AuthResponseDto>> Authenticate([FromBody]AuthRequestDto authRequestDto)
        {
            var isValid = await _connectService.ValidateLogin(authRequestDto.UserName, authRequestDto.Password);

            if(isValid == false)
            {
                _logger.LogError("Invalid Authentication Attempt");
                return Unauthorized();
            }
                

            var token = _connectService.CreateJwtToken(authRequestDto.UserName);

            if (token == string.Empty)
            {
                _logger.LogError("Failed to create JWT TOKEN");
                return BadRequest();
            }

            AuthResponseDto dto = new AuthResponseDto();
            dto.Token = token;
            dto.UserName = authRequestDto.UserName;

            return Ok(dto);
        }
    }
}
