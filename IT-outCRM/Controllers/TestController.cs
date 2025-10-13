using Microsoft.AspNetCore.Mvc;

namespace IT_outCRM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Приложение работает!");
        }

        [HttpGet("db-test")]
        public async Task<IActionResult> TestDatabase()
        {
            return Ok("База данных подключена!");
        }
    }
}