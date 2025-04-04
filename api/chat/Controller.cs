using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Trasgo.Server.Controllers
{
    [ApiController]
    [Route("api/v1/Chat")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _ChatService;

        public ChatController(IChatService ChatService)
        {
            _ChatService = ChatService;
        }

        [HttpPost("sendWA")]
        public async Task<IActionResult> SendChatWA([FromBody] CreateChatDto dto)
        {
            try
            {
                var result = await _ChatService.SendChatWAAsync(dto);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("getWA")]
        public async Task<IActionResult> GetChatWA([FromBody] GetChatDto dto)
        {
            try
            {
                var result = await _ChatService.GetChatWAAsync(dto);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
