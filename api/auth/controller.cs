
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

        [AllowAnonymous]
        [HttpPost]
        [Route("googleSign")]
        public async Task<object> RegisterGoogleAsync([FromBody] RegisterGoogleDto login)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(login.Token) as JwtSecurityToken;

                if (jsonToken != null)
                {
                    var payloadJson = jsonToken.Payload.SerializeToJson();
                    var payload = JsonSerializer.Deserialize<JwtPayloads>(payloadJson);
                    if (payload.Audience == "831730691096-hsuqs4noja9rc5r3c0sbj050q5st4pmq.apps.googleusercontent.com")
                    {
                        var response = await _IAuthService.RegisterGoogleAsync(payloadJson, login);
                        return Ok(response);
                    }
                    else
                    {
                        throw new CustomException(400, "Token", "Invalid Token");
                    }
                }
                else
                {
                    throw new CustomException(400, "Token", "Invalid Token");
                }

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
        [Route("verifySessions")]
        public object VerifySessions()
        {
            try
            {
                var claims = User.Claims;
                if (claims == null)
                {
                    return new CustomException(400, "Error", "Unauthorized");
                }
                return new { code = 200, message = "not expired" };
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