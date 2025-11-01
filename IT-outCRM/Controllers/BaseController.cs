using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IT_outCRM.Controllers
{
    /// <summary>
    /// Базовый контроллер для устранения дублирования кода (DRY) 
    /// и соблюдения Single Responsibility Principle (SRP)
    /// </summary>
    [Authorize]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected readonly ILogger Logger;

        protected BaseController(ILogger logger)
        {
            Logger = logger;
        }

        /// <summary>
        /// Проверка соответствия ID в URL и DTO
        /// </summary>
        protected ActionResult<T>? ValidateUpdateId<T>(Guid id, Guid dtoId, string entityName = "Сущность")
        {
            if (id != dtoId)
            {
                return BadRequest($"ID в URL не совпадает с ID в теле запроса");
            }
            return null;
        }

        /// <summary>
        /// Обработка случая, когда сущность не найдена
        /// </summary>
        protected ActionResult<T>? HandleNotFound<T>(T? entity, Guid id, string entityName)
        {
            if (entity == null)
            {
                return NotFound($"{entityName} с ID {id} не найден");
            }
            return null;
        }

        /// <summary>
        /// Стандартизированное логирование создания сущности
        /// </summary>
        protected void LogCreated<T>(T entity, Guid id, string entityName)
        {
            Logger.LogInformation("Создан {EntityName}: ID {Id}", entityName, id);
        }

        /// <summary>
        /// Стандартизированное логирование обновления сущности
        /// </summary>
        protected void LogUpdated(Guid id, string entityName)
        {
            Logger.LogInformation("Обновлён {EntityName}: ID {Id}", entityName, id);
        }

        /// <summary>
        /// Стандартизированное логирование удаления сущности
        /// </summary>
        protected void LogDeleted(Guid id, string entityName)
        {
            Logger.LogInformation("Удалён {EntityName}: ID {Id}", entityName, id);
        }
    }
}

