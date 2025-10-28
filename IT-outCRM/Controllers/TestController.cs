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

        [HttpGet("test-error")]
        public IActionResult TestError()
        {
            throw new Exception("Это тестовая ошибка для проверки глобального обработчика исключений");
        }

        [HttpGet("test-not-found")]
        public IActionResult TestNotFound()
        {
            throw new KeyNotFoundException("Тестовый ресурс не найден");
        }

        [HttpGet("test-bad-request")]
        public IActionResult TestBadRequest()
        {
            throw new ArgumentException("Тестовые неверные параметры");
        }
    }
}