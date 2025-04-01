using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Trasgo.Server.Controllers
{
    [ApiController]
    [Route("api/v1/order")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _OrderService;
        private readonly ConvertJWT _ConvertJwt;

        public OrderController(ConvertJWT convert, IOrderService OrderService)
        {
            _OrderService = OrderService;
            _ConvertJwt = convert;
        }

        [HttpGet("GetOrder")]
        public async Task<IActionResult> GetOrder()
        {
            try
            {
                var claims = User.Claims;
                if (claims == null)
                {
                    return Unauthorized(new { code = 400, error = "Error", message = "Unauthorized" });
                }
                string accessToken = HttpContext.Request.Headers["Authorization"];
                string idUser = await _ConvertJwt.ConvertString(accessToken);
                var result = await _OrderService.GetOrder(idUser);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("getRider/{idOrder}")]
        public async Task<IActionResult> GetRider([FromRoute] string idOrder)
        {
            try
            {
                var result = await _OrderService.GetRider(idOrder);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("ride")]
        public async Task<IActionResult> OrderRide([FromBody] CreateOrderDto dto)
        {
            try
            {
                var claims = User.Claims;
                if (claims == null)
                {
                    return Unauthorized(new { code = 400, error = "Error", message = "Unauthorized" });
                }
                string accessToken = HttpContext.Request.Headers["Authorization"];
                string idUser = await _ConvertJwt.ConvertString(accessToken);
                var result = await _OrderService.OrderRide(dto, idUser);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
