using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Trasgo.Server.Controllers
{
    [ApiController]
    [Route("api/v1/order")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _OrderService;

        public OrderController(IOrderService OrderService)
        {
            _OrderService = OrderService;
        }

        [HttpPost("ride")]
        public async Task<IActionResult> OrderRide([FromBody] CreateOrderDto dto)
        {
            try
            {
                var result = await _OrderService.OrderRide(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
