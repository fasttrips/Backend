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

        [HttpGet("GetOrder/Detail/{id}")]
        public async Task<IActionResult> GetOrderDetail([FromRoute]string id)
        {
            try
            {
                var result = await _OrderService.GetOrderDetail(id);
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

        [HttpGet("cancelOrderByUser/{idOrder}")]
        public async Task<IActionResult> CancelOrderByUser([FromRoute] string idOrder)
        {
            try
            {
                var result = await _OrderService.CancelOrderByUser(idOrder);
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

        [HttpGet("terimaOrder/{idOrder}")]
        public async Task<IActionResult> TerimaOrder([FromRoute] string idOrder)
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
                var result = await _OrderService.TerimaOrder(idUser, idOrder);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("lanjutOrder/{idOrder}")]
        public async Task<IActionResult> LanjutOrder([FromRoute] string idOrder)
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
                var result = await _OrderService.LanjutkanOrder(idUser, idOrder);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("selesaiOrder/{idOrder}")]
        public async Task<IActionResult> SelesaiOrder([FromRoute] string idOrder)
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
                var result = await _OrderService.SelesaiOrder(idUser, idOrder);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("cancelOrder/{idOrder}")]
        public async Task<IActionResult> CancelOrder([FromRoute] string idOrder)
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
                var result = await _OrderService.CancelOrder(idUser, idOrder);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("driverlistOrder")]
        public async Task<IActionResult> DriverOrder()
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
                var result = await _OrderService.DriverOrder(idUser);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
