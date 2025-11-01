namespace IT_outCRM.Application.Interfaces.Repositories
{
    /// <summary>
    /// Интерфейс для репозиториев с поддержкой пагинации
    /// Соблюдение SOLID Interface Segregation Principle:
    /// Разделение интерфейсов по функциональности - пагинация выделена отдельно
    /// </summary>
    /// <typeparam name="T">Тип сущности</typeparam>
    public interface IPagedRepository<T> where T : class
    {
        /// <summary>
        /// Получить сущности с пагинацией
        /// </summary>
        /// <param name="pageNumber">Номер страницы (начинается с 1)</param>
        /// <param name="pageSize">Размер страницы</param>
        /// <returns>Кортеж (элементы страницы, общее количество записей)</returns>
        Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize);
    }
}

