

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Trasgo.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/maps")]
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

        [HttpPost("getSearchLocation")]
        public async Task<object> GetSearchLocation([FromBody] CreateDirectionsDto createDirectionsDto)
        {
            try
            {
                var data = await _IMapsService.GetSearchLocation(createDirectionsDto);
                return Ok(data);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        [HttpPost("getPlaceLocation")]
        public async Task<object> GetPlaceLocation([FromBody] CreateDirectionsDto createDirectionsDto)
        {
            try
            {
                var data = await _IMapsService.GetPlaceLocation(createDirectionsDto);
                return Ok(data);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        [HttpPost("getAddressFromLatLon")]
        public async Task<object> GetAddressFromLatLon([FromBody] CreateDirectionsDto createDirectionsDto)
        {
            try
            {
                var data = await _IMapsService.GetAddressFromLatLon(createDirectionsDto);
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