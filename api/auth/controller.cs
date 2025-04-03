
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Trasgo.Server.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _IAuthService;
        private readonly ConvertJWT _ConvertJwt;
        private readonly ErrorHandlingUtility _errorUtility;

        public AuthController(IAuthService authService, ConvertJWT convert)
        {
            _IAuthService = authService;
            _ConvertJwt = convert;
            _errorUtility = new ErrorHandlingUtility();

        }

        // [AllowAnonymous]
        // [HttpPost]
        // [Route("googleSign")]
        // public async Task<object> RegisterGoogleAsync([FromBody] RegisterGoogleDto login)
        // {
        //     try
        //     {
        //         var handler = new JwtSecurityTokenHandler();
        //         var jsonToken = handler.ReadToken(login.Token) as JwtSecurityToken;

        //         if (jsonToken != null)
        //         {
        //             var payloadJson = jsonToken.Payload.SerializeToJson();
        //             var payload = JsonSerializer.Deserialize<JwtPayloads>(payloadJson);
        //             if (payload.Audience == "831730691096-hsuqs4noja9rc5r3c0sbj050q5st4pmq.apps.googleusercontent.com")
        //             {
        //                 var response = await _IAuthService.RegisterGoogleAsync(payloadJson, login);
        //                 return Ok(response);
        //             }
        //             else
        //             {
        //                 throw new CustomException(400, "Token", "Invalid Token");
        //             }
        //         }
        //         else
        //         {
        //             throw new CustomException(400, "Token", "Invalid Token");
        //         }

        //     }
        //     catch (CustomException ex)
        //     {
        //         int errorCode = ex.ErrorCode;
        //         var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
        //         return _errorUtility.HandleError(errorCode, errorResponse);
        //     }
        // }

        [Authorize]
        [HttpGet]
        [Route("verifySessions")]
        public async Task<object> VerifySessionsAsync()
        {
            try
            {
                var claims = User.Claims;
                if (claims == null)
                {
                    return new CustomException(400, "Error", "Unauthorized");
                }
                string accessToken = HttpContext.Request.Headers["Authorization"];
                string idUser = await _ConvertJwt.ConvertString(accessToken);
                var data = await _IAuthService.Aktifasi(idUser);
                return data;
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
        [Route("updateProfile")]
        public async Task<object> UpdateProfile([FromBody] UpdateProfileDto updateProfileDto)
        {
            try
            {
                var claims = User.Claims;
                if (claims == null)
                {
                    return new CustomException(400, "Error", "Unauthorized");
                }
                string accessToken = HttpContext.Request.Headers["Authorization"];
                string idUser = await _ConvertJwt.ConvertString(accessToken);
                var data = await _IAuthService.UpdateProfile(idUser, updateProfileDto);
                return data;
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
        [Route("updateFCMUser")]
        public async Task<object> UpdateUserProfile([FromBody] UpdateFCMProfileDto updateProfileDto)
        {
            try
            {
                var claims = User.Claims;
                if (claims == null)
                {
                    return new CustomException(400, "Error", "Unauthorized");
                }
                string accessToken = HttpContext.Request.Headers["Authorization"];
                string idUser = await _ConvertJwt.ConvertString(accessToken);
                var data = await _IAuthService.UpdateUserProfile(idUser, updateProfileDto);
                return data;
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
        [Route("updateFCMDriver")]
        public async Task<object> UpdateDriverProfile([FromBody] DriverAvalibleModelDTO updateProfileDto)
        {
            try
            {
                var claims = User.Claims;
                if (claims == null)
                {
                    return new CustomException(400, "Error", "Unauthorized");
                }
                string accessToken = HttpContext.Request.Headers["Authorization"];
                string idUser = await _ConvertJwt.ConvertString(accessToken);
                var data = await _IAuthService.UpdateDriverProfile(idUser, updateProfileDto);
                return data;
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
        [Route("updateLocationDriver")]
        public async Task<object> UpdateDriverLocationProfile([FromBody] DriverAvalibleModelDTO updateProfileDto)
        {
            try
            {
                var claims = User.Claims;
                if (claims == null)
                {
                    return new CustomException(400, "Error", "Unauthorized");
                }
                string accessToken = HttpContext.Request.Headers["Authorization"];
                string idUser = await _ConvertJwt.ConvertString(accessToken);
                var data = await _IAuthService.UpdateDriverLocationProfile(idUser, updateProfileDto);
                return data;
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
        [Route("updateStatusDriver")]
        public async Task<object> UpdateDriverStatusProfile([FromBody] DriverStatusServe updateProfileDto)
        {
            try
            {
                var claims = User.Claims;
                if (claims == null)
                {
                    return new CustomException(400, "Error", "Unauthorized");
                }
                string accessToken = HttpContext.Request.Headers["Authorization"];
                string idUser = await _ConvertJwt.ConvertString(accessToken);
                var data = await _IAuthService.UpdateDriverStatusProfile(idUser, updateProfileDto);
                return data;
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        [Authorize]
        [HttpGet]
        [Route("updateStatusDriver")]
        public async Task<object> GetDriverStatusProfile()
        {
            try
            {
                var claims = User.Claims;
                if (claims == null)
                {
                    return new CustomException(400, "Error", "Unauthorized");
                }
                string accessToken = HttpContext.Request.Headers["Authorization"];
                string idUser = await _ConvertJwt.ConvertString(accessToken);
                var data = await _IAuthService.GetDriverStatusProfile(idUser);
                return data;
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