using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace chat_in_realtime.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                Status = "Online",
                Message = "La API está levantada y corriendo 🚀",
                ServerTime = DateTime.Now
            });
        }
    }
}