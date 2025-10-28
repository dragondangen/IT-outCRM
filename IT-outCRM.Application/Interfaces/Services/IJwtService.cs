using IT_outCRM.Domain.Entity;

namespace IT_outCRM.Application.Interfaces.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        Guid? GetUserIdFromToken(string token);
    }
}

