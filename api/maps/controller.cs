

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Trasgo.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/otp")]
    public class MapsController : ControllerBase
    {
        private readonly IMapsService _IMapsService;
        private readonly ErrorHandlingUtility _errorUtility;
        private readonly ValidationMasterDto _masterValidationService;
        public MapsController(IMapsService MapsService)
        {
            _IMapsService = MapsService;
            _errorUtility = new ErrorHandlingUtility();
            _masterValidationService = new ValidationMasterDto();
        }

        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                var data = await _IMapsService.Get();
                return Ok(data);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        [HttpPost("getDirections")]
        public async Task<object> GetDirections([FromBody] CreateDirectionsDto createDirectionsDto)
        {
            try
            {
                var data = await _IMapsService.GetDirections(createDirectionsDto);
                return Ok(data);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

    }
}