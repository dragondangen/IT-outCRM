using IT_outCRM.Middleware;

namespace IT_outCRM.Middleware
{
    /// <summary>
    /// Фабрика для создания ответов на исключения
    /// Соблюдение SOLID Single Responsibility Principle
    /// </summary>
    public interface IExceptionResponseFactory
    {
        /// <summary>
        /// Создать ответ на основе исключения
        /// </summary>
        ErrorResponse CreateResponse(Exception exception, IHostEnvironment environment);
    }

    /// <summary>
    /// Реализация фабрики ответов на исключения
    /// </summary>
    public class ExceptionResponseFactory : IExceptionResponseFactory
    {
        public ErrorResponse CreateResponse(Exception exception, IHostEnvironment environment)
        {
            var response = new ErrorResponse();

            switch (exception)
            {
                case ArgumentNullException:
                case ArgumentException:
                    response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                    response.Message = "Неверные параметры запроса";
                    response.Details = environment.IsDevelopment() ? exception.Message : null;
                    break;

                case KeyNotFoundException:
                    response.StatusCode = (int)System.Net.HttpStatusCode.NotFound;
                    response.Message = "Ресурс не найден";
                    response.Details = environment.IsDevelopment() ? exception.Message : null;
                    break;

                case UnauthorizedAccessException:
                    response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                    response.Message = "Доступ запрещен";
                    response.Details = environment.IsDevelopment() ? exception.Message : null;
                    break;

                case InvalidOperationException:
                    response.StatusCode = (int)System.Net.HttpStatusCode.Conflict;
                    response.Message = "Операция не может быть выполнена";
                    response.Details = environment.IsDevelopment() ? exception.Message : null;
                    break;

                default:
                    response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                    response.Message = "Внутренняя ошибка сервера";
                    response.Details = environment.IsDevelopment() 
                        ? $"{exception.Message}\n{exception.StackTrace}" 
                        : null;
                    break;
            }

            return response;
        }
    }
}

