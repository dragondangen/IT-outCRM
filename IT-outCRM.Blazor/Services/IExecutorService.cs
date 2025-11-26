using IT_outCRM.Blazor.Models;

namespace IT_outCRM.Blazor.Services
{
    public interface IExecutorService
    {
        Task<List<ExecutorModel>> GetAllAsync();
        Task<ExecutorModel?> GetByIdAsync(Guid id);
        Task<ExecutorModel?> CreateAsync(ExecutorModel model);
        Task<ExecutorModel?> UpdateAsync(ExecutorModel model);
        Task<bool> DeleteAsync(Guid id);
        void SetToken(string token);
    }
}
