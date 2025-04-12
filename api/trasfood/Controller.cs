

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Trasgo.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/warung")]
    public class TrasFoodController : ControllerBase
    {
        private readonly ITrasFoodService _ITrasFoodService;
        private readonly ErrorHandlingUtility _errorUtility;
        private readonly ValidationMasterDto _masterValidationService;
        public TrasFoodController(ITrasFoodService TrasFoodService)
        {
            _ITrasFoodService = TrasFoodService;
            _errorUtility = new ErrorHandlingUtility();
            _masterValidationService = new ValidationMasterDto();
        }

        [Authorize]
        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                var data = await _ITrasFoodService.GetWarung();
                return Ok(data);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<object> GetbyId([FromRoute] string id)
        {
            try
            {
                var data = await _ITrasFoodService.GetWarungbyId(id);
                return Ok(data);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        [Authorize]
        [HttpGet("Distance/{lat}/{lng}")]
        public async Task<object> Get([FromRoute] double lat, [FromRoute] double lng)
        {
            try
            {
                var data = await _ITrasFoodService.GetWarungDistance(lat,lng);
                return Ok(data);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<object> CreateWarung(CreateWarungDto dto)
        {
            try
            {
                var data = await _ITrasFoodService.CreateWarung(dto);
                return Ok(data);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        [Authorize]
        [HttpPost("Food")]
        public async Task<object> CreateFood(CreateFoodDto dto)
        {
            try
            {
                var data = await _ITrasFoodService.CreateFood(dto);
                return Ok(data);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        // [HttpGet("{id}")]
        // public async Task<object> GetById([FromRoute] string id)
        // {
        //     try
        //     {
        //         var data = await _ITrasFoodService.GetById(id);
        //         return Ok(data);
        //     }
        //     catch (CustomException ex)
        //     {
        //         int errorCode = ex.ErrorCode;
        //         var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
        //         return _errorUtility.HandleError(errorCode, errorResponse);
        //     }
        // }

        // [Authorize]
        // [HttpPost]
        // public async Task<object> Post([FromBody] CreateTrasFoodDto item)
        // {
        //     try
        //     {
        //         var validationErrors = _masterValidationService.ValidateCreateInput(item);
        //         if (validationErrors.Count > 0)
        //         {
        //             var errorResponse = new { code = 400, errorMessage = validationErrors };
        //             return BadRequest(errorResponse);
        //         }
        //         var data = await _ITrasFoodService.Post(item);
        //         return Ok(data);
        //     }
        //     catch (CustomException ex)
        //     {
        //         int errorCode = ex.ErrorCode;
        //         var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
        //         return _errorUtility.HandleError(errorCode, errorResponse);
        //     }
        // }

        // [Authorize]
        // [HttpPut("{id}")]
        // public async Task<object> Put([FromRoute] string id, [FromBody] CreateTrasFoodDto item)
        // {
        //     try
        //     {
        //         var validationErrors = _masterValidationService.ValidateCreateInput(item);
        //         if (validationErrors.Count > 0)
        //         {
        //             var errorResponse = new { code = 400, errorMessage = validationErrors };
        //             return BadRequest(errorResponse);
        //         }
        //         var data = await _ITrasFoodService.Put(id, item);
        //         return Ok(data);
        //     }
        //     catch (CustomException ex)
        //     {
        //         int errorCode = ex.ErrorCode;
        //         var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
        //         return _errorUtility.HandleError(errorCode, errorResponse);
        //     }
        // }

        // [Authorize]
        // [HttpDelete("{id}")]
        // public async Task<object> Delete([FromRoute] string id)
        // {
        //     try
        //     {
        //         var data = await _ITrasFoodService.Delete(id);
        //         return Ok(data);
        //     }
        //     catch (CustomException ex)
        //     {
        //         int errorCode = ex.ErrorCode;
        //         var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
        //         return _errorUtility.HandleError(errorCode, errorResponse);
        //     }
        // }
    }
}